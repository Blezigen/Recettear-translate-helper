using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Scene
    {
        public string[] Lines { get; set; }

        public int MsgCount { get; set; }

        public string FileName { get; set; }

        public List<SceneMessage> Msgs = new List<SceneMessage>();

        public Scene(string filename, string[] lines)
        {
            this.Lines = lines;
            this.FileName = filename;
            for (int i = 0; i < Lines.Length; i++)
            {
                string pattern = @"msg:([0-9]+):([0-9]+):(.*)";
                foreach (Match match in Regex.Matches(Lines[i], pattern, RegexOptions.IgnoreCase))
                {
                    MsgCount++;
                    var tempSM = new SceneMessage(i, Lines[i]);
                    tempSM.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args)
                    {
                        var temp = sender as SceneMessage;
                        Lines[temp.Index] = temp.ToString();
                    };
                    Msgs.Add(tempSM);
                }
            }
        }
    }
}
