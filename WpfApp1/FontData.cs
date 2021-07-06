using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Annotations;
using WpfApp1.Domain;

namespace WpfApp1
{
    public class FontData : INotifyPropertyChanged
    {
        public const int LENGTH = 40;

        private UInt32 _shift = 0;
        private UInt32 _imageSize = 0;
        private UInt32 _boxWidth = 0;
        private UInt32 _scale = 0; 
        private UInt32 _horizontalOffset = 0; 
        private UInt32 _verticalOffset = 0; 
        private UInt32 _verticalOffsetCorrection = 0;
        private UInt32 _width = 0;
        private UInt32 _height = 0;
        private UInt32 _zero = 0;

        private byte[] _bitmapData;
        private Bitmap _bitmap;


        public int VerticalOffset
        {
            get => Convert.ToInt32(_verticalOffset);
            set
            {
                _verticalOffset = Convert.ToUInt32(value);
                OnPropertyChanged("VerticalOffset");
            }
        }

        public int HorizontalOffset
        {
            get => Convert.ToInt32(_horizontalOffset);
            set
            {
                _horizontalOffset = Convert.ToUInt32(value);
                OnPropertyChanged("HorizontalOffset");
            }
        }

        public int Zero
        {
            get => Convert.ToInt32(_zero);
            set
            {
                _zero = Convert.ToUInt32(value);
                OnPropertyChanged("Zero");
            }
        }

        public int Shift
        {
            get => Convert.ToInt32(_shift);
            set
            {
                _shift = Convert.ToUInt32(value);
                OnPropertyChanged("Shift");
            }
        }

        public int ImageSize
        {
            get => Convert.ToInt32(_imageSize);
            set
            {
                _imageSize = Convert.ToUInt32(value);
                OnPropertyChanged("ImageSize");
            }
        }

        public int BoxWidth
        {
            get => Convert.ToInt32(_boxWidth);
            set
            {
                _boxWidth = Convert.ToUInt32(value);
                OnPropertyChanged("BoxWidth");
            }
        }

        public int Scale
        {
            get => Convert.ToInt32(_scale);
            set
            {
                _scale = Convert.ToUInt32(value);
                OnPropertyChanged("Scale");
            }
        }

        public int VerticalOffsetCorrection
        {
            get => Convert.ToInt32(_verticalOffsetCorrection);
            set
            {
                _verticalOffsetCorrection = Convert.ToUInt32(value);
                OnPropertyChanged("VerticalOffsetCorrection");
            }
        }

        public int Width
        {
            get => Convert.ToInt32(_width);
            set
            {
                _width = Convert.ToUInt32(value);
                OnPropertyChanged("Width");
            }
        }

        public int Height
        {
            get => Convert.ToInt32(_height);
            set
            {
                _height = Convert.ToUInt32(value);
                OnPropertyChanged("Height");
            }
        }

        public Bitmap Bitmap => _bitmap;


        public static FontData FromBytes(byte[] bytes)
        {
            return new FontData()
            {
                _shift = BitConverter.ToUInt32(bytes, 0),
                _imageSize = BitConverter.ToUInt32(bytes, 1 * 4),
                _boxWidth = BitConverter.ToUInt32(bytes, 2 * 4),
                _scale = BitConverter.ToUInt32(bytes, 3 * 4),
                _horizontalOffset = BitConverter.ToUInt32(bytes, 4 * 4),
                _verticalOffset = BitConverter.ToUInt32(bytes, 5 * 4),
                _verticalOffsetCorrection = BitConverter.ToUInt32(bytes, 6 * 4),
                _width = BitConverter.ToUInt32(bytes, 7 * 4),
                _height = BitConverter.ToUInt32(bytes, 8 * 4),
                _zero = BitConverter.ToUInt32(bytes, 9 * 4),
            };
        }

        public void SetBitmapDataFromFontData(byte[] bytes)
        {
            List<byte> bitmapHead = new List<byte>() {0x42, 0x4D}; // BM
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(this.ImageSize * 1078))); // filesize
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // creator
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1078))); // bmp_offset
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(40))); // header_sz
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToInt32(Width))); // width
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToInt32(Height))); // height
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToInt16(1))); // nplanes
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToInt16(8))); // bitspp
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // compress_type
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(this.ImageSize * 1078))); // bmp_bytesz
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToInt32(3780))); // hres
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToInt32(3780))); // vres
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(265))); // ncolors
            bitmapHead.AddRange(BitConverter.GetBytes(Convert.ToUInt32(256))); // nimpcolors


            byte[] bmpHeadWithGS = bitmapHead.ToArray().Concat(Settings.GrayScaleBytes).ToArray();
            this._bitmapData = bytes;
            byte[] newbytes = Tools.PadLines(bytes, Height, Width);
            byte[] combined = bmpHeadWithGS.Concat(newbytes).ToArray();

            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            Bitmap im = (Bitmap) tc.ConvertFrom(combined);

            //Bitmap im = new Bitmap(Width, Height, Width,
            //    PixelFormat.Format16bppGrayScale,
            //    Marshal.UnsafeAddrOfPinnedArrayElement(newbytes, 0));

            this._bitmap = im;
        }

        public byte[] ToBytes()
        {
            var temp = new List<byte>();
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_shift)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_imageSize)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_boxWidth)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_scale)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_horizontalOffset)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_verticalOffset)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_verticalOffsetCorrection)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_width)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_height)));
            temp.AddRange(BitConverter.GetBytes(Convert.ToUInt32(_zero)));
            return temp.ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}