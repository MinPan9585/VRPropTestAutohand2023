using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace CustomShaderGUI.SurfaceShader
{
    public sealed class SurfaceOptionGUI : BaseShaderGUI
    {
        private const string surfaceOptionHeader = "SurfaceOption";

        private readonly GUIContent guicontent_surfaceOption = new GUIContent("SurfaceOption");
        private readonly GUIContent guicontent_surfaceType = new GUIContent("Surface Type");
        private readonly GUIContent guicontent_blendMode = new GUIContent("Blend Mode");
        private readonly GUIContent guicontent_alphaTest = new GUIContent("Alpha Clip");
        private readonly GUIContent guicontent_alphaValue = new GUIContent("Threshold");
        private readonly GUIContent guicontent_cullMode = new GUIContent("Cull Mode");
        private readonly GUIContent guicontent_receiveShadow = new GUIContent("ReceiveShadows");

        private const string property_name_surfaceType = "_Surface";
        private const string property_name_blend = "_Blend";
        private const string property_name_alphaClip = "_AlphaClip";
        private const string property_name_alphaValue = "_AlphaValue";
        private const string property_name_srcBlend = "_SrcBlend";
        private const string property_name_destBlend = "_DstBlend";
        private const string property_name_zwrite = "_ZWrite";
        private const string property_name_cull = "_Cull";
        private const string property_name_receiveShadows = "_ReceiveShadows";


        private MaterialProperty _surfaceTypeProp;
        private MaterialProperty _blendProp;
        private MaterialProperty _alphaClipProp;
        private MaterialProperty _alphaValueProp;
        private MaterialProperty _srcBlendProp;
        private MaterialProperty _dstBlendProp;
        private MaterialProperty _cullModeProp;
        private MaterialProperty _receiveShadowsProp;


        private SaveBool _surfaceHeader;
        private SurfaceType _surfaceType = SurfaceType.Oqapue;
        private BlendMode _blendMode = BlendMode.Alpha;
        private bool _alphaClip = false;
        private float _alphaValue = 0.0f;
        private CullMode _cullMode = CullMode.Back;
        private UnityEngine.Rendering.BlendMode _srcBlend = UnityEngine.Rendering.BlendMode.One;
        private UnityEngine.Rendering.BlendMode _dstBlend = UnityEngine.Rendering.BlendMode.Zero;
        private bool _receiveShadows;
        public SurfaceOptionGUI(Material material, MaterialProperty[] properties) : base(material, properties)
        {
            _surfaceHeader = new SaveBool(_material.shader + "SurfaceOption", true);
            _surfaceTypeProp = FindProperty(property_name_surfaceType, properties);
            _blendProp = FindProperty(property_name_blend, properties);
            _alphaClipProp = FindProperty(property_name_alphaClip, properties);
            _alphaValueProp = FindProperty(property_name_alphaValue, properties);
            _srcBlendProp = FindProperty(property_name_srcBlend, properties);
            _dstBlendProp = FindProperty(property_name_destBlend, properties);
            _cullModeProp = FindProperty(property_name_cull, properties);
            _receiveShadowsProp = FindProperty(property_name_receiveShadows,properties);

            _surfaceType = (SurfaceType) _surfaceTypeProp.floatValue;
            _blendMode = (BlendMode) _blendProp.floatValue;
            _alphaClip = _alphaClipProp.floatValue == 0 ? false : true;
            _alphaValue = _alphaValueProp.floatValue;
            _cullMode = (CullMode) _cullModeProp.floatValue;
            _srcBlend = (UnityEngine.Rendering.BlendMode) _srcBlendProp.floatValue;
            _dstBlend = (UnityEngine.Rendering.BlendMode) _dstBlendProp.floatValue;
            _receiveShadows = _receiveShadowsProp.floatValue == 1 ? true : false;
        }

        public override void OnGUI()
        {
            base.OnGUI();
            EditorGUI.BeginChangeCheck();
            _surfaceHeader.Value =
                EditorGUILayout.BeginFoldoutHeaderGroup(_surfaceHeader.Value, guicontent_surfaceOption);
            if (_surfaceHeader.Value)
            {
                EditorGUI.indentLevel++;
                _surfaceType = (SurfaceType) EditorGUILayout.EnumPopup(guicontent_surfaceType, _surfaceType);

                if (_surfaceType == SurfaceType.Transparent)
                {
                    EditorGUI.indentLevel++;
                    _blendMode = (BlendMode) EditorGUILayout.EnumPopup(guicontent_blendMode, _blendMode);
                    EditorGUI.indentLevel--;
                }

                _cullMode = (CullMode) EditorGUILayout.EnumPopup(guicontent_cullMode, _cullMode);
                _alphaClip = EditorGUILayout.Toggle(guicontent_alphaTest, _alphaClip);
                if (_alphaClip)
                {
                    EditorGUI.indentLevel++;
                    _alphaValue = EditorGUILayout.Slider(guicontent_alphaValue, _alphaValue, 0, 1);
                    EditorGUI.indentLevel--;
                }

                _receiveShadows = EditorGUILayout.Toggle(guicontent_receiveShadow,_receiveShadows);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                _surfaceTypeProp.floatValue = (float)_surfaceType;
                _blendProp.floatValue = (float) _blendMode;
                _cullModeProp.floatValue = (float) _cullMode;
                _alphaClipProp.floatValue =  _alphaClip?1:0;
                _alphaValueProp.floatValue = _alphaValue;
                _receiveShadowsProp.floatValue = _receiveShadows ? 1 : 0;
                _material.doubleSidedGI = (CullMode)_cullModeProp.floatValue != CullMode.Back;
                
                if (_alphaClip)
                    _material.EnableKeyword("_ALPHATEST_ON");
                else
                    _material.DisableKeyword("_ALPHATEST_ON");

                if (_surfaceType == SurfaceType.Oqapue)
                {
                    if (_alphaClip)
                    {
                        _material.renderQueue = (int)RenderQueue.AlphaTest;
                        _material.SetOverrideTag("RenderType", "TransparentCutout");
                    }
                    else
                    {
                        _material.renderQueue = (int)RenderQueue.Geometry;
                        _material.SetOverrideTag("RenderType", "Opaque");
                    }
                    _material.SetFloat(property_name_srcBlend,(float)UnityEngine.Rendering.BlendMode.One);
                    _material.SetFloat(property_name_destBlend,(float)UnityEngine.Rendering.BlendMode.Zero);
                    _material.SetFloat(property_name_zwrite,1.0f);
                    _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    _material.SetShaderPassEnabled("ShadowCaster", true);
                }
                else
                {
                    switch (_blendMode)
                    {
                        case BlendMode.Alpha:
                            _material.SetFloat(property_name_srcBlend,(float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            _material.SetFloat(property_name_destBlend,(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            break;
                        case BlendMode.Additive:
                            _material.SetFloat(property_name_srcBlend, (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            _material.SetFloat(property_name_destBlend, (float)UnityEngine.Rendering.BlendMode.One);
                            _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                            break;
                        case BlendMode.Multiply:
                            _material.SetFloat(property_name_srcBlend, (float)UnityEngine.Rendering.BlendMode.DstColor);
                            _material.SetFloat(property_name_destBlend, (float)UnityEngine.Rendering.BlendMode.Zero);
                            _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                            _material.EnableKeyword("_ALPHAMODULATE_ON");
                            break;
                        case BlendMode.Premultiply:
                            _material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                            _material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            _material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                            break;
                    }
                    _material.SetFloat(property_name_zwrite,0);
                    _material.SetOverrideTag("RenderType", "Transparent");
                    _material.renderQueue = (int)RenderQueue.Transparent;
                    _material.SetShaderPassEnabled("ShadowCaster", false);
                }
                CoreUtils.SetKeyword(_material, "_RECEIVE_SHADOWS_OFF", !_receiveShadows);
                CoreUtils.SetKeyword(_material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", false);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}