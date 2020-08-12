# Polyretina VR

This project simulates the artificial vision provided by a POLYRETINA epi-retinal prosthesis using Unity, C# and HLSL/Cg. Simulations can be viewed in real-time on either a computer screen or in virtual reality (using either a FOVE or VIVE Pro Eye head-mounted display).

## Setup

1. Download Unity (Polyretina VR has only been tested on version 2019.2.16f1).
2. Download Polyretina VR, either by:
   - Downloading the zipped files from Github,
   - Forking the repository, or
   - Downloading the latest release
   
   Downloading the zipped files or forking the repository will give you the whole Unity project which can then be opened using Unity. Downloading the latest release will give you a Unity Package file which can be imported into an existing Unity project.
3. After opening/importing the project, there will be potentially be some warning/error messages. You can ignore them by clicking clear in the console window. They will not appear again.

### Viewing the simulation on a computer screen

1. In the Game tab, create a new resolution that matches the output of the simulation (most likely 2036 x 2260).
2. Open one of the demo sceenes (Assets/Polyretina/SPV/Demos/), such as Words.
3. Run the simulation.

### Viewing the simulation in the Vive Pro Eye

1. Install the Vive Pro Eye eye-tracking software.
   - Follow the links [here](https://developer.vive.com/resources/knowledgebase/vive-sranipal-sdk/).
   - Run the installer and download the SDK.
   - Unzip the SDK and open **02_Unity/Vive-SRanipal-Unity-Plugin** to import it into the the Unity project.
2. Click on the Polyretina dropdown menu -> Settings.
   - Check the Vive Pro Eye option under Eye Tracking SDKs
   - Check the VRInput Support option
3. Open one of the demo scenes (Assets/Polyretina/SPV/Demos/), such as **Objects*.
4. Make sure the **SRanipal Eye Framework** GameObject is enabled.
5. Make sure **Virtual Reality Supported** is enabled in Unity's XR Settings.
6. Run the simulation.
7. Select the **Remote** resolution from the Game tab for more accurate viewing in the Unity Editor.

**3D models cannot be distributed with this project, so your own will need to be added. High quality free 3D models can be found on websites such as TurboSquid.com or the Unity Asset Store. Once imported into the Unity project, add them to the "Objects" list in the "Object Spawner" GameObject*

### Changing Prosthetic Vision parameters

1. Select the **Prosthetic Vision** GameObject in the Hierarchy tab.
2. Scroll down the Prosthesis script in the Inspector tab.
3. Adjust the settings as you see fit (changes made while the simulation is running will not be saved).

## Adding prosthetic vision to a Unity scene

To add prosthetic vision to another Unity scene, you only need to add the Prosthesis component to the Scene's camera and select the desired external processor and implant.  
  
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

## Using your own External Processors/Implants

External Processors/Implants are explained [here](https://github.com/lne-lab/polyretina_vr/tree/master/Assets/Polyretina/SPV), under **Renderers**.

1. Extended either the ExternalProcessor.cs or Implant.cs base classes, implenting your own image processing logic.
   - Overridable methods include: Start, Update, GetDimensions and OnRenderImage.
   - Add a **CreateAssetMenu** attribute to your class (See [EpiretinalImplant.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Epiretinal/EpiretinalImplant.cs) as an example).
2. Right click the Project tab -> Create and then follow the menu set in your CreateAssetMenu attribute to create the asset.
4. Add the Prosthesis component to the Scene's camera (if not already done).
5. Assign the asset as either the External Processor or Implant of the Prosthesis component.

## Images

![Kitchen](https://github.com/lne-lab/polyretina_vr/blob/master/Images/kitchen.png)
![Street](https://github.com/lne-lab/polyretina_vr/blob/master/Images/street.png)

