using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public class FontItem : ICloneable
    {
        private Int32 _index;
        private FontData _fontData;

        public String Title => String.Format("{0:00000}", this._index);
        public Char Char => Convert.ToChar(this._index);

        private ImageSource _ImageData;

        public int Index => _index;

        public FontData FontData
        {
            get { return _fontData; }
            set { _fontData = value; }
        }

        public ImageSource ImageData
        {
            get  { return this._ImageData; }
            set { this._ImageData = value; }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(_fontData.Width, _fontData.Height));
            }
            finally { DeleteObject(handle); }
        }
        public FontItem(Int32 index, FontData data)
        {
            this._index = index;
            this._fontData = data;
            if (_fontData.Bitmap!=null)
                this._ImageData = this.ImageSourceFromBitmap(data.Bitmap);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IEnumerable<byte> ToBytes()
        {
            return FontData.ToBytes();
        }
    }
}