using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Looxid.Link
{
    public class LinkDataValue
    {
        public double value;
        public double target;
        public double min;
        public double max;

        public LinkDataValue()
        {
            this.value = 0.0;
            this.target = 0.0;
            this.min = 0.0;
            this.max = 0.0;
        }

        public void SetScale(List<double> list)
        {
            this.min = Min(list);
            this.max = Max(list);
        }

        public double Min(List<double> minList)
        {
            if (minList.Count <= 0) return 0.0;
            double min = minList[0];
            for (int i = 0; i < minList.Count; i++)
            {
                if (!double.IsNaN(minList[i]))
                {
                    if (minList[i] < min) min = minList[i];
                }
            }
            if (double.IsNaN(min)) return 0.0;
            else return min;
        }
        public double Max(List<double> maxList)
        {
            if (maxList.Count <= 0) return 0.0;
            double max = maxList[0];
            for (int i = 0; i < maxList.Count; i++)
            {
                if (!double.IsNaN(maxList[i]))
                {
                    if (maxList[i] > max) max = maxList[i];
                }
            }
            if (double.IsNaN(max)) return 0.0;
            else return max;
        }
    }

    public class LooxidLinkData : MonoBehaviour
    {
        #region Singleton

        private static LooxidLinkData _instance;
        public static LooxidLinkData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(LooxidLinkData)) as LooxidLinkData;
                    if (_instance == null)
                    {
                        _instance = new GameObject("LooxidLinkData").AddComponent<LooxidLinkData>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        #endregion

        private const int MAX_SAMPLES = 100;

        public static System.Action<MindIndex> OnReceiveMindIndexes;
        public static System.Action<EEGFeatureIndex> OnReceiveEEGFeatureIndexes;
        public static System.Action<EEGRawSignal> OnReceiveEEGRawSignals;
        public static System.Action<EEGSensor> OnReceiveEEGSensorStatus;

        private List<MindIndex> mindIndexList;
        private List<EEGFeatureIndex> eegFeatureIndexList;
        private List<EEGSensor> eegSensorStatusList;

        public void Initialize()
        {
        }

        void OnEnable()
        {
            mindIndexList = new List<MindIndex>();
            eegFeatureIndexList = new List<EEGFeatureIndex>();
            eegSensorStatusList = new List<EEGSensor>();

            NetworkManager.OnNetworkReceiveMindIndexes += OnNetworkReceiveMindIndexes;
            NetworkManager.OnNetworkReceiveEEGFeatureIndexes += OnNetworkReceiveEEGFeatureIndexes;
            NetworkManager.OnNetworkReceiveEEGRawSignals += OnNetworkReceiveEEGRawSignals;
            NetworkManager.OnNetworkReceiveEEGSensorStatus += OnNetworkReceiveEEGSensorStatus;
        }


        #region Mind Indexes

        void OnNetworkReceiveMindIndexes(MindIndex mindIndex)
        {
            if (OnReceiveMindIndexes != null)
            {
                OnReceiveMindIndexes.Invoke(mindIndex);
            }

            mindIndexList.Insert(0, mindIndex);
            if (mindIndexList.Count > MAX_SAMPLES)
            {
                mindIndexList.RemoveAt(mindIndexList.Count - 1);
            }
        }

        public List<MindIndex> GetMindIndexData(float seconds)
        {
            double nowTime = LooxidLinkUtility.GetUTCTimestamp();
            List<MindIndex> recentMindIndexList = new List<MindIndex>();
            for (int i = 0; i < mindIndexList.Count; i++)
            {
                if (nowTime - mindIndexList[i].timestamp <= seconds)
                {
                    recentMindIndexList.Add(mindIndexList[i]);
                }
            }
            return recentMindIndexList;
        }

        #endregion


        #region EEG Feature Indexes

        void OnNetworkReceiveEEGFeatureIndexes(EEGFeatureIndex eegFeatureIndex)
        {
            if (OnReceiveEEGFeatureIndexes != null)
            {
                OnReceiveEEGFeatureIndexes.Invoke(eegFeatureIndex);
            }

            eegFeatureIndexList.Insert(0, eegFeatureIndex);
            if (eegFeatureIndexList.Count > MAX_SAMPLES)
            {
                eegFeatureIndexList.RemoveAt(eegFeatureIndexList.Count - 1);
            }
        }

        public List<EEGFeatureIndex> GetEEGFeatureIndexData(float seconds)
        {
            double nowTime = LooxidLinkUtility.GetUTCTimestamp();
            List<EEGFeatureIndex> recentEEGFeatureIndexList = new List<EEGFeatureIndex>();
            for (int i = 0; i < eegFeatureIndexList.Count; i++)
            {
                if (nowTime - eegFeatureIndexList[i].timestamp <= seconds)
                {
                    recentEEGFeatureIndexList.Add(eegFeatureIndexList[i]);
                }
            }
            return recentEEGFeatureIndexList;
        }

        #endregion


        #region EEG Raw Signal

        void OnNetworkReceiveEEGRawSignals(EEGRawSignal eegRawSignals)
        {
            if (OnReceiveEEGRawSignals != null)
            {
                OnReceiveEEGRawSignals.Invoke(eegRawSignals);
            }
        }

        #endregion


        #region EEG Sensor Staus

        void OnNetworkReceiveEEGSensorStatus(EEGSensor eegSensors)
        {
            if (OnReceiveEEGSensorStatus != null)
            {
                OnReceiveEEGSensorStatus.Invoke(eegSensors);
            }
        }

        public List<EEGSensor> GetEEGSensorStatus(float seconds)
        {
            double nowTime = LooxidLinkUtility.GetUTCTimestamp();
            List<EEGSensor> recentEEGSensorStatusList = new List<EEGSensor>();
            for (int i = 0; i < eegSensorStatusList.Count; i++)
            {
                if (nowTime - eegSensorStatusList[i].timestamp <= seconds)
                {
                    recentEEGSensorStatusList.Add(eegSensorStatusList[i]);
                }
            }
            return recentEEGSensorStatusList;
        }

        #endregion
    }
}