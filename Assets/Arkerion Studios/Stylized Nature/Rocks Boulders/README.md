ARKERION SHADERS - USAGE GUIDE

––––––––––––––––––––––––––––––––––––––––––



ArkerionHardSurface (URP)

Unity version: 2022.x
 Render Pipeline: URP
 Shader type: Lit (Shader Graph)

Contents
Top-projected moss shader (world-space)
 Slope-based moss masking
 Color or texture-based moss
 Edge-softened moss blending
 Detail normal support
 Global moss override system
 Separate surface smoothness control

Description
Moss Projection Shader for Unity URP, inspired by the Unreal Engine reference material.
 Designed to add moss to upward-facing and sloped surfaces with flexible artistic control.
The shader is intended for environment assets such as rocks, trees, cliffs, stumps, and terrain props.
 It is fully material-instance driven and does not require per-object setup.

Key Features
Top-down moss projection (world-space)
 Slope-based masking (top & angle control)
 Switch between moss color and moss texture
 Edge softening for natural moss borders
 Detail normal map with tiling & offset
 Separate smoothness control for base and moss
 Optional global override (affects all materials)
 No custom scripts required for basic usage

Shader Parameters
Moss Control
MossEnabled - Enables or disables moss
 TopStrength - Overall influence of top projection
 TopSharpness - Sharpness of the top mask
 SlopeMin / SlopeMax - Controls slope range where moss appears
 EdgeSoftness - Softens moss edges to avoid hard cutoffs
Moss Appearance
MossColor - Tint color for moss
 MossTex - Optional moss texture
 MossUseTexture - Switch between color or texture
 MossTexScale - Moss texture tiling
 MossBlend - Blending strength with base material
Normal & Detail
Base Normal - Uses original model normal together with moss and detail normals
 BaseNormalStrength - Intensity of base normal before blending
 DetailNormal - Detail normal map for moss
 DetailTiling - UV tiling for detail normal
 DetailOffset - UV offset for detail normal
 DetailStrength - Intensity of detail normal
Surface Properties
BaseSmoothness - Smoothness of base material
 MossSmoothness - Smoothness of moss layer

Global Override
The shader supports global parameters, allowing you to:
Enable or disable moss globally
 Override moss color or moss texture across all materials
 Perform fast art-direction changes or seasonal variations

How to Use
Assign the shader to a Lit material
 Apply the material to environment meshes
 Adjust top projection and slope values
 Choose between moss color or moss texture
 Tune blending, normals, and smoothness as needed

Usage Notes
Works best on assets with properly oriented normals
 Moss texture is optional - color-only mode is fully supported
 Designed for URP Lit workflow
 Fully material-instance driven




––––––––––––––––––––––––––––––––––––––––––




ArkerionProceduralSkybox (URP, Unity 2022)

Unity version: 2022 LTS (URP)
Render Pipeline: URP
Shader type: Shader Graph (Custom Mesh Sky)


Contents
Custom stylized sky solution using a mesh instead of Unity Skybox
Procedural sky gradient
Procedural sun and glow
Procedural stars
Procedural animated clouds
Runtime sun synchronization via helper script

Key Features

Fully procedural (no baked skybox)
Mesh-based sky (no Unity Skybox dependency)
Artist-friendly material parameters
Performance-friendly design

Includes:
Sky Gradient
Sun
Stars
Procedural Clouds

What's Included

Sky Gradient
Procedural vertical sky gradient with customizable colors.

Sun
Procedural sun disc and glow.
Sun position is driven by the Directional Light.
Can be enabled or disabled.

Stars
Procedural star field.
Can be enabled or disabled independently.

Procedural Clouds
Fully procedural (no baked skybox).
Animated and layered for depth.
Designed to blend naturally with the sky.
Can be faded out or fully disabled via intensity control.

Runtime Sun Synchronization (Script)

The project includes a small helper script attached to a GameObject in the scene.

Purpose:
Automatically synchronizes the sky shader with the active Directional Light.
Updates sun direction in the shader in real time.

Notes:
Works in Play Mode and Edit Mode.
Requires:

* Reference to the sky material.
* A Directional Light (or RenderSettings.sun).
  No expensive per-frame operations.

Rotating the Directional Light automatically updates the sun position in the sky.

How to use

Assign the sky material to the provided mesh.
Assign a Directional Light.
Ensure the sun synchronization script is active.
Adjust visual settings via the material inspector.

Most parameters are visually driven and safe to tweak.

Technical Notes

Uses a custom curved mesh instead of Unity Skybox.
Built for URP.
Designed to work without HDR by default for better performance.
HDR can be enabled if higher color depth is required.

Performance

No volumetric rendering.
No raymarching.
No post-processing dependency.
Suitable for PC and mobile-friendly setups.

Notes

This shader was built iteratively and visually tuned.
Not every internal parameter is documented by design.
The system is meant to be artist-friendly and exploratory.



