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
using WpfApp1.Domain;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для EncodingWindow.xaml
    /// </summary>
    public partial class EncodingWindow : Window
    {
        public EncodingWindow()
        {
            InitializeComponent();
        }

        private void TBEncodingExeInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBEncodingExeFontOutput.Text = Tools.StringToExeFontIndexString(",", TBEncodingExeInput.Text);
            TBEncodingExeOutput.Text = Tools.StringToExeEncodingString(" ", TBEncodingExeInput.Text);
        }

        private void TBEncodingScriptInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBEncodingScriptFontOutput.Text = Tools.StringToScriptFontIndexString(",", TBEncodingScriptInput.Text);
            TBEncodingScriptOutput.Text = Tools.StringToScriptEncodingString("", TBEncodingScriptInput.Text);
        }
    }
}
