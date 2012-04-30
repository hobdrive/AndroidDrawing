using System;
using System.Reflection;
using System.IO;

namespace System.Drawing
{
    public class Graphics : IDisposable
    {
        public Android.Graphics.Canvas ACanvas;
        Android.Graphics.Paint APaint = new Android.Graphics.Paint();
        public static int PointsDPI = 72;
        public static int DeviceDPI = 92;

        public int LineWidth = 1;
        public Android.Graphics.PaintFlags Flags = 0;

        public Graphics (Image image)
        {
            ACanvas = new Android.Graphics.Canvas((image as Bitmap).ABitmap);
            Init ();
        }

        public Graphics (Android.Graphics.Canvas canvas)
        {
            ACanvas = canvas;
            Init ();
        }

        void Init()
        {
            LineWidth = 1;//Math.Max (1, (int)((float)DeviceDPI / (float)PointsDPI));
            Flags = Android.Graphics.PaintFlags.AntiAlias;
        }

        public static Graphics FromImage(Image image)
        {
            return new Graphics(image);
        }

        /**
         * Converts from Points (72pt per inch) to android font size pixels
         * TODO: take into account density here?
         */
        public static int APixels(int pt)
        {
            var px = (int) ((float)DeviceDPI/(float)PointsDPI * pt);
            //var px = pt;
            //Android.Util.Log.Info("APixels", "pt="+pt+" px="+px);
            return px;
        }

        public void DrawImage(Image image, Rectangle target, Rectangle source, GraphicsUnit gu)
        {
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            var sa = source.ToA();
            var ta = target.ToA();
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, sa, ta, APaint);
            sa.Dispose();
            ta.Dispose();
        }

        public void DrawImage(Image image, int x, int y)
        {
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, x, y, APaint);
        }

        public void DrawImage(Image image, int x, int y, Rectangle source, GraphicsUnit gu)
        {
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            var sa = source.ToA();
            var da = new Android.Graphics.Rect(x,y, x+image.Width, y+image.Height);
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, sa, da, APaint);
            da.Dispose();
            sa.Dispose();
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            APaint.Color = pen.Color.AColor;
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            APaint.StrokeWidth = LineWidth;
            ACanvas.DrawLine(x1, y1, x2, y2, APaint);
        }

        public void DrawRectangle(Pen pen, int x1, int y1, int w, int h)
        {
            APaint.Color = pen.Color.AColor;
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            APaint.StrokeWidth = LineWidth;
            ACanvas.DrawRect(x1, y1, x1+w, y1+h, APaint);
        }
        public void DrawEllipse(Pen pen, int x, int y, int w, int h)
        {
            APaint.Color = pen.Color.AColor;
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            APaint.StrokeWidth = LineWidth;
            using (var r = new Android.Graphics.RectF(x, y, x+w, y+h))
            {
                ACanvas.DrawOval(r, APaint);
            }
        }
        public void FillRectangle(Brush brush, int x1, int y1, int w, int h)
        {
            APaint.Color = brush.Color.AColor;
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.StrokeWidth = LineWidth;
            ACanvas.DrawRect(x1, y1, x1+w, y1+h, APaint);
        }
        public void FillEllipse(Brush brush, int x, int y, int w, int h)
        {
            APaint.Color = brush.Color.AColor;
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            using(var r = new Android.Graphics.RectF(x, y, x+w, y+h))
            {
                ACanvas.DrawOval(r, APaint);
            }
        }

        public void DrawString(string text, Font font, Brush brush, float x, float y)
        {
            DrawString(text, font, brush, (int)x, (int)y);
        }
        public void DrawString(string text, Font font, Brush brush, int x, int y)
        {
            APaint.Color = brush.Color.AColor;
            APaint.TextSize = APixels(font.Size);
            APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
            //Android.Graphics.TypefaceStyle
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.Flags = Android.Graphics.PaintFlags.AntiAlias;
            using(var fm = APaint.GetFontMetricsInt())
            {
                var height = -fm.Top;
                ACanvas.DrawText(text, x, y+height, APaint);
            }
        }

        public void DrawString(string text, Font font, Brush brush, RectangleF rect)
        {
            APaint.Color = brush.Color.AColor;
            APaint.TextSize = APixels(font.Size);
            APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.Flags = Android.Graphics.PaintFlags.AntiAlias;

            var fm = APaint.GetFontMetricsInt();
            var height = -fm.Top;
            var cline = 0;
            var lineheight = -fm.Top + fm.Bottom;

            var coffset = 0;
            while(coffset < text.Length)
            {
                var tpart = text.Substring(coffset);
                var tlen = APaint.BreakText(tpart, true, rect.Width, null);
                int spaceoffset = tpart.LastIndexOf(' ', tlen-1, tlen/2);
                if (spaceoffset > 0 && tlen < text.Length)
                    tlen = spaceoffset;
                ACanvas.DrawText(tpart.Substring(0,tlen), (int)rect.X, (int)rect.Y + height + lineheight*cline, APaint);
                coffset += tlen;
                cline++;
            }

            fm.Dispose();
        }

        public Size MeasureStringWidth(string text, Font font, int width)
        {
            APaint.TextSize = APixels(font.Size);
            APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.Flags = Android.Graphics.PaintFlags.AntiAlias;

            var fm = APaint.GetFontMetricsInt();
            var height = -fm.Top;
            var cline = 0;
            var lineheight = -fm.Top + fm.Bottom;

            var coffset = 0;
            while(coffset < text.Length)
            {
                var tpart = text.Substring(coffset);
                var tlen = APaint.BreakText(tpart, true, width, null);
                int spaceoffset = tpart.LastIndexOf(' ', tlen-1, tlen/2);
                if (spaceoffset > 0 && tlen < text.Length)
                    tlen = spaceoffset;
                //ACanvas.DrawText(tpart.Substring(0,tlen), (int)rect.X, (int)rect.Y + height + lineheight*cline, APaint);
                coffset += tlen;
                cline++;
            }
            fm.Dispose();
            return new Size(width, (int)(lineheight*cline));
        }

        public Size MeasureString(string text, Font font)
        {
            APaint.TextSize = APixels(font.Size);
            APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
            APaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            var fm = APaint.GetFontMetricsInt();
            var bounds = new Android.Graphics.Rect();
            APaint.GetTextBounds(text, 0, text.Length, bounds);
            var width = bounds.Width();
            var height = -fm.Top + fm.Bottom;
            fm.Dispose();
            bounds.Dispose();
            return new Size(width, height);
        }

        public void Clear()
        {
            Clear(Color.Transparent);
        }

        public void Clear(Color fill)
        {
            ACanvas.DrawARGB(fill.A, fill.R, fill.G, fill.B);
            //FillRectangle(new Brush(fill), 0,0, ACanvas.Width, ACanvas.Height);
        }

        public int DpiX{
            get{
                return DeviceDPI;
            }
        }

        public void Dispose()
        {
            APaint.Dispose();
            ACanvas.Dispose();
        }
    }

    public enum GraphicsUnit
    {
        Pixel,
    }

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

        public void Clear(){
            ABitmap.EraseColor(Color.Transparent.Value);
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

    public enum FontStyle
    {
        Regular,
        Bold,
        Italic,
    }

    public class Font : IDisposable
    {
        public string Face;
        public int Size;
        public FontStyle Style;

        public Font(string face, int size, FontStyle style)
        {
            Size = size;
            Face = face;
            Style = style;
        }
        public void Dispose()
        {
        }
    }

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

        public Android.Graphics.Rect ToA()
        {
            return new Android.Graphics.Rect(Left, Top, Left+Width, Top+Height);
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

        public Android.Graphics.RectF ToA()
        {
            return new Android.Graphics.RectF(Left, Top, Left+Width, Top+Height);
        }
    }

}

