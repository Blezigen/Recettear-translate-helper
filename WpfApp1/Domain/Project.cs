using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Domain
{
    public class Project
    {
        private string _fontIdxPath = null;
        private string _fontDataPath = null;
        private string _fontIdxSavePath = null;
        private string _fontDataSavePath = null;

        private byte[] fontIdxBytes;
        private byte[] fontDataBytes;

        public Project()
        {
        }

        public static Project Create(string fontIdxPath, string fontDataPath, string fontIdxSavePath,
            string fontDataSavePath)
        {
            var temp = new Project();
            temp._fontDataPath = fontDataPath;
            temp._fontDataSavePath = fontDataSavePath;

            temp._fontIdxPath = fontIdxPath;
            temp._fontIdxSavePath = fontIdxSavePath;

            return temp;
        }

        public void Load()
        {
            this.fontIdxBytes = File.ReadAllBytes(this._fontIdxPath);
            this.fontDataBytes = File.ReadAllBytes(this._fontDataPath);

            if (fontIdxBytes == null)
                throw new Exception("Not valid font idx format!");

            if (fontDataBytes == null)
                throw new Exception("Not valid font data format!");
        }

        public void SaveBitmaps(string paths, int[] ids)
        {
            foreach (var fontItem in this.Take(ids))
            {
                fontItem.FontData.Bitmap.Save(String.Format("{0}/{1,5:00000}.bmp", paths, fontItem.Index));
            }
        }

        public FontItem[] Take(int[] indexList)
        {
            var collections = new List<FontItem>();
            foreach (var i in indexList)
            {
                var temp = FontData.FromBytes(this.fontIdxBytes.Skip(i * FontData.LENGTH).Take(FontData.LENGTH)
                    .ToArray());
                if (temp.ImageSize != 0)
                {
                    temp.SetBitmapDataFromFontData(fontDataBytes.Skip(temp.Shift).Take(temp.ImageSize).ToArray());
                }

                collections.Add(new FontItem(i, temp));
            }

            return collections.ToArray();
        }

        private FontData GetFontData(int index)
        {
            return FontData.FromBytes(fontIdxBytes.Skip(index * FontData.LENGTH).Take(FontData.LENGTH).ToArray());
        }

        public void SaveFontData(int index, FontData idData)
        {
            var startIndex = index * FontData.LENGTH;
            var tempFontData = idData.ToBytes();

            for (int i = 0; i < tempFontData.Length; i++)
                fontIdxBytes[ startIndex + i ] = tempFontData[i];
        }

        public void ChangeBitmap(int index, Bitmap bitmapFile = null)
        {
            var fontData = this.GetFontData(index);

            if (bitmapFile != null)
            {
                ImageConverter converter = new ImageConverter();
                byte[] bytes = (byte[]) converter.ConvertTo(bitmapFile, typeof(byte[]));

                fontData.Width = bitmapFile.Width;
                fontData.Height = bitmapFile.Height;
                fontData.ImageSize = fontData.Width * fontData.Height;
                fontData.Shift = this.fontDataBytes.Length;

                byte[] bmpData = bytes.Skip(54 + (256 * 4)).Take(fontData.ImageSize).ToArray();

                this.SaveFontData(index, fontData);

                var data = new List<byte>();
                data.AddRange(this.fontDataBytes);
                data.AddRange(Tools.PadLines(bmpData, fontData.Height, fontData.Width));
                fontDataBytes = data.ToArray();
            }
        }
        

        public void OptimizationDataBytes()
        {
            var tempIdx = new List<byte>();
            var tempData = new List<byte>();
            var len = this.fontIdxBytes.Length / FontData.LENGTH;
            FontData fontData = null;
            for (int i = 0; i < len; i++)
            {
                Console.WriteLine(i);
                var imageSize = BitConverter.ToUInt32(fontIdxBytes, (i * FontData.LENGTH) + 4);
                if (imageSize == 0)
                    continue;

                var currentShift = BitConverter.ToUInt32(fontIdxBytes, (i * FontData.LENGTH));
                var shift = BitConverter.GetBytes(Convert.ToUInt32(tempData.Count));

                for (int j = 0; j < shift.Length; j++) // Заменяем биты напрямую
                {
                    fontIdxBytes[(i * FontData.LENGTH) + j] = shift[j];
                }

                tempData.AddRange(fontDataBytes.Skip(Convert.ToInt32(currentShift)).Take(Convert.ToInt32(imageSize))
                    .ToArray());
            }

            this.fontDataBytes = tempData.ToArray();
        }

        public void Save()
        {
            File.WriteAllBytes(this._fontIdxSavePath, this.fontIdxBytes);
            File.WriteAllBytes(this._fontDataSavePath, this.fontDataBytes);
        }

        public void ClearFontData(int[] ids)
        {
            FontData emptyData = new FontData();
            foreach (var index in ids)
            {
                this.SaveFontData(index, emptyData);
            }
        }
    }
}