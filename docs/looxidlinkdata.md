# LooxidLinkData

The Looxid Link SDK provides biometric data in real time. The raw signal information for the EEG sensor can be divided into the EEG feature index and the mind index. In addition to the real time information, the data from the last few seconds can be provided for visualization and other calculations.

## LooxidLinkData: Method Summary
| Type | Method and Description |
|---|---|
| **EEGSensor** | `GetEEGSensorData(EEGSensorID sensorID)`<br>It returns the most recent raw signal information for each EEG sensor. |
| **EEGFeatureIndex** | `GetFeatureIndexData(EEGSensorID sensorID)`<br>It returns the most recent feature index data for each EEG sensor. |
| List<**EEGFeatureIndex**> | `GetFeatureIndexData(EEGSensorID sensorID, float seconds)`<br>Returns the feature index data in the last few seconds for each EEG sensor. Up to 10 seconds. |
| **MindIndex** | `GetMindIndexData()`<br>Returns the most recent mind index data. |
| List<**MindIndex**> | `GetMindIndexData(float seconds)`<br>Returns the mind index data in the last few seconds. Up to 10 seconds. |

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
| rawSignal | **double[]** | Filtered EEG raw signal data in the last 4 seconds. 250 data per second, 1,000 double data is included. |
| isSensorOn | **bool** | It returns the contact status of the respective sensor with the forehead. |
| timestamp | **double** | Unix timestamp (UTC) |

## EEG Feature Index
`GetEEGFeatureIndexData()` function brings back the EEG feature index data from each sensor in real time or in the last few seconds. You can use the provided indexes calculated by the SDK or manually calculate the indexes by using the frequency band power values of 1-45Hz.
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
| gamma | **double** | Gamma waves (38-45Hz) |
| bandPower | **double[]** | Frequency band power (1-45Hz) |
| timestamp | **double** | Unix timestamp (UTC) |

## Mind Index
The Link SDK provides a variety of mind indexes through its own algorithms, such as attention and relaxation. You can use `GetMindIndexData()` function to bring back the mind index data in real time or for the last few seconds.

## MindIndex: Field Summary
| Field | Type | Description |
|---|---|---|
| attention | **double** | Concentration index |
| relaxation | **double** | Relaxation index |
| asymmetry | **double** | Left-right brain balance index |
| leftActivity | **double** | Left brain activity index |
| rightActivity | **double** | Right brain activity index |
| timestamp | **double** | Unix Timestamp (UTC) |
