using KeyMacroApp.Common;
using KeyMacroApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using ServiceKeyHookWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace KeyMacroApp.ViewModels
{
    internal class MacroViewModel : BindableBase
    {
        private HookDelegateManaged _hookDelegate;
        public MacroGroup? MacroAllData
        {
            get => MacroData.MacroAllData;
        }

        private object? _selectedItem = null;
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                RaisePropertyChanged(nameof(SelectedMacro));
                RaisePropertyChanged(nameof(CanExecuteMacroCommand));
            }
        }

        public MacroInfo? SelectedMacro
        {
            get => _selectedItem as MacroInfo;
        }

        #region Command
        public bool CanExecuteMacroCommand
        {
            get => (SelectedMacro != null);
        }

        private DelegateCommand<object>? _selectMacroCommand = null;
        public DelegateCommand<object> SelectMacroCommand { get => _selectMacroCommand ??= new DelegateCommand<object>(OnSelectMacro); }

        private DelegateCommand? _addMacroCommand = null;
        public DelegateCommand AddMacroCommand { get => _addMacroCommand ??= new DelegateCommand(OnAddMacro); }

        private DelegateCommand? _addGroupCommand = null;
        public DelegateCommand AddGroupCommand { get => _addGroupCommand ??= new DelegateCommand(OnAddGroup); }

        private DelegateCommand<bool?>? _recordCommand = null;
        public DelegateCommand<bool?> RecordCommand
        {
            get => _recordCommand ??= new DelegateCommand<bool?>(OnRecordKey).ObservesCanExecute(() => CanExecuteMacroCommand);
        }

        private DelegateCommand? _replayCommand = null;
        public DelegateCommand ReplayCommand
        {
            get => _replayCommand ??= new DelegateCommand(OnReplayKeyHooked).ObservesCanExecute(() => CanExecuteMacroCommand);
        }
        #endregion

        private string _recordState = string.Empty;
        public string RecordState
        {
            get => _recordState;
            set => SetProperty(ref _recordState, value);
        }

        public MacroViewModel()
        {
            _hookDelegate = new HookDelegateManaged(HandlerKeyHook);
        }

        private void OnSelectMacro(object info)
        {
            SelectedItem = info;
        }

        private void OnAddGroup()
        {
            string GenerateNewName(IEnumerable<MacroGroup>? groups)
            {
                int i = 0;
                string name;
                do
                {
                    i++;
                    name = $"New Group {i}";
                }
                while (groups?.FirstOrDefault(g => g.Name == name) != null);   // Check duplicate name
                return name;
            }

            void DoAddGroup(ICollection<MacroGroup> groups, MacroGroup group)
            {
                groups.Add(group);
                group.IsSelected = true;    // Select new group adding on TreeView
            }

            if (_selectedItem == null)      // Add new group as root level
            {
                var groups = MacroAllData?.SubGroups;
                if (groups != null)
                {
                    var newGroup = new MacroGroup(GenerateNewName(groups), parent: MacroAllData);
                    DoAddGroup(groups, newGroup);
                }
            }
            else if (_selectedItem is MacroInfo macroInfo)   // Add new group in same level with current selected item
            {
                var groups = macroInfo?.Parent?.SubGroups;
                if (groups != null)
                {
                    var newGroup = new MacroGroup(GenerateNewName(groups), macroInfo?.Parent);
                    DoAddGroup(groups, newGroup);
                }
            }
            else if (_selectedItem is MacroGroup macroGroup)
            {
                var groups = macroGroup?.SubGroups;
                if (groups != null)
                {
                    var newGroup = new MacroGroup(GenerateNewName(groups), parent: macroGroup);;
                    DoAddGroup(groups, newGroup);
                }
            }
        }

        private void OnAddMacro()
        {
            string GenerateNewName(IEnumerable<MacroInfo>? infos)
            {
                int i = 0;
                string name;
                do
                {
                    i++;
                    name = $"New Macro {i}";
                }
                while (infos?.FirstOrDefault(info => info.Name == name) != null);   // Check duplicate name
                return name;
            }

            void DoAddMacro(ICollection<MacroInfo> infos, MacroInfo macro)
            {
                infos.Add(macro);
                macro.Save();
                macro.IsSelected = true;    // Select new item adding on TreeView
            }

            if (_selectedItem == null)      // Add new macro as root level
            {
                var infos = MacroAllData?.MacroInfos;
                if (infos != null)
                {
                    var newMacro = new MacroInfo(GenerateNewName(infos), parent: null);
                    DoAddMacro(infos, newMacro);
                }
            }
            else if (_selectedItem is MacroInfo macroInfo)           // Add new macro in same level with current selected item
            {
                var infos = macroInfo?.Parent?.MacroInfos ?? MacroAllData?.MacroInfos;  // If none parent, get list infos at root level
                if (infos != null)
                {
                    var newMacro = new MacroInfo(GenerateNewName(infos), macroInfo?.Parent);
                    DoAddMacro(infos, newMacro);
                }
            }
            else if (_selectedItem is MacroGroup macroGroup)    // Add new macro as child of current selected group
            {
                var infos = macroGroup?.MacroInfos;
                if (infos != null)
                {
                    var newMacro = new MacroInfo(GenerateNewName(infos), macroGroup);
                    DoAddMacro(infos, newMacro);
                }
            }
        }

        private void OnRecordKey(bool? bIsRecord)
        {
            if (bIsRecord ?? false)
            {
                SelectedMacro?.KeyHooks?.Clear();
                APIWrapper.StartRecord(_hookDelegate);
                RecordState = "Recording...";
            }
            else
            {
                APIWrapper.EndRecord();
                SelectedMacro?.Save();
                RecordState = string.Empty;
            }
        }

        private void HandlerKeyHook(uint vkCode, uint flags)
        {
            try
            {
                SelectedMacro?.KeyHooks?.Add(new KeyHook() { KeyCode = vkCode, State = (KeyHook.KeyState)(flags >> 7) });
            }
            catch (Exception ex)
            {
                Debug.Print($"[Error]: MacroViewModel.HandlerKeyHook()\n {ex}");
            }
        }

        private void OnReplayKeyHooked()
        {
            try
            {
                List<INPUT_Managed>? keyInputs = ConvertHelper.ToListInputManaged(SelectedMacro?.KeyHooks);
                APIWrapper.ReplayKeys(keyInputs);
            }
            catch (Exception ex)
            {
                Debug.Print($"[Error]: MacroViewModel.OnReplayKeyHooked()\n {ex}");
            }
        }
    }
}
