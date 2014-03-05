using System;
using System.Linq;
using System.Reflection;
using System.IO;

namespace System.Drawing.Imaging
{
    public class ImageAttributes
    {
        ColorMatrix cm;
        public void SetColorMatrix(ColorMatrix cm)
        {
            this.cm = cm;
        }
        public ColorMatrix GetColorMatrix()
        {
            return cm;
        }
    }
    public class ColorMatrix
    {
        float[][] matrix;
        public float[][] Matrix{
            get{ return matrix; }
        }
        public ColorMatrix(float[][] matrix)
        {
            this.matrix = matrix;
        }
    }

}

    
namespace System.Drawing
{

    using System.Drawing.Imaging;
    
    
    public enum GraphicsUnit
    {
        Pixel,
    }

#if OLDMFA
    public struct Point : IDisposable
    {
        public int X{get; set;}
        public int Y{get; set;}

        public static Point Empty = new Point(0,0);

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static bool operator== (Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator!= (Point left, Point right)
        {
            return !(left == right);
        }

        public override bool Equals (object obj)
        {
            return this == (Point)obj;
        }
        public override int GetHashCode ()
        {
            return base.GetHashCode ()+X+Y;
        }

        public void Dispose()
        {
        }
    }

    public struct Color
    {
        public int Value{get; set;}

        public int A{ get{ return (Value >> 24)&0xFF; } }
        public int R{ get{ return (Value >> 16)&0xFF; } }
        public int G{ get{ return (Value >> 8)&0xFF; } }
        public int B{ get{ return (Value >> 0)&0xFF; } }
        /*
         * A 255 - opaque
         * A 0   - transparent
         */
        public Color(int argb) : this()
        {
            Value = argb;
        }

        public Android.Graphics.Color AColor{ get{
            return new Android.Graphics.Color(this.Value);
        }}

        public Color(int r, int g, int b) : this(0xFF, r, g, b)
        {
        }

        public Color(int a, int r, int g, int b) : this()
        {
            Value = ((a&0xFF) << 24) | ((r&0xFF)<<16) | ((g&0xFF)<<8) | ((b&0xFF)<<0);
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a,r,g,b);
        }

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color(r,g,b);
        }

        public static Color FromArgb(int rgb)
        {
            return new Color(rgb);
        }

        public static bool operator== (Color left, Color right)
        {
            return left.Value == right.Value;
        }

        public static bool operator!= (Color left, Color right)
        {
            return !(left == right);
        }
        public override bool Equals (object obj)
        {
            return this == (Color)obj;
        }
        public override int GetHashCode ()
        {
            return base.GetHashCode ()+Value;
        }

        public static Color Black = new Color(0,0,0);
        public static Color White = new Color(255,255,255);
        public static Color Red = new Color(255,0,0);
        public static Color Green = new Color(0,255,0);
        public static Color Yellow = new Color(255,255,0);
        public static Color Transparent = new Color(0, 0,0,0);
        public static Color DarkGray = new Color(0x60,0x60,0x60);
        public static Color Empty = new Color(0,0,0);
    }
#else

    public static class ColorHelper{
        public static Android.Graphics.Color AColor(this Color c){
            return new Android.Graphics.Color(c.ToArgb());
        }
    }
#endif

    public class MouseEventArgs
    {
        public int X{get; set;}
        public int Y{get; set;}

        public MouseEventArgs(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public abstract class Image : IDisposable
    {
        public abstract int Width{get;}
        public abstract int Height{get;}

        public Size Size{ get{ return new Size(Width, Height); }}

        public virtual void Dispose()
        {
        }
    }

    public class Bitmap : Image
    {
        public Android.Graphics.Bitmap ABitmap;

        public Bitmap(int w, int h)
        {
            ABitmap = Android.Graphics.Bitmap.CreateBitmap(w,h, Android.Graphics.Bitmap.Config.Argb8888);
        }
        public Bitmap(MemoryStream ms)
        {
        }
        public Bitmap(Stream rs)
        {
        }

        public void Clear(Color color){
            ABitmap.EraseColor(color.ToArgb());
        }

        public override int Width{
            get{ return ABitmap.Width; }
        }
        public override int Height{
            get{ return ABitmap.Height; }
        }

        public override void Dispose(){
            if (ABitmap != null)
            {
                ABitmap.Dispose();
                ABitmap = null;
            }
        }

    }

    public class Pen : IDisposable
    {
        public Color Color;
        public int Width;

        public Pen(Color c)
        {
            this.Color = c;
            this.Width = 1;
        }

        public Pen(Color c, int width)
        {
            this.Color = c;
            this.Width = width;
        }
        public void Dispose()
        {
        }
    }


    public class Brush : IDisposable
    {
        public Color Color;

        public Brush(Color c)
        {
            this.Color = c;
        }
        public void Dispose()
        {
        }
    }

    public class SolidBrush : Brush
    {
        public SolidBrush(Color c) : base(c)
        {
        }
    }

#if OLDMFA
    public struct Size
    {
        public static Size Empty = new Size(0,0);
        public int Width{get; set;}
        public int Height{get; set;}

        public Size(int Width, int Height) : this()
        {
            this.Width = Width;
            this.Height = Height;
        }

        public static bool operator== (Size left, Size right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }

        public static bool operator!= (Size left, Size right)
        {
            return !(left == right);
        }
        public override bool Equals (object obj)
        {
            return this == (Size)obj;
        }
        public override int GetHashCode ()
        {
            return base.GetHashCode ()+Width+Height;
        }
        public bool IsEmpty{ get{ return this == Empty; }}

    }

    public struct SizeF
    {
        public float Width{get; set;}
        public float Height{get; set;}

        public SizeF(float Width, float Height) : this()
        {
            this.Width = Width;
            this.Height = Height;
        }
    }
#endif

    public enum FontStyle
    {
        Regular,
        Bold,
        Italic,
    }

    public class Font : IDisposable
    {
        public string Face;
        public string Name;
        public int Size;
        public FontStyle Style;

        public Font(string face, int size, FontStyle style)
        {
            Size = size;
            Face = face;
            Style = style;
            Name = Face;
        }
        public void Dispose()
        {
        }
    }

#if OLDMFA
    public struct Rectangle
    {
        public int Left{get; set;}
        public int Top{get; set;}
        public int Width{get; set;}
        public int Height{get; set;}

        public int X{ get{ return Left;} set{ Left = value;}}
        public int Y{ get{ return Top;} set{ Top = value;}}

        public int Right{ get{ return Left+Width; }}
        public int Bottom{ get{ return Top+Height; }}

        public Point Location{ get{ return new Point(Left, Top); } set{ this.X = value.X; this.Y = value.Y; }}

        public Size Size{ get{ return new Size(Width, Height); }}

        public Rectangle(int Left, int Top, int Width, int Height) : this()
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
        }

        public bool Contains(Rectangle r)
        {
            return this.Contains(r.Location) && r.Contains(r.Right, r.Bottom);
        }

        public bool Contains(Point p)
        {
            return Contains (p.X, p.Y);
        }

        public bool Contains(int x, int y)
        {
            return this.Left <= x && this.Right >= x && this.Top <= y && this.Bottom >= y;
        }

        public bool IntersectsWith(Rectangle r)
        {
            return this.Contains(r.Left, r.Top) || this.Contains(r.Right, r.Bottom) || r.Contains(this.Left, this.Top) || r.Contains(this.Right, this.Bottom);
        }

    }

    public struct RectangleF
    {
        public float Left{get; set;}
        public float Top{get; set;}
        public float Width{get; set;}
        public float Height{get; set;}

        public float X{ get{ return Left;} set{ Left = value;}}
        public float Y{ get{ return Top;} set{ Top = value;}}

        public float Right{ get{ return Left+Width; }}
        public float Bottom{ get{ return Top+Height; }}

        public Point Location{ get{ return new Point((int)Left, (int)Top); }}

        public RectangleF(float Left, float Top, float Width, float Height) : this()
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
        }

        public bool Contains(RectangleF r)
        {
            return this.Contains(r.Location) && r.Contains(new Point((int)r.Right, (int)r.Bottom));
        }

        public bool Contains(Point p)
        {
            return this.Left <= p.X && this.Right >= p.X && this.Top <= p.Y && this.Bottom >= p.Y;
        }

    }
#else
    public static class RectHelpers{

        public static Android.Graphics.Rect ToA(this Rectangle r)
        {
            return new Android.Graphics.Rect(r.Left, r.Top, r.Left+r.Width, r.Top+r.Height);
        }

        public static Android.Graphics.RectF ToA(this RectangleF r)
        {
            return new Android.Graphics.RectF(r.Left, r.Top, r.Left+r.Width, r.Top+r.Height);
        }
    }
#endif

}