using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Android.Util;

namespace System.Drawing
{

    public class Graphics : IDisposable
    {

        public const int BSIZE = 20*1024;
        public const int BTHRESHOLD = BSIZE - 200;

        private const byte OP_END = 0;
        private const byte OP_DRAWSTRING = 1;
        private const byte OP_DRAWRECT = 2;
        private const byte OP_DRAWLINE = 3;
        private const byte OP_DRAWELLIPSE = 4;
        private const byte OP_FILLRECT = 5;
        private const byte OP_FILLPOLY = 6;
        private const byte OP_FILLELLIPSE = 7;
        private const byte OP_DRAWSTRING2 = 8;
        private const byte OP_FLAGS = 9;
        private const byte OP_DRAWARC = 10;

        IntPtr jHost;
        IntPtr jBufferAddress;
        IntPtr jFlushMethod;
        int jBufferPosition = 0;

        public Android.Graphics.Canvas ACanvas;
        protected Bitmap SourceBitmap;

        Android.Graphics.Paint APaint = new Android.Graphics.Paint();
        public static int PointsDPI = 72;
        public static int DeviceDPI = 92;

        public int LineWidth = 1;

        public static Android.Graphics.PaintFlags FlagsDefault = 0;
        public Android.Graphics.PaintFlags Flags = 0;

        public Graphics (Image image)
        {
            ACanvas = new Android.Graphics.Canvas((image as Bitmap).ABitmap);
            SourceBitmap = image as Bitmap;
            Init ();
        }

        public Graphics (Android.Graphics.Canvas canvas)
        {
            ACanvas = canvas;
            Init ();
        }

        void Init()
        {

            var devclass = Android.Runtime.JNIEnv.FindClass("hobdrive/android/FGHost");
            var devconstr = Android.Runtime.JNIEnv.GetMethodID(devclass, "<init>", "(Landroid/graphics/Canvas;)V");
            var jLocalHost = Android.Runtime.JNIEnv.NewObject(devclass, devconstr, new Android.Runtime.JValue[]{ new Android.Runtime.JValue(ACanvas) });

            this.jHost = Android.Runtime.JNIEnv.NewGlobalRef(jLocalHost);
            Android.Runtime.JNIEnv.DeleteLocalRef(jLocalHost);

            jFlushMethod = Android.Runtime.JNIEnv.GetMethodID(devclass, "flush", "()V");

            var jGetBufferMethod = Android.Runtime.JNIEnv.GetMethodID(devclass, "getBuffer", "()Ljava/nio/ByteBuffer;");
            var jBuffer = Android.Runtime.JNIEnv.CallObjectMethod(this.jHost, jGetBufferMethod, new Android.Runtime.JValue[]{});
            jBufferAddress = Android.Runtime.JNIEnv.GetDirectBufferAddress(jBuffer);
            Android.Runtime.JNIEnv.DeleteLocalRef(jBuffer);

            Android.Runtime.JNIEnv.DeleteGlobalRef(devclass);

            LineWidth = 1;//Math.Max (1, (int)((float)DeviceDPI / (float)PointsDPI));
            Flags = FlagsDefault;

            SetFlags(Flags);
        }

        public void Flush()
        {
            if (jBufferPosition == 0)
                return;
            // OP END
            Write(OP_END);
            Write(OP_END);
            Write(OP_END);
            Write(OP_END);
            //Android.Util.Log.Error("FastGraphics", "Flush " + jHost.ToString("X") + " size="+ jBufferPosition);
            Android.Runtime.JNIEnv.CallVoidMethod(this.jHost, jFlushMethod);
            jBufferPosition = 0;
        }

        void WriteByte(byte b)
        {
            if (jBufferPosition > BTHRESHOLD)
            {
                Android.Util.Log.Error("FastGraphics", "BTHRESHOLD " + jHost.ToString("X"));
                Flush ();
            }
            Marshal.WriteByte(jBufferAddress, jBufferPosition++, b);
        }

        void WriteString(string text)
        {
            Marshal.WriteInt32(jBufferAddress, jBufferPosition, text.Length);
            jBufferPosition += 4;
            for(int i = 0; i < text.Length; i++)
            {
                // Put strings in big endian. TODO: check if this always true for UTF-16 in java
                Marshal.WriteInt16(jBufferAddress, jBufferPosition, (byte) ((((int)text[i])&0xFF00) >> 8));
                jBufferPosition ++;
                Marshal.WriteByte(jBufferAddress, jBufferPosition, (byte) (((int)text[i])&0xFF));
                jBufferPosition ++;
            }
        }

        void Write(int value)
        {
            Marshal.WriteInt32(jBufferAddress, jBufferPosition, value);
            jBufferPosition += 4;
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
            Flush();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            var sa = source.ToA();
            var ta = target.ToA();
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, sa, ta, APaint);
            sa.Dispose();
            ta.Dispose();
        }

        public void DrawImage(Image image, int x, int y)
        {
            Flush();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, x, y, APaint);
        }

        public void DrawImage(Image image, int x, int y, Rectangle source, GraphicsUnit gu)
        {
            Flush();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            var sa = source.ToA();
            var da = new Android.Graphics.Rect(x,y, x+source.Width, y+source.Height);
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, sa, da, APaint);
            da.Dispose();
            sa.Dispose();
        }

        public void DrawImage(Image image, Rectangle to, int fromx, int fromy, int fromw, int fromh, GraphicsUnit gu, ImageAttributes ia)
        {
            Flush();

            APaint.Flags = (Android.Graphics.PaintFlags)0;
            var sa = new Android.Graphics.Rect(fromx, fromy, fromx+fromw, fromy+fromh);
            var da = to.ToA();

            Android.Graphics.Paint p = null;
            Android.Graphics.ColorMatrixColorFilter cmf = null;
            if (ia != null && ia.GetColorMatrix() != null){
                p = new Android.Graphics.Paint(APaint);
                var values = ia.GetColorMatrix().Matrix;
                float[] v2 = values[0].Concat(values[1]).Concat(values[2]).Concat(values[3]).ToArray();
                cmf = new Android.Graphics.ColorMatrixColorFilter(v2);
                p.SetColorFilter(cmf);
            }
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, sa, da, p == null ? APaint : p);
            if (p != null) p.Dispose();
            if (cmf != null) cmf.Dispose();
            da.Dispose();
            sa.Dispose();
        }

        public void SetFlags(Android.Graphics.PaintFlags flags)
        {
            WriteByte(OP_FLAGS);
            Write((int)flags);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            WriteByte(OP_DRAWLINE);
            Write(pen.Color.ToArgb());
            Write(pen.Width);
            Write(x1);
            Write(y1);
            Write(x2);
            Write(y2);
        }

        public void DrawRectangle(Pen pen, int x1, int y1, int w, int h)
        {
            WriteByte(OP_DRAWRECT);
            Write(pen.Color.ToArgb());
            Write(x1);
            Write(y1);
            Write(w);
            Write(h);
        }

        public void DrawEllipse(Pen pen, int x, int y, int w, int h)
        {
            WriteByte(OP_DRAWELLIPSE);
            Write(pen.Color.ToArgb());
            Write(x);
            Write(y);
            Write(w);
            Write(h);
        }

        public void DrawArc(Pen pen, int x, int y, int w, int h, float sw, float swa)
        {
            WriteByte(OP_DRAWARC);
            Write(pen.Color.ToArgb());
            Write(x);
            Write(y);
            Write(w);
            Write(h);
            Write((int)(sw*1000));
            Write((int)(swa*1000));
        }

        public void FillRectangle(Brush brush, int x1, int y1, int w, int h)
        {
            WriteByte(OP_FILLRECT);
            Write(brush.Color.ToArgb());
            Write(x1);
            Write(y1);
            Write(w);
            Write(h);
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            WriteByte(OP_FILLPOLY);
            Write(brush.Color.ToArgb());
            Write(points.Length);
            foreach(var p in points)
            {
                Write (p.X);
                Write (p.Y);
            }
        }

        public void FillEllipse(Brush brush, int x, int y, int w, int h)
        {
            WriteByte(OP_FILLELLIPSE);
            Write(brush.Color.ToArgb());
            Write(x);
            Write(y);
            Write(w);
            Write(h);
        }

        public void DrawString(string text, Font font, Brush brush, float x, float y)
        {
            DrawString(text, font, brush, (int)x, (int)y);
        }
        public void DrawString(string text, Font font, Brush brush, int x, int y)
        {
            WriteByte(OP_DRAWSTRING);
            WriteString(text);
            Write(x);
            Write(y);
            Write(brush.Color.ToArgb());
            Write(APixels(font.Size));
        }

        public void DrawString(string text, Font font, Brush brush, RectangleF rect)
        {
            WriteByte(OP_DRAWSTRING2);
            WriteString(text);
            Write((int)rect.X);
            Write((int)rect.Y);
            Write((int)rect.Width);
            Write((int)rect.Height);
            Write(brush.Color.ToArgb());
            Write(APixels(font.Size));
        }

        public Size MeasureStringWidth(string text, Font font, int width)
        {
            //APaint.Color = brush.Color.AColor();
            APaint.TextSize = APixels(font.Size);
            APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.Flags = Android.Graphics.PaintFlags.AntiAlias;
            
            var fm = APaint.GetFontMetricsInt();
            var height = -fm.Top;
            var cline = 0;
            var lineheight = -fm.Top + fm.Bottom;

            if (width == 0)
                return Size.Empty;
            
            var coffset = 0;
            while(coffset < text.Length)
            {
                var tpart = text.Substring(coffset);
                var tlen = APaint.BreakText(tpart, true, width, null);
                var extralen = 0;
                int spaceoffset = tpart.Substring(tlen/2, tlen - tlen/2).LastIndexOf(' ');
                if (spaceoffset <= 0)
                    spaceoffset = tpart.Substring(tlen/2, tlen - tlen/2).LastIndexOf('.');
                if (spaceoffset > 0 && coffset+tlen < text.Length)
                {
                    tlen = spaceoffset + tlen/2;
                    extralen = 1;
                }
                int croffset = tpart.IndexOf('\n');
                if (croffset >= 0 && croffset < tlen)
                {
                    tlen = croffset;
                    extralen = 1;
                }
                //ACanvas.DrawText(tpart.Substring(0,tlen), (int)rect.X, (int)rect.Y + height + lineheight*cline, APaint);
                coffset += tlen+extralen;
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
            jBufferPosition = 0;
            Flush();
            if (SourceBitmap != null)
                SourceBitmap.Clear(fill);
            else
                ACanvas.DrawARGB(fill.A, fill.R, fill.G, fill.B);
        }

        public int DpiX{
            get{
                return DeviceDPI;
            }
        }

        public void TranslateTransform(int x, int y)
        {
            Flush();
            ACanvas.Translate((float)x, (float)y);
        }

        public void RotateTransform(float angle)
        {
            Flush();
            ACanvas.Rotate(angle);
        }

        public void ScaleTransform(float sx, float sy)
        {
            Flush();
            ACanvas.Scale(sx, sy);
        }

        public Android.Graphics.Matrix Transform{
            get{
                return ACanvas.Matrix;
            }
            set{
                Flush();
                ACanvas.Matrix = value;
            }
        }

        float ctransparency = 0;

        public float Transparency{
            get{
                return ctransparency;
            }
            set{
                ctransparency = value;
            }
        }

        public void ResetTransform()
        {
            Flush();
            using(var m = new Android.Graphics.Matrix())
                ACanvas.Matrix = m;
        }

        public void Dispose()
        {
            APaint.Dispose();
            ACanvas.Dispose();
            Android.Runtime.JNIEnv.DeleteGlobalRef(jHost);
        }
    }

}

