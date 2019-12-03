# LooxidLinkUtility

LooxidLinkUtility provides several utility functions to help you leverage your data. Out of these functions, `Scale()` function sets the min and max values of each figure and returns the data after scaling it. If you want to use the recommended min and max values by the Link SDK, refer to `LooxidLink.MIND_INDEX_SCALE_MIN` and `LooxidLink.MIND_INDEX_SCALE_MAX` variables.
```csharp
MindIndex mindIndex = LooxidLinkData.GetMindIndexData();

// Returns the attention values after min-max scaling them between 0 and 1.
double attention = LooxidLinkUtlity.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0, 1.0, mindIndex.attention);
```

## LooxidLinkUtility: Method Summary
| Type | Method and Description |
|---|---|
| **float** | `Scale(float inputLow, float inputHigh, float outputLow, float outputHigh, float value)`<br>Float type data scaling function. |
| **double** | `Scale(double inputLow, double inputHigh, double outputLow, double outputHigh, double value)`<br>Double type data scaling function. |
| **double** | `AverageBandPower(int startFrequency, int endFrequency, double[] bandpower)`<br>Returns the average value of the selected range in the frequency band power array. |
| **double** | `GetUTCTimestamp()`<br>Returns the current Unix Timestamp (UTC). |
