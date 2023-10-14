
![Preview](https://raw.githubusercontent.com/FritzTools/FRED/main/Screenshots/Preview.png)

# FRED - Der `Fritz!Box` Konfigurationsdatei-Editor
Mit **FRED** bekommst du ein einfachen Editor, der die Konfiguration der `Fritz!Box` bearbeiten kann. Du hast die volle Kontrolle über deine `Fritz!Box`-Einstellungen. Dieses leistungsstarke Tool wurde speziell entwickelt, um die Konfigurationsdateien einer `Fritz!Box` zu bearbeiten und zu verwalten, und bietet gleichzeitig Benutzerfreundlichkeit und Flexibilität.

| | |
|--|--|
| 🛠️ ****Einfache Konfigurationsdateibearbeitung**** | 📕 **Unterstützung von `.export`-Dateien** |
| **FRED** der Konfigurationsdatei-Editor ermöglicht es dir, die Einstellungen Ihrer `Fritz!Box` mühelos anzupassen. Ändere Netzwerkeinstellungen, Sicherheitsparameter, WLAN-Optionen und vieles mehr mit nur wenigen Mausklicks. | Unser Editor ist kompatibel mit den `Fritz!Box` `.export`-Dateien, was dir ermöglicht, diese Dateien nahtlos zu öffnen, zu bearbeiten und zu speichern. Du musst dir keine Sorgen um Konvertierungen oder Kompatibilitätsprobleme machen. |
| 🌐 **Direkter Zugriff auf Ihre Fritz!Box** | 🔐 **Sicherheit und Datenschutz** |
| Verbinde dich direkt mit deiner `Fritz!Box` über das Netzwerk, ohne komplizierte manuelle Eingriffe. **FRED** ermöglicht es dir, auf deiner eigenen `Fritz!Box` zuzugreifen und die Konfigurationsdateien ohne Umwege zu bearbeiten. | Wir nehmen die Sicherheit der Daten ernst. **FRED** bietet dir eine sichere Verbindungen zu deiner `Fritz!Box`. |
| 💝 **Benutzerfreundliche Oberfläche** | 🔄 **Aktualisierungen und Unterstützung** |
| Die Oberfläche von **FRED** ist intuitiv gestaltet, sodass sowohl Einsteiger als auch erfahrene Nutzer sich schnell zurechtfinden können. Die übersichtliche Benutzeroberfläche erleichtert die Navigation und die Bearbeitung von Konfigurationsdateien. | Die Community hält **FRED** ständig auf dem neuesten Stand und bietet über `GitHub` regelmäßige Updates und technischen Support an, um sicherzustellen, dass du stets die bestmögliche Nutzererfahrung hast. |

Der `Fritz!Box` Konfigurationsdatei-Editor ist ein unverzichtbares Werkzeug für jeden, der seine `Fritz!Box` individuell anpassen und optimieren möchte. Ob du deine Netzwerkeinstellungen anpassen, Sicherheitsparameter aktualisieren oder einfach nur den WLAN-Setup verfeinern möchtest, **FRED** macht es dir leicht. Erhalte die volle Kontrolle über deine `Fritz!Box` mit dem benutzerfreundlichen, leistungsstarken und sicheren Konfigurationsdatei-Editor.

## 💡 Warum heißt der Editor "FRED"?
Eigentlich war der Name schnell gefunden. Sinngemäß hat sich der Name aus zwei Wörtern ergeben.

Der Name **FRED** setzt sich zusammen aus **FR**itz!Box und **ED**itor. Cool, oder?

## 💥 Aktuelle ToDo-Liste
Wenn du wissen möchtest, woran aktuell gearbeitet wird, schaue doch mal in der [ToDo-Liste](https://github.com/orgs/FritzTools/projects/1) vorbei!

## 🐞 Fehler/Bugs?
Melde Fehler bittre direkt bei den [Issues](https://github.com/FritzTools/FRED/issues/new)!

Eine komplette Liste aller aktuellen Bugs findest du [hier](https://github.com/FritzTools/FRED/issues).

## 🎨 Screenshots
Du kannst [hier](https://github.com/FritzTools/FRED/blob/main/Screenshots/Readme.md) alle Screenshots der Software einsehen.

## 📃 Disclaimer
Der Name `Fritz!Box`, das dazugehörige Logo und der Name `AVM` sind eingetragene Warenmarken der **AVM Computersysteme Vertriebs GmbH** ([https://avm.de](https://avm.de)). Weder arbeiten die `Contributor` bei AVM, noch vertreten sie diese. Dieses Projekt ist ein OpenSource-Projekt und unter [MIT-Lizenz](https://github.com/FritzTools/FRED/blob/main/LICENSE) gestellt.

## 📚 Depencies
**FRED** verwendet einige Drittanbieter-Bibliotheken. Nachfolgend findest du hier eine Liste der verwendeten Bibliotheken:
| Name | Version | Beschreibung |
|--|--|--|
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | `13.0.3` | Diese Bibliothek wird benutzt um `JSON`-Dateien zu handhaben. Diese werden bei **FRED** für die eigenen Konfigurationsdateien verwendet. |
| [AvalonEdit](http://www.avalonedit.net) | `6.3.0.90` | Da ein Text-Editor mit Syntaxhighlightning sehr komplex ist, verwendet **FRED** den bekanntesten Editor für `C#`/`WPF`-Projekte. |
| [WPFHexaEditor](https://github.com/abbaye/WpfHexEditorControl) | `2.1.7` | F**FRED**RED Verwendet bei binären Daten einen extra HEX-Editor, damit auch diese bearbeitet werden können. |
| [Rssdp](https://github.com/Yortw/RSSDP) | `4.0.4` | Diese Bibliothek stellt das `Simple Service Discovery (SSDP)`-Protokoll bereit, damit **FRED** automatisch eine im Netzwerk registrierte `Fritz!Box` finden kann. |
| [Constura.Fody](https://github.com/Fody/Costura) | `5.8.0-alpha0098` | Diese Bibliothek dient ausschließlich der Bereitstellung. Wenn **FRED** als Software kompiliert wird, packt `Fody` alle Abhängigkeiten und Bibliotheken zusammen, damit der Software-Ordner nicht "zugemüllt" wird. |
