# Inventory Management App

A cross-platform .NET MAUI inventory application for Windows and Android.

## Features
- Add and manage products
- Track stock quantity and price
- Search products
- Local SQLite storage
- Automatic update check against the latest GitHub Release
- Versioned Windows installer EXE and Android APK release assets

## Version format
Use Semantic Versioning tags: `vMAJOR.MINOR.PATCH`, for example `v1.0.0`, `v1.0.1`, or `v1.1.0`. Every version tag triggers Windows and Android builds and publishes the installer and APK as separate assets in a GitHub Release.

## Update behavior
When the app starts, it checks the latest GitHub Release. If a newer version exists and the matching platform asset is available, the user is prompted to download it. Windows launches the EXE installer. Android opens the downloaded APK in the system package installer; Android normally requires user confirmation and the same APK signing certificate across versions.

## Important Android signing requirement
Before distributing updates to Android users, configure a stable release keystore in GitHub Actions secrets and use it for every APK. A different signing key will prevent Android from installing an update over an existing installation. Normal Android apps cannot silently install APK updates without device-owner/managed-device privileges or a store-managed update channel.
