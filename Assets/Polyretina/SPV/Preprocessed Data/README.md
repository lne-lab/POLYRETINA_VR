# Preprocessed data files

These files contain pre-calculated per-pixel data so that less calculations need to be performed at runtime.  
Because they are per-pixel, they need to calculated per resolution (e.g., 2036x2260 for the Vive Pro Eye).  
Files either contain phosphene position data in micrometres or axonal fibre trajectory information stored in polar coordinates.  

See [Assets/Polyretina/SPV/Scripts/PhospheneMatrix.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/PhospheneMatrix.cs), [Assets/Polyretina/SPV/Scripts/Epiretinal/AxonMatrix.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Epiretinal/AxonMatrix.cs) and [Assets/Polyretina/SPV/Scripts/Editor/PreprocessedDataFactory.cs](https://github.com/lne-lab/polyretina_vr/blob/master/Assets/Polyretina/SPV/Scripts/Editor/PreprocessedDataFactory.cs) for more information about how these files are created.

To create more preprocessed data files, use the "Preprocessed Data" tab in the Unity project or for steps on how to create your own preprocessed data files, see the main README.
