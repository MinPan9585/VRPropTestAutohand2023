using System;
using UnityEditor;
using UnityEngine;

namespace CustomShaderGUI
{
    public abstract class BaseShaderGUI:IShaderGUI
    {
        protected bool _enabled = true;
        public bool enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                    _enabled = value;
            }
        }
        protected MaterialProperty[] _properties;
        protected Material _material;
        
        public BaseShaderGUI(Material material, MaterialProperty[] properties)
        {
            _material = material;
            _properties = properties;
        }

        public virtual void OnGUI()
        {
            if(!_enabled) return;
        }

        protected MaterialProperty FindProperty(string name,MaterialProperty[] properties)
        {
            if (properties != null)
            {
                foreach (var item in _properties)
                {
                    if (item.name == name)
                        return item;
                }
            }
            throw new ArgumentNullException("the name of MaterialProperty is null");
        }
    }
}