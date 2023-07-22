# AiRHockey
This project is a Unity multiplayer Airhockey game for the Hololens 2. It uses a client-hosted LAN network framework with service discovery to allow ad-hoc creation and joining of lobbies. The AR arena is placed using a QR code containing "pivot". <br/>
AiRHockey uses MRTK 3 and OpenXR to support Hololens 2 input.

## Use and Development
Open the project using Unity Version 2021.3.24 and build/deploy to the Hololens 2 as Universal Windows Platform. 
The game starts in the main menu, allowing you to change settings, like volume level and arena size, joining an open lobby or creating a lobby. To enable networking to work, both Hololens' must be in one subnet with the internetClientServer and privateNetworkClientServer capabilities activated. <br/>
For debugging in the Unity editor disable the QRCodesManager and enable EnvironmentContainer manually. The Hololens input can be simulated using the MRTKInputSimulator.

## Dependencies
[UnityModuledNet](https://github.com/CENTIS-HTW/UnityModuledNet/tree/upm) <br/>
[MRTK3](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/releases/3.0.0-pre.17) <br/>
[OpenXR](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.7/manual/index.html) <br/>
