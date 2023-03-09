using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomShaderGUI.SurfaceShader
{
    public class SurfaceInputGUI : BaseShaderGUI
    {
        private const string property_name_baseMap = "_BaseMap";
        private const string property_name_baseColor = "_BaseColor";
        private const string property_name_smoothness = "_Smoothness";
        private const string property_name_glossMapScale = "_GlossMapScale";

        private const string property_name_smoothnesstexturechannel = "_SmoothnessTextureChannel";
        private const string property_name_metallic = "_Metallic";
        private const string property_name_metallicGlossMap = "_MetallicGlossMap";

        private const string property_name_specColor = "_SpecColor";
        private const string property_name_specGlossMap = "_SpecGlossMap";

        private const string property_name_bump = "_BumpMap";
        private const string property_name_bumpScale = "_BumpScale";

        private const string property_name_occlusionStrenght = "_OcclusionStrength";
        private const string property_name_occlusionMap = "_OcclusionMap";

        private const string property_name_emissionColor = "_EmissionColor";
        private const string property_name_emissionMap = "_EmissionMap";

        private MaterialProperty _baseMapProp;
        private MaterialProperty _baseColorProp;

        private MaterialProperty _smoothnessProp;
        private MaterialProperty _glossMapScaleProp;
        private MaterialProperty _smoothnessTextureChannelProp;
        private MaterialProperty _metallicProp;
        private MaterialProperty _metallicMapProp;

        private MaterialProperty _specColorProp;
        private MaterialProperty _specGlossMapProp;

        private MaterialProperty _bumpMapProp;
        private MaterialProperty _bumpScaleProp;

        private MaterialProperty _occlusionStrengthProp;
        private MaterialProperty _occlusionMapProp;

        private MaterialProperty _emissionColorProp;
        private MaterialProperty _emissionMapProp;

        private readonly GUIContent guicontent_surfaceInputHeader = new GUIContent("SurfaceInput");
        private readonly GUIContent guicontent_baseMap = new GUIContent("Base Map");
        private readonly GUIContent guicontent_metallic = new GUIContent("Metallic Map");
        private readonly GUIContent guicontent_smoothness = new GUIContent("Smoothness");
        private readonly GUIContent guicontent_source = new GUIContent("Source");
        private readonly GUIContent guicontent_normal = new GUIContent("Normal Map");
        private readonly GUIContent guicontent_occlusion = new GUIContent("Occlusion Map");
        private readonly GUIContent guicontent_emission = new GUIContent("Emission");

        private SaveBool _surfaceInputHeader;
        private MaterialEditor _editor;

        public override void OnGUI()
        {
            base.OnGUI();
            _surfaceInputHeader.Value =
                EditorGUILayout.BeginFoldoutHeaderGroup(_surfaceInputHeader.Value, guicontent_surfaceInputHeader);
            if (_surfaceInputHeader.Value)
            {
                EditorGUI.indentLevel++;
                _editor.TexturePropertySingleLine(guicontent_baseMap, _baseMapProp, _baseColorProp);
                _editor.TexturePropertySingleLine(guicontent_metallic, _metallicMapProp, _metallicProp);
                _editor.ShaderProperty(_smoothnessProp,guicontent_smoothness);

                SurfaceType surfaceType = (SurfaceType)_material.GetFloat("_Surface");
                EditorGUI.BeginDisabledGroup(surfaceType != SurfaceType.Oqapue);
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                var smoothnessMapChannel = (SmoothnessMapChannel)EditorGUILayout.EnumPopup(guicontent_source,
                    (SmoothnessMapChannel) _smoothnessTextureChannelProp.floatValue);
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    _smoothnessTextureChannelProp.floatValue =  (smoothnessMapChannel == SmoothnessMapChannel.SpecularMetallicAlpha) ?0:1;
                    CoreUtils.SetKeyword(_material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
                        smoothnessMapChannel == SmoothnessMapChannel.AlbedoAlpha && surfaceType == SurfaceType.Oqapue);
                }
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.BeginChangeCheck();
                _editor.TexturePropertySingleLine(guicontent_normal, _bumpMapProp, _bumpScaleProp);
                if (EditorGUI.EndChangeCheck())
                {
                    CoreUtils.SetKeyword(_material,"_NORMALMAP",_bumpMapProp.textureValue != null);
                }
                
                EditorGUI.BeginChangeCheck();
                _editor.TexturePropertySingleLine(guicontent_occlusion, _occlusionMapProp, _occlusionStrengthProp);
                if (EditorGUI.EndChangeCheck())
                {
                    CoreUtils.SetKeyword(_material,"_OCCLUSIONMAP",_material.GetTexture(property_name_occlusionMap));
                }
                var emmiss = _editor.EmissionEnabledProperty();
                EditorGUI.BeginDisabledGroup(!emmiss);
                EditorGUI.BeginChangeCheck();
                _editor.TexturePropertyWithHDRColor(guicontent_emission, _emissionMapProp, _emissionColorProp,false);
                var brightness = _emissionColorProp.colorValue.maxColorComponent;
                if (_emissionMapProp.textureValue != null && brightness <= 0f)
                    _emissionColorProp.colorValue = Color.white; 
                if (emmiss)
                {
                    var oldFlags = _material.globalIlluminationFlags;
                    var newFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

                    if (brightness <= 0f)
                        newFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;

                    if (newFlags != oldFlags)
                        _material.globalIlluminationFlags = newFlags;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    bool shouldEmissionBeEnabled =
                        (_material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
                    if (_material.HasProperty("_EmissionEnabled") && !shouldEmissionBeEnabled)
                        shouldEmissionBeEnabled = _material.GetFloat("_EmissionEnabled") >= 0.5f;
                    CoreUtils.SetKeyword(_material, "_EMISSION", shouldEmissionBeEnabled);

                }
                EditorGUI.EndDisabledGroup();
                _editor.TextureScaleOffsetProperty(_baseMapProp);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public SurfaceInputGUI(MaterialEditor editor, Material material, MaterialProperty[] properties) : base(material,
            properties)
        {
            this._editor = editor;
            _surfaceInputHeader = new SaveBool(_material.shader.name + "_Surface", true);
            _baseMapProp = FindProperty(property_name_baseMap, properties);
            _baseColorProp = FindProperty(property_name_baseColor, properties);

            _metallicMapProp = FindProperty(property_name_metallicGlossMap, properties);
            _metallicProp = FindProperty(property_name_metallic, properties);
            _smoothnessProp = FindProperty(property_name_smoothness, properties);

            _smoothnessTextureChannelProp = FindProperty(property_name_smoothnesstexturechannel, properties);

            _bumpMapProp = FindProperty(property_name_bump, properties);
            _bumpScaleProp = FindProperty(property_name_bumpScale, properties);

            _occlusionMapProp = FindProperty(property_name_occlusionMap,properties);
            _occlusionStrengthProp = FindProperty(property_name_occlusionStrenght, properties);

            _emissionMapProp = FindProperty(property_name_emissionMap, properties);
            _emissionColorProp = FindProperty(property_name_emissionColor, properties);
        }
    }
}