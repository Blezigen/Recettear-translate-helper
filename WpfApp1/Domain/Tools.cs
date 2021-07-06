using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pfim;

namespace WpfApp1.Domain
{
    public static class Tools
    {
        private static List<GCHandle> handles = new List<GCHandle>();

        static Dictionary<Char, int> encodingScriptFont = new Dictionary<char, int>{
            {'А',19057},
            {'Б',19058},
            {'В',19059},
            {'Г',19060},
            {'Д',19061},
            {'Е',19062},
            {'Ё',19042},
            {'Ж',19063},
            {'З',19064},
            {'И',19065},
            {'Й',19066},
            {'К',19067},
            {'Л',19068},
            {'М',19069},
            {'Н',19070},
            {'О',19071},
            {'П',19072},
            {'Р',19073},
            {'С',19074},
            {'Т',19075},
            {'У',19076},
            {'Ф',19077},
            {'Х',19078},
            {'Ц',19079},
            {'Ч',19080},
            {'Ш',19081},
            {'Щ',19082},
            {'Ъ',19083},
            {'Ы',19084},
            {'Ь',19085},
            {'Э',19086},
            {'Ю',19087},
            {'Я',19088},
            {'а',19089},
            {'б',19090},
            {'в',19091},
            {'г',19092},
            {'д',19093},
            {'е',19094},
            {'ё',19314},
            {'ж',19095},
            {'з',19096},
            {'и',19097},
            {'й',19098},
            {'к',19099},
            {'л',19100},
            {'м',19101},
            {'н',19102},
            {'о',19103},
            {'п',19104},
            {'р',19297},
            {'с',19298},
            {'т',19299},
            {'у',19300},
            {'ф',19301},
            {'х',19302},
            {'ц',19303},
            {'ч',19304},
            {'ш',19305},
            {'щ',19306},
            {'ъ',19307},
            {'ы',19308},
            {'ь',19309},
            {'э',19310},
            {'ю',19311},
            {'я',19312},
        }; 
        
        static Dictionary<Char, int> encodingExeFont = new Dictionary<char, int>{
            {'А',328},
            {'Б',329},
            {'В',330},
            {'Г',331},
            {'Д',332},
            {'Е',335},
            {'Ё',337},
            {'Ж',338},
            {'З',339},
            {'И',340},
            {'Й',341},
            {'К',342},
            {'Л',343},
            {'М',344},
            {'Н',345},
            {'О',346},
            {'П',347},
            {'Р',348},
            {'С',349},
            {'Т',350},
            {'У',351},
            {'Ф',352},
            {'Х',353},
            {'Ц',354},
            {'Ч',355},
            {'Ш',356},
            {'Щ',357},
            {'Ъ',358},
            {'Ы',359},
            {'Ь',360},
            {'Э',361},
            {'Ю',362},
            {'Я',363},
            {'а',364},
            {'б',365},
            {'в',366},
            {'г',367},
            {'д',368},
            {'е',369},
            {'ё',370},
            {'ж',372},
            {'з',472},
            {'и',473},
            {'й',474},
            {'к',475},
            {'л',476},
            {'м',477},
            {'н',478},
            {'о',479},
            {'п',480},
            {'р',481},
            {'с',482},
            {'т',483},
            {'у',484},
            {'ф',485},
            {'х',486},
            {'ц',487},
            {'ч',488},
            {'ш',489},
            {'щ',490},
            {'ъ',491},
            {'ы',513},
            {'ь',514},
            {'э',515},
            {'ю',516},
            {'я',517},
        };

        static Dictionary<Char, string> encodingExe = new Dictionary<char, string>{
            {'А',"82 a1"},
            {'Б',"82 a3"},
            {'В',"82 a5"},
            {'Г',"82 a7"},
            {'Д',"82 e1"},
            {'Е',"82 c1"},
            {'Ё',"82 a0"},
            {'Ж',"82 a2"},
            {'З',"82 a4"},
            {'И',"82 a6"},
            {'Й',"82 a8"},
            {'К',"82 a9"},
            {'Л',"82 ab"},
            {'М',"82 ad"},
            {'Н',"82 af"},
            {'О',"82 b1"},
            {'П',"82 b3"},
            {'Р',"82 b5"},
            {'С',"82 b7"},
            {'Т',"82 b9"},
            {'У',"82 bb"},
            {'Ф',"82 bd"},
            {'Х',"82 bf"},
            {'Ц',"82 c2"},
            {'Ч',"82 c4"},
            {'Ш',"82 c6"},
            {'Щ',"82 c8"},
            {'Ъ',"82 c9"},
            {'Ы',"82 ca"},
            {'Ь',"82 cb"},
            {'Э',"82 cc"},
            {'Ю',"82 cd"},
            {'Я',"82 d0"},
            {'а',"82 d3"},
            {'б',"82 d6"},
            {'в',"82 d9"},
            {'г',"82 dc"},
            {'д',"82 dd"},
            {'е',"82 de"},
            {'ё',"82 df"},
            {'ж',"82 e2"},
            {'з',"82 aa"},
            {'и',"82 ac"},
            {'й',"82 ae"},
            {'к',"82 b0"},
            {'л',"82 b2"},
            {'м',"82 b4"},
            {'н',"82 b6"},
            {'о',"82 b8"},
            {'п',"82 ba"},
            {'р',"82 bc"},
            {'с',"82 be"},
            {'т',"82 c0"},
            {'у',"82 c3"},
            {'ф',"82 c5"},
            {'х',"82 c7"},
            {'ц',"82 ce"},
            {'ч',"82 d1"},
            {'ш',"82 d4"},
            {'щ',"82 d7"},
            {'ъ',"82 da"},
            {'ы',"82 cf"},
            {'ь',"82 d2"},
            {'э',"82 d5"},
            {'ю',"82 d8"},
            {'я',"82 db"},
        };

        public static String StringToScriptFontIndexString(string split, string data)
        {
            string temp = "";

            foreach (char charData in data)
            {
                if (Tools.encodingScriptFont.ContainsKey(charData))
                    temp += Tools.encodingScriptFont[charData];
                else
                    temp += ((int)charData);

                temp += split;
            }

            return temp;
        }

        public static string StringToScriptEncodingString(string split, string data)
        {
            string temp = "";

            foreach (char charData in data)
            {
                temp += charData;
                temp += split;
            }

            return temp;
        }

        public static String StringToExeFontIndexString(string split, string data)
        {
            string temp = "";

            foreach (char charData in data)
            {
                if (Tools.encodingExeFont.ContainsKey(charData))
                    temp += Tools.encodingExeFont[charData];
                else
                    temp += ((int)charData);

                temp += split;
            }

            return temp;
        }


        public static string StringToExeEncodingString(string split, string data)
        {
            string temp = "";

            foreach (char charData in data)
            {
                if (Tools.encodingExe.ContainsKey(charData))
                    temp += Tools.encodingExe[charData];
                else
                    temp += $"{(byte) (charData):x2} ";// ((int)charData);

                temp += split;
            }

            return temp;
        }

        public static byte[] PadLines(byte[] bytes, int rows, int columns)
        {
            List<byte> newBytes = new List<byte>();
            for (int i = rows - 1; i >= 0; i--)
            {
                var temp = bytes.Skip(i * columns).Take(columns);
                newBytes.AddRange(temp);
            }

            return newBytes.ToArray();
        }


        public static void FreeHandle()
        {
            foreach (var handle in handles)
            {
                handle.Free();
            }
        }

        public static ImageSource WpfImage(IImage image)
        {
            var pinnedArray = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            var addr = pinnedArray.AddrOfPinnedObject();
            var bsource = BitmapSource.Create(image.Width, image.Height, 96.0, 96.0,
                PixelFormat(image), null, addr, image.DataLen, image.Stride);
            handles.Add(pinnedArray);
            return bsource;
        }

        public static PixelFormat PixelFormat(IImage image)
        {
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    return PixelFormats.Bgr24;
                case ImageFormat.Rgba32:
                    return PixelFormats.Bgra32;
                case ImageFormat.Rgb8:
                    return PixelFormats.Gray8;
                case ImageFormat.R5g5b5a1:
                case ImageFormat.R5g5b5:
                    return PixelFormats.Bgr555;
                case ImageFormat.R5g6b5:
                    return PixelFormats.Bgr565;
                default:
                    throw new Exception($"Unable to convert {image.Format} to WPF PixelFormat");
            }
        }

        public static void ParseIvt(string[] readAllLines)
        {
            var temps = new List<Scene>();
            return;
        }
    }
}
