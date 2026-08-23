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

## Fan Control sensor

Fan Control requires the plugin to create a sensor, which will appear in the list of physical sensors available when selecting a sensor from a sensor card.

The sensor is created only to comply with the Fan Control plugin architecture and is not used to provide any actual sensor data.

## Version

1.0
