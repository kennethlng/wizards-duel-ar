Shader "Hidden/MK/CommonFree"
{
	Properties
	{
		//Main
		_Color ("Color", Color) = (1,1,1,0.1)
		_MainTex ("Color (RGB)", 2D) = "white" {}
		_MainTint("Main Tint", Range(0,2)) = 0.0
		[Toggle] _AlbedoMap ("Color source map", int) = 0

		//Normalmap
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Distortion("Distortion", Range(0,1)) = 0.30

		//Specular
		_Shininess ("Shininess",  Range (0.01, 1)) = 0.275
		_SpecColor ("Specular Color", Color) = (1,1,1,0.5)
		_SpecularIntensity("Intensity", Range (0, 2)) = 0.5

		//Emission
		_EmissionColor("Emission Color", Color) = (0,0,0)

		//Rim 
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimSize ("Rim Size", Range(0.0,5.0)) = 2.3
		_RimIntensity("Intensity", Range (0, 1)) = 0.3

		//Editor
		[HideInInspector] _MKEditorShowMainBehavior ("Main Behavior", int) = 1
		[HideInInspector] _MKEditorShowRenderBehavior ("Render Behavior", int) = 0
		[HideInInspector] _MKEditorShowSpecularBehavior ("Specular Behavior", int) = 0
		[HideInInspector] _MKEditorShowRimBehavior ("Rim Behavior", int) = 0
	}
	SubShader
	{
		//single Framebuffer Grab Refraction
		GrabPass {Tags { "LightMode" = "ForwardBase" } Name "GRAB_SHARED_FWD" }

		//Vertex Lit Framebuffer Grab Refraction
		GrabPass {Tags { "LightMode" = "Vertex" }  Name "GRAB_SHARED_VRTLT" }
		GrabPass {Tags { "LightMode" = "VertexLM" }  Name "GRAB_SHARED_VRTLTLM" }
		GrabPass {Tags { "LightMode" = "VertexLMRGBM" } Name "GRAB_SHARED_VRTLTLMRGBM" }

		/////////////////////////////////////////////////////////////////////////////////////////////
		// LEGACY VERTEX LIT PASSES
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Use very basic shader to support very old hardware
		//Try at least if the hardware supports programable shaders... else... fallback to fixed function vertexlit

		Pass
		{
			Tags { "LightMode" = "Vertex" } 
			Name "VERTEXNLM_LIT" 
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 2.0

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertvl
			#pragma fragment fragvl

			#ifndef _MK_VERTEXNLM_LIT
				#define _MK_VERTEXNLM_LIT 1
			#endif

			#include "Inc/VertexLit/MKGlassVertexLitSetup.cginc"
			#include "Inc/VertexLit/MKGlassVertexLit.cginc"
			
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "VertexLM" } 
			Name "VERTEXLM_LIT" 
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 2.0

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertvl
			#pragma fragment fragvl

			#ifndef _MK_VERTEXLM_LIT
				#define _MK_VERTEXLM_LIT 1
			#endif

			#include "Inc/VertexLit/MKGlassVertexLitSetup.cginc"
			#include "Inc/VertexLit/MKGlassVertexLit.cginc"
			
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "VertexLMRGBM" } 
			Name "VERTEXLMRGBM_LIT" 
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 2.0

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertvl
			#pragma fragment fragvl

			#ifndef _MK_VERTEXLMRGBM_LIT
				#define _MK_VERTEXLMRGBM_LIT 1
			#endif

			#include "Inc/VertexLit/MKGlassVertexLitSetup.cginc"
			#include "Inc/VertexLit/MKGlassVertexLit.cginc"
			
			ENDCG
		}
	}
}
