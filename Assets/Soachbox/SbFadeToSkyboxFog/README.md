# Sb FadeToSkyboxFog

## Overview
A fog effect that fades out to the skybox color the according to fog density.
This asset targets for URP 17.0 or higher.

## Demo Setting
To check the demo scene, you need to change the Render Pipeline Asset setting.
Open Quality settings (Edit menu -> Project Settings -> Quality),
and then set "Render Pipeline Asset" to "FadeToSkyboxDemoURPAsset".

## Basic Usage
After install package, select "Tools -> FadeToSkyboxFog > Open MainCamera Renderer Data" from menu.
Current main camera's renderer setting will open in inspector.

Click "Add Renderer Feature" button below and select "Sb Fade To Skybox Fog".
The fog feature will be attached.

Now you can now use the "Sb Fade To Skybox Sub Graph" sub graph in Shader Graph.
Or to use full-screen fog effect, enable "Enable Screen Fog".

Adjust "Fog Type" and other parameters to suit your preferences.

---
For more information about parameters, check **Documentation.pdf**.