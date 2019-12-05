# Looxid Link Core
Looxid Link Core, a Looxid Labs’ proprietary software, helps you integrate the retrieved signals from the EEG sensors into the VR contents. This is where the raw EEG data is processed and hence, the mind indexes such as attention, relaxation, and brain activity, as well as the different brain frequencies are identified. The bridge between the hardware components of Looxid Link and your VR headset, you can check the connection status of your Looxid Link and the status of the app you are running.

## Notch filter
A notch filter is a filter that blocks a specific range of frequencies from passing through. It is the opposite of a bandpass filter(BPF), which filters all but a particular frequency range. It is for removing unwanted signals such as interference or harmonics emitted by transmitters.
When supplying power from wall sockets to electronic devices, power noise is unavoidable. Looxid Link Core provides a notch filter to remove this noise. To set the filter frequency, click the settings button from the Core window and select the frequency you want from the `Notch` menu.

![Click the settings button.][notch-filter-1]
![Select the frequency you want from the Notch menu.][notch-filter-2]

As the AC noise frequency differs by country, [check which frequency your country uses first](https://www.worldstandards.eu/electricity/plug-voltage-by-country/) to make sure that you are using the correct filter.

## Multi-connect feature
You can connect multiple Unity applications to Looxid Link Core at the same time. This feature can be useful if you want to receive EEG data from several applications simultaneously. For example, you can run a game using the mind indexes on your VR headset while running a brain-activity analyzing application on your desktop. When you run more than one application using the Link SDK, two arrow-shaped buttons will appear on the Core. You can check which applications are connected to the Core by clicking these buttons.

![Multi-connect feature][multi-connect-feature]

[notch-filter-1]: img/core-notch-filter-1.png "Click the settings button."
[notch-filter-2]: img/core-notch-filter-2.png "Select the frequency you want from the Notch menu."
[multi-connect-feature]: img/core-multi-connect.png "Multi-connect feature"
