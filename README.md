# Polyretina VR

This project simulates the artificial vision provided by a POLYRETINA epi-retinal prosthesis using Unity, C# and HLSL/Cg. Simulations can be viewed in real-time on either a computer screen or in virtual reality (using either a FOVE or VIVE Pro Eye head-mounted display).

## Quick Setup

The project can either be forked or downloaded and used directly or can be imported into your own Unity project as an asset package (see Releases). It has currently only been tested using Unity version 2019.2.16f1.

1. Click on the Polyretina dropdown menu -> Settings.
   - Select the VR headsets you want to use (if any). If you do not plan to use a certain VR headset you can also delete it's associated assets (FoveUnityPlugin for the FOVE or ViveSR for the VIVE Pro Eye).
   - Select the "VRInput Support" checkbox if you are using the VIVE Pro Eye to enable controller input support.

### If viewing the simulation on a computer screen:

2. Make sure "Virtual Reality Supported" is disabled in Unity's XR Settings.
3. In the game tab, create a new resolution to match the output of the simulation (if unsure, use 2036x2260).
4. Open one of the demo scenes (Assets/Polyretina/SPV/Demos/), such as Objects.
5. Ensure the "SRanipal Eye Framework" GameObject is disabled.
6. Run the simulation.

### If viewing in the VIVE Pro Eye:

7. Make sure "Virtual Reality Supported" is enabled in Unity's XR Settings.
8. Open one of the demo scenes (Assets/Polyretina/SPV/Demos/), such as Objects.
9. Ensure the "SRanipal Eye Framework" GameObject is enabled.
10. Run the simulation.
11. Select the "Remote" resolution from the game tab for accurate viewing in the Unity Editor.

## Adding prosthetic vision to a Unity scene

To add prosthetic vision to a Unity scene, you only need to add the Prosthesis component to the Scene's camera and select the desired external processor and implant.  
  
The external processor and implant can be configured directly from the component, but know that these changes will also be reflected in the original files.  
  
You can view the original files by clicking the "Select" button at the bottom of either the external processor or the implant configuration windows.

## Using your own prosthesis design

1. Add a value to the ElectrodePattern enum (Assets/Polyretina/SPV/Scripts/ElectrodePattern.cs) to represent your design.
2. Add a method to the ElectrodePatternExtensions class (same file) that returns the x, y positions (μm) of each electrode in your design. See Polyretina(ElectrodeLayout layout, float fov) for an example.
3. Add a value to the ElectrodeLayout enum (Assets/Polyretina/SPV/Scripts/ElectrodeLayout.cs) to represent the diameter and pitch of your design.
4. Add cases to the switch statements in the methods ToValue() and ToAnatomicalValue() that return the diameter and pitch of the electrodes in your design.
   - If unsure, then have the ToAnatomicalValue() method return the same values as the ToValue() method.
5. Click on the Poylretina dropdown menu -> Preprocessed Data.
   - Choose your design pattern and layout as well as an output resolution/field of view by choosing a headset model.
   - Click start and choose a location to save the file.
   - Repeat this for both the phosphene and axon data types.
6. Create or duplicate an existing implant.
7. Assign the phosphene and axon data textures to appropriate parts of the Preprocessed Data section in the implant configuration window.

## Images

![Kitchen](https://github.com/lne-lab/polyretina_vr/blob/master/Images/kitchen.png)
![Street](https://github.com/lne-lab/polyretina_vr/blob/master/Images/street.png)

