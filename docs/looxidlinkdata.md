# LooxidLinkData

Looxid Link SDK provides biometric data including the raw signal information for the EEG sensors, the EEG feature index, and the mind index in real-time. In addition to the real-time information, the data from the last few seconds is also provided so you can utilize it for visualization and other calculations. In order to receive these data in real-time, subscribe them by using respective action delegate functions.

CAUTION: Do not use Unity engine related functions (UI, for example) inside of the action delegate function.

## LooxidLinkData: Method Summary
| Type | Method and Description |
|---|---|
| List<**EEGSensorStatus**> | `GetEEGSensorStatus(float seconds)`<br>Returns the EEG sensor status data from the last few seconds (up to 10 seconds). |
| List<**EEGFeatureIndex**> | `GetFeatureIndexData(EEGSensorID sensorID, float seconds)`<br>Returns the feature index data from the last few seconds (up to 10 seconds) for each EEG sensor. |
| List<**MindIndex**> | `GetMindIndexData(float seconds)`<br>Returns the mind index data from the last few seconds (up to 10 seconds). |

## LooxidLinkData: Field Summary
| Field | Type | Description |
|---|---|---|
| OnReceiveMindIndexes | **Action<MindIndex>** | Subscribe to the mind index data. |
| OnReceiveEEGFeatureIndexes | **Action<EEGFeatureIndex>** | Subscribe to the feature index data. |
| OnReceiveEEGRawSignals | **Action<EEGRawSignal>** | Subscribe to the raw signal data. |
| OnReceiveEEGSensorStatus | **Action<EEGSensor>** | Subscribe to the most recent sensor status data of each EEG sensor. |

## Sensor Status
Use `OnReceiveEEGSensorStatus` action delegate to check whether the EEG sensor is correctly attached to the user's forehead, or whether the EEG signals are noisy.

```csharp
void OnEnable()
{
    LooxidLinkData.OnReceiveEEGSensorStatus += OnReceiveEEGSensorStatus;
}
void OnDisable()
{
    LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceiveEEGSensorStatus;
}
void OnReceiveEEGSensorStatus(EEGSensor sensorStatus)
{
    bool AF3_isSensorOn = sensorStatus.IsSensorOn(EEGSensorID.AF3);
    Debug.Log(“AF3 Sensor is on: ” + AF3_isSensorOn);
}
```

## EEGSensor: Method Summary
| Type | Method and Description |
|---|---|
| bool | `IsSensorOn(EEGSensorID sensorID)`<br>Returns whether the specified sensor is attached to the forehead or not. |
| bool | `IsNoiseSignal(EEGSensorID sensorID)`<br>Returns whether the signal from the specified sensor is noisy or not. |

## EEGSensor: Field Summary
| Field | Type | Description |
|---|---|---|
| timestamp | **double** | Unix timestamp (UTC) |

## Raw Signal
Use `OnReceiveEEGRawSignals` action delegate to get raw signal data from EEG sensors in real-time. You will receive the data in a class type called `EEGRawSignalData`. This class comes with a list-type field called `rawSignal`, and `FilteredRawSignal` function which is for drawing filtered EEG graphs.
`EEGRawSignalData` consists of `seq_num`, `ch_data`, and `timestamp`. It allows you to acquire the raw signal data directly from the board of your Looxid Link.
```csharp
void OnEnable()
{
    LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
}
void OnDisable()
{
    LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
}
void OnReceiveEEGRawSignals(EEGRawSignal rawSignalData)
{
    Debug.Log("EEG Raw Signal Data: " + rawSignalData.rawSignal.Count + "samples");
}
```

## EEGRawSignal: Method Summary
| Type | Method and Description |
|---|---|
| double[] | `FilteredRawSignal(EEGSensorID sensorID)`<br>Filtered EEG raw signal data in the last 4 seconds. Includes 2,000 double-type data (500 data per second). |

## EEGRawSignal: Field Summary
| Field | Type | Description |
|---|---|---|
| rawSignal | List<**EEGRawSignalData**> | Returns an unfiltered EEG raw signal data list |
| timestamp | **double** | Unix timestamp (UTC) |

## EEGRawSignalData: Field Summary
| Field | Type | Description |
|---|---|---|
| seq_num | **int** | A number label which can be used when analyzing the raw signal |
| ch_data | **double[]** | Raw signal data from each channel in following order: AF3, AF4, Fp1, Fp2, AF7, AF8 |
| timestamp | **double** | Unix timestamp (UTC) |

## EEG Feature Index
You can use `OnReceiveEEGFeatureIndexes` action delegate to receive each sensor's EEG feature index data in real-time. Otherwise, you can also use `GetEEGFeatureIndexData()` function to acquire the EEG feature index data from the last few seconds. You can use the indexes provided by the SDK or manually calculate the indexes by using the frequency band power values of 1-45Hz.
```csharp
void OnEnable()
{
    LooxidLinkData.OnReceiveEEGFeatureIndexes += OnReceiveEEGFeatureIndexes;
}
void OnDisable()
{
    LooxidLinkData.OnReceiveEEGFeatureIndexes -= OnReceiveEEGFeatureIndexes;
}
void OnReceiveEEGFeatureIndexes(EEGFeatureIndex featureIndex)
{
    Debug.Log(“AF3_Alpha: ” + featureIndex.Alpha(EEGSensorID.AF3));
}

void Update()
{
    // Returns EEG feature index data from last 3 seconds
    List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(3.0f);
}
```

## EEGFeatureIndex: Method Summary
| Type | Method and Description |
|---|---|
| double | `Delta(EEGSensorID sensorID)`<br>Delta waves (1-3Hz) |
| double | `Theta(EEGSensorID sensorID)`<br>Theta waves (3-8Hz) |
| double | `Alpha(EEGSensorID sensorID)`<br>Alpha waves (8-12Hz) |
| double | `Beta(EEGSensorID sensorID)`<br>Beta waves (12-38Hz) |
| double | `Gamma(EEGSensorID sensorID)`<br>Gamma waves (38-45Hz) |
| double[] | `BandPower(EEGSensorID sensorID)`<br>Frequency band power (1-45Hz) |

## EEGFeatureIndex: Field Summary
| Field | Type | Description |
|---|---|---|
| timestamp | **double** | Unix timestamp (UTC) |

## Mind Index
The Link SDK provides a variety of mind indexes through its own algorithms, such as attention and relaxation. You can use `OnReceiveMindIndexes` action delegate to receive mind index data in real-time. Otherwise, you can also use `GetMindIndexData()` function to acquire the mind index data from the last few seconds.

## MindIndex: Field Summary
| Field | Type | Description |
|---|---|---|
| attention | **double** | Attention index |
| relaxation | **double** | Relaxation index |
| asymmetry | **double** | Left-right brain balance index |
| leftActivity | **double** | Left brain activity index |
| rightActivity | **double** | Right brain activity index |
| timestamp | **double** | Unix Timestamp (UTC) |
