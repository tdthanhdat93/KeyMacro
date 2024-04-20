using KeyMacroApp.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyMacroApp.Views
{
    /// <summary>
    /// Interaction logic for MacroView.xaml
    /// </summary>
    public partial class MacroView : UserControl
    {
        public MacroView()
        {
            InitializeComponent();
            this.DataContextChanged += MacroView_DataContextChanged;
        }

        private void MacroView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"MacroView_DataContextChanged -> {e.NewValue?.GetType()}");
        }

        private void ButtonReplay_Click(object sender, RoutedEventArgs e)
        {
            txtbxReplay.Focus();
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = ControlHelper.FindAncestor<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                treeViewItem.Focus();
            }
        }
    }
}
