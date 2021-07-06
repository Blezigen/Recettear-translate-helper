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
using WpfApp1.Annotations;

namespace WpfApp1.uix
{
    /// <summary>
    /// Логика взаимодействия для FontItemControl.xaml
    /// </summary>
    public partial class FontItemControl : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(FontItem),
                typeof(FontItemControl),
                new PropertyMetadata(new FontItem(1, new FontData()
                {
                    BoxWidth = 48,
                    Height = 48,
                    Width = 48,
                    ImageSize = 0,
                    VerticalOffset = 0,
                    HorizontalOffset = 0,
                    VerticalOffsetCorrection = 0,
                    Scale = 50,
                    Shift = 0,
                    Zero = 0
                })));


        public static readonly DependencyProperty ShowInfoProperty =
            DependencyProperty.Register(
                "ShowInfo",
                typeof(bool),
                typeof(FontItemControl),
                new PropertyMetadata(false));


        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness",
                typeof(Thickness),
                typeof(FontItemControl),
                new PropertyMetadata(new Thickness(0, 0, 1, 0)));

        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(
                "Viewport",
                typeof(Rect),
                typeof(FontItemControl),
                new PropertyMetadata(new Rect(0, 0, 0, 0)));

        public bool ShowInfo
        {
            get { return (bool) GetValue(ShowInfoProperty); }
            set { SetValue(ShowInfoProperty, value); }
        }

        public Thickness BorderThickness
        {
            get { return new Thickness(0, 0, ((Thickness) GetValue(BorderThicknessProperty)).Right, 0); }
            set { SetValue(BorderThicknessProperty, new Thickness(0, 0, value.Right, 0)); }
        }

        public FontItem Value
        {
            get
            {
                return (FontItem) GetValue(ValueProperty);
            }
            set
            {
                var fontItemValue = value;
                Viewport = new Rect(fontItemValue.FontData.HorizontalOffset,
                    fontItemValue.FontData.VerticalOffset - fontItemValue.FontData.VerticalOffsetCorrection,
                    fontItemValue.FontData.BoxWidth, 48);

                if (!ShowInfo)
                {
                    OutlineBorder.Width = fontItemValue.FontData.BoxWidth;
                    OutlineBorder.Height = fontItemValue.FontData.Height;
                    Width = fontItemValue.FontData.BoxWidth;
                    Height = fontItemValue.FontData.Height;
                }

                SetValue(ValueProperty, fontItemValue);
            }
        }

        public Rect Viewport
        {
            get
            {
                return (Rect) GetValue(ViewportProperty);
            }
            set
            {
                SetValue(ViewportProperty, value);
            }
        }

        public FontItemControl()
        {
            InitializeComponent();

            this.FontBox.BorderThickness = BorderThickness;
        }
    }
}