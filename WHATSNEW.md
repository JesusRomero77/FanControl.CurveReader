# What's New

## 2.2

- The language chosen in CurveReaderConfigurator is now also used by the plugin to write `CurveReader.log`: the profile summary headings, the `Name` / `Value` labels and the element types (`SENSOR`, `CONTROL`) of each record. In Spanish they appear as `Nombre`, `Valor`, and so on.
- The chosen language is saved in `CurveReaderLoggerConfig.xml` as a new optional `Language` attribute. The rest of the format does not change, and files saved by earlier versions keep working. The plugin reads it when it starts and whenever the active profile changes.
- The log is now written in English by default. Before, the profile summary and some messages were always in Spanish. Without the app, or without a saved language, everything is in English; choose Spanish in the app to get it in Spanish.
- Messages the plugin sends to FanControl's own log (such as "Unknown origin") also follow the chosen language.
- The language menu of CurveReaderConfigurator is now built from the available languages, so adding a new language only requires adding a new class to the `Language.cs` file in the code of both the configurator and the plugin, and then recompiling the configurator and the plugin.
- The language saved by earlier versions of the app is not carried over: choose it again in Settings > Language.

## 2.1.1

- Internal code improvements. Slightly less work is done every second.
- Nothing changes in what is logged or in how the plugin is configured.
- CurveReaderConfigurator has no changes; it only receives the new version number.

## 2.1

- Plugin errors are now written to FanControl's own log instead of `CurveReader.log`. Each message includes the class and method where the error happened, so it can be told apart from the messages of FanControl and other plugins. `CurveReader.log` now only contains the profile summaries and the logged values.
- The plugin now checks the modification date of FanControl's `CACHE` file and only reads it again when it has changed, reducing the work done every second.
- Internal reorganization of the code. What is logged and the format of `CurveReaderLoggerConfig.xml` do not change.
- CurveReaderConfigurator has no changes; it only receives the new version number.

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
