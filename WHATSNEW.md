# What's New

## 1.0.2

- Internal rework of the log purge logic: rewriting the entire log file on every new record (once the 100 MB limit was reached) has been replaced with a purge margin, reducing unnecessary disk writes.

## 1.0.1

- Added a 100 MB limit to the log file. Older records are removed automatically when necessary to make room for new entries.
- Added documentation in Readme.md file explaining that the Fan Control interface must be open for the plugin to receive updates and record changes.

## 1.0.0

- Initial release.
