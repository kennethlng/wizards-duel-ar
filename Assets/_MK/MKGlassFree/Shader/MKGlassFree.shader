Shader "MK/Glass/Free"
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

	/////////////////////////////////////////////////////////////////////////////////////////////
	// SM 3.0
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader
	{
		LOD 300
		Tags {"RenderType"="Transparent" "Queue"="Transparent+21" "PerformanceChecks"="False" "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Grab Refraction
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Get shared refraction
		UsePass "Hidden/MK/CommonFree/GRAB_SHARED_FWD"

		/////////////////////////////////////////////////////////////////////////////////////////////
		// VERTEX LIT
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Get Vertex Lit from Fallback shader

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD BASE
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardBase" } 
			Name "FORWARDBASE" 
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertfwd
			#pragma fragment fragfwd

			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile_instancing

			#include "Inc/Forward/MKGlassForwardBaseSetup.cginc"
			#include "Inc/Forward/MKGlassForward.cginc"
			
			ENDCG
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" "PerformanceChecks"="False"} 
			Name "FORWARDADD"
			Cull Back
			Blend SrcAlpha One
			ZWrite Off
			ZTest LEqual
			Fog { Color (0,0,0,0) }

			CGPROGRAM
			#pragma target 3.0

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertfwd
			#pragma fragment fragfwd
			#pragma multi_compile_fwdadd_fullshadows

			#pragma multi_compile_fog

			#include "Inc/Forward/MKGlassForwardAddSetup.cginc"
			#include "Inc/Forward/MKGlassForward.cginc"
			
			ENDCG
		}

		//TODO deferred shading pass
		/////////////////////////////////////////////////////////////////////////////////////////////
		// DEFERRED
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////


		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode"="Meta" "PerformanceChecks"="False"}
			Name "META" 

			Cull Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex metavert
			#pragma fragment metafrag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature __ EDITOR_VISUALIZATION

			#include "Inc/Meta/MKGlassMetaSetup.cginc"
			#include "Inc/Meta/MKGlassMeta.cginc"
			ENDCG
		}
    }


	/////////////////////////////////////////////////////////////////////////////////////////////
	// SM 2.5 - Mobile
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader
	{
		LOD 150
		Tags {"RenderType"="Transparent" "Queue"="Transparent+21" "PerformanceChecks"="False" "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Grab Refraction
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Get shared refraction
		UsePass "Hidden/MK/CommonFree/GRAB_SHARED_FWD"

		/////////////////////////////////////////////////////////////////////////////////////////////
		// VERTEX LIT
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Get Vertex Lit from Fallback shader

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD BASE
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardBase" } 
			Name "FORWARDBASE" 
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 2.5

			#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertfwd
			#pragma fragment fragfwd

			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase

			#include "Inc/Forward/MKGlassForwardBaseSetup.cginc"
			#include "Inc/Forward/MKGlassForward.cginc"
			
			ENDCG
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" "PerformanceChecks"="False"} 
			Name "FORWARDADD"
			Cull Back
			Blend SrcAlpha One
			ZWrite Off
			ZTest LEqual
			Fog { Color (0,0,0,0) }

			CGPROGRAM
			#pragma target 2.5

			#pragma skip_variants SHADOWS_SOFT

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertfwd
			#pragma fragment fragfwd
			#pragma multi_compile_fwdadd_fullshadows

			#pragma multi_compile_fog

			#include "Inc/Forward/MKGlassForwardAddSetup.cginc"
			#include "Inc/Forward/MKGlassForward.cginc"
			
			ENDCG
		}

		//TODO deferred shading pass
		/////////////////////////////////////////////////////////////////////////////////////////////
		// DEFERRED
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Skip shadow pass because dithering requires shader model 3.0

		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode"="Meta" "PerformanceChecks"="False"}
			Name "META" 

			Cull Off

			CGPROGRAM
			#pragma target 2.5
			#pragma vertex metavert
			#pragma fragment metafrag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature __ EDITOR_VISUALIZATION

			#include "Inc/Meta/MKGlassMetaSetup.cginc"
			#include "Inc/Meta/MKGlassMeta.cginc"
			ENDCG
		}
    }

	/////////////////////////////////////////////////////////////////////////////////////////////
	// SM 2.0 Very Old - Skip some features
	/////////////////////////////////////////////////////////////////////////////////////////////
	SubShader
	{
		LOD 150
		Tags {"RenderType"="Transparent" "Queue"="Transparent+21" "PerformanceChecks"="False" "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Grab Refraction
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Get shared refraction
		UsePass "Hidden/MK/CommonFree/GRAB_SHARED_FWD"
		UsePass "Hidden/MK/CommonFree/GRAB_SHARED_VRTLT"
		UsePass "Hidden/MK/CommonFree/GRAB_SHARED_VRTLTLM"
		UsePass "Hidden/MK/CommonFree/GRAB_SHARED_VRTLTLMRGBM"

		/////////////////////////////////////////////////////////////////////////////////////////////
		// VERTEX LIT
		/////////////////////////////////////////////////////////////////////////////////////////////
		UsePass "Hidden/MK/CommonFree/VERTEXNLM_LIT"
		UsePass "Hidden/MK/CommonFree/VERTEXLM_LIT"
		UsePass "Hidden/MK/CommonFree/VERTEXLMRGBM_LIT"

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD BASE
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardBase" } 
			Name "FORWARDBASE" 
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma target 2.0

			#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertfwd
			#pragma fragment fragfwd

			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase

			#include "Inc/Forward/MKGlassForwardBaseSetup.cginc"
			#include "Inc/Forward/MKGlassForward.cginc"
			
			ENDCG
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// FORWARD ADD
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" "PerformanceChecks"="False"} 
			Name "FORWARDADD"
			Cull Back
			Blend SrcAlpha One
			ZWrite Off
			ZTest LEqual
			Fog { Color (0,0,0,0) }

			CGPROGRAM
			#pragma target 2.0

			#pragma skip_variants SHADOWS_SOFT

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertfwd
			#pragma fragment fragfwd
			#pragma multi_compile_fwdadd_fullshadows

			#pragma multi_compile_fog

			#include "Inc/Forward/MKGlassForwardAddSetup.cginc"
			#include "Inc/Forward/MKGlassForward.cginc"
			
			ENDCG
		}

		//TODO deferred shading pass
		/////////////////////////////////////////////////////////////////////////////////////////////
		// DEFERRED
		/////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////
		// SHADOWCASTER
		/////////////////////////////////////////////////////////////////////////////////////////////
		//Skip shadow pass because dithering requires shader model 3.0

		/////////////////////////////////////////////////////////////////////////////////////////////
		// META
		/////////////////////////////////////////////////////////////////////////////////////////////
		Pass
		{
			Tags { "LightMode"="Meta" "PerformanceChecks"="False"}
			Name "META" 

			Cull Off

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex metavert
			#pragma fragment metafrag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature __ EDITOR_VISUALIZATION

			#include "Inc/Meta/MKGlassMetaSetup.cginc"
			#include "Inc/Meta/MKGlassMeta.cginc"
			ENDCG
		}
    }
	FallBack "Legacy Shaders/Transparent/Diffuse"
	CustomEditor "MK.Glass.MKGlassFreeEditor"
}