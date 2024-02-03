# ScribbleHunter using MonoGame ![GitHub](https://img.shields.io/github/license/b3nk4n/scribblehunter-monogame)

A cross-platform port of the [Windows Phone / Windows 8 scribbled space action game](https://github.com/b3nk4n/scribblehunter-game) using MonoGame.

<p align="center">
    <img alt="App Logo" src="assets/play_store_512.png">
</p>

## Getting Started

Restore the .Net tools using the following command:

```
dotnet tool restore
```

### IDE Setup

When using Visual Studio for Mac, I needed to disable **Fast Assembly Deployment** when deploying the App on a physical Android device (Pixel 4).
This option can be disabled via _Project Properties > Android > Build > Packaging and Deployment_. 
Otherwise, the app was just crashing right at startup most of the time when using the default settings.


## References

App icons have been generated using https://icon.kitchen/.

## App Publishing

In Visual Studio for Mac, do the following:

1. Import the singing key via _Tools > Preferences_ and _Publishing > Android Signing Keys_
2. Create an AAB file for publishing by right clicking the project file and selecting _Archive for Publishing_

There is also the possibility to include the signing information in the `.csproj` file. However, only plain text worked for me,
which is neither secure nor recommended. And the alternative options to use `env:` or `file:` as described in
[MSDN](https://learn.microsoft.com/en-us/xamarin/android/deploy-test/building-apps/build-properties#androidsigningkeypass)
did not work somehow.

## Troubleshooting

### Deployment to Android device stops working

In case the deployment using Visual Studio for Mac stops working with `Mono.AndroidTools.AdbException: Attempted to read past the end of the stream.`,
then either restart the Visual Studio IDE, or kill all related `dotnet` processes.

### The archive for publishing contains not the updated `versionName` and `versionCode`

This somehow happens for whatever reason, even after updating it in both the Manifest and the project settings.
One workaround might be to manually delete the `obj` and `bin` folders, and rebuild the project.
In one case, I then ended up with a build error in Release mode only, related to `Xamarin.Essentials`,
and that an [xamarin_essentials_fileprovider_file_paths.xml](https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/Resources/xml/xamarin_essentials_fileprovider_file_paths.xml)
file is missing. This also seems like a flaky bug, which I resolved by removing and re-adding the `Xamarin.Essentials` package.
