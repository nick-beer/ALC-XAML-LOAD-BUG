using System.Windows;
using System.Windows.Controls.Primitives;

namespace Library
{
    public static class Properties
    {
        public static readonly DependencyProperty ButtonNameProperty; 

        static Properties()
        {
            ButtonNameProperty = DependencyProperty.RegisterAttached("ButtonName", typeof(string), typeof(Properties));
        }

        public static void SetButtonName(ButtonBase button, string name)
            => button.SetValue(ButtonNameProperty, name);

        public static string GetButtonName(ButtonBase button)
            => (string)button.GetValue(ButtonNameProperty);
    }
}