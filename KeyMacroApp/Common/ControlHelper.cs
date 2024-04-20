using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace KeyMacroApp.Common
{
    public class ControlHelper
    {
        public static T? FindAncestor<T>(DependencyObject? source) where T : DependencyObject
        {
            while (source != null && source is not T) { 
                source = VisualTreeHelper.GetParent(source);
            }

            return source as T;
        }
    }
}
