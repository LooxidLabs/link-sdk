# LooxidLinkData

Looxid Link SDK provides biometric data including the raw signal information for the EEG sensors, the EEG feature index, and the mind index in real time. In addition to the real time information, the data from the last few seconds is also provided so you can utilize it for visualization and other calculations.

## LooxidLinkData: Method Summary
| Type | Method and Description |
|---|---|
| **EEGSensor** | `GetEEGSensorData(EEGSensorID sensorID)`<br>Returns the most recent raw signal information for each EEG sensor. |
| **EEGFeatureIndex** | `GetFeatureIndexData(EEGSensorID sensorID)`<br>Returns the most recent feature index data for each EEG sensor. |
| List<**EEGFeatureIndex**> | `GetFeatureIndexData(EEGSensorID sensorID, float seconds)`<br>Returns the feature index data in the last few seconds (up to 10 seconds) for each EEG sensor. |
| **MindIndex** | `GetMindIndexData()`<br>Returns the most recent mind index data. |
| List<**MindIndex**> | `GetMindIndexData(float seconds)`<br>Returns the mind index data in the last few seconds (up to 10 seconds). |

## Raw Signal
`GetEEGSensorData()` returns the real time raw signal data for each sensor in the form of `EEGSensor` class.
```csharp
EEGSensor sensorData = LooxidLinkData.GetEEGSensorData(EEGSensorID.AF3);
double[] rawSignal = sensorData.rawSignal;
bool isSensorOn = sensorData.isSensorOn;
```

## EEGSensor: Field Summary
| Field | Type | Description |
|---|---|---|
| rawSignal | **double[]** | Filtered EEG raw signal data in the last 4 seconds. Includes 1,000 double data (250 data per second). |
| isSensorOn | **bool** | Returns if the respective sensor is contacted with the forehead. |
| timestamp | **double** | Unix timestamp (UTC) |

## EEG Feature Index
`GetEEGFeatureIndexData()` function acquires the EEG feature index data from each sensor in real time or in the last few seconds. You can use the indexes provided by the SDK or manually calculate the indexes by using the frequency band power values of 1-50Hz.
```csharp
EEGFeatureIndex index = LooxidLinkData.GetEEGFeatureIndexData(EEGSensorID.AF3);
double alpha = index.alpha;
double[] bandPower = index.bandPower;

// EEG feature index data list from the last 4 seconds.
List<EEGFeatureIndex> indexList = LooxidLinkData.GetEEGFeatureIndexData(EEGSensorID.AF3, 4.0f);
```

## EEGFeatureIndex: Field Summary
| Field | Type | Description |
|---|---|---|
| delta | **double** | Delta waves (1-3Hz) |
| theta | **double** | Theta waves (3-8Hz) |
| alpha | **double** | Alpha waves (8-12Hz) |
| beta | **double** | Beta waves (12-38Hz) |
| gamma | **double** | Gamma waves (38-50Hz) |
| bandPower | **double[]** | Frequency band power (1-50Hz) |
| timestamp | **double** | Unix timestamp (UTC) |

## Mind Index
The Link SDK provides a variety of mind indexes through its own algorithms, such as attention and relaxation. You can use `GetMindIndexData()` function to acquire the mind index data in real time or for the last few seconds.

## MindIndex: Field Summary
| Field | Type | Description |
|---|---|---|
| attention | **double** | Concentration index |
| relaxation | **double** | Relaxation index |
| asymmetry | **double** | Left-right brain balance index |
| leftActivity | **double** | Left brain activity index |
| rightActivity | **double** | Right brain activity index |
| timestamp | **double** | Unix Timestamp (UTC) |
