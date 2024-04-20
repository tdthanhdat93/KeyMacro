using KeyMacroApp.Common;
using KeyMacroApp.Models;
using Microsoft.Windows.Themes;
using Prism.Commands;
using Prism.Mvvm;
using ServiceKeyHookWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KeyMacroApp.ViewModels
{
    class BindKeyDialogViewModel : BindableBase
    {
        private BindKeyInfo _bindKeyInfo;
        public BindKeyInfo BindKeyInfo { get => _bindKeyInfo; }

        public string CurrentMacroName
        {
            get
            {
                var macro = MacroData.FindMacro(_bindKeyInfo.MacroId);
                if (macro == null)
                {
                    return "<None>";
                }
                else
                {
                    return macro.Name;
                }
            }
        }

        public string Title { get => $"Bind [{_bindKeyInfo.KeyName}] key setting"; }

        private DelegateCommand? _applyCommand = null;
        public DelegateCommand ApplyCommand { get => _applyCommand ??= new DelegateCommand(ExecuteApplyCommand); }

        private DelegateCommand? _cancelCommand = null;
        public DelegateCommand CancelCommand { get => _cancelCommand ??= new DelegateCommand(ExecuteCancelCommand); }

        private DelegateCommand? _resetCommand = null;
        public DelegateCommand? ResetCommand { get => _resetCommand ??= new DelegateCommand(ExecuteResetCommand); }

        public Action<bool>? CloseAction { get; set; }

        private MacroViewModel? _macroViewModel = null;
        public MacroViewModel MacroViewModel { get => _macroViewModel ??= new MacroViewModel() { ActionMacroDeleted = OnMacroDeleted}; }


        public BindKeyDialogViewModel(BindKeyInfo bindKeyInfo)
        {
            _bindKeyInfo = bindKeyInfo;
        }

        private void ExecuteApplyCommand()
        {
            _bindKeyInfo.MacroId = MacroViewModel.SelectedMacro?.Id ?? string.Empty;

            if (MacroViewModel.SelectedMacro != null)
            {
                var keyInputs = ConvertHelper.ToListInputManaged(MacroViewModel.SelectedMacro.KeyHooks);
                APIWrapper.BindKey(_bindKeyInfo.KeyCode, keyInputs);
            }
            else
            {
                APIWrapper.ClearMacroOfKey(_bindKeyInfo.KeyCode);
            }
            
            this.CloseAction?.Invoke(true);
        }

        private void ExecuteCancelCommand()
        {
            this.CloseAction?.Invoke(false);
        }

        private void ExecuteResetCommand()
        {
            APIWrapper.ClearMacroOfKey(_bindKeyInfo.KeyCode);
            _bindKeyInfo.MacroId = string.Empty;
            RaisePropertyChanged(nameof(CurrentMacroName));
        }

        private void OnMacroDeleted()
        {
            var macro = MacroData.FindMacro(_bindKeyInfo.MacroId);
            if (macro == null)
            {
                APIWrapper.ClearMacroOfKey(_bindKeyInfo.KeyCode);
            }
            RaisePropertyChanged(nameof(CurrentMacroName));
        }
    }
}
