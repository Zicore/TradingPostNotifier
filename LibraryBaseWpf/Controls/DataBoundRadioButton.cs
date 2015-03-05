using System.Windows;
using System.Windows.Controls;

namespace LibraryBase.Wpf.Controls
{
    public class DataBoundRadioButton : RadioButton
    {
        protected override void OnChecked(RoutedEventArgs e)
        {
            // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
        }

        protected override void OnToggle()
        {
            // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
        }
    }
}
