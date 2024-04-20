using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KeyMacroApp.Services
{
    public interface IDialogService
    {
        public bool? ShowDialog(Type typeViewModel, object? parameter = null);
    }

    public class DialogService : IDialogService
    {
        private static readonly Dictionary<Type, Type> _mapsViewModel = new Dictionary<Type, Type>();

        public static void RegisterDialog<TView, TViewModel>()
        {
            _mapsViewModel[typeof(TViewModel)] = typeof(TView);
        }

        public bool? ShowDialog(Type typeViewModel, object? parameter = null)
        {
            _ = _mapsViewModel.TryGetValue(typeViewModel, out Type? typeView);

            if (typeView != null)
            {
                var dialog = Activator.CreateInstance(typeView, parameter) as Window;
                return dialog?.ShowDialog();
            }
            else
            {
                return null;
            }
        }
    }
}
