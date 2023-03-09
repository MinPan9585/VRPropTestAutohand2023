using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CustomShaderGUI.SurfaceShader;

namespace CustomShaderGUI
{
    public interface IShaderGUI
    {
        bool enabled { get; set; }
        void OnGUI();
    }

    public enum SurfaceType
    {
        Oqapue,
        Transparent,
    }
    public enum BlendMode
    {
        Alpha,
        Premultiply, 
        Additive,
        Multiply
    }
    
    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }


    public sealed class CustomLitGUI : ShaderGUI
    {
        private SurfaceOptionGUI _surfaceOptionGUI;
        private SurfaceInputGUI _surfaceInputGUI;
        private StencilOptionGUI _stencilOptionGUI;
        

        private bool _initinalized = false;
        
        
        private Material _material;
        private MaterialEditor _materialEditor;
        private MaterialProperty[] _properties;
        private void DrawShaderGUI()
        {
            _surfaceOptionGUI.OnGUI();
            _surfaceInputGUI.OnGUI();
            _stencilOptionGUI.OnGUI();
        }
        private void Initialize()
        {
            _surfaceOptionGUI = new SurfaceOptionGUI(_material,_properties);
            _surfaceInputGUI = new SurfaceInputGUI(_materialEditor,_material,_properties);
            _stencilOptionGUI = new StencilOptionGUI(_material,_properties);
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

