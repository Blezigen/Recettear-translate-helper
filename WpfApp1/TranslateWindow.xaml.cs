using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    }
}
