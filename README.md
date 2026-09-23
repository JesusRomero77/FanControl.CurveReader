# FanControl.CurveReader

Logger plugin for Fan Control that records the active profile and changes in sensors and fan controls, with an optional companion app to choose what is recorded and how often.

## Features

- Records the active Fan Control profile.
- Records the profile path.
- Lists the controls, curves and sensors used by the active profile.
- Logs changes in sensor values.
- Logs changes in fan control values.
- Optional per-profile configuration: choose which sensors and fan controls are logged, how often each one is checked and how much its value must change before it is recorded.

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
- **Threshold**: minimum change (1 to 10) from the last recorded value required to record a new entry.

Once the interval has passed, the element is checked every second and a new entry is written as soon as its value differs from the last recorded one by at least the threshold. The first reading of each element is always recorded.

Settings are saved per profile in `CurveReaderLoggerConfig.xml`, located in `%LocalAppData%\FanControl`. The plugin reads this file when it starts and whenever the active profile changes. If you save changes for the profile that is currently active, they are applied the next time that profile is loaded or Fan Control is restarted. Profiles without saved settings use the defaults: everything logged, interval 1 second, threshold 1.

Curves are listed in the profile summary, but their values are not logged.

## Companion app

CurveReaderConfigurator is an optional, portable app (a single .exe, no installation) available in English and Spanish. It requires Windows and .NET Framework 4.8. Download it from the same release as the plugin.

1. Run the app and, from its configuration menu, indicate where FanControl.exe (or a shortcut to it) is.
2. Choose a profile. The active Fan Control profile is selected by default.
3. Adjust the log option, interval and threshold of each sensor and fan control.
4. Save from the File menu.

The app only reads Fan Control's profile files and never modifies them. The app's own settings are stored in `CurveReaderConfiguratorSettings.xml`, in `%LocalAppData%\FanControl`.

## Fan Control sensor

Fan Control requires the plugin to create a sensor, which will appear in the list of physical sensors available when selecting a sensor from a sensor card.

The sensor named "Logger - Curve Reader" is created only to comply with the Fan Control plugin architecture. It always reports 0°C, which does not represent the temperature of any physical component.

## Version

2.0
