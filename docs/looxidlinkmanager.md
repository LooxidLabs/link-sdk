# LooxidLinkManager

LooxidLinkManager can be called from anywhere as it's a singleton object, and can help you connect to the Link Hub.
By calling the `Initialize()` function, you will be ready to receive the biometric data.

```csharp
void Start()
{
    bool isIntialized = LooxidLinkManager.Instance.Initialize();
}
```

In the Link SDK, you can check if the Link Core and the Link Hub are connected correctly.

If the Link Core is installed, it will run automatically when the `Initialize()` function is called. However, if the Link Core is not installed or if there is a problem with the file, the SDK will not function properly.

The Link SDK includes the action delegate and boolean variables for when the connection with the Link Hub is lost or for when unexpected errors occur while the VR application is running.

```csharp
// Action
void Start()
{
    LooxidLinkManager.OnLinkCoreConnected += OnLinkCoreConnected;
    LooxidLinkManager.OnLinkCoreDisconnected += OnLinkCoreDisconnected;
    LooxidLinkManager.OnLinkHubConnected += OnLinkHubConnected;
    LooxidLinkManager.OnLinkHubDisconnected += OnLinkHubDisconnected;
}

void OnLinkCoreConnected()
{
    Debug.Log("Link Core is connected.");
}
void OnLinkCoreDisconnected()
{
    Debug.Log("Link Core is disconnected.");
}
void OnLinkHubConnected()
{
    Debug.Log("Link Hub is connected.");
}
void OnLinkHubDisconnected()
{
    Debug.Log("Link Hub is disconnected.");
}

// Boolean
void Update()
{
    if( !LooxidLinkManager.isLinkCoreConnected ) return;
    if( !LooxidLinkManager.isLinkHubConnected ) return;
}
```

## LooxidLinkManager: Method Summary

| Type | Method and Description |
|------|---|
| bool | `Initialize()`<br>Initializes the Link SDK and prepares it to receive data. Returns `True` when the initialization is successful. |
| void | `SetDebug(bool isDebug)`<br>Determines whether to print the Link SDK related debug messages in the console window or not. |
| void | `SetDisplayDisconnectedMessage​(​bool​ isDisplay)`<br>Determines whether to show the default message provided with the SDK saying the Link is disconnected. |
| void | `SetDisplaySensorOffMessage​(​bool​ isDisplay)`<br>Determines whether to show the default message provided with the SDK saying the sensors are not properly attached to the user's forehead. |
| void | `SetDisplayNoiseSignalMessage​(​bool​ isDisplay)`<br>Determines whether to show the default message provided with the SDK saying the input signals are noisy. |

## LooxidLinkManager: Field Summary

| Field | Type | Description |
|---|---|---|
| OnLinkCoreConnection | Action | A delegate that is called when the Link Core is connected properly. |
| OnLinkCoreDisconnection | Action | A delegate that is called when connection to the Link Core is terminated. |
| OnLinkHubConnection | Action | A delegate that is called when the Link Hub is connected properly. |
| OnLinkHubDisconnection | Action | A delegate that is called when connection to the Link Hub is terminated. |
| isLinkCoreConnected | bool | Returns if the Link Core is connected. |
| isLinkHubConnected | bool | Returns if the Link Hub is connected. |
| OnShowSensorOffMessage | void | A delegate that is called when the 'sensor disconnected' message appears. |
| OnHideSensorOffMessage | void | A delegate that is called when the 'sensor disconnected' message disappears. |
| OnShowNoiseSignalMessage | void | A delegate that is called when the 'noise detected' message appears. |
| OnHideNoiseSignalMessage | void | A delegate that is called when the 'noise detected' message disappears. |
