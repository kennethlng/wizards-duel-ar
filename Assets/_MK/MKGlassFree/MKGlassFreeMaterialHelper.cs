using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.Glass
{
    public static class MKGlassFreeMaterialHelper
    {
        public static class PropertyNames
        {
            //Editor Properties
            public const string SHOW_MAIN_BEHAVIOR = "_MKEditorShowMainBehavior";
            public const string SHOW_LIGHT_BEHAVIOR = "_MKEditorShowLightBehavior";
            public const string SHOW_RENDER_BEHAVIOR = "_MKEditorShowRenderBehavior";
            public const string SHOW_SPECULAR_BEHAVIOR = "_MKEditorShowSpecularBehavior";
            public const string SHOW_RIM_BEHAVIOR = "_MKEditorShowRimBehavior";

            //Main
            public const string MAIN_TEXTURE = "_MainTex";
            public const string MAIN_COLOR = "_Color";
            public const string MAIN_TINT = "_MainTint";

            //Normalmap
            public const string BUMP_MAP = "_BumpMap";
            public const string DISTORTION = "_Distortion";

            //Rim
            public const string RIM_COLOR = "_RimColor";
            public const string RIM_SIZE = "_RimSize";
            public const string RIM_INTENSITY = "_RimIntensity";

            //Specular
            public const string SPECULAR_SHININESS = "_Shininess";
            public const string SPEC_COLOR = "_SpecColor";
            public const string SPECULAR_INTENSITY = "_SpecularIntensity";

            //Emission
            public const string EMISSION_COLOR = "_EmissionColor";
            public const string EMISSION = "_Emission";
        }

        //Main
        public static void SetMainTint(Material material, float tint)
        {
            material.SetFloat(PropertyNames.MAIN_TINT, tint);
        }
        public static float GetMainTint(Material material)
        {
            return material.GetFloat(PropertyNames.MAIN_TINT);
        }

        public static void SetMainTexture(Material material, Texture tex)
        {
            material.SetTexture(PropertyNames.MAIN_TEXTURE, tex);
        }
        public static Texture GetMainTexture(Material material)
        {
            return material.GetTexture(PropertyNames.MAIN_TEXTURE);
        }

        public static void SetMainColor(Material material, Color color)
        {
            material.SetColor(PropertyNames.MAIN_COLOR, color);
        }
        public static Color GetMainColor(Material material)
        {
            return material.GetColor(PropertyNames.MAIN_COLOR);
        }
        
        //Normalmap
        public static void SetNormalmap(Material material, Texture tex)
        {
            material.SetTexture(PropertyNames.BUMP_MAP, tex);
        }
        public static Texture GetBumpMap(Material material)
        {
            return material.GetTexture(PropertyNames.BUMP_MAP);
        }

        public static void SetDistortion(Material material, float distortion)
        {
            material.SetFloat(PropertyNames.DISTORTION, distortion);
        }
        public static float GetDistortion(Material material)
        {
            return material.GetFloat(PropertyNames.DISTORTION);
        }

        //Rim
        public static void SetRimColor(Material material, Color color)
        {
            material.SetColor(PropertyNames.RIM_COLOR, color);
        }
        public static Color GetRimColor(Material material)
        {
            return material.GetColor(PropertyNames.RIM_COLOR);
        }

        public static void SetRimSize(Material material, float size)
        {
            material.SetFloat(PropertyNames.RIM_SIZE, size);
        }
        public static float GetRimSize(Material material)
        {
            return material.GetFloat(PropertyNames.RIM_SIZE);
        }

        public static void SetRimIntensity(Material material, float intensity)
        {
            material.SetFloat(PropertyNames.RIM_INTENSITY, intensity);
        }
        public static float GetRimIntensity(Material material)
        {
            return material.GetFloat(PropertyNames.RIM_INTENSITY);
        }

        //Specular
        public static void SetSpecularShininess(Material material, float shininess)
        {
            material.SetFloat(PropertyNames.SPECULAR_SHININESS, shininess);
        }
        public static float GetSpecularShininess(Material material)
        {
            return material.GetFloat(PropertyNames.SPECULAR_SHININESS);
        }

        public static void SetSpecularColor(Material material, Color color)
        {
            material.SetColor(PropertyNames.SPEC_COLOR, color);
        }
        public static Color GetSpecularColor(Material material)
        {
            return material.GetColor(PropertyNames.SPEC_COLOR);
        }

        public static void SetSpecularIntensity(Material material, float intensity)
        {
            material.SetFloat(PropertyNames.SPECULAR_INTENSITY, intensity);
        }
        public static float GetSpecularIntensity(Material material)
        {
            return material.GetFloat(PropertyNames.SPECULAR_INTENSITY);
        }

        //Emission
        public static void SetEmissionColor(Material material, Color color)
        {
            material.SetColor(PropertyNames.EMISSION_COLOR, color);
        }
        public static Color GetEmissionColor(Material material)
        {
            return material.GetColor(PropertyNames.EMISSION_COLOR);
        }
    }
}