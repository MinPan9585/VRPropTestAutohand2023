using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomShaderGUI.SurfaceShader
{
    public class StencilOptionGUI : BaseShaderGUI
    {
        private const string property_name_stencilRef = "_StencilRef";
        private const string property_name_stencilComp = "_StencilComp";
        private const string property_name_stencilOp = "_StencilOp";

        private readonly GUIContent guicontent_stencilOption_header = new GUIContent("StencilOption");
        private readonly GUIContent guicontent_stencilRef = new GUIContent("StencilRef");
        private readonly GUIContent guicontent_stencilComp = new GUIContent("StencilComp");
        private readonly GUIContent guicontent_stencilOp = new GUIContent("StencilOp");


        private MaterialProperty _stencilRefProp;
        private MaterialProperty _stencilCompProp;
        private MaterialProperty _stencilOpProp;

        private float _steniclRef;
        private CompareFunction _stencilComp;
        private StencilOp _stencilOp;

        private bool _foldOut = false;
        private SaveBool _saveBool;
        private SaveBool _enabled;

        public override void OnGUI()
        {
            base.OnGUI();

            EditorGUI.BeginChangeCheck();
            _foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_foldOut, guicontent_stencilOption_header);
            if (_foldOut)
            {
                EditorGUI.indentLevel++;
                _enabled.Value = EditorGUILayout.Toggle(new GUIContent("Enabled"), _enabled.Value);
                GUI.enabled = _enabled.Value;

                _steniclRef = EditorGUILayout.FloatField(guicontent_stencilRef, _steniclRef);
                _stencilComp = (CompareFunction) EditorGUILayout.EnumPopup(guicontent_stencilComp, _stencilComp);
                _stencilOp = (StencilOp) EditorGUILayout.EnumPopup(guicontent_stencilOp, _stencilOp);

                GUI.enabled = true;
                EditorGUI.indentLevel--;
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _saveBool.Value = _foldOut;
                if ( _enabled.Value)
                {
                    //写入的材质球时需要使用Float，int写入会失败
                    _material.SetFloat(property_name_stencilRef, _steniclRef);
                    _material.SetFloat(property_name_stencilComp, (float) _stencilComp);
                    _material.SetFloat(property_name_stencilOp, (float) _stencilOp);
                }else{
                    _material.SetFloat(property_name_stencilRef, 0);
                    _material.SetFloat(property_name_stencilComp, (float) CompareFunction.Always);
                    _material.SetFloat(property_name_stencilOp, (float) StencilOp.Keep);
                }
            }
        }

        public StencilOptionGUI(Material material, MaterialProperty[] properties) : base(material, properties)
        {
            _stencilRefProp = FindProperty(property_name_stencilRef, properties);
            _stencilCompProp = FindProperty(property_name_stencilComp, properties);
            _stencilOpProp = FindProperty(property_name_stencilOp, properties);

            _steniclRef = _stencilRefProp.floatValue;
            _stencilComp = (CompareFunction) _stencilCompProp.floatValue;
            _stencilOp = (StencilOp) _stencilOpProp.floatValue;

            _saveBool = new SaveBool(_material.shader.name + "_stencilOption", false);
            _foldOut = _saveBool.Value;

            _enabled = new SaveBool(_material.shader.name + "_stencilOption_enabled", false);
        }
    }
}