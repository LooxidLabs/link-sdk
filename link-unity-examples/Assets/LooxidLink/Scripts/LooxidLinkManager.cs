/**
 * @author   Looxidlabs
 * @version  1.0
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Looxid.Link
{
    public class LooxidLinkManager : MonoBehaviour
    {
        #region Singleton

        private static LooxidLinkManager _instance;
        public static LooxidLinkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(LooxidLinkManager)) as LooxidLinkManager;
                    if (_instance == null)
                    {
                        _instance = new GameObject("LooxidLinkManager").AddComponent<LooxidLinkManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        #endregion



        #region Variables

        private NetworkManager networkManager;

        private LinkCoreStatus linkCoreStatus = LinkCoreStatus.Disconnected;
        private LinkHubStatus linkHubStatus = LinkHubStatus.Disconnected;

        private bool isInitialized = false;

        [HideInInspector]
        public bool isLinkCoreConnected = false;
        [HideInInspector]
        public bool isLinkHubConnected = false;

        public static System.Action OnLinkCoreConnected;
        public static System.Action OnLinkCoreDisconnected;
        public static System.Action OnLinkHubConnected;
        public static System.Action OnLinkHubDisconnected;

        private LooxidLinkMessage messageUI;

        public EEGSensor sensorStatusData;
        private List<int> sensorScoreList;
        private List<int> noiseScoreList;

        private bool isDisplayDisconnectedMessage = true;
        private bool isDisplaySensorOffMessage = true;
        private bool isDisplayNoiseSignalMessage = true;

        private bool sensorOffMessage = false;
        private bool noiseSignalMessage = false;

        public static System.Action OnShowSensorOffMessage;
        public static System.Action OnHideSensorOffMessage;
        public static System.Action OnShowNoiseSignalMessage;
        public static System.Action OnHideNoiseSignalMessage;

        // Colors
        public static Color32 linkColor = new Color32(124, 64, 254, 255);

        #endregion


        #region MonoBehavior Life Cycle

        void OnEnable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus += OnReceivedEEGSensorStatus;
            NetworkManager.OnLinkCoreConnected += OnNetworkLinkCoreConnected;
            NetworkManager.OnLinkCoreDisconnected += OnNetworkLinkCoreDisconnected;
            NetworkManager.OnLinkHubConnected += OnNetworkLinkHubConnected;
            NetworkManager.OnLinkHubDisconnected += OnNetworkLinkHubDisconnected;

            sensorScoreList = new List<int>();
            noiseScoreList = new List<int>();

            if (isInitialized)
            {
                StartCoroutine(DetectSensor());

                if (linkCoreStatus == LinkCoreStatus.Disconnected)
                {
                    StartCoroutine(AutoConnection());
                }
            }
        }
        void OnDisable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceivedEEGSensorStatus;
            NetworkManager.OnLinkCoreConnected -= OnNetworkLinkCoreConnected;
            NetworkManager.OnLinkCoreDisconnected -= OnNetworkLinkCoreDisconnected;
            NetworkManager.OnLinkHubConnected -= OnNetworkLinkHubConnected;
            NetworkManager.OnLinkHubDisconnected -= OnNetworkLinkHubDisconnected;
        }

        void OnApplicationQuit()
        {
            isInitialized = false;
        }

        void OnNetworkLinkCoreConnected()
        {
            if (OnLinkCoreConnected != null) OnLinkCoreConnected.Invoke();
        }
        void OnNetworkLinkCoreDisconnected()
        {
            if (OnLinkCoreDisconnected != null) OnLinkCoreDisconnected.Invoke();
        }
        void OnNetworkLinkHubConnected()
        {
            if (OnLinkHubConnected != null) OnLinkHubConnected.Invoke();
        }
        void OnNetworkLinkHubDisconnected()
        {
            if (OnLinkHubDisconnected != null) OnLinkHubDisconnected.Invoke();
        }

        void OnReceivedEEGSensorStatus(EEGSensor sensorStatusData)
        {
            this.sensorStatusData = sensorStatusData;
        }

        void Update()
        {
            if (networkManager == null) return;

            linkCoreStatus = networkManager.LinkCoreStatus;
            isLinkCoreConnected = (linkCoreStatus == LinkCoreStatus.Connected);

            linkHubStatus = networkManager.LinkHubStatus;
            isLinkHubConnected = (linkHubStatus == LinkHubStatus.Connected);
        }

        IEnumerator DetectSensor()
        {
            while (this.gameObject.activeSelf && isInitialized)
            {
                if (isLinkHubConnected && isLinkCoreConnected && sensorStatusData != null)
                {
                    // Sensor Connection
                    int sensorScore = 0;
                    int numSensor = System.Enum.GetValues(typeof(EEGSensorID)).Length;
                    for (int i = 0; i < numSensor; i++)
                    {
                        int score = sensorStatusData.IsSensorOn((EEGSensorID)i) ? 1 : 0;
                        sensorScore += score;
                    }
                    sensorScoreList.Add(sensorScore);
                    if (sensorScoreList.Count > 20) sensorScoreList.RemoveAt(0);
                    if (sensorScoreList.Count >= 20 && sensorScoreList.Sum() < 110)
                    {
                        ShowMessage(LooxidLinkMessageType.SensorOff);
                    }
                    else
                    {
                        HideMessage(LooxidLinkMessageType.SensorOff);
                    }

                    // Noise Signal
                    int noiseScore = 0;
                    for (int i = 0; i < numSensor; i++)
                    {
                        int score = sensorStatusData.IsNoiseSignal((EEGSensorID)i) ? 0 : 1;
                        noiseScore += score;
                    }
                    noiseScoreList.Add(noiseScore);
                    if (noiseScoreList.Count > 20) noiseScoreList.RemoveAt(0);
                    if (noiseScoreList.Count >= 20 && noiseScoreList.Sum() < 110)
                    {
                        ShowMessage(LooxidLinkMessageType.NoiseSignal);
                    }
                    else
                    {
                        HideMessage(LooxidLinkMessageType.NoiseSignal);
                    }
                }
                else
                {
                    HideMessage(LooxidLinkMessageType.SensorOff);
                    HideMessage(LooxidLinkMessageType.NoiseSignal);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion



        #region Initialize

        public bool Initialize()
        {
            if (isInitialized) return true;

            if (networkManager == null)
            {
                networkManager = gameObject.AddComponent<NetworkManager>();
                networkManager.CreateUserData();
            }
            LooxidLinkData.Instance.Initialize();

            isInitialized = networkManager.Initialize();
            if (isInitialized)
            {
                StartCoroutine(AutoConnection());
                StartCoroutine(DetectSensor());
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Terminate()
        {
            isInitialized = false;
            if (messageUI != null) {
                if (messageUI.gameObject != null)
                {
                    Destroy(messageUI.gameObject);
                }
                messageUI = null;
            }
            networkManager.DisconnectMessage();
        }

        #endregion



        #region Connect & Disconnect

        IEnumerator AutoConnection()
        {
            while (true)
            {
                if (isInitialized && linkCoreStatus == LinkCoreStatus.Disconnected)
                {
                    networkManager.ClearNetwork();
                    networkManager.Connect(OnLinkCoreStatus, OnOnLinkHubStatus);
                }
                yield return new WaitForSeconds(3.0f);
            }
        }

        void OnLinkCoreStatus(LinkCoreStatus coreStatus)
        {
            if (!isInitialized) return;

            if (coreStatus == LinkCoreStatus.Connected)
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
            else if (coreStatus == LinkCoreStatus.Disconnected)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
                HideMessage(LooxidLinkMessageType.NoiseSignal);
                ShowMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }
        void OnOnLinkHubStatus(LinkHubStatus hubStatus)
        {
            if (!isInitialized) return;

            if (hubStatus == LinkHubStatus.Connected)
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
            else if (hubStatus == LinkHubStatus.Disconnected)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
                HideMessage(LooxidLinkMessageType.NoiseSignal);
                ShowMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }

        #endregion


        #region Settings

        public void SetDebug(bool isDebug)
        {
            LXDebug.isDebug = isDebug;
        }

        public void SetDisplayDisconnectedMessage(bool isDisplay)
        {
            this.isDisplayDisconnectedMessage = isDisplay;

            if( !isDisplay )
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }

        public void SetDisplaySensorOffMessage(bool isDisplay)
        {
            this.isDisplaySensorOffMessage = isDisplay;

            if (!isDisplay)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
            }
        }

        public void SetDisplayNoiseSignalMessage(bool isDisplay)
        {
            this.isDisplayNoiseSignalMessage = isDisplay;

            if (!isDisplay)
            {
                HideMessage(LooxidLinkMessageType.NoiseSignal);
            }
        }

        #endregion


        #region Link Message

        private void InstantiateMessageUI()
        {
            GameObject messageObj = Resources.Load("Prefabs/LooxidLinkMessage") as GameObject;
            if (messageObj != null)
            {
                if (Camera.main == null)
                {
                    Camera mainCamera = GameObject.FindObjectOfType<Camera>();
                    if (mainCamera != null)
                    {
                        GameObject newMessageObj = Instantiate(messageObj, mainCamera.transform) as GameObject;
                        newMessageObj.name = "LooxidLinkMessage";
                        messageUI = newMessageObj.GetComponent<LooxidLinkMessage>();
                    }
                }
                else
                {
                    GameObject newMessageObj = Instantiate(messageObj, Camera.main.transform) as GameObject;
                    newMessageObj.name = "LooxidLinkMessage";
                    messageUI = newMessageObj.GetComponent<LooxidLinkMessage>();
                }
            }
        }

        private void ShowMessage(LooxidLinkMessageType messageType)
        {
            if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (!noiseSignalMessage)
                {
                    networkManager.WriteLog("INFO", "Show MessageUI: Noise Signal");
                    if (OnShowNoiseSignalMessage != null) OnShowNoiseSignalMessage.Invoke();
                }
                noiseSignalMessage = true;
            }
            if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (!sensorOffMessage)
                {
                    networkManager.WriteLog("INFO", "Show MessageUI: Sensor Off");
                    if (OnShowSensorOffMessage != null) OnShowSensorOffMessage.Invoke();
                }
                sensorOffMessage = true;
            }

            if (messageType == LooxidLinkMessageType.CoreDisconnected || messageType == LooxidLinkMessageType.HubDisconnected)
            {
                if (this.isDisplayDisconnectedMessage)
                {
                    if(messageUI == null)
                    {
                        InstantiateMessageUI();
                    }
                    if(messageUI != null) messageUI.ShowMessage(messageType);
                }
            }
            else if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (this.isDisplaySensorOffMessage)
                {
                    if (messageUI == null)
                    {
                        InstantiateMessageUI();
                    }
                    if (messageUI != null) messageUI.ShowMessage(messageType);
                }
            }
            else if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (this.isDisplayNoiseSignalMessage)
                {
                    if (messageUI == null)
                    {
                        InstantiateMessageUI();
                    }
                    if (messageUI != null) messageUI.ShowMessage(messageType);
                }
            }
        }

        private void HideMessage(LooxidLinkMessageType messageType)
        {
            if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (noiseSignalMessage)
                {
                    networkManager.WriteLog("INFO", "Hide MessageUI: Noise Signal");
                    if (OnHideNoiseSignalMessage != null) OnHideNoiseSignalMessage.Invoke();
                }
                noiseSignalMessage = false;
            }
            if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (sensorOffMessage)
                {
                    networkManager.WriteLog("INFO", "Hide MessageUI: Sensor Off");
                    if (OnHideSensorOffMessage != null) OnHideSensorOffMessage.Invoke();
                }
                sensorOffMessage = false;
            }

            if (messageUI != null)
            {
                messageUI.HideMessage(messageType);
            }
        }

        #endregion
    }
}