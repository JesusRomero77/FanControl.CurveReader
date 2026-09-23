# What's New

## 2.0

- New optional companion app, CurveReaderConfigurator (portable, English and Spanish), to configure the logger per profile. For each sensor and fan control you can choose whether it is logged, the seconds between records (1 to 3600) and the change threshold (1 to 10).
- The plugin now reads the configuration saved by the app (`CurveReaderLoggerConfig.xml` in `%LocalAppData%\FanControl`). Settings are loaded when the plugin starts and whenever the active profile changes.
- The plugin still works on its own: without the app, or for profiles without saved settings, everything is logged as before (interval 1 second, threshold 1).
- The first reading of each sensor and fan control is now always recorded, even if its value is 0.
- Fixed an issue where an unusual profile file could cause the plugin to ignore the whole profile.

## 1.1

- Changes to the record purge logic.
- Fixed issues that could cause the plugin to start incorrectly.
- Added a purge margin when the 100 MB file size limit is reached, to reduce unnecessary disk writes.

## 1.0.1

- Added a 100 MB limit to the log file. Older records are removed automatically when necessary to make room for new entries.
- Added documentation in Readme.md file explaining that the Fan Control interface must be open for the plugin to receive updates and record changes.

## 1.0.0

- Initial release.
