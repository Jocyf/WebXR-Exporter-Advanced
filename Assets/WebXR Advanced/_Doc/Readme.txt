//===============================================================================
//
// WebXR Advanced. 
// Creation of some managers to control interaction, movement and teleportation.
// Modifying original code to read stick controllers.
//
//===============================================================================


Original github repo: https://github.com/MozillaReality/unity-webxr-export/pull/415
You can download the original repo as an UnityPackage from here: https://cdn.discordapp.com/attachments/759166366967660584/759846381752942592/WebXR-Exporter1.unitypackage
Notice that this is a specific pull from original webxr exporter project.



Assets Used in this project: 
Quick Outline: https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488 (If you use it, just take a moment to go there and and rate it!!!!)
Unity Simple VR Teleporter: https://codeload.github.com/IJEMIN/Simple-Unity-VR-Teleporter/zip/master
							https://assetstore.unity.com/packages/tools/input-management/simple-vr-teleport-115996


WebXR original scripts changes to make it work:
- WebXRManager: Debug.Log show json (Lin. 134)
				GetWebXRDisplayCapabilities() (Lin. 103)
- WebXRController:  TextUI for Debug (in case you want to show the values in some UI in 3D)
					GetAxis() function extra check (Lin. 81). No nullReference errors anymore!
					Add Get2DAxis() to read the stick in browser mode (Lin. 91)
					fixupAngle declaration (Lin. 44) - FixupAngleRotation in WebGL (lin. 244)
					 _t.localPosition = mHandPos - Vector3.up * 1.0f; /**/; (Lin 383) (Hands position correctionwhen using the WebXRMovev2.cs)

 - VRTeleporter: Changing the teleport target point (Lin. 57)


Problems:
- NO antialiasing (if using Oculus Browser or Firefox browser inside Oculus). Activating antialiasing result in black screen.
- Bad render quality.