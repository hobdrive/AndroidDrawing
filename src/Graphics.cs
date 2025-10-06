using System;
using System.Linq;
using System.Reflection;
using System.IO;

#if SLOW_GRAPHICS

namespace System.Drawing
{
    using System.Drawing.Imaging;

    public class Graphics : IDisposable
    {
        public Android.Graphics.Canvas ACanvas;
        protected Bitmap SourceBitmap;
        Android.Graphics.Paint APaint = new Android.Graphics.Paint();
        public static int PointsDPI = 72;
        public static int DeviceDPI = 92;

        public int LineWidth = 1;
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
            LineWidth = 1;//Math.Max (1, (int)((float)DeviceDPI / (float)PointsDPI));
            Flags = Android.Graphics.PaintFlags.AntiAlias;
        }

        public void Flush()
        {
            // Doesn nothing
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
            var da = new Android.Graphics.Rect(x,y, x+source.Width, y+source.Height);
            ACanvas.DrawBitmap((image as Bitmap).ABitmap, sa, da, APaint);
            da.Dispose();
            sa.Dispose();
        }

        public void DrawImage(Image image, Rectangle to, int fromx, int fromy, int fromw, int fromh, GraphicsUnit gu, ImageAttributes ia)
        {
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

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            APaint.Color = pen.Color.AColor();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            APaint.StrokeWidth = LineWidth;
            ACanvas.DrawLine(x1, y1, x2, y2, APaint);
        }

        public void DrawRectangle(Pen pen, int x1, int y1, int w, int h)
        {
            APaint.Color = pen.Color.AColor();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            APaint.StrokeWidth = LineWidth;
            ACanvas.DrawRect(x1, y1, x1+w, y1+h, APaint);
        }
        public void DrawEllipse(Pen pen, int x, int y, int w, int h)
        {
            APaint.Color = pen.Color.AColor();
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
            APaint.Color = brush.Color.AColor();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.StrokeWidth = LineWidth;
            ACanvas.DrawRect(x1, y1, x1+w, y1+h, APaint);
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            APaint.Color = brush.Color.AColor();
            APaint.Flags = (Android.Graphics.PaintFlags)0;
            APaint.Flags = Flags;
            APaint.SetStyle(Android.Graphics.Paint.Style.Fill);
            APaint.StrokeWidth = LineWidth;
            var p = new Android.Graphics.Path();
            p.MoveTo(points[0].X, points[0].Y);
            foreach(var pt in points)
                p.LineTo(pt.X, pt.Y);
            ACanvas.DrawPath(p, APaint);
            p.Dispose();
        }

        public void FillEllipse(Brush brush, int x, int y, int w, int h)
        {
            APaint.Color = brush.Color.AColor();
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
            APaint.Color = brush.Color.AColor();
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
            APaint.Color = brush.Color.AColor();
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
                var extralen = 0;
                int spaceoffset = tpart.LastIndexOf(' ', tlen-1, tlen/2);
                if (spaceoffset > 0 && coffset+tlen < text.Length)
                {
                    tlen = spaceoffset;
                    extralen = 1;
                }
                int croffset = tpart.IndexOf('\n');
                if (croffset >= 0 && croffset < tlen)
                {
                    tlen = croffset;
                    extralen = 1;
                }
                ACanvas.DrawText(tpart.Substring(0,tlen), (int)rect.X, (int)rect.Y + height + lineheight*cline, APaint);
                coffset += tlen+extralen;
                cline++;
            }

            fm.Dispose();
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
            
            var coffset = 0;
            while(coffset < text.Length)
            {
                var tpart = text.Substring(coffset);
                var tlen = APaint.BreakText(tpart, true, width, null);
                var extralen = 0;
                int spaceoffset = tpart.LastIndexOf(' ', tlen-1, tlen/2);
                if (spaceoffset > 0 && coffset+tlen < text.Length)
                {
                    tlen = spaceoffset;
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
            ACanvas.Translate((float)x, (float)y);
        }

        public void ResetTransform()
        {
            using(var m = new Android.Graphics.Matrix())
                ACanvas.Matrix = m;
        }

        public void Dispose()
        {
            APaint.Dispose();
            ACanvas.Dispose();
        }
    }

}

#endif