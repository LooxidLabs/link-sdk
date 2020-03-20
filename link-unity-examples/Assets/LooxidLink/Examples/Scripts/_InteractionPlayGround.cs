using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    public enum InteractionData
    {
        ATTENTION = 0,
        RELAXATION = 1,
        LEFT_ACTIVTY = 2,
        RIGHT_ACTIVITY = 3,
        DELTA_INDEX = 4,
        THETA_INDEX = 5,
        ALPHA_INDEX = 6,
        BETA_INDEX = 7,
        GAMMA_INDEX = 8
    }

    public class _InteractionPlayGround : MonoBehaviour
    {
        public _2DVisualizer visualizer;

        [Header("Cube")]
        public Transform CubeTransform;
        public GameObject[] CubeTypes;

        [Header("Mateirls")]
        public Material materialB;
        public Material materialO;
        public Material materialR;

        [Header("Selection")]
        public InteractionData selectInteraction;
        public Image[] outlines;

        private List<GameObject> CubeList = new List<GameObject>();

        private List<float> powerData;
        private float prevPowerData = -1.0f;

        private float gravity;
        private float targetGravity = -9.81f;

        private EEGSensor sensorStatusData;

        private LinkDataValue leftActivity;
        private LinkDataValue rightActivity;
        private LinkDataValue attention;
        private LinkDataValue relaxation;

        private LinkDataValue delta;
        private LinkDataValue theta;
        private LinkDataValue alpha;
        private LinkDataValue beta;
        private LinkDataValue gamma;

        void Start()
        {
            int children = CubeTransform.childCount;
            for (int i = 0; i < children; ++i)
            {
                CubeList.Add(CubeTransform.GetChild(i).gameObject);
            }

            powerData = new List<float>();

            materialB.DisableKeyword("_EMISSION");
            materialO.DisableKeyword("_EMISSION");
            materialR.DisableKeyword("_EMISSION");

            leftActivity = new LinkDataValue();
            rightActivity = new LinkDataValue();
            attention = new LinkDataValue();
            relaxation = new LinkDataValue();

            delta = new LinkDataValue();
            theta = new LinkDataValue();
            alpha = new LinkDataValue();
            beta = new LinkDataValue();
            gamma = new LinkDataValue();
        }

        void OnEnable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus += OnReceiveEEGSensorStatus;
            LooxidLinkData.OnReceiveMindIndexes += OnReceiveMindIndexes;
            LooxidLinkData.OnReceiveEEGFeatureIndexes += OnReceiveEEGFeatureIndexes;

            StartCoroutine(SuperPower());
        }
        void OnDisable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceiveEEGSensorStatus;
            LooxidLinkData.OnReceiveMindIndexes -= OnReceiveMindIndexes;
            LooxidLinkData.OnReceiveEEGFeatureIndexes -= OnReceiveEEGFeatureIndexes;
        }

        void OnReceiveEEGSensorStatus(EEGSensor sensorStatusData)
        {
            this.sensorStatusData = sensorStatusData;
        }
        void OnReceiveMindIndexes(MindIndex mindIndexData)
        {
            leftActivity.value = double.IsNaN(mindIndexData.leftActivity) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.leftActivity);
            rightActivity.value = double.IsNaN(mindIndexData.rightActivity) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.rightActivity);
            attention.value = double.IsNaN(mindIndexData.attention) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.attention);
            relaxation.value = double.IsNaN(mindIndexData.relaxation) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.relaxation);
        }
        void OnReceiveEEGFeatureIndexes(EEGFeatureIndex featureIndexData)
        {
            double deltaValue = featureIndexData.Delta(visualizer.SelectChannel);
            double thetaValue = featureIndexData.Theta(visualizer.SelectChannel);
            double alphaValue = featureIndexData.Alpha(visualizer.SelectChannel);
            double betaValue = featureIndexData.Beta(visualizer.SelectChannel);
            double gammaValue = featureIndexData.Gamma(visualizer.SelectChannel);

            delta.value = (double.IsInfinity(deltaValue) || double.IsNaN(deltaValue)) ? 0.0f : LooxidLinkUtility.Scale(delta.min, delta.max, 0.0f, 1.0f, deltaValue);
            theta.value = (double.IsInfinity(thetaValue) || double.IsNaN(thetaValue)) ? 0.0f : LooxidLinkUtility.Scale(theta.min, theta.max, 0.0f, 1.0f, thetaValue);
            alpha.value = (double.IsInfinity(alphaValue) || double.IsNaN(alphaValue)) ? 0.0f : LooxidLinkUtility.Scale(alpha.min, alpha.max, 0.0f, 1.0f, alphaValue);
            beta.value = (double.IsInfinity(betaValue) || double.IsNaN(betaValue)) ? 0.0f : LooxidLinkUtility.Scale(beta.min, beta.max, 0.0f, 1.0f, betaValue);
            gamma.value = (double.IsInfinity(gammaValue) || double.IsNaN(gammaValue)) ? 0.0f : LooxidLinkUtility.Scale(gamma.min, gamma.max, 0.0f, 1.0f, gammaValue);
        }

        void FixedUpdate()
        {
            if (!LooxidLinkManager.Instance.isLinkCoreConnected) return;

            gravity = Mathf.Lerp(gravity, targetGravity, 0.1f);
            Physics.gravity = new Vector3(0, gravity, 0f);

            float value = 0.0f;

            if (selectInteraction == InteractionData.ATTENTION)
            {
                value = (float)attention.value;
            }
            else if (selectInteraction == InteractionData.RELAXATION)
            {
                value = (float)relaxation.value;
            }
            else if (selectInteraction == InteractionData.LEFT_ACTIVTY)
            {
                value = (float)leftActivity.value;
            }
            else if (selectInteraction == InteractionData.RIGHT_ACTIVITY)
            {
                value = (float)rightActivity.value;
            }
            else if (selectInteraction == InteractionData.DELTA_INDEX)
            {
                List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
                if (featureIndexList.Count > 0)
                {
                    List<double> deltaScaleDataList = new List<double>();
                    for (int i = 0; i < featureIndexList.Count; i++)
                    {
                        double deltaValue = featureIndexList[i].Delta(visualizer.SelectChannel);

                        if (!double.IsInfinity(deltaValue) && !double.IsNaN(deltaValue)) deltaScaleDataList.Add(deltaValue);
                    }
                    delta.SetScale(deltaScaleDataList);
                }
                value = (float)delta.value;
            }
            else if (selectInteraction == InteractionData.THETA_INDEX)
            {
                List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
                if (featureIndexList.Count > 0)
                {
                    List<double> thetaScaleDataList = new List<double>();
                    for (int i = 0; i < featureIndexList.Count; i++)
                    {
                        double thetaValue = featureIndexList[i].Theta(visualizer.SelectChannel);

                        if (!double.IsInfinity(thetaValue) && !double.IsNaN(thetaValue)) thetaScaleDataList.Add(thetaValue);
                    }
                    theta.SetScale(thetaScaleDataList);
                }
                value = (float)theta.value;
            }
            else if (selectInteraction == InteractionData.ALPHA_INDEX)
            {
                List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
                if (featureIndexList.Count > 0)
                {
                    List<double> alphaScaleDataList = new List<double>();
                    for (int i = 0; i < featureIndexList.Count; i++)
                    {
                        double alphaValue = featureIndexList[i].Alpha(visualizer.SelectChannel);

                        if (!double.IsInfinity(alphaValue) && !double.IsNaN(alphaValue)) alphaScaleDataList.Add(alphaValue);
                    }
                    alpha.SetScale(alphaScaleDataList);
                }
                value = (float)alpha.value;
            }
            else if (selectInteraction == InteractionData.BETA_INDEX)
            {
                List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
                if (featureIndexList.Count > 0)
                {
                    List<double> betaScaleDataList = new List<double>();
                    for (int i = 0; i < featureIndexList.Count; i++)
                    {
                        double betaValue = featureIndexList[i].Beta(visualizer.SelectChannel);

                        if (!double.IsInfinity(betaValue) && !double.IsNaN(betaValue)) betaScaleDataList.Add(betaValue);
                    }
                    beta.SetScale(betaScaleDataList);
                }
                value = (float)beta.value;
            }
            else if (selectInteraction == InteractionData.GAMMA_INDEX)
            {
                List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);
                if (featureIndexList.Count > 0)
                {
                    List<double> gammaScaleDataList = new List<double>();
                    for (int i = 0; i < featureIndexList.Count; i++)
                    {
                        double gammaValue = featureIndexList[i].Gamma(visualizer.SelectChannel);

                        if (!double.IsInfinity(gammaValue) && !double.IsNaN(gammaValue)) gammaScaleDataList.Add(gammaValue);
                    }
                    beta.SetScale(gammaScaleDataList);
                }
                value = (float)gamma.value;
            }

            powerData.Add(value);
        }

        IEnumerator SuperPower()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);

                if (sensorStatusData != null)
                {
                    if (!sensorStatusData.IsSensorOn(EEGSensorID.AF3) && !sensorStatusData.IsSensorOn(EEGSensorID.AF4) &&
                     !sensorStatusData.IsSensorOn(EEGSensorID.Fp1) && !sensorStatusData.IsSensorOn(EEGSensorID.Fp2) &&
                     !sensorStatusData.IsSensorOn(EEGSensorID.AF7) && !sensorStatusData.IsSensorOn(EEGSensorID.AF8))
                    {
                        materialB.DisableKeyword("_EMISSION");
                        materialO.DisableKeyword("_EMISSION");
                        materialR.DisableKeyword("_EMISSION");
                        targetGravity = -9.81f;
                        powerData.Clear();
                    }
                    else
                    {
                        if (prevPowerData < 0.0f)
                        {
                            prevPowerData = powerData.Average();

                            materialB.DisableKeyword("_EMISSION");
                            materialO.DisableKeyword("_EMISSION");
                            materialR.DisableKeyword("_EMISSION");
                            targetGravity = -9.81f;
                        }
                        else
                        {
                            if (powerData.Average() > prevPowerData || powerData.Average() > 0.7f)
                            {
                                materialB.EnableKeyword("_EMISSION");
                                materialO.EnableKeyword("_EMISSION");
                                materialR.EnableKeyword("_EMISSION");

                                float gravity_power = powerData.Average() * 1.5f;
                                targetGravity = gravity_power;

                                for (int i = 0; i < CubeList.Count; i++)
                                {
                                    CubeList[i].GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f)));
                                }
                            }
                            else
                            {
                                materialB.DisableKeyword("_EMISSION");
                                materialO.DisableKeyword("_EMISSION");
                                materialR.DisableKeyword("_EMISSION");
                                targetGravity = -9.81f;
                            }
                        }

                        prevPowerData = powerData.Average();
                        powerData.Clear();
                    }
                }
            }
        }

        public void CreateCube()
        {
            int randomRot = Random.Range(0, 360);
            int randomCube = Random.Range(0, CubeTypes.Length - 1);

            GameObject boxClone = Instantiate(CubeTypes[randomCube], new Vector3(Random.Range(-10f, 10f), Random.Range(1f, 10f), Random.Range(-10f, 10f)),
                Quaternion.Euler(new Vector3(randomRot, randomRot, randomRot)));
            boxClone.transform.parent = CubeTransform;
            CubeList.Add(boxClone);
        }

        public void SelectData(int dataType)
        {
            selectInteraction = (InteractionData)dataType;
            for ( int i = 0; i < outlines.Length; i++ )
            {
                outlines[i].color = (dataType == i) ? new Color(0.486f, 0.251f, 1.0f, 1.0f) : new Color(0.486f, 0.251f, 1.0f, 0.0f);
            }
            prevPowerData = -1.0f;
            powerData.Clear();
        }
    }
}