using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Annotations;
using WpfApp1.Domain;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;
using Settings = WpfApp1.Properties.Settings;
using TextBox = System.Windows.Controls.TextBox;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Project project = new Project();

        public String FontIndexPath;
        public String SavePath;
        public String FontDataPath;
        public String BitmapPath;

        public FontItem CurrentSelectedFontItem;

        public byte[] FontIndexByte;
        public byte[] FontDataByte;

        public int[] indexList;
        public ObservableCollection<FontItem> FontItems {
            get {
                return (ObservableCollection<FontItem>)GetValue(FontItemsProperty);
            }
            set {
                SetValue(FontItemsProperty, value);
            }
        }

        public static readonly DependencyProperty FontItemsProperty =
            DependencyProperty.Register(
                "FontItems",
                typeof(ObservableCollection<FontItem>),
                typeof(MainWindow),
                new PropertyMetadata(new ObservableCollection<FontItem>()));

        public static readonly DependencyProperty FontDataProperty =
            DependencyProperty.Register(
                "FontData",
                typeof(FontData),
                typeof(MainWindow),
                new PropertyMetadata(new FontData()));

        public FontData FontData
        {
            get {
                return (FontData)GetValue(FontDataProperty);
            }
            set {
                SetValue(FontDataProperty, value);
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            this.project = new Project();

            this.TvBox.ItemsSource = FontItems;
            indexList = this.parseSearchString(this.SearchBox.Text);
            GridFontData.DataContext = FontData;
        }


        private int[] parseSearchString(string search)
        {
            List<int> result = new List<int>();
            try
            {
                var splittedComas = search.Split(',');
                foreach (var splittedComa in splittedComas)
                {
                    var splittedTire = splittedComa.Split('-');
                    if (splittedTire.Length == 2)
                        for (int i = Convert.ToInt32(splittedTire[0]); i <= Convert.ToInt32(splittedTire[1]); i++)
                            result.Add(i);
                    if (splittedTire.Length == 1)
                        result.Add(Convert.ToInt32(splittedTire[0]));
                }
            }
            catch
            {
                MessageBox.Show("В написании допущена ошибка");
            }

            return result.ToArray();
        }


        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        
            openFileDialog.Filter = "Font Indexes |fontidx.bin";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                this.FontIndexPath = openFileDialog.FileName;
                Settings.Default["FontIndexPath"] = this.FontIndexPath;
            }
            else return;

            openFileDialog.Filter = "Font Data |fontdata.bin";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                this.FontDataPath = openFileDialog.FileName;
                Settings.Default["FontDataPath"] = this.FontIndexPath;
            }
            else return;

            this.project = Project.Create(FontIndexPath, FontDataPath, FontIndexPath, FontDataPath);
            this.project.Load();
            this.TvBox.ItemsSource = this.project.Take(this.parseSearchString(SearchBox.Text));
        }


        public System.Collections.IList GetSelectedFontItems()
        {
            return ((ListView)TvBox).SelectedItems;
        }

        public void ClearForm()
        {
            FontData.Shift = 0;
            FontData.ImageSize = 0;
            FontData.Width = 0;
            FontData.Height = 0;
            FontData.Zero = 0;

            FontData.BoxWidth = 0;
            FontData.Scale = 0;
            FontData.VerticalOffset = 0;
            FontData.HorizontalOffset = 0;
            FontData.VerticalOffsetCorrection = 0;
        }

        public bool isMultipleSelected()
        {
            return this.GetSelectedFontItems().Count > 1;
        }
        public bool isSingleSelected()
        {
            return this.GetSelectedFontItems().Count == 1;
        }

        public bool isEmptySelected()
        {
            return this.GetSelectedFontItems().Count <= 0;
        }

        private void TvBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Collections.IList selections = this.GetSelectedFontItems();
            var items = new List<FontItem>();

            if (isEmptySelected())
            {
                this.ClearForm();
                return;
            }

            if (isSingleSelected())
            {
                var selected = (FontItem)selections[0];
                SettingFontItem.Value = selected;

                FontData.Shift = selected.FontData.Shift;
                FontData.ImageSize = selected.FontData.ImageSize;
                FontData.BoxWidth = selected.FontData.BoxWidth;
                FontData.Scale = selected.FontData.Scale;
                FontData.HorizontalOffset = selected.FontData.HorizontalOffset;
                FontData.VerticalOffset = selected.FontData.VerticalOffset;
                FontData.VerticalOffsetCorrection = selected.FontData.VerticalOffsetCorrection;
                FontData.Width = selected.FontData.Width;
                FontData.Height = selected.FontData.Height;
                FontData.Zero = selected.FontData.Zero;

                items.Add(selected);
            }

            if (isMultipleSelected())
            {
                this.ClearForm();

                var selected = (FontItem) selections[0];
                FontData.Zero = selected.FontData.Zero;
                FontData.Shift = selected.FontData.Shift;
                FontData.Width = selected.FontData.Width;
                FontData.Height = selected.FontData.Height;
                FontData.ImageSize = selected.FontData.ImageSize;

                FontData.BoxWidth = selected.FontData.BoxWidth;
                FontData.Scale = selected.FontData.Scale;
                FontData.VerticalOffset = selected.FontData.VerticalOffset;
                FontData.HorizontalOffset = selected.FontData.HorizontalOffset;
                FontData.VerticalOffsetCorrection = selected.FontData.VerticalOffsetCorrection;

                foreach (FontItem selection in selections)
                {
                    if (FontData.Zero != selection.FontData.Zero)
                        FontData.Zero = 0;

                    if (FontData.Shift != selection.FontData.Shift)
                        FontData.Shift = 0;

                    if (FontData.Width != selection.FontData.Width)
                        FontData.Width = 0;

                    if (FontData.Height != selection.FontData.Height)
                        FontData.ImageSize = 0;

                    if (FontData.ImageSize != selection.FontData.ImageSize)
                        FontData.ImageSize = 0;


                    if (FontData.BoxWidth != selection.FontData.BoxWidth)
                        TextBoxWidth.Text = "--";

                    if (FontData.Scale != selection.FontData.Scale)
                        TextScale.Text = "--";

                    if (FontData.VerticalOffset != selection.FontData.VerticalOffset)
                        TextVerticalOffset.Text = "--";

                    if (FontData.HorizontalOffset != selection.FontData.HorizontalOffset)
                        TexHorizontalOffset.Text = "--";

                    if (FontData.VerticalOffsetCorrection != selection.FontData.VerticalOffsetCorrection)
                        TextVerticalPosition.Text = "--";

                    items.Add(selection);
                }
            }
        }

        public void Update()
        {
            try
            {
                var selected = new List<FontItem>();
                var selectedIndex = new List<int>();
                foreach (FontItem temp in TvBox.SelectedItems)
                {
                    selectedIndex.Add(temp.Index);
                }

                this.TvBox.ItemsSource = this.project.Take(this.parseSearchString(SearchBox.Text));
                this.StringView.ItemsSource = this.project.Take(this.parseSearchString(TextString.Text));

                foreach (FontItem fontIndex in this.TvBox.ItemsSource)
                {
                    if (selectedIndex.Contains(fontIndex.Index))
                    {
                        this.TvBox.SelectedItems.Add(fontIndex);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Проект не загружен");
            }
        }


        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    if (this.CurrentSelectedFontItem == null)
        //    {
        //        return;
        //    }

        //    var index = this.CurrentSelectedFontItem.Index;
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "FontItem | *" + index + "*.bmp|Bitmap file | *.bmp";
        //    openFileDialog.FilterIndex = 1;
        //    openFileDialog.RestoreDirectory = true;
        //    var result = openFileDialog.ShowDialog();
        //    if (result != null && result == true)
        //    {
        //        this.BitmapPath = openFileDialog.FileName;
        //    }
        //    else return;

        //    var BitmapBytes = File.ReadAllBytes(this.BitmapPath);
        //    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
        //    Bitmap im = (Bitmap) tc.ConvertFrom(BitmapBytes);
        //    var width = im.Width;
        //    var height = im.Height;
        //    var size = width * height;
        //    byte[] bmpData = BitmapBytes.Skip(54 + (256 * 4)).Take(size).ToArray();

        //    var temp = FontData.FromBytes(this.FontIndexByte.Skip(1 * FontData.LENGTH).Take(FontData.LENGTH).ToArray());
        //    temp.Width = width;
        //    temp.Height = height;
        //    temp.ImageSize = size;
        //    temp.Shift = FontDataByte.Length;
        //    temp.BoxWidth = 52;
        //    temp.Scale = 50;
        //    temp.VerticalOffset = 2;
        //    temp.HorizontalOffset = 37;
        //    temp.VerticalOffsetCorrection = 36;

        //    var fontIndexPatch = new List<byte>();
        //    fontIndexPatch.AddRange(FontIndexByte.Take(index * FontData.LENGTH));
        //    fontIndexPatch.AddRange(temp.ToBytes());
        //    fontIndexPatch.AddRange(
        //        FontIndexByte.Skip((index + 1) * FontData.LENGTH).Take(FontIndexByte.Length - ((index + 1) * FontData.LENGTH)));
        //    var TempData = new List<byte>();
        //    TempData.AddRange(this.FontDataByte);
        //    TempData.AddRange(Tools.PadLines(bmpData, height, width));
        //    this.FontDataByte = TempData.ToArray();
        //    this.FontIndexByte = fontIndexPatch.ToArray();

        //    Update();
        //}

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            this.project.Save();
        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selections = this.GetSelectedFontItems();
            OpenFileDialog openDialog = new OpenFileDialog();
            string filter = "Bitmap file | ";
            foreach (FontItem selection in selections)
            {
                filter += $"{selection.Index,5:00000}.bmp;";
            }

            openDialog.Filter = filter;
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;

            openDialog.Multiselect = true;

            if(openDialog.ShowDialog() == null)
                return;

            foreach (var fileName in openDialog.FileNames)
            {

                var index = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileName));
                var BitmapBytes = File.ReadAllBytes(fileName);
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                Bitmap im = (Bitmap)tc.ConvertFrom(BitmapBytes);
                this.project.ChangeBitmap(index, im);
            }
            this.Update();
        }

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    if (this.CurrentSelectedFontItem == null)
        //    {
        //        MessageBox.Show("Не выбран элемент");
        //        return;
        //    }

        //    var index = this.CurrentSelectedFontItem.Index;

        //    var temp = FontData.FromBytes(this.FontIndexByte.Skip(1 * FontData.LENGTH).Take(FontData.LENGTH).ToArray());
        //    temp.Shift = FontData.Shift;
        //    temp.ImageSize = FontData.ImageSize;
        //    temp.BoxWidth = FontData.BoxWidth;
        //    temp.Scale = FontData.Scale;
        //    temp.VerticalOffset = FontData.VerticalOffset;
        //    temp.HorizontalOffset = FontData.HorizontalOffset;
        //    temp.VerticalOffsetCorrection = FontData.VerticalOffsetCorrection;
        //    temp.Width = FontData.Width;
        //    temp.Height = FontData.Height;
        //    temp.Zero = FontData.Zero;

        //    var fontIndexPatch = new List<byte>();
        //    fontIndexPatch.AddRange(FontIndexByte.Take(index * FontData.LENGTH));
        //    fontIndexPatch.AddRange(temp.ToBytes());
        //    fontIndexPatch.AddRange(
        //        FontIndexByte.Skip((index + 1) * FontData.LENGTH)
        //                             .Take(FontIndexByte.Length - ((index + 1) * FontData.LENGTH)));

        //    this.FontIndexByte = fontIndexPatch.ToArray();
        //    Update();
        //}

        private void TextBoxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
           // ChangeFontData(sender);
        }

        private void ChangeFontData(object sender)
        {
            System.Collections.IList selections = this.GetSelectedFontItems();

            if (isEmptySelected())
            {
                return;
            }

            if (isMultipleSelected())
            {
                var senderObject = sender as TextBox;

                if (string.IsNullOrEmpty(senderObject.Text))
                    return;

                int numericValue = 0;

                foreach (FontItem selected in selections)
                {
                    if (senderObject == TextBoxWidth && Int32.TryParse(senderObject.Text, out numericValue))
                        selected.FontData.BoxWidth = numericValue;

                    if (senderObject == TextScale && Int32.TryParse(senderObject.Text, out numericValue))
                        selected.FontData.Scale = numericValue;

                    if (senderObject == TextVerticalOffset && Int32.TryParse(senderObject.Text, out  numericValue))
                        selected.FontData.VerticalOffset = numericValue;

                    if (senderObject == TexHorizontalOffset && Int32.TryParse(senderObject.Text, out numericValue))
                        selected.FontData.HorizontalOffset = numericValue;

                    if (senderObject == TextVerticalPosition && Int32.TryParse(senderObject.Text, out numericValue))
                        selected.FontData.VerticalOffsetCorrection = numericValue;

                }
            }

            if (isSingleSelected())
            {
                var senderObject = sender as TextBox;
                var selected = (FontItem)((FontItem)selections[0]);

                if (string.IsNullOrEmpty(senderObject.Text))
                    return;

                if (senderObject == TextBoxWidth)
                    selected.FontData.BoxWidth = Convert.ToInt32(TextBoxWidth.Text);

                if (senderObject == TextScale)
                    selected.FontData.Scale = Convert.ToInt32(TextScale.Text);

                if (senderObject == TextVerticalOffset)
                    selected.FontData.VerticalOffset = Convert.ToInt32(TextVerticalOffset.Text);

                if (senderObject == TexHorizontalOffset)
                    selected.FontData.HorizontalOffset = Convert.ToInt32(TexHorizontalOffset.Text);

                if (senderObject == TextVerticalPosition)
                    selected.FontData.VerticalOffsetCorrection = Convert.ToInt32(TextVerticalPosition.Text);

                SettingFontItem.Value = selected;
            }
        }

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            this.Update();
        }

        private void Button_SaveBitmap(object sender, RoutedEventArgs e)
        {
            string paths = "";

            System.Collections.IList selections = this.GetSelectedFontItems();
            if (selections == null)
                return;

            FolderSelectDialog dialog = new FolderSelectDialog();
            if (dialog.Show())
            {
                paths = dialog.FileName;
            } else return;


            List<int> ids = new List<int>();

            foreach (FontItem selection in selections)
            {
                ids.Add(selection.Index);
            }

            this.project.SaveBitmaps(paths, ids.ToArray());
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {

            System.Collections.IList selections = this.GetSelectedFontItems();
            if (selections == null)
                return;

            List<int> ids = new List<int>();

            foreach (FontItem selection in selections)
            {
                ids.Add(selection.Index);
            }

            this.project.ClearFontData(ids.ToArray());

            Update();
        }

        private void TextString_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void TextString_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    Update();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Проект не загружен");
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selections = this.GetSelectedFontItems();

            if (isEmptySelected())
            {
                return;
            }

            if (isSingleSelected())
            {
                var selected = (FontItem)((FontItem)selections[0]);
                this.project.SaveFontData(selected.Index, selected.FontData);
            }

            Update();
        }

        private void TextBoxWidth_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    System.Collections.IList selections = this.GetSelectedFontItems();

                    if (isSingleSelected())
                    {
                        ChangeFontData(sender);
                        var selected = (FontItem)((FontItem)selections[0]);
                        this.project.SaveFontData(selected.Index, selected.FontData);
                    }

                    if (isMultipleSelected())
                    {
                        ChangeFontData(sender);
                        foreach (FontItem selected in selections)
                            this.project.SaveFontData(selected.Index, selected.FontData);
                    }

                    Update();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Проект не загружен");
            }
        }

        private void MenuItem_Encoding(object sender, RoutedEventArgs e)
        {
            EncodingWindow window = new EncodingWindow();
            window.Show();
        }

        private void MenuItem_Optimization(object sender, RoutedEventArgs e)
        {
            this.project.OptimizationDataBytes();
            MessageBox.Show("Optimization file size finished!");
        }

        private void MenuItem_Translate(object sender, RoutedEventArgs e)
        {
            TranslateWindow window = new TranslateWindow();
            window.Show();
        }
    }
}