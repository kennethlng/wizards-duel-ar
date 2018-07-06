using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.Utils;
using UnityEditorInternal;

#if UNITY_EDITOR
namespace MK.Glass
{
    public class MKGlassFreeEditor : ShaderGUI
    {
        public static class GuiStyles
        {
            public static GUIStyle header = new GUIStyle("ShurikenModuleTitle")
            {
                font = (new GUIStyle("Label")).font,
                border = new RectOffset(15, 7, 4, 4),
                fixedHeight = 22,
                contentOffset = new Vector2(20f, -2f),
            };

            public static GUIStyle headerCheckbox = new GUIStyle("ShurikenCheckMark");
            public static GUIStyle headerCheckboxMixed = new GUIStyle("ShurikenCheckMarkMixed");
        }

        private static class GUIContentCollection
        {
            public static GUIContent mainColor = new GUIContent("Color", "Basic color tint");
            public static GUIContent mainTint = new GUIContent("Main tint", "Tint amount");
            public static GUIContent specularShininess = new GUIContent("Shininess", "The level of blur for the specular highlight");
            public static GUIContent specularColor = new GUIContent("Color", "Color tint of specular highlights");
            public static GUIContent emissionColor = new GUIContent("Emission", "Tint of the emission");
            public static GUIContent specular = new GUIContent("intensity", "Spec (R) Gloss (G) Aniso (B)");
            public static GUIContent normalMap = new GUIContent("Normal map", "Normal map (Bump)");
            public static GUIContent distortion = new GUIContent("Distortion", "Distortion");
            public static GUIContent mainTex = new GUIContent("Albedo", "Albedo (RGBA)");
            public static GUIContent rimSize = new GUIContent("Size", "Amount of highlighted areas by rim");
            public static GUIContent rimColor = new GUIContent("Color", "Color of the rim highlight");
            public static GUIContent rimIntensity = new GUIContent("Intensity", "Intensity of the rim highlight");
        }

        //hdr config
        private ColorPickerHDRConfig colorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);

        //Editor Properties
        private MaterialProperty showMainBehavior = null;
        private MaterialProperty showRenderBehavior = null;
        private MaterialProperty showSpecularBehavior = null;
        private MaterialProperty showRimBehavior = null;

        //Main
        private MaterialProperty mainColor = null;
        private MaterialProperty mainTex = null;
        private MaterialProperty mainTint = null;

        //Normalmap
        private MaterialProperty normalMap = null;
        private MaterialProperty distortion = null;

        //Specular
        private MaterialProperty specularIntensity = null;
        private MaterialProperty shininess = null;
        private MaterialProperty specularColor = null;

        //Emission
        private MaterialProperty emissionColor = null;

        //Rim
        private MaterialProperty rimColor = null;
        private MaterialProperty rimSize = null;
        private MaterialProperty rimIntensity = null;

        private bool showGIField = false;

        public void FindProperties(MaterialProperty[] props, Material mat)
        {
            //Editor Properties
            showMainBehavior = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SHOW_MAIN_BEHAVIOR, props);
            showRenderBehavior = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SHOW_RENDER_BEHAVIOR, props);
            showSpecularBehavior = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SHOW_SPECULAR_BEHAVIOR, props);
            showRimBehavior = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SHOW_RIM_BEHAVIOR, props);

            //Main
            mainColor = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.MAIN_COLOR, props);
            mainTint = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.MAIN_TINT, props);
            mainTex = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.MAIN_TEXTURE, props);

            //Normalmap
            normalMap = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.BUMP_MAP, props);
            distortion = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.DISTORTION, props);

            //Specular
            shininess = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SPECULAR_SHININESS, props);
            specularColor = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SPEC_COLOR, props);
            specularIntensity = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.SPECULAR_INTENSITY, props);

            //Emission
            emissionColor = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.EMISSION_COLOR, props);

            //Rim
            rimColor = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.RIM_COLOR, props);
            rimSize = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.RIM_SIZE, props);
            rimIntensity = FindProperty(MKGlassFreeMaterialHelper.PropertyNames.RIM_INTENSITY, props);
        }

        //Colorfield
        private void ColorProperty(MaterialProperty prop, bool showAlpha, bool hdrEnabled, GUIContent label)
        {
            EditorGUI.showMixedValue = prop.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            Color c = EditorGUILayout.ColorField(label, prop.colorValue, false, showAlpha, hdrEnabled, colorPickerHDRConfig);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.colorValue = c;
        }

        //Setup GI emission
        private void SetGIFlags()
        {
            foreach (Material obj in emissionColor.targets)
            {
                bool emissive = true;
                if (MKGlassFreeMaterialHelper.GetEmissionColor(obj) == Color.black)
                {
                    emissive = false;
                }
                MaterialGlobalIlluminationFlags flags = obj.globalIlluminationFlags;
                if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0)
                {
                    flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    if (!emissive)
                        flags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;

                    obj.globalIlluminationFlags = flags;
                }
            }
        }

        //BoldToggle
        private void ToggleBold(MaterialEditor materialEditor, MaterialProperty prop)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(prop.displayName, EditorStyles.boldLabel, GUILayout.Width(100));
            materialEditor.ShaderProperty(prop, "");
            EditorGUILayout.EndHorizontal();
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material.HasProperty(MKGlassFreeMaterialHelper.PropertyNames.EMISSION))
            {
                MKGlassFreeMaterialHelper.SetEmissionColor(material, material.GetColor(MKGlassFreeMaterialHelper.PropertyNames.EMISSION));
            }
           
            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            MaterialProperty[] properties = MaterialEditor.GetMaterialProperties(new Material[] { material });
            FindProperties(properties, material);

            SetGIFlags();
        }

        private bool HandleBehavior(string title, ref MaterialProperty behavior, MaterialEditor materialEditor)
        {
            EditorGUI.showMixedValue = behavior.hasMixedValue;
            var rect = GUILayoutUtility.GetRect(16f, 22f, GuiStyles.header);
            rect.x -= 10;
            rect.width += 10;
            var e = Event.current;

            GUI.Box(rect, title, GuiStyles.header);

            var foldoutRect = new Rect(EditorGUIUtility.currentViewWidth * 0.5f, rect.y + 2, 13f, 13f);
            if (behavior.hasMixedValue)
            {
                foldoutRect.x -= 13;
                foldoutRect.y -= 2;
            }

            EditorGUI.BeginChangeCheck();
            if (e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    if (behavior.hasMixedValue)
                        behavior.floatValue = 0.0f;
                    else
                        behavior.floatValue = Convert.ToSingle(!Convert.ToBoolean(behavior.floatValue));
                    e.Use();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (Convert.ToBoolean(behavior.floatValue))
                    materialEditor.RegisterPropertyChangeUndo(behavior.displayName + " Show");
                else
                    materialEditor.RegisterPropertyChangeUndo(behavior.displayName + " Hide");
            }

            EditorGUI.showMixedValue = false;

            if (e.type == EventType.Repaint && behavior.hasMixedValue)
                EditorStyles.radioButton.Draw(foldoutRect, "", false, false, true, false);
            else
                EditorGUI.Foldout(foldoutRect, Convert.ToBoolean(behavior.floatValue), "");

            if (behavior.hasMixedValue)
                return true;
            else
                return Convert.ToBoolean(behavior.floatValue);
        }

        override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material targetMat = materialEditor.target as Material;
            //MeshRenderer[] targetMeshRenderers = materialEditor.targets as MeshRenderer[];
            //get properties
            FindProperties(properties, targetMat);

            if (emissionColor.colorValue != Color.black)
                showGIField = true;
            else
                showGIField = false;

            EditorGUI.BeginChangeCheck();

            //main settings
            if (HandleBehavior("Main", ref showMainBehavior, materialEditor))
            {
                ColorProperty(mainColor, false, false, GUIContentCollection.mainColor);
                materialEditor.TexturePropertySingleLine(GUIContentCollection.mainTex, mainTex, mainTint);
                materialEditor.TexturePropertySingleLine(GUIContentCollection.normalMap, normalMap);

                ColorProperty(emissionColor, false, true, GUIContentCollection.emissionColor);
                if (showGIField)
                    materialEditor.LightmapEmissionProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
                materialEditor.TextureScaleOffsetProperty(mainTex);
            }

            //Render settings
            if (HandleBehavior("Render", ref showRenderBehavior, materialEditor))
            {
                materialEditor.EnableInstancingField();
                if (normalMap.textureValue != null)
                {
                    materialEditor.ShaderProperty(distortion, GUIContentCollection.distortion);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please set up a normal map to use distortion feature", MessageType.Info);
                }
            }

            //specular settings
            if (HandleBehavior("Specular", ref showSpecularBehavior, materialEditor))
            {
                ColorProperty(specularColor, false, false, GUIContentCollection.specularColor);
                materialEditor.ShaderProperty(shininess, GUIContentCollection.specularShininess);
                materialEditor.ShaderProperty(specularIntensity, GUIContentCollection.specular);
            }

            if (HandleBehavior("Rim", ref showRimBehavior, materialEditor))
            {
                ColorProperty(rimColor, false, false, GUIContentCollection.rimColor);
                materialEditor.ShaderProperty(rimSize, GUIContentCollection.rimSize);
                materialEditor.ShaderProperty(rimIntensity, GUIContentCollection.rimIntensity);
            }
            EditorGUI.EndChangeCheck();
        }
    }
}
#endif