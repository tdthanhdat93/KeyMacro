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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyMacroApp.UIControl
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>
    public partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(EditableTextBlock),
                new PropertyMetadata(string.Empty));

        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
                "IsEditable",
                typeof(bool),
                typeof(EditableTextBlock),
                new PropertyMetadata(false, OnIsEditablePropertyChanged));

        private static void OnIsEditablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (d is EditableTextBlock editableTextBlock)
                {
                    editableTextBlock._oldText = editableTextBlock.Text;
                }
            }
        }

        // Keep the old text when go into editmode
        // in case the user aborts with the escape key
        private string _oldText = string.Empty;

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            textBox?.Focus();
            textBox?.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsEditable = false;
            var binding = this.GetBindingExpression(TextProperty);
            binding?.UpdateSource();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.IsEditable = false;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsEditable = false;
                Text = _oldText;
            }
        }
    }
}
