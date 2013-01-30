using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ZicoresTradingPostNotifier.Helper
{
    public class DependencyProperties
    {
        public static readonly DependencyProperty ColumnIdentifierProperty =
            DependencyProperty.RegisterAttached("ColumnIdentifier", typeof(String), typeof(GridViewColumn),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

        public static void SetColumnIdentifier(UIElement element, String value)
        {
            element.SetValue(ColumnIdentifierProperty, value);
        }

        public static Orientation GetColumnIdentifier(UIElement element)
        {
            return (Orientation)element.GetValue(ColumnIdentifierProperty);
        }
    }
}
