using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WpfApp1.Annotations;

namespace WpfApp1.uix
{
    /// <summary>
    /// Логика взаимодействия для FontViewControl.xaml
    /// </summary>
    public partial class FontViewControl : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(FontItem),
                typeof(FontViewControl),
                new PropertyMetadata(null));

        public FontItem Value
        {
            get {
                return (FontItem)GetValue(ValueProperty);
            }
            set {
                SetValue(ValueProperty, value);
                OnPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FontViewControl()
        {
            InitializeComponent();
        }
    }
}
