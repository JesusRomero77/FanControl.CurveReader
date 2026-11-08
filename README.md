# FanControl.CurveReader

Logger plugin for Fan Control that records the active profile and changes in sensors and fan controls, with an optional companion app to choose what is recorded and how often.

## Features

- Records the active Fan Control profile.
- Records the profile path.
- Lists the controls, curves and sensors used by the active profile.
- Logs changes in sensor values.
- Logs changes in fan control values.
- Optional per-profile configuration: choose which sensors and fan controls are logged, how often each one is checked and how much its value must change before it is recorded.
- Optional log language: English (default) or Spanish, chosen in the companion app.

## Requirements

- Tested with Fan Control v273 and later
- .NET Framework 4.8

## Operation

The Fan Control interface must be open for the plugin to log sensor and fan control changes.

When the Fan Control interface is closed, Fan Control continues operating through its background service and keeps controlling the fans according to the configured curves. In this state, the plugin does not receive the updates required for logging.

When the interface is opened, control is transferred from the background service to the interface and the plugin starts receiving updates and recording changes. Closing the interface returns control to the background service and logging stops.

## Configuration

The plugin works on its own. Without any configuration, every sensor and fan control of the active profile is logged whenever its value changes.

To customize this behavior, use the optional companion app (see below). For each sensor and fan control of a profile you can set:

- **Log**: whether it is recorded in the log or not.
- **Interval**: seconds (1 to 3600) the element waits after being recorded before it is checked again. Useful for values that change slowly or that you do not need every second.
- **Threshold**: minimum change (0 to 10) from the last recorded value required to record a new entry. With 0, the element is recorded every time its interval passes, even if its value has not changed.

Once the interval has passed, the element is checked every second and a new entry is written as soon as its value differs from the last recorded one by at least the threshold. The first reading of each element is always recorded.

**Tip:** If you want to know how long a sensor or fan control remains at the same value, you can use the default threshold of 1 and check the time between consecutive entries for that element. This provides the same information as setting the threshold to 0, helping keep the log file smaller.

Settings are saved per profile in `CurveReaderLoggerConfig.xml`, located in `%LocalAppData%\FanControl`. The plugin reads this file when it starts and whenever the active profile changes. If you save changes for the profile that is currently active, they are applied the next time that profile is loaded or Fan Control is restarted. Profiles without saved settings use the defaults: everything logged, interval 1 second, threshold 1.

Curves are listed in the profile summary, but their values are not logged.

## Companion app

### Recommended: WatchTower

The recommended way to configure CurveReader is [WatchTower](https://github.com/JesusRomero77/FanControl.WatchTower), a portable program (a single .exe, no installation, available in English and Spanish) that configures CurveReader and the other plugins of the family (RemoteSensors and DataServer) in one place. It requires Windows and .NET Framework 4.8. **WatchTower is the configurator that will keep receiving support and new features.**

1. Download `WatchTower.exe` from the WatchTower Releases page and run it.
2. In its Settings menu, use "Specify FanControl path" and indicate where FanControl.exe (or a shortcut to it) is.
3. Open the CurveReader tab and choose a profile. The active Fan Control profile is selected by default.
4. Adjust the log option, interval and threshold of each sensor and fan control.
5. Save from the File menu.

### CurveReaderConfigurator

CurveReaderConfigurator is the original companion app. It only configures CurveReader and it is still included in this release for those who prefer a small app dedicated to this plugin. It receives no new features and may be removed in a future version, so new users should choose WatchTower. It requires Windows and .NET Framework 4.8 and is available in English and Spanish. Download it from the same release as the plugin.

Both apps save the same `CurveReaderLoggerConfig.xml`, so you can use either of them. The steps are the same: indicate where FanControl.exe is, choose a profile, adjust each sensor and fan control, and save from the File menu.

Neither app modifies Fan Control's profile files; they only read them. CurveReaderConfigurator's own settings are stored in `CurveReaderConfiguratorSettings.xml`, in `%LocalAppData%\FanControl`.

The language chosen in either app is also saved in `CurveReaderLoggerConfig.xml` and used by the plugin to write the log. Without an app, the log is written in English.

## Fan Control sensor

Fan Control requires the plugin to create a sensor, which will appear in the list of physical sensors available when selecting a sensor from a sensor card.

The sensor named "Logger - Curve Reader" is created only to comply with the Fan Control plugin architecture. It always reports 0°C, which does not represent the temperature of any physical component.

## License

This project is licensed under the [MIT License](LICENSE).

CurveReaderConfigurator embeds [Newtonsoft.Json](https://www.newtonsoft.com/json) (MIT). See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for details. Fan Control and the libraries it provides to plugins are not part of this repository and remain under their own licenses. This project is not affiliated with Fan Control.

## Version

2.3
