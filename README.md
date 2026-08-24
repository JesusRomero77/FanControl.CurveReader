# FanControl.CurveReader

Logger plugin for Fan Control that records the active profile and changes in sensors and fan controls.

## Features

- Records the active Fan Control profile.
- Records the profile path.
- Lists the controls, curves and sensors used by the active profile.
- Logs changes in sensor values.
- Logs changes in fan control values.

## Requirements

- Fan Control v273
- .NET Framework 4.8

## Operation

The Fan Control interface must be open for the plugin to log sensor and fan control changes.

When the Fan Control interface is closed, Fan Control continues operating through its background service and keeps controlling the fans according to the configured curves. In this state, the plugin does not receive the updates required for logging.

When the interface is opened, control is transferred from the background service to the interface and the plugin starts receiving updates and recording changes. Closing the interface returns control to the background service and logging stops.

## Fan Control sensor

Fan Control requires the plugin to create a sensor, which will appear in the list of physical sensors available when selecting a sensor from a sensor card.

The sensor named "Logger - Curve Reader" is created only to comply with the Fan Control plugin architecture. It always reports 0°C, which does not represent the temperature of any physical component.

## Version

1.0.1
