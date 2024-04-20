using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeyMacroApp.Models;
using KeyMacroApp.ViewModels;
using ServiceKeyHookWrapper;

namespace KeyMacroApp.Views
{
    /// <summary>
    /// Interaction logic for BindKeyDialog.xaml
    /// </summary>
    public partial class BindKeyDialog : Window
    {
        public BindKeyDialog(BindKeyInfo bindKeyInfo)
        {
            InitializeComponent();
            BindKeyDialogViewModel viewModel = new BindKeyDialogViewModel(bindKeyInfo);
            this.DataContext = viewModel;
            viewModel.CloseAction = (bool isApplied) =>
            {
                this.DialogResult = isApplied;
            };
            this.Owner = Application.Current.MainWindow;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            APIWrapper.StartMacro();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            APIWrapper.StopMacro();
        }
    }
}
