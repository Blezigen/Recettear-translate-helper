using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsInput;
using WindowsInput.Native;
using Pfim;
using WpfApp1.Domain;
using Keyboard = WpfApp1.Domain.Keyboard;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для TranslateWindow.xaml
    /// </summary>
    public partial class TranslateWindow : Window
    {
        private string workPath = "";
        private string workExePath = "";
        private Process exeProcess;

        [DllImport("USER32.DLL")] public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")] private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("user32.dll")] public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_SHOWWINDOW = 0x0040;

        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x101;
        const uint WM_CHAR = 0x102;
        const uint WM_SYSKEYDOWN = 260;
        const uint WM_SYSKEYUP = 261;
        const uint WM_SYSCOMMAND = 0x018;
        const uint WM_SYSCHAR = 0x106;
        const uint SC_CLOSE = 0x053;
        const uint KEY_Z = 0x5a;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNORMAL = 1;
        public static void BringToFront(int processId)
        {
            Process process = Process.GetProcessById(processId);
            IntPtr handle = process.MainWindowHandle;

            if (process.MainWindowHandle == IntPtr.Zero)
                process.Refresh();

            ShowWindow(handle, SW_SHOWNORMAL);
            SetForegroundWindow(handle);
        }

        public static readonly DependencyProperty FileCollectionProperty =
            DependencyProperty.Register(
                "FontData",
                typeof(ObservableCollection<Scene>),
                typeof(TranslateWindow),
                new PropertyMetadata(new ObservableCollection<Scene>()));

        public ObservableCollection<Scene> FileCollection
        {
            get {
                return (ObservableCollection<Scene>) GetValue(FileCollectionProperty);
            }
            set {
                SetValue(FileCollectionProperty, value);
            }
        }

        private void toExeMainMenu()
        {
            Process currentProcess = Process.GetCurrentProcess();
            SetForegroundWindow(exeProcess.MainWindowHandle);
            (new InputSimulator()).Keyboard.ModifiedKeyStroke(new []{VirtualKeyCode.MENU}, new []{ VirtualKeyCode.VK_F });
            (new InputSimulator()).Keyboard.KeyDown(VirtualKeyCode.VK_N);
            
            //PostMessage(exeProcess.MainWindowHandle, WM_SYSKEYDOWN, 0x00000012, 0x20380001);
            //PostMessage(exeProcess.MainWindowHandle, WM_SYSKEYDOWN, 0x00000046, 0x20210001);
            //PostMessage(exeProcess.MainWindowHandle, WM_SYSCHAR, 0x00000046, 0x20210001);
            //PostMessage(exeProcess.MainWindowHandle, WM_KEYUP, 0x00000012, 0xD0380001);
            //PostMessage(exeProcess.MainWindowHandle, WM_KEYUP, 0x00000046, 0xD0210001);
            //PostMessage(exeProcess.MainWindowHandle, WM_KEYDOWN, 0x0000004E, 0x10310001);
            //PostMessage(exeProcess.MainWindowHandle, WM_CHAR, 0x0000004E, 0x10310001);
            //PostMessage(exeProcess.MainWindowHandle, WM_KEYUP, 0x0000004E, 0xC0310001);
            Thread.Sleep(100);
            return;
        }
        private void runScane(Scene scene)
        {
            SetForegroundWindow(exeProcess.MainWindowHandle);
            (new InputSimulator()).Keyboard.KeyDown(VirtualKeyCode.VK_Z);
            return;
        }

        public TranslateWindow()
        {
            InitializeComponent();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Recettear.exe | recettear*.exe";
            if (dialog.ShowDialog() == false)
            {
                return;
            }

            workExePath = dialog.FileName;
            workPath = Path.GetDirectoryName(dialog.FileName);

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = workPath;
            startInfo.FileName = workExePath;
            try
            {
                exeProcess = Process.Start(startInfo);
            }
            catch
            {
                // Log error.
            }
        }

        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderSelectDialog()
            {
                Title = "Выберите папку с файлами ivt",
            };

            if (dialog.Show() != true)
            {
                return;
            }

            FileCollection.Clear();
            foreach (string fileName in Directory.GetFiles(dialog.FileName))
            {
                if (Path.GetExtension(fileName)  != ".ivt")
                    continue;
               
                var temp = new Scene(fileName, File.ReadAllLines(fileName));
                this.FileCollection.Add(temp);
            }
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderSelectDialog
            {
                Title = "Open Files with Pfim"

            };

            if (dialog.Show() != true)
            {
                return;
            }

            foreach (var scene in FileCollection)
            {
                var pathToBackup = this.workPath + "\\iv\\"+ Path.GetFileNameWithoutExtension(scene.FileName)+".ivt";
                if (File.Exists(pathToBackup))
                    File.WriteAllLines(pathToBackup + ".bac", File.ReadAllLines(pathToBackup));

                File.WriteAllLines(pathToBackup, scene.Lines);
            }
            MessageBox.Show("Сохранено");
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoxInfo.ItemsSource = ((TvBox?.SelectedItem) as Scene)?.Msgs;
        }

        private void Menu_Run(object sender, RoutedEventArgs e)
        {
            toExeMainMenu();
            if (TvBox.SelectedItem != null)
            {
                var pathToBackup = this.workPath + "\\iv\\iv1_1.ivt";
                if (!File.Exists(pathToBackup+".bac"))
                {
                    File.WriteAllLines(pathToBackup+".bac",File.ReadAllLines(pathToBackup));
                }

                var scene = TvBox.SelectedItem as Scene;
                File.WriteAllLines(pathToBackup, scene.Lines);
                runScane(scene);
            }
        }

        private void MenuItem_SaveOnlyLines(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> k = new Dictionary<string, int>()
            {
                {"iv0_18", 1519560},
                {"iv0_1", 1519558},
                {"iv0_2", 1519559},
                {"iv0_17", 1519557},
                {"iv1_10", 1519567},
                {"iv1_15", 1519566},
                {"iv0_6", 1519565},
                {"iv1_18", 1519564},
                {"iv1_11", 1519563},
                {"iv1_13", 1519562},
                {"iv0_4", 1519561},
                {"iv1_19", 1519570},
                {"iv1_21", 1519568},
                {"iv1_9", 1519569},
                {"iv12_20", 1519574},
                {"iv12_2", 1519573},
                {"iv10_2", 1519572},
                {"iv12_8", 1519571},
                {"iv12_15", 1519575},
                {"iv1_5", 1519592},
                {"iv10_3", 1519591},
                {"iv12_9", 1519590},
                {"iv1_22", 1519589},
                {"iv1_12", 1519588},
                {"iv12_17", 1519587},
                {"iv1_7", 1519585},
                {"iv1_8", 1519584},
                {"iv10_1", 1519586},
                {"iv1_4", 1519583},
                {"iv0_8", 1519582},
                {"iv1_6", 1519581},
                {"iv0_3", 1519580},
                {"iv14_2", 1519579},
                {"iv16_1", 1519578},
                {"iv15_8", 1519577},
                {"iv0_19", 1519576},
                {"iv1_14", 1519607},
                {"iv12_4", 1519608},
                {"iv13_2", 1519606},
                {"iv12_19", 1519605},
                {"iv12_21", 1519604},
                {"iv17_22", 1519603},
                {"iv0_5", 1519602},
                {"iv10_6", 1519601},
                {"iv1_1", 1519600},
                {"iv12_1", 1519598},
                {"iv17_26", 1519599},
                {"iv12_14", 1519596},
                {"iv13_1", 1519597},
                {"iv15_5", 1519595},
                {"iv12_10", 1519594},
                {"iv1_3", 1519593},
                {"iv16_4", 1519623},
                {"iv13_4", 1519622},
                {"iv12_6", 1519621},
                {"iv15_2", 1519620},
                {"iv10_4", 1519619},
                {"iv12_11", 1519618},
                {"iv12_3", 1519616},
                {"iv13_3", 1519617},
                {"iv12_13", 1519615},
                {"iv1_25", 1519614},
                {"iv12_18", 1519613},
                {"iv12_16", 1519612},
                {"iv1_16", 1519610},
                {"iv1_2", 1519611},
                {"iv17_12", 1519609},
                {"iv12_7", 1519629},
                {"iv15_4", 1519628},
                {"iv16_2", 1519627},
                {"iv14_16", 1519626},
                {"iv14_4", 1519625},
                {"iv14_21", 1519624},
                {"iv17_38", 1519666},
                {"iv7_9", 1519660},
                {"iv15_10", 1519659},
                {"iv17_42", 1519655},
                {"iv15_9", 1519658},
                {"iv17_18", 1519657},
                {"iv17_28", 1519656},
                {"iv17_11", 1519654},
                {"iv14_3", 1519653},
                {"iv17_2", 1519652},
                {"iv17_4", 1519651},
                {"iv17_24", 1519649},
                {"iv17_17", 1519650},
                {"iv14_14", 1519648},
                {"iv17_36", 1519647},
                {"iv17_39", 1519646},
                {"iv14_15", 1519645},
                {"iv17_34", 1519644},
                {"iv17_15", 1519643},
                {"iv17_30", 1519642},
                {"iv16_6", 1519641},
                {"iv1_17", 1519640},
                {"iv12_12", 1519639},
                {"iv1_20", 1519638},
                {"iv16_5", 1519637},
                {"iv14_19", 1519636},
                {"iv1_24", 1519635},
                {"iv14_13", 1519632},
                {"iv17_19", 1519634},
                {"iv11_1", 1519633},
                {"iv17_1", 1519631},
                {"iv14_11", 1519630},
                {"iv17_23", 1519687},
                {"iv17_47", 1519685},
                {"iv17_31", 1519686},
                {"iv17_40", 1519684},
                {"iv17_49", 1519683},
                {"iv17_33", 1519682},
                {"iv14_5", 1519681},
                {"iv17_29", 1519680},
                {"iv14_6", 1519678},
                {"iv17_16", 1519679},
                {"iv17_3", 1519677},
                {"iv14_12", 1519676},
                {"iv15_7", 1519674},
                {"iv17_20", 1519675},
                {"iv17_44", 1519669},
                {"iv17_13", 1519673},
                {"iv14_20", 1519672},
                {"iv17_10", 1519671},
                {"iv17_27", 1519670},
                {"iv5_1", 1519668},
                {"iv17_37", 1519667},
                {"iv14_18", 1519665},
                {"iv17_25", 1519663},
                {"iv17_14", 1519664},
                {"iv16_3", 1519662},
                {"iv14_1", 1519661},
                {"iv17_7", 1519703},
                {"iv2_10", 1519702},
                {"iv7_6", 1519701},
                {"iv18_4", 1519700},
                {"iv2_12", 1519699},
                {"iv2_2", 1519698},
                {"iv2_1", 1519697},
                {"iv18_9", 1519694},
                {"iv17_46", 1519696},
                {"iv17_35", 1519695},
                {"iv2_8", 1519693},
                {"iv17_6", 1519692},
                {"iv17_48", 1519690},
                {"iv17_21", 1519691},
                {"iv17_41", 1519688},
                {"iv4_3", 1519689},
                {"iv17_5", 1519723},
                {"iv8_3", 1519721},
                {"iv7_7", 1519720},
                {"iv5_7", 1519722},
                {"iv7_5", 1519719},
                {"iv18_6", 1519718},
                {"iv5_2", 1519717},
                {"iv4_6", 1519716},
                {"iv19_1", 1519715},
                {"iv5_3", 1519714},
                {"iv6_1", 1519713},
                {"iv7_4", 1519712},
                {"iv18_10", 1519711},
                {"iv3_3", 1519709},
                {"iv2_3", 1519710},
                {"iv3_4", 1519708},
                {"iv3_8", 1519707},
                {"iv2_15", 1519706},
                {"iv7_10", 1519705},
                {"iv19_2", 1519704},
                {"iv2_11", 1519769},
                {"iv6_3", 1519768},
                {"iv2_6", 1519767},
                {"iv6_6", 1519762},
                {"iv2_7", 1519761},
                {"iv5_5", 1519758},
                {"iv4_1", 1519756},
                {"iv2_13", 1519760},
                {"iv3_6", 1519759},
                {"iv17_8", 1519757},
                {"iv3_7", 1519755},
                {"iv18_3", 1519754},
                {"iv19_3", 1519753},
                {"iv18_2", 1519752},
                {"iv4_5", 1519751},
                {"iv5_6", 1519750},
                {"iv7_14", 1519749},
                {"iv18_8", 1519735},
                {"iv7_11", 1519743},
                {"iv3_1", 1519748},
                {"iv8_4", 1519742},
                {"iv4_4", 1519744},
                {"iv2_5", 1519741},
                {"iv4_2", 1519739},
                {"iv18_1", 1519746},
                {"iv7_2", 1519740},
                {"iv3_2", 1519747},
                {"iv7_8", 1519745},
                {"iv2_9", 1519737},
                {"iv8_6", 1519738},
                {"iv9_2", 1519736},
                {"iv6_2", 1519734},
                {"iv18_7", 1519733},
                {"iv5_4", 1519732},
                {"iv6_5", 1519731},
                {"iv9_1", 1519727},
                {"iv7_12", 1519728},
                {"iv3_5", 1519729},
                {"iv6_4", 1519730},
                {"iv7_15", 1519726},
                {"iv8_5", 1519725},
                {"iv18_5", 1519724},
                {"iv7_3", 1519771},
                {"iv8_1", 1519770},
                {"iv17_9", 1519763},
                {"iv2_14", 1519766},
                {"iv7_1", 1519765},
                {"iv8_2", 1519764},
            };

            
            List<string> lanes = new List<string>(); 

            foreach (var scene in FileCollection)
            {
                var name = Path.GetFileNameWithoutExtension(scene.FileName);
                lanes.Add("<NAME:"+name+":"+k[name]+">");

                var i = 0;


                var parameters = new Dictionary<string, string>();
                parameters.Add("set_translate", "1");
                foreach (SceneMessage sceneMessage in scene.Msgs)
                {
                    var first = "t[txt][" + i + "]";
                    var temp = Regex.Replace(sceneMessage.Text, "<BR>", "\r",RegexOptions.IgnorePatternWhitespace,TimeSpan.FromSeconds(.25));
                    parameters.Add(first, temp);
                    i++;
                }
                var query = new FormDataCollection(parameters).ReadAsNameValueCollection().ToString();
                lanes.Add(query);
                lanes.Add("<END>");
            }
            File.WriteAllLines(workPath+@"//rulate.txt", lanes);
            MessageBox.Show("Сохранено");
        }
    }
}
