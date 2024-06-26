﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;
using KeyMacroApp.Common;
using Microsoft.Win32;
using Prism.Mvvm;
using Path = System.IO.Path;

namespace KeyMacroApp.Models
{
    public class KeyHook
    {
        public enum KeyState { Down = 0, Up = 1 };

        [XmlElement]
        public KeyState State { get; set; }
        [XmlElement]
        public uint KeyCode { get; set; }
    }

    [XmlRoot]
    public class MacroInfo : BindableBase
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
        [XmlIgnore]
        public string Path { get => _path; }

        private MacroGroup? _parent = null;
        [XmlIgnore]
        public MacroGroup? Parent { get => _parent; }

        [XmlElement]
        public string Id { get; set; } = string.Empty;

        private ObservableCollection<KeyHook>? _keyHooks = null;
        [XmlElement]
        public ObservableCollection<KeyHook>? KeyHooks { get => _keyHooks; set => _keyHooks = value; }

        private bool _isInEditMode = false;
        [XmlIgnore]
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set => SetProperty(ref _isInEditMode, value);
        }

        private bool _isSelected = false;
        [XmlIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public MacroInfo() { }

        public MacroInfo(string name, MacroGroup? parent = null)
        {
            Id = Guid.NewGuid().ToString();

            _parent = parent;
            _name = name;
            _keyHooks = new ObservableCollection<KeyHook>();
            SetFilePath();
        }

        public void SetFilePath()
        {
            if (this.Parent != null)
            {
                _path = $@"{this.Parent.Path}\{_name}.xml";
            }
            else
            {
                _path = $@"Macros\{_name}.xml";
            }
        }

        private void ChangeFilePath()
        {
            try
            {
                var oldPath = _path;
                if (this.Parent != null)
                {
                    _path = $@"{this.Parent.Path}\{_name}.xml";
                    File.Move(oldPath, _path);
                }
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: MacroInfo.ChangeFilePath()\n{e}");
            }
        }

        public void Save()
        {
            XmlSerializerHelper<MacroInfo>.Save(Path, this);
        }

        public static MacroInfo? Load(string path, MacroGroup? parent = null)
        {
            MacroInfo? macroInfo = XmlSerializerHelper<MacroInfo>.Read(path);
            if (macroInfo != null)
            {
                macroInfo._parent = parent;
                macroInfo._name = System.IO.Path.GetFileNameWithoutExtension(path);
                macroInfo._path = path;
            }

            return macroInfo;
        }

        public void Delete()
        {
            try
            {
                File.Delete(Path);
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: MacroInfo.Delete({Path})\n{e}");
            }
        }
    }

    public class MacroGroup : BindableBase
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ChangeDirPath();

                foreach (var group in SubGroups)
                {
                    group.SetDirPath();
                }

                foreach (var info in MacroInfos)
                {
                    info.SetFilePath();
                }
            }
        }

        private string _path = string.Empty;
        public string Path { get => _path; }
        public MacroGroup? Parent { get; set; }
        public ObservableCollection<MacroGroup> SubGroups { get; set; } = new ObservableCollection<MacroGroup>();
        public ObservableCollection<MacroInfo> MacroInfos { get; set; } = new ObservableCollection<MacroInfo>();

        private bool _isInEditMode = false;
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set => SetProperty(ref _isInEditMode, value);
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public IEnumerable<object> Items
        {
            get
            {
                foreach (var item in SubGroups)
                {
                    yield return item;
                }
                foreach (var item in MacroInfos)
                {
                    yield return item;
                }
            }
        }

        public MacroGroup(string name, MacroGroup? parent = null)
        {
            _name = name;
            Parent = parent;
            SetDirPath();

            try
            {
                var subDirs = Directory.GetDirectories(Path);
                foreach (var dir in subDirs)
                {
                    var dName = System.IO.Path.GetFileName(dir) ?? string.Empty;
                    SubGroups.Add(new MacroGroup(dName, this));
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[Error]: Get sub dirs of [{Path}]\n{ex}");
            }

            try
            {
                var subFiles = Directory.GetFiles(Path, "*.xml");
                foreach (var file in subFiles)
                {
                    var macroInfo = MacroInfo.Load(file, parent: this);
                    if (macroInfo != null)
                    {
                        MacroInfos.Add(macroInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[Error]: Get sub files (*.xml) of [{Path}]\n{ex}");
            }

            MacroInfos.CollectionChanged += MacroInfos_CollectionChanged;
            SubGroups.CollectionChanged += MacroInfos_CollectionChanged;
        }

        private void SetDirPath()
        {
            if (this.Parent != null)
            {
                _path = $@"{this.Parent.Path}\{_name}";
            }
            else
            {
                _path = $@".\{_name}";
            }
            Directory.CreateDirectory(Path);
        }

        private void ChangeDirPath()
        {
            try
            {
                var oldPath = _path;
                if (this.Parent != null)
                {
                    _path = $@"{this.Parent.Path}\{_name}";
                    Directory.Move(oldPath, _path);
                }
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: MacroGroup.ChangeDirPath()\n{e}");
            }
        }

        private void MacroInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Items));
        }

        public void Delete()
        {
            try
            {
                Directory.Delete(Path, recursive: true);    // Delete dir and all sub dirs
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: MacroGroup.Delete({Path})\n{e}");
            }
        }

        public MacroInfo? FindMacro(string id)
        {
            MacroInfo? macroInfo = null;
            macroInfo = MacroInfos.FirstOrDefault(info => info.Id == id);
            if (macroInfo == null)
            {
                foreach (var group in SubGroups)
                {
                    macroInfo = group.FindMacro(id);
                    if (macroInfo != null)
                    {
                        break;
                    }
                }
            }

            return macroInfo;
        }

        public void ClearSelectStatus()
        {
            foreach (var info in MacroInfos)
            {
                info.IsSelected = false;
            }
            foreach (var group in SubGroups)
            {
                group.ClearSelectStatus();
            }
        }
    }
}
