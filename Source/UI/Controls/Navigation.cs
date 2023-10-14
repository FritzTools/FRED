using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FRED.UI.Controls {
    public class Navigation : ItemsControl {
        public Navigation() {
            Loaded += OnControlLoaded;
        }

        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);

            FrameworkElementFactory conatiner = new FrameworkElementFactory(typeof(StackPanel));
            conatiner.SetValue(StackPanel.IsItemsHostProperty, true);
            conatiner.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

            var template = new ItemsPanelTemplate {
                VisualTree = conatiner
            };
            
            ItemsPanel = template;
        }

        private void OnControlLoaded(object? sender, RoutedEventArgs e) {
            foreach(var entry in Items) {
                if(entry is NavigationItem item) {
                    if(item != null) {
                        RegisterItem(item);
                    }
                }
            }

            foreach(var entry in Items) {
                if (entry is NavigationItem) {
                    if ((NavigationItem) entry != null){
                        break;
                    }
                }
            }
        }

        private void RegisterItem(NavigationItem item) {
            item.ArrowVisibility = ShowArrow ? Visibility.Visible : Visibility.Hidden;
            item.Collapse();
            item.OnExpandedEvent    += OnItemExpanded;
            item.OnCollapsedEvent   += OnItemCollapsed;
        }

        void OnItemExpanded(NavigationItem expandedItem) {
            foreach(var entry in Items) {
                if(entry is NavigationItem item) {
                    if(item != null) {
                        if(expandedItem != item) {
                            if(item.IsExpanded) {
                                item.Collapse();
                            }
                        }
                    }
                }
            }
        }

        void OnItemCollapsed(NavigationItem collapsedItem) {
            foreach(var entry in Items) {
                if(entry is NavigationItem item) {
                    if(item != null) {
                        if(collapsedItem != item) {
                            if(item.IsExpanded) {
                                item.Collapse();
                                return;
                            }
                        }
                    }
                }
            }

            collapsedItem.Expand();
        }

        public bool ShowArrow {
            get {
                return (bool) GetValue(ShowArrowProperty);
            }

            set {
                SetValue(ShowArrowProperty, value);
            }
        }

        public static readonly DependencyProperty ShowArrowProperty = DependencyProperty.Register("ShowArrow", typeof(bool), typeof(Navigation), new FrameworkPropertyMetadata(true, OnShowArrowPropertyChanged));

        private static void OnShowArrowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Navigation control = (Navigation) d;

            if(control == null) {
                return;
            }

            foreach(var entry in control.Items) {
                NavigationItem item = (NavigationItem) entry;

                if(item != null) {
                    item.ArrowVisibility = (bool) e.NewValue ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }   
    }

    public class NavigationItemToggleButton : ToggleButton {
        public bool IsExpanded {
            get {
                return (bool) GetValue(IsExpandedProperty);
            }

            set {
                SetValue(IsExpandedProperty, value);
            }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(NavigationItemToggleButton), new FrameworkPropertyMetadata(false));

        public Visibility ArrowVisibility {
            get {
                return (Visibility) GetValue(ArrowVisibilityProperty);
            }

            set {
                SetValue(ArrowVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty ArrowVisibilityProperty = DependencyProperty.RegisterAttached("ArrowVisibility", typeof(Visibility), typeof(NavigationItemToggleButton), new FrameworkPropertyMetadata(Visibility.Visible));

        protected override void OnToggle() {
            if(!IsExpanded) {
                base.OnToggle();
            }
        }
    }

    public class ToggleCommand : ICommand {
        private NavigationItem? owner;

        public ToggleCommand(NavigationItem owner) {
            this.owner = owner;
        }

        public event EventHandler? CanExecuteChanged {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) {
            return true; // !owner.IsExpanded;
        }

        public void Execute(object? parameter) {
            owner?.OnToggle();
        }
    }

    public class NavigationItem : HeaderedContentControl, INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate void OnExpanded(NavigationItem item);
        public delegate void OnCollapsed(NavigationItem item);
        public event OnExpanded? OnExpandedEvent;
        public event OnCollapsed? OnCollapsedEvent;

        public NavigationItem() {
            ToggleCommand = new ToggleCommand(this);
        }

        protected override void OnInitialized(EventArgs e) {
            base.OnInitialized(e);

            var resources = new ResourceDictionary {
                Source = new Uri("/FRED;component/UI/Design/Navigation.xaml", UriKind.RelativeOrAbsolute)
            };

            Resources   = resources;
            Template    = resources["RevealExpander"] as ControlTemplate;
        }

        private void OnNavigationItemControlLoaded(object sender, RoutedEventArgs e) { }
   
        internal void OnToggle() {
            if(IsExpanded) {
                OnCollapsedEvent?.Invoke(this);
                Collapse();
            } else {
                OnExpandedEvent?.Invoke(this);
                Expand();
            }
        }

        internal void Collapse() {
            IsExpanded                          = false;
            NavigationItemToggleButton? item    = (NavigationItemToggleButton) Template.FindName("ExpanderButton", this);

            if(item != null) {
                item.IsExpanded                 = false;
            }
        }

        internal void Expand() {
            IsExpanded                          = true;
            NavigationItemToggleButton? item    = (NavigationItemToggleButton) Template.FindName("ExpanderButton", this);

            if(item != null) {
                item.IsExpanded                 = true;
            }
        }

        public bool IsExpanded {
            get {
                return (bool) GetValue(IsItemExpandedProperty);
            }

            private set {
                SetValue(IsItemExpandedProperty, value);
            }
        }

        public static readonly DependencyProperty IsItemExpandedProperty = DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(NavigationItem), new FrameworkPropertyMetadata(false));

        public Visibility ArrowVisibility {
            get {
                return (Visibility) GetValue(ArrowVisibilityProperty);
            }

            set {
                SetValue(ArrowVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty ArrowVisibilityProperty = DependencyProperty.RegisterAttached("ArrowVisibility", typeof(Visibility), typeof(NavigationItem), new FrameworkPropertyMetadata(Visibility.Visible));

        public ICommand ToggleCommand {
            get {
                return (ICommand) GetValue(ToggleCommandProperty);
            }

            set {
                SetValue(ToggleCommandProperty, value);
            }
        }

        public static readonly DependencyProperty ToggleCommandProperty = DependencyProperty.RegisterAttached("ToggleCommand", typeof(ICommand), typeof(NavigationItem), new FrameworkPropertyMetadata(null));
    }
}
