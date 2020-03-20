using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    public enum Tab3DVisualizer
    {
        MIND_INDEX = 0,
        FEATURE_INDEX = 1,
        RAW_SIGNAL = 2
    }

    public enum FeatureIndexEnum
    {
        DELTA = 0,
        THETA = 1,
        ALPHA = 2,
        BETA = 3,
        GAMMA = 4
    }

    public class _3DVisualizer : MonoBehaviour
    {
        [Header("Colors")]
        public Color ConnectedColor = Color.white;
        public Color DisconnectedColor = Color.white;

        [Header("Device Status")]
        public Transform VRCamera;
        public Text[] ConnectedText;
        public Image[] ConnectedImage;
        public Sprite[] ConnectedSprite;
        public Image[] SensorStatusImage;
        public Text[] SensorStatusText;
        public Text HeadTrackingText;
        public Transform HeadModeling;
        public Transform HeadPositionCircle;

        [Header("Mind Index")]
        public LineChart LeftActivityChart;
        public LineChart RightActivityChart;
        public BarIndicator LeftActivityIndicator;
        public BarIndicator RightActivityIndicator;
        public BarIndicator AttentionIndicator;
        public BarIndicator RelaxationIndicator;
        public LineChart AttentionChart;
        public LineChart RelaxationChart;
        public Text AsymmetryText;

        [Header("Raw Signals")]
        public LineChart[] RawSignalCharts;

        [Header("Feature Indexes")]
        public EEGSensorID SelectFeatureSensorID;
        public BarChart[] FeatureIndexCharts;
        public GameObject[] FeatureChannelTab;

        private const int linkFequency = 5;
        private Vector3 playerPos;

        private EEGSensor sensorStatusData;
        private EEGRawSignal rawSignalData;

        private LinkDataValue leftActivity;
        private LinkDataValue rightActivity;
        private LinkDataValue attention;
        private LinkDataValue relaxation;
        private LinkDataValue asymmetry;


        void Start()
        {
            LooxidLinkManager.Instance.SetDebug(true);
            LooxidLinkManager.Instance.Initialize();

            leftActivity = new LinkDataValue();
            rightActivity = new LinkDataValue();
            attention = new LinkDataValue();
            relaxation = new LinkDataValue();
            asymmetry = new LinkDataValue();

            playerPos = VRCamera.transform.position;

            SetFeautreIndexTab(EEGSensorID.AF3);
        }

        void OnEnable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus += OnReceiveEEGSensorStatus;
            LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
            LooxidLinkData.OnReceiveMindIndexes += OnReceiveMindIndexes;

            StartCoroutine(DisplayData());
        }
        void OnDisable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceiveEEGSensorStatus;
            LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
            LooxidLinkData.OnReceiveMindIndexes -= OnReceiveMindIndexes;
        }

        // Data Subscription
        void OnReceiveEEGSensorStatus(EEGSensor sensorStatusData)
        {
            this.sensorStatusData = sensorStatusData;
        }
        void OnReceiveMindIndexes(MindIndex mindIndexData)
        {
            leftActivity.target = double.IsNaN(mindIndexData.leftActivity) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.leftActivity);
            rightActivity.target = double.IsNaN(mindIndexData.rightActivity) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.rightActivity);
            attention.target = double.IsNaN(mindIndexData.attention) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.attention);
            relaxation.target = double.IsNaN(mindIndexData.relaxation) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.relaxation);
            asymmetry.target = double.IsNaN(mindIndexData.asymmetry) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.asymmetry);
        }
        void OnReceiveEEGRawSignals(EEGRawSignal rawSignalData)
        {
            int numChannel = System.Enum.GetValues(typeof(EEGSensorID)).Length;
            for (int i = 0; i < numChannel; i++)
            {
                RawSignalCharts[i].SetValue(rawSignalData.FilteredRawSignal((EEGSensorID)i));
            }
        }


        IEnumerator DisplayData()
        {
            while (this.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(0.1f);

                // Device Status: Connected Status
                if (ConnectedText[0] != null)
                {
                    ConnectedText[0].text = (LooxidLinkManager.Instance.isLinkHubConnected) ? "Connected" : "Disconnected";
                    ConnectedText[0].color = (LooxidLinkManager.Instance.isLinkHubConnected) ? ConnectedColor : DisconnectedColor;
                }
                if (ConnectedImage[0] != null)
                {
                    ConnectedImage[0].sprite = (LooxidLinkManager.Instance.isLinkHubConnected) ? ConnectedSprite[0] : ConnectedSprite[1];
                }
                if (ConnectedText[1] != null)
                {
                    ConnectedText[1].text = (LooxidLinkManager.Instance.isLinkCoreConnected) ? "Connected" : "Disconnected";
                    ConnectedText[1].color = (LooxidLinkManager.Instance.isLinkCoreConnected) ? ConnectedColor : DisconnectedColor;
                }
                if (ConnectedImage[1] != null)
                {
                    ConnectedImage[1].sprite = (LooxidLinkManager.Instance.isLinkCoreConnected) ? ConnectedSprite[0] : ConnectedSprite[1];
                }

                // Device Status: Sensor Status
                if (sensorStatusData != null)
                {
                    int numChannel = System.Enum.GetValues(typeof(EEGSensorID)).Length;
                    for (int i = 0; i < numChannel; i++)
                    {
                        bool isSensorOn = sensorStatusData.IsSensorOn((EEGSensorID)i);
                        if (SensorStatusText[i] != null)
                        {
                            SensorStatusText[i].text = isSensorOn ? "ON" : "OFF";
                            SensorStatusText[i].color = isSensorOn ? ConnectedColor : DisconnectedColor;
                            SensorStatusImage[i].color = isSensorOn ? ConnectedColor : DisconnectedColor;
                        }
                    }
                }


                // Mind Indexes: Indicators
                LeftActivityIndicator.SetValue((float)leftActivity.value);
                RightActivityIndicator.SetValue((float)rightActivity.value);
                AttentionIndicator.SetValue((float)attention.value);
                RelaxationIndicator.SetValue((float)relaxation.value);
                AsymmetryText.text = asymmetry.value.ToString("F3");


                // Mind Indexes: Charts
                List<double> leftActivityDataList = new List<double>();
                List<double> rightActivityDataList = new List<double>();
                List<double> attentionDataList = new List<double>();
                List<double> relaxationDataList = new List<double>();
                int numIndexList = 5 * linkFequency;
                List<MindIndex> mindIndexList = LooxidLinkData.Instance.GetMindIndexData(7.0f);
                for (int i = 0; i < mindIndexList.Count; i++)
                {
                    if (i < numIndexList)
                    {
                        double mindLeftActivity = (double.IsNaN(mindIndexList[i].leftActivity)) ? 0.0f : LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexList[i].leftActivity);
                        double mindRightActivity = (double.IsNaN(mindIndexList[i].rightActivity)) ? 0.0f : LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexList[i].rightActivity);
                        double mindAttention = (double.IsNaN(mindIndexList[i].attention)) ? 0.0f : LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexList[i].attention);
                        double mindRelaxation = (double.IsNaN(mindIndexList[i].relaxation)) ? 0.0f : LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexList[i].relaxation);
                        leftActivityDataList.Add(mindLeftActivity);
                        rightActivityDataList.Add(mindRightActivity);
                        attentionDataList.Add(mindAttention);
                        relaxationDataList.Add(mindRelaxation);
                    }
                }
                leftActivityDataList.Reverse();
                rightActivityDataList.Reverse();
                attentionDataList.Reverse();
                relaxationDataList.Reverse();

                if (LeftActivityChart != null) LeftActivityChart.SetValue(leftActivityDataList.ToArray());
                if (RightActivityChart != null) RightActivityChart.SetValue(rightActivityDataList.ToArray());
                if (AttentionChart != null) AttentionChart.SetValue(attentionDataList.ToArray());
                if (RelaxationChart != null) RelaxationChart.SetValue(relaxationDataList.ToArray());

                // Feature Indexes: Charts
                int numIndexes = System.Enum.GetValues(typeof(FeatureIndexEnum)).Length;
                for (int i = 0; i < numIndexes; i++)
                {
                    FeatureIndexCharts[i].SetValue(GetFeatureDataList(SelectFeatureSensorID, (FeatureIndexEnum)i).ToArray());
                }
            }
        }

        private List<double> GetFeatureDataList(EEGSensorID sensorID, FeatureIndexEnum featureIndex)
        {
            List<double> dataList = new List<double>();
            List<double> ScaleDataList = new List<double>();
            List<EEGFeatureIndex> featureScaleList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
            if (featureScaleList.Count > 0)
            {
                for (int i = 0; i < featureScaleList.Count; i++)
                {
                    if (featureIndex == FeatureIndexEnum.DELTA && !double.IsNaN(featureScaleList[i].Delta(sensorID))) ScaleDataList.Add(featureScaleList[i].Delta(sensorID));
                    if (featureIndex == FeatureIndexEnum.THETA && !double.IsNaN(featureScaleList[i].Theta(sensorID))) ScaleDataList.Add(featureScaleList[i].Theta(sensorID));
                    if (featureIndex == FeatureIndexEnum.ALPHA && !double.IsNaN(featureScaleList[i].Alpha(sensorID))) ScaleDataList.Add(featureScaleList[i].Alpha(sensorID));
                    if (featureIndex == FeatureIndexEnum.BETA && !double.IsNaN(featureScaleList[i].Beta(sensorID))) ScaleDataList.Add(featureScaleList[i].Beta(sensorID));
                    if (featureIndex == FeatureIndexEnum.GAMMA && !double.IsNaN(featureScaleList[i].Gamma(sensorID))) ScaleDataList.Add(featureScaleList[i].Gamma(sensorID));
                }
            }

            List<EEGFeatureIndex> featureDataList = LooxidLinkData.Instance.GetEEGFeatureIndexData(8.0f);
            if (featureDataList.Count > 0)
            {
                double min = Min(ScaleDataList);
                double max = Max(ScaleDataList);

                int numIndexList = 6 * linkFequency;
                for (int i = 0; i < featureDataList.Count; i++)
                {
                    if (i < numIndexList)
                    {
                        if (featureIndex == FeatureIndexEnum.DELTA)
                        {
                            double delta = double.IsNaN(featureDataList[i].Delta(sensorID)) ? 0.0 : LooxidLinkUtility.Scale(min, max, 0.0f, 1.0f, featureDataList[i].Delta(sensorID));
                            dataList.Add(delta);
                        }
                        if (featureIndex == FeatureIndexEnum.THETA)
                        {
                            double theta = double.IsNaN(featureDataList[i].Theta(sensorID)) ? 0.0 : LooxidLinkUtility.Scale(min, max, 0.0f, 1.0f, featureDataList[i].Theta(sensorID));
                            dataList.Add(theta);
                        }
                        if (featureIndex == FeatureIndexEnum.ALPHA)
                        {
                            double alpha = double.IsNaN(featureDataList[i].Alpha(sensorID)) ? 0.0 : LooxidLinkUtility.Scale(min, max, 0.0f, 1.0f, featureDataList[i].Alpha(sensorID));
                            dataList.Add(alpha);
                        }
                        if (featureIndex == FeatureIndexEnum.BETA)
                        {
                            double beta = double.IsNaN(featureDataList[i].Beta(sensorID)) ? 0.0 : LooxidLinkUtility.Scale(min, max, 0.0f, 1.0f, featureDataList[i].Beta(sensorID));
                            dataList.Add(beta);
                        }
                        if (featureIndex == FeatureIndexEnum.GAMMA)
                        {
                            double gamma = double.IsNaN(featureDataList[i].Gamma(sensorID)) ? 0.0 : LooxidLinkUtility.Scale(min, max, 0.0f, 1.0f, featureDataList[i].Gamma(sensorID));
                            dataList.Add(gamma);
                        }
                    }
                }
            }
            return dataList;
        }

        void Update()
        {
            if (HeadPositionCircle != null) HeadPositionCircle.localPosition = new Vector3((VRCamera.position.x - playerPos.x) * 4.0f, (VRCamera.position.z - playerPos.z) * 4.0f, 0.0f);
            if (HeadModeling != null) HeadModeling.localEulerAngles = new Vector3(VRCamera.eulerAngles.x, -VRCamera.eulerAngles.y + 90.0f, -VRCamera.eulerAngles.z);
            if (HeadTrackingText != null) HeadTrackingText.text = new Vector2(HeadPositionCircle.localPosition.x, HeadPositionCircle.localPosition.y).ToString();

            leftActivity.value = Mathf.Lerp((float)leftActivity.value, (float)leftActivity.target, 0.2f);
            rightActivity.value = Mathf.Lerp((float)rightActivity.value, (float)rightActivity.target, 0.2f);
            attention.value = Mathf.Lerp((float)attention.value, (float)attention.target, 0.2f);
            relaxation.value = Mathf.Lerp((float)relaxation.value, (float)relaxation.target, 0.2f);
            asymmetry.value = Mathf.Lerp((float)asymmetry.value, (float)asymmetry.target, 0.2f);
        }

        public void OnFeatureIndexTabClick(int numTab)
        {
            SetFeautreIndexTab((EEGSensorID)numTab);
        }
        public void OnFeatureIndexTabHoverEnter(int numTab)
        {
            Image TabImage = FeatureChannelTab[numTab].GetComponent<Image>();
            if (TabImage != null) TabImage.color = ConnectedColor;

            FeatureChannelTab[numTab].SendMessage("SetNormalColor", ConnectedColor);
        }
        public void OnFeatureIndexTabHoverExit(int numTab)
        {
            Image TabImage = FeatureChannelTab[numTab].GetComponent<Image>();
            if (TabImage != null) TabImage.color = ((EEGSensorID)numTab == SelectFeatureSensorID) ? ConnectedColor : new Color(0.098f, 0.102f, 0.110f, 1.0f);

            FeatureChannelTab[numTab].SendMessage("SetNormalColor", ((EEGSensorID)numTab == SelectFeatureSensorID) ? ConnectedColor : new Color(0.098f, 0.102f, 0.110f, 1.0f));
        }
        public void SetFeautreIndexTab(EEGSensorID sensorID)
        {
            this.SelectFeatureSensorID = sensorID;

            if (FeatureChannelTab != null)
            {
                for (int i = 0; i < FeatureChannelTab.Length; i++)
                {
                    Image TabImage = FeatureChannelTab[i].GetComponent<Image>();
                    if (TabImage != null) TabImage.color = ((EEGSensorID)i == SelectFeatureSensorID) ? ConnectedColor : new Color(0.098f, 0.102f, 0.110f, 1.0f);

                    FeatureChannelTab[i].SendMessage("SetNormalColor", ((EEGSensorID)i == SelectFeatureSensorID) ? ConnectedColor : new Color(0.098f, 0.102f, 0.110f, 1.0f));
                }
            }
        }

        public double Min(List<double> minList)
        {
            if (minList.Count <= 0) return 0.0;
            double min = minList[0];
            for (int i = 0; i < minList.Count; i++)
            {
                if( !double.IsNaN(minList[i]) )
                {
                    if (minList[i] < min) min = minList[i];
                }
            }
            return min;
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
            return max;
        }
    }
}
 