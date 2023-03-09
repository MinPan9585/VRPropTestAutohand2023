using UnityEditor;

namespace CustomShaderGUI
{
    public abstract class SerializedParam<T>
    {
        public delegate T Getter(string key,T t);

        public delegate void Setter(string key, T t);

        private Getter _getter;
        private Setter _setter;
        private bool m_Load = false;
        private string _key;

        private void Load()
        {
            if(m_Load) return;
            m_Load = true;
            _Value = _getter(_key,_Value);
        }

        private T _Value;
        public T Value
        {
            get
            {
                Load();
                return _Value;
            }
            set
            {
                Load();
                if (!_Value.Equals(value))
                {
                    _Value = value;
                    _setter(_key, _Value);
                }
            }
        }

        public SerializedParam(string key, T defaultValue, Setter setter, Getter getter)
        {
            this._key = key;
            this._Value = defaultValue;
            this.m_Load = false;
            this._getter = getter;
            this._setter = setter;
        }
    }

    public class SaveBool : SerializedParam<bool>
    {
        public SaveBool(string key, bool defaultValue) : base(key, defaultValue, EditorPrefs.SetBool, EditorPrefs.GetBool)
        {
        }
    }
}