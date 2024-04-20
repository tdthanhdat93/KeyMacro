using KeyMacroApp.Common;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace KeyMacroApp.Models
{
    public class BindKeyInfo : BindableBase
    {
        private string _keyName = string.Empty;
        [XmlElement]
        public string KeyName
        {
            get
            {
                if (string.IsNullOrEmpty(_keyName))
                {
                    _keyName = ConvertHelper.GetKeyName(KeyCode);
                }
                return _keyName;
            }
            set => SetProperty(ref _keyName, value);
        }

        private uint _keyCode;
        [XmlElement]
        public uint KeyCode
        {
            get => _keyCode;
            set => SetProperty(ref _keyCode, value);
        }

        private string _macroId = string.Empty;
        [XmlElement]
        public string MacroId
        {
            get => _macroId;
            set => SetProperty(ref _macroId, value);
        }

        public BindKeyInfo()
        { }

        public BindKeyInfo(uint keyCode)
        {
            _keyCode = keyCode;
        }
    }

    [XmlRoot]
    public class ProfileBindKey : BindableBase
    {
        private string _name = string.Empty;
        [XmlIgnore]
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ChangeFilePath();
            }
        }

        private string _path = string.Empty;

        [XmlElement]
        public List<BindKeyInfo> BindKeys { get; set; } = new List<BindKeyInfo>();

        private bool _isInEditMode = false;

        [XmlIgnore]
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set => SetProperty(ref _isInEditMode, value);
        }

        public ProfileBindKey()
        {
        }

        public ProfileBindKey(string name)
        {
            _name = name;
            _path = $@".\Profiles\{_name}.xml";
        }

        public void SetBindKey(BindKeyInfo bindKeyInfo)
        {
            var foundInfo = BindKeys.FirstOrDefault(bind => bind.KeyCode == bindKeyInfo.KeyCode);
            if (foundInfo != null)
            {
                foundInfo = bindKeyInfo;
            }
            else
            {
                BindKeys.Add(bindKeyInfo);
            }
        }

        public BindKeyInfo? GetBindKey(uint keyCode)
        {
            return BindKeys.FirstOrDefault(bind => bind.KeyCode == keyCode);
        }

        public void Save()
        {
            BindKeys.RemoveAll(bind => string.IsNullOrEmpty(bind.MacroId));

            Directory.CreateDirectory($@".\Profiles");
            XmlSerializerHelper<ProfileBindKey>.Save(_path, this);
        }

        public static void Load(string path, out ProfileBindKey? profile)
        {
            profile = XmlSerializerHelper<ProfileBindKey>.Read(path);
            if (profile != null)
            {
                profile._path = path;
                profile._name = Path.GetFileNameWithoutExtension(path);
            }
        }

        public void Delete()
        {
            try
            {
                File.Delete(_path);
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: ProfileBindKey.Delete({_path})\n{e}");
            }
        }

        private void ChangeFilePath()
        {
            try
            {
                var oldPath = _path;
                _path = $@".\Profiles\{_name}.xml";
                File.Move(oldPath, _path);
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: ProfileBindKey.ChangeFilePath()\n{e}");
            }
        }
    }
}
