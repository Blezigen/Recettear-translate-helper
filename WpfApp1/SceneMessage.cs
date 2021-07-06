using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pfim;
using WpfApp1.Annotations;
using WpfApp1.Domain;
using WpfApp1.Properties;
using Settings = WpfApp1.Domain.Settings;

namespace WpfApp1
{
    public class SceneMessage : INotifyPropertyChanged
    {
        private string _text;
        private string _systemText;

        public int Index { get; set; }

        public int WindowIndex { get; set; }

        public String Text
        {
            get { return _text;}
            set { 
                this._text = value;
                OnPropertyChanged("Text");
            }

        }

        public int CharIndex { get; set; }
        public String CharName {
            get
            {
                return Settings.CharIndexToNameDictionary[CharIndex];
            }
        }

        public ImageSource _image;

        public ImageSource Image
        {
            get
            {
                try
                {
                    if (_image == null)
                    {
                        Tools.FreeHandle();
                        var rm = Properties.Resources.ResourceManager.GetObject(CharName.ToLower());
                        using (MemoryStream memory = new MemoryStream(rm as byte[] ?? Array.Empty<byte>()))
                        {
                            IImage image = Pfim.Pfim.FromStream(memory);
                            _image = Tools.WpfImage(image) as ImageSource;
                        }
                    }
                    return _image;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public SceneMessage(int index, string message)
        {
            Index = index;

            var messages = message.Split(':');

            WindowIndex = Convert.ToInt32(messages[1]);
            CharIndex = Convert.ToInt32(messages[2]);

            Text = messages[3].ToString();

            Text = Regex.Replace(Text, "//.*", "",
                RegexOptions.IgnorePatternWhitespace,
                TimeSpan.FromSeconds(.25));

            string pattern = @"(\<(?:KEY|W|C)\>)";
            Regex rgx = new Regex(pattern);
            foreach (Match match in rgx.Matches(Text))
            {
                _systemText += match.Value;
            }

            Text = Regex.Replace(Text, @"(\<(?:KEY|W|C)\>)", "",
                RegexOptions.IgnorePatternWhitespace,TimeSpan.FromSeconds(.25));

        }

        public override string ToString()
        {
            return $"msg:{WindowIndex}:{CharIndex}:{Text}{_systemText}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
