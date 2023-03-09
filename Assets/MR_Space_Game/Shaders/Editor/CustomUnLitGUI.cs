using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomShaderGUI.SurfaceShader
{
    public class CustomUnLitGUI : ShaderGUI
    {
        private const string surfaceOptionHeader = "SurfaceOption";

        private readonly GUIContent guicontent_surfaceOption = new GUIContent("SurfaceOption");
        private readonly GUIContent guicontent_surfaceType = new GUIContent("Surface Type");
        private readonly GUIContent guicontent_blendMode = new GUIContent("Blend Mode");
        private readonly GUIContent guicontent_alphaTest = new GUIContent("Alpha Clip");
        private readonly GUIContent guicontent_alphaValue = new GUIContent("Threshold");
        private readonly GUIContent guicontent_cullMode = new GUIContent("Cull Mode");
        private readonly GUIContent guicontent_surfaceInputHeader = new GUIContent("SurfaceInput");
        private readonly GUIContent guicontent_baseMap = new GUIContent("Base Map");
        
        
        
        private const string property_name_baseMap = "_BaseMap";
        private const string property_name_baseColor = "_BaseColor";
        private const string property_name_surfaceType = "_Surface";
        private const string property_name_blend = "_Blend";
        private const string property_name_alphaClip = "_AlphaClip";
        private const string property_name_alphaValue = "_Cutoff";
        private const string property_name_srcBlend = "_SrcBlend";
        private const string property_name_destBlend = "_DstBlend";
        private const string property_name_zwrite = "_ZWrite";
        private const string property_name_cull = "_Cull";


        
        private MaterialProperty _surfaceTypeProp;
        private MaterialProperty _blendProp;
        private MaterialProperty _alphaClipProp;
        private MaterialProperty _alphaValueProp;
        private MaterialProperty _srcBlendProp;
        private MaterialProperty _dstBlendProp;
        private MaterialProperty _cullModeProp;
        private MaterialProperty _baseMapProp;
        private MaterialProperty _baseColorProp;
        
        private SaveBool _surfaceInputHeader;
        private SaveBool _surfaceHeader;
        private SurfaceType _surfaceType = SurfaceType.Oqapue;
        private BlendMode _blendMode = BlendMode.Alpha;
        private bool _alphaClip = false;
        private float _alphaValue = 0.0f;
        private CullMode _cullMode = CullMode.Back;
        private UnityEngine.Rendering.BlendMode _srcBlend = UnityEngine.Rendering.BlendMode.One;
        private UnityEngine.Rendering.BlendMode _dstBlend = UnityEngine.Rendering.BlendMode.Zero;
        
        
        private StencilOptionGUI _stencilOptionGUI;

        private bool _initinalized = false;
        
        private Material _material;
        private MaterialEditor _materialEditor;
        private MaterialProperty[] _properties;
        private void DrawShaderGUI()
        {
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
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (EditorGUI.EndChangeCheck())
            {
                _surfaceTypeProp.floatValue = (float)_surfaceType;
                _blendProp.floatValue = (float) _blendMode;
                _cullModeProp.floatValue = (float) _cullMode;
                _alphaClipProp.floatValue =  _alphaClip?1:0;
                _alphaValueProp.floatValue = _alphaValue;
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
                }
                CoreUtils.SetKeyword(_material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", false);
            }
            DrawSurfaceInput();
            _stencilOptionGUI.OnGUI();
        }

        void DrawSurfaceInput()
        {
            _surfaceInputHeader.Value =
                EditorGUILayout.BeginFoldoutHeaderGroup(_surfaceInputHeader.Value, guicontent_surfaceInputHeader);
            if (_surfaceInputHeader.Value)
            {
                EditorGUI.indentLevel++;
                _materialEditor.TexturePropertySingleLine(guicontent_baseMap, _baseMapProp, _baseColorProp);
                _materialEditor.TextureScaleOffsetProperty(_baseMapProp);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void Initialize()
        {
            _stencilOptionGUI = new StencilOptionGUI(_material,_properties);
            _surfaceHeader = new SaveBool(_material.shader + "SurfaceOption", true);
            _surfaceTypeProp = FindProperty(property_name_surfaceType, _properties);
            _blendProp = FindProperty(property_name_blend, _properties);
            _alphaClipProp = FindProperty(property_name_alphaClip, _properties);
            _alphaValueProp = FindProperty(property_name_alphaValue, _properties);
            _srcBlendProp = FindProperty(property_name_srcBlend, _properties);
            _dstBlendProp = FindProperty(property_name_destBlend, _properties);
            _cullModeProp = FindProperty(property_name_cull, _properties);

            _surfaceType = (SurfaceType) _surfaceTypeProp.floatValue;
            _blendMode = (BlendMode) _blendProp.floatValue;
            _alphaClip = _alphaClipProp.floatValue == 0 ? false : true;
            _alphaValue = _alphaValueProp.floatValue;
            _cullMode = (CullMode) _cullModeProp.floatValue;
            _srcBlend = (UnityEngine.Rendering.BlendMode) _srcBlendProp.floatValue;
            _dstBlend = (UnityEngine.Rendering.BlendMode) _dstBlendProp.floatValue;
            
            _surfaceInputHeader = new SaveBool(_material.shader.name + "_Surface", true);
            _baseMapProp = FindProperty(property_name_baseMap, _properties);
            _baseColorProp = FindProperty(property_name_baseColor, _properties);

        }
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (!_initinalized)
            {
                _materialEditor = materialEditor;
                _material = _materialEditor.target as Material;
                _properties = properties;
                Initialize();
                _initinalized = true;
            }
            DrawShaderGUI();
        }
    }
}