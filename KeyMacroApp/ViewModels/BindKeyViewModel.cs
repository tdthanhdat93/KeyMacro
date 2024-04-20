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

        private DelegateCommand<string>? _bindKeyCommand;
        public DelegateCommand<string> BindKeyCommand { get => _bindKeyCommand ??= new DelegateCommand<string>(OnBindKey); }

        public BindKeyViewModel()
        {
            DialogService.RegisterDialog<BindKeyDialog, BindKeyDialogViewModel>();
        }

        private void OnBindKey(string hexKey)
        {
            uint keyCode = ConvertHelper.HexStringToUInt(hexKey);
            var bindKeyInfo = new BindKeyInfo(keyCode);

            bool? bResult = _dialogService.ShowDialog(typeof(BindKeyDialogViewModel), bindKeyInfo);
        }
    }
}
