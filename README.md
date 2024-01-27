# ScribbleHunter using MonoGame

A cross-platform port of the Windows Phone / Windows 8 scribbled space action game using MonoGame.

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

## Troubleshooting

### Deployment to Android device stops working

In case the deployment using Visual Studio for Mac stops working with `Mono.AndroidTools.AdbException: Attempted to read past the end of the stream.`,
then either restart the Visual Studio IDE, or kill all related `dotnet` processes.