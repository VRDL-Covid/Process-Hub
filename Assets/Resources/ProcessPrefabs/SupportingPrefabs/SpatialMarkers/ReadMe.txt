Spatial Awareness
=================

The MarkerPosition prefab object is extracted from a full scale model Blender model of the space that the process is run within.

The extracted objects are a set of markers that are used in the setting up of the process which is to align and scale the full scale model.

The top level object of this prefab has a script component attached that generates json to store the position, rotation and scale of each marker and also (importantly) the distance to a wall, ceiling or floor in the real world).  

SmartMarkers:
=============

A SmartMarker object that is spatially aware of it's environment.  It has 3-axes (x,y,z) that are initialised in length to represent the distance to the boundary wall etc.  
The SmartMarker is also used in conjuction with a runtime-generated spatial mesh obtained from the HoloLens.  This spatialmesh is represented in Unity as a set of gameobjects with a mesh-collider attached.

The SmartMarker is spatially aware in that it runs a coRoutine to raycast along each axis looking for the distance to a mesh-collider on the "Spatial-Awareness" layer in Unity.

A camera facing tooltip contains text that informs the operator of:
- The length in metres required.
- Current hit distance along that axis in metres, (if a hit is obtained).

A SmartMarker has a toolbar that allows it to be moved and rotated in the Y-axis. 

The SnapMarkers scene:
======================

The SnapMarkers scene's job is to  generate the information necessary to align the full scale model to the real world.  Thus this scene controls the Instantiation of the SmartMarkers.

This scene contains a toolbar that allows the system to generate and reset the SmartMarkers.  Is is also the intention that the functionality to allow the SmartMarkers to self-locate within a specific range.

Coordinate Json file:
=====================

This json file stored in the application's data space on both the dev box and the HoloLens.  It is used in the SnapMarkers scene to instantiate a SmartMarker for each marker in the json file.
