using KeyMacroApp.Common;
using KeyMacroApp.Models;
using KeyMacroApp.Services;
using KeyMacroApp.Views;
using Prism.Commands;
using Prism.Mvvm;
using ServiceKeyHookWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyMacroApp.ViewModels
{
    public class BindKeyViewModel : BindableBase
    {
        private IDialogService _dialogService = new DialogService();

        private ProfileViewModel? _profileViewModel = null;
        public ProfileViewModel ProfileViewModel { get => _profileViewModel ??= new ProfileViewModel(); }

        private DelegateCommand<string>? _bindKeyCommand;
        public DelegateCommand<string> BindKeyCommand { get => _bindKeyCommand ??= new DelegateCommand<string>(OnBindKey); }

        private DelegateCommand? _resetCommand;
        public DelegateCommand ResetCommand { get => _resetCommand ??= new DelegateCommand(OnReset); }


        public BindKeyViewModel()
        {
            DialogService.RegisterDialog<BindKeyDialog, BindKeyDialogViewModel>();
        }

        private void OnBindKey(string hexKey)
        {
            var profile = _profileViewModel?.SelectedProfile;
            if (profile == null)
            {
                return;
            }

            uint keyCode = ConvertHelper.HexStringToUInt(hexKey);
            var bindKeyInfo = profile.GetBindKey(keyCode);
            bindKeyInfo ??= new BindKeyInfo(keyCode);

            bool? bResult = _dialogService.ShowDialog(typeof(BindKeyDialogViewModel), bindKeyInfo);

            if (bResult == true)
            {
                profile.SetBindKey(bindKeyInfo);
                profile.Save();
            }
        }
 
        private void OnReset()
        {
            if (_profileViewModel != null)
            {
                _profileViewModel.SelectedProfile = null;
            }
        }
    }
}
