WebXR Exporter Advanced
=======================


Advanced features to be used using the WebXR Exporter Plugin for Unity
(Included in this project)
You can test it here (Standalone & Oculus Quest with Firefox Browser): https://www.jocyf.com/Preview/WebXRAdvanced2019/index.html


![Screenshot](WebXRExport.jpg)


Asteroids project notes:
------------------------

Original github repo: https://github.com/MozillaReality/unity-webxr-export/pull/415
You can download the original repo as an UnityPackage from here: https://cdn.discordapp.com/attachments/759166366967660584/759846381752942592/WebXR-Exporter1.unitypackage
Notice that this is a specific pull from original webxr exporter project.



Assets Used in this project: 
Quick Outline: https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488 (If you use it, just take a moment to go there and and rate it!!!!)
Unity Simple VR Teleporter: https://codeload.github.com/IJEMIN/Simple-Unity-VR-Teleporter/zip/master
							https://assetstore.unity.com/packages/tools/input-management/simple-vr-teleport-115996


WebXR original scripts changes to make it work. Details in the Readme provided in the project.


Project key scripts:
-------------------
Managers.

WebXRGrabManager -> Used to grab/drop things (based on the original example provided).

WebXRInputManager -> Read the input from keyboard/Mouse/VR Controllers.

WebXRMove -> Basic player movement.

WebXRMovev2 -> Basic player movement (using CharacterController) against environment collision detection.

XRMouseLook -> Camera rotation when using the mouse.


Interactions:
XRTeleporterController -> Teleporter trigger based on input.

VREyeRaycaster -> Distance object interaction (based on raycast)

VRShowOutlineByClick -> Helper class to identify interactive object in the environment (using line as helper and interactive object outline).


Project notes:
-------------------
There is a lot of room for improvements in this project.
For example, interaction scripts use use their own raycast. The "usual" way to do it, is to create a general Raycast "sensor" and store all 
raycast positive detections in a independent script/manager. 
Layers/Tags to be used as interactive objects, should be exposed in the inspector.



License
-------
Unity project and source code license:
[MIT License](https://opensource.org/licenses/MIT).


WebXR Exporter original license:
License
Copyright 2017 - 2020 Mozilla Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.





