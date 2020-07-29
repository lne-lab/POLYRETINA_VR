# SPV

## Preprocessed data files

These files contain pre-calculated per-pixel data so that less calculations need to be performed at runtime.  
Because they are per-pixel, they need to calculated per resolution (e.g., 2036x2260 for the Vive Pro Eye).  
Files either contain phosphene position data in micrometres or axonal fibre trajectory information stored in polar coordinates.  

See [PhospheneMatrix.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/PhospheneMatrix.cs), [AxonMatrix.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Epiretinal/AxonMatrix.cs) and [PreprocessedDataFactory.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Editor/PreprocessedDataFactory.cs) for more information about how these files are created.

To create more preprocessed data files, use the "Preprocessed Data" tab in the Unity project, or for steps on how to create your own preprocessed data files, see the main README.

## Renderers

Renderers are assets that control the logic of a [Prosthesis](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Prosthesis.cs). 
Specifically, a prosthesis can have two renderers, an [External Processor](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Renderers/ExternalProcessor.cs) which handles the preprocessing of the captured image (such as applying edge detection)
and an [Implant](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Renderers/Implant.cs) which handles the conversion of the image into a pattern of phosphenes.

Importantly both external processors and implants can be extended to perform custom logic, allowing novel external processors and implants to be used.
For example, the [Edge Detector](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Processors/EdgeDetector.cs) is an extended external processor and the [Epiretinal Implant](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Epiretinal/EpiretinalImplant.cs) is an extended implant.

## Scripts

C# scripts related to simulated prosthetic vision. Mostly structural as the main SPV logic is performed in shaders for performance reasons.

## Shaders

Shaders are programs that are run on a computers GPU. Unlike CPUs, GPUs are focused on efficient and large scale parallel processing. As such, real-time SPV images can be computed 
quickly (<10ms) regardless of the number or size of phosphenes and also taking into account the stimulation of axon pathways.

As such, the majority of SPV logic is kept in shaders for maximum efficiency:  
[Edge detection](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Shaders/Edge%20Detection.shader)  
[Conversion to phosphene pattern](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Shaders/Phospherisation.cginc)  
[Stimulation of axon pathways](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Shaders/Tail%20Distortion.cginc)
