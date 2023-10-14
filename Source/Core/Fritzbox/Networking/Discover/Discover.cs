using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using Rssdp;
using System;
using System.Collections.Generic;
using FRED.Core.Interfaces;
using FRED.Core.Data;

namespace FRED.Core.Fritzbox.Networking.Discover {
    public static class Discovery {
        public static String? Box                       = null;
        public static List<Action<String>> callbacks    = new List<Action<String>>();
        public static List<Changeable> observers        = new List<Changeable>();

        public static void SearchForDevices() {
            using (var deviceLocator = new SsdpDeviceLocator(new Rssdp.Infrastructure.SsdpCommunicationsServer(new SocketFactory(GetLocalIpAddress())))) {
                deviceLocator.NotificationFilter    = "upnp:rootdevice";
                deviceLocator.DeviceAvailable       += OnDeviceResponse;
                deviceLocator.StartListeningForNotifications();
                deviceLocator?.SearchAsync();
            }
        }

        public static void AddObserver(Changeable callback) {
            observers.Add(callback);
        }

        private async static void OnDeviceResponse(object? sender, DeviceAvailableEventArgs e) {
            SsdpDevice fullDevice = await e.DiscoveredDevice.GetDeviceInfo();

            if (fullDevice.FriendlyName.Contains("FRITZ!Box") && fullDevice.PresentationUrl != null) {
                if (Box == null) {
                    Box = fullDevice.PresentationUrl.ToString();

                    foreach(Action<String> callback in callbacks) {
                        callback(Box);
                    }

                    foreach(Changeable observer in observers) {
                        observer.OnChange(fullDevice, e);
                    }
                }

                //System.Diagnostics.Debug.Print("Found " + e.DiscoveredDevice.Usn + " at " + e.DiscoveredDevice.DescriptionLocation.ToString() +  ": " + fullDevice.PresentationUrl.ToString());
            }
        }

        public static void WhenReady(Action<String> callback) {
            if(Box == null) {
                callbacks.Add(callback);
                return;
            }

            callback(Box);
        }

        public static string GetLocalIpAddress() {
            UnicastIPAddressInformation? mostSuitableIp     = null;
            var networkInterfaces                           = NetworkInterface.GetAllNetworkInterfaces();

            foreach(var network in networkInterfaces) {
                if(network.OperationalStatus != OperationalStatus.Up) {
                    continue;
                }

                var properties = network.GetIPProperties();

                if(properties.GatewayAddresses.Count == 0) {
                    continue;
                }

                foreach(var address in properties.UnicastAddresses) {
                    if(address.Address.AddressFamily != AddressFamily.InterNetwork) {
                        continue;
                    }

                    if(IPAddress.IsLoopback(address.Address)) {
                        continue;
                    }

                    if(!address.IsDnsEligible) {
                        if(mostSuitableIp == null) {
                            mostSuitableIp = address;
                        }

                        continue;
                    }

                    if(address.PrefixOrigin != PrefixOrigin.Dhcp) {
                        if(mostSuitableIp == null || !mostSuitableIp.IsDnsEligible) {
                            mostSuitableIp = address;
                        }

                        continue;
                    }

                    return address.Address.ToString();
                }
            }

            return mostSuitableIp != null ? mostSuitableIp.Address.ToString() : "127.0.0.1";
        }
    }
}
