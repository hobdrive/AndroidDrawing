 AndroidDrawing
==============
  This is a porting layer between System.Drawing.Graphics and Android.Drawing.Canvas classes.

  Its primary use is in MAUI/Xamarin Android framework.

  This layer allows to share the same primitive drawing code between .net Windows Mobile/Windows Phone platforms and Android platform.

  AndroidDrawing also contains a trivial implementation of System.Graphics Pen, Color, Brush, Size, SizeF, Rectangle.

  FastGraphics implementation (which is default) uses a simple operation batching method.
  This method increases performance if many simple draw operations are performed on Graphics canvas one by one.
  Main gain comes from eliminating too many JNI-Java switches.

 Usage
==============

     // Optional:
     Graphics.DeviceDPI = android_dpi;

     var g = new Graphics(android_canvas);
 
     // Optional:
     g.LineWidth = 2;
     g.Flags = Android.Graphics.PaintFlags.AntiAlias;
     g.DrawLine(new Pen(Color.Red), 0,0,100,100);
     g.DrawText(new Pen(Color.Red), 0,0,100,100);
     g.DrawString("Hello Android", new Font("serif", 12, FontStyle.Italic),
                         new Brush(Color.Black), new RectangleF(0,0,100,100));
     g.DrawImage(new Bitmap(100,100), 0, 0);


 Links
==============

  Issues: http://github.com/hobdrive/AndroidDrawing/issues

  Fork me on github: http://github.com/hobdrive/AndroidDrawing

 Author
==============
  http://github.com/hobdrive

 License
==============
  "MPL 1.1" (With fallback on your choice to GPL 2.0/LGPL 2.1)
