using KeyMacroApp.Models;
using Prism.Commands;
using Prism.Mvvm;
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

        private DelegateCommand? _deleteCommand = null;
        public DelegateCommand DeleteCommand { get => _deleteCommand ??= new DelegateCommand(OnDelete); }
        #endregion
        public MacroViewModel()
        {
        }

        private void OnSelectMacro(object info)
        {
            SelectedItem = info;
        }

        private void OnAddGroup()
        {
        }

        private void OnAddMacro()
        {
        }

        private void OnDelete()
        {
        }
    }
}
