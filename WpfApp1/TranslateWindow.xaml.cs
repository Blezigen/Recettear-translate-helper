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
                {"iv0_17", 1516681},
                {"iv0_1", 1516679},
                {"iv0_2", 1516682},
                {"iv0_6", 1516683},
                {"iv0_3", 1516684},
                {"iv0_18", 1516680},
                {"iv0_5", 1516685},
                {"iv1_1", 1516688},
                {"iv1_13", 1516691},
                {"iv1_10", 1516690},
                {"iv1_12", 1516689},
                {"iv1_15", 1516687},
                {"iv0_8", 1516686},
                {"iv1_2", 1516694},
                {"iv10_4", 1516693},
                {"iv1_3", 1516692},
                {"iv12_18", 1516695},
                {"iv0_4", 1516696},
                {"iv0_19", 1516697},
                {"iv1_11", 1516698},
                {"iv1_14", 1516699},
                {"iv1_19", 1516701},
                {"iv12_11", 1516702},
                {"iv12_21", 1516700},
                {"iv1_16", 1516706},
                {"iv1_21", 1516707},
                {"iv13_4", 1516705},
                {"iv1_17", 1516704},
                {"iv1_18", 1516703},
                {"iv1_4", 1516708},
                {"iv1_24", 1516720},
                {"iv10_6", 1516719},
                {"iv1_22", 1516718},
                {"iv1_25", 1516717},
                {"iv1_6", 1516716},
                {"iv1_5", 1516715},
                {"iv14_4", 1516714},
                {"iv10_1", 1516712},
                {"iv10_3", 1516713},
                {"iv1_9", 1516711},
                {"iv1_7", 1516710},
                {"iv13_2", 1516709},
                {"iv12_14", 1516726},
                {"iv10_2", 1516725},
                {"iv12_15", 1516724},
                {"iv1_8", 1516723},
                {"iv12_2", 1516722},
                {"iv1_20", 1516721},
                {"iv12_10", 1516727},
                {"iv12_16", 1516731},
                {"iv12_17", 1516730},
                {"iv17_11", 1516733},
                {"iv16_2", 1516736},
                {"iv15_4", 1516738},
                {"iv11_1", 1516737},
                {"iv12_13", 1516735},
                {"iv12_20", 1516734},
                {"iv12_12", 1516732},
                {"iv15_7", 1516729},
                {"iv12_1", 1516728},
                {"iv12_19", 1516739},
                {"iv17_19", 1516740},
                {"iv13_1", 1516745},
                {"iv12_4", 1516744},
                {"iv12_9", 1516742},
                {"iv12_3", 1516743},
                {"iv14_11", 1516741},
                {"iv14_18", 1516750},
                {"iv14_15", 1516751},
                {"iv14_14", 1516749},
                {"iv12_8", 1516748},
                {"iv14_16", 1516747},
                {"iv14_1", 1516746},
                {"iv14_3", 1516752},
                {"iv12_6", 1516754},
                {"iv13_3", 1516756},
                {"iv14_13", 1516757},
                {"iv12_7", 1516755},
                {"iv14_5", 1516753},
                {"iv14_6", 1516765},
                {"iv17_24", 1516764},
                {"iv14_12", 1516763},
                {"iv17_27", 1516762},
                {"iv14_21", 1516761},
                {"iv14_20", 1516760},
                {"iv14_19", 1516759},
                {"iv14_2", 1516758},
                {"iv16_6", 1516768},
                {"iv17_17", 1516773},
                {"iv17_16", 1516783},
                {"iv15_2", 1516782},
                {"iv17_48", 1516781},
                {"iv16_5", 1516780},
                {"iv17_14", 1516778},
                {"iv17_6", 1516779},
                {"iv17_20", 1516776},
                {"iv17_12", 1516777},
                {"iv17_15", 1516775},
                {"iv17_10", 1516772},
                {"iv15_9", 1516774},
                {"iv16_4", 1516771},
                {"iv17_40", 1516770},
                {"iv17_1", 1516769},
                {"iv17_29", 1516767},
                {"iv17_23", 1516766},
                {"iv2_10", 1516795},
                {"iv15_5", 1516794},
                {"iv16_1", 1516793},
                {"iv15_10", 1516792},
                {"iv17_18", 1516791},
                {"iv17_13", 1516790},
                {"iv17_2", 1516789},
                {"iv17_22", 1516788},
                {"iv15_8", 1516787},
                {"iv17_31", 1516786},
                {"iv18_3", 1516785},
                {"iv16_3", 1516784},
                {"iv17_21", 1516796},
                {"iv17_41", 1516803},
                {"iv17_36", 1516802},
                {"iv17_49", 1516801},
                {"iv17_26", 1516799},
                {"iv17_25", 1516800},
                {"iv17_35", 1516798},
                {"iv17_30", 1516797},
                {"iv17_3", 1516804},
                {"iv17_9", 1516808},
                {"iv17_34", 1516807},
                {"iv17_37", 1516806},
                {"iv17_39", 1516805},
                {"iv17_33", 1516809},
                {"iv17_8", 1516810},
                {"iv17_5", 1516811},
                {"iv17_38", 1516813},
                {"iv18_2", 1516812},
                {"iv17_47", 1516814},
                {"iv17_4", 1516816},
                {"iv17_42", 1516815},
                {"iv17_7", 1516817},
                {"iv17_46", 1516819},
                {"iv2_13", 1516818},
                {"iv18_9", 1516820},
                {"iv18_1", 1516821},
                {"iv18_10", 1516823},
                {"iv18_5", 1516822},
                {"iv17_28", 1516826},
                {"iv18_4", 1516825},
                {"iv19_3", 1516824},
                {"iv17_44", 1516828},
                {"iv19_1", 1516827},
                {"iv2_14", 1516829},
                {"iv19_2", 1516830},
                {"iv18_8", 1516831},
                {"iv2_1", 1516832},
                {"iv18_6", 1516834},
                {"iv2_12", 1516833},
                {"iv4_3", 1516837},
                {"iv2_5", 1516838},
                {"iv5_4", 1516835},
                {"iv18_7", 1516836},
                {"iv5_2", 1516839},
                {"iv3_1", 1516840},
                {"iv5_7", 1516845},
                {"iv2_11", 1516844},
                {"iv7_12", 1516843},
                {"iv3_5", 1516842},
                {"iv4_2", 1516841},
                {"iv7_14", 1516847},
                {"iv3_7", 1516846},
                {"iv8_1", 1516851},
                {"iv2_7", 1516850},
                {"iv7_3", 1516849},
                {"iv7_5", 1516848},
                {"iv2_8", 1516854},
                {"iv2_9", 1516853},
                {"iv4_5", 1516852},
                {"iv6_4", 1516856},
                {"iv6_5", 1516855},
                {"iv5_1", 1516859},
                {"iv6_1", 1516858},
                {"iv7_4", 1516857},
                {"iv7_7", 1516863},
                {"iv3_4", 1516865},
                {"iv2_15", 1516866},
                {"iv9_1", 1516867},
                {"iv7_1", 1516869},
                {"iv3_2", 1516868},
                {"iv7_2", 1516864},
                {"iv2_2", 1516862},
                {"iv2_6", 1516861},
                {"iv3_6", 1516860},
                {"iv2_3", 1516872},
                {"iv4_4", 1516871},
                {"iv8_4", 1516870},
                {"iv6_2", 1516873},
                {"iv7_10", 1516875},
                {"iv3_3", 1516876},
                {"iv5_3", 1516874},
                {"iv4_1", 1516879},
                {"iv7_8", 1516878},
                {"iv5_6", 1516877},
                {"iv3_8", 1516880},
                {"iv4_6", 1516882},
                {"iv7_11", 1516884},
                {"iv6_6", 1516885},
                {"iv6_3", 1516883},
                {"iv7_15", 1516881},
                {"iv7_9", 1516888},
                {"iv5_5", 1516887},
                {"iv8_2", 1516886},
                {"iv7_6", 1516889},
                {"iv8_5", 1516890},
                {"iv9_2", 1516892},
                {"iv8_3", 1516893},
                {"iv8_6", 1516891},
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
