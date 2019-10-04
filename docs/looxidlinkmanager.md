# LooxidLinkManager

LooxidLinkManager can be called from anywhere as it's a singleton object, and can help you connect to the Link Hub.
By calling the `Initialize()` function, you will be ready to receive the biometric data.

```csharp
void Start()
{
    bool isIntialized = LooxidLinkManager.Instance.Initialize();
}
```

In the Link SDK you can check if the Link Core and the Link Hub are connected properly.

If the Link Core is installed, it will run automatically when the `Initialize()` function is called. However, if the Link Core is not installed or if there is a problem with the file, the SDK will not function properly.

The Link SDK includes the Action delegate and boolean variables for when the connection with the Link Hub is loss or for when unexpected errors occur while the VR application is running.

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
    Debug.Log(“Link Core is connected.”);
}
void OnLinkCoreDisconnected()
{
    Debug.Log(“Link Core is disconnected.”);
}
void OnLinkHubConnected()
{
    Debug.Log(“Link Hub is connected.”);
}
void OnLinkHubDisconnected()
{
    Debug.Log(“Link Hub is disconnected.”);
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
| bool | `Initialize()`<br>Initializes the Link SDK and prepares it to receive data. It returns `True` when the initialization is successful. |
| void | `SetDebug(bool isDebug)`<br>Determines whether if the Link SDK related debugging is necessary in the Console window. |

## LooxidLinkManager: Field Summary

| Field | Type | Description |
|---|---|---|
| OnLinkCoreConnection | Action | A called delegate when the Link Core is connected properly |
| OnLinkCoreDisconnection | Action | A called delegate when connection to the Link Core is terminated |
| OnLinkHubConnection | Action | A called delegate when the Link Hub is connected properly |
| OnLinkHubDisconnection | Action | A called delegate when connection to the Link Hub is terminated |
| isLinkCoreConnected | bool | A return that determines if the Link Core is connected |
| isLinkHubConnected | bool | A return that determines if the Link Hub is connected |
