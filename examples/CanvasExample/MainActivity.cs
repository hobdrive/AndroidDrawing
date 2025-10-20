using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using System.Drawing;

namespace CanvasExample
{
    [Activity(Label = "Canvas Example", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create a custom view to demonstrate canvas operations
            var canvasView = new DrawingView(this);
            SetContentView(canvasView);
        }
    }

    public class DrawingView : View
    {
        public DrawingView(Context context) : base(context)
        {
        }

        protected override void OnDraw(Android.Graphics.Canvas? canvas)
        {
            base.OnDraw(canvas);
            
            if (canvas == null)
                return;

            // Use AndroidDrawing Graphics wrapper
            var g = new Graphics(canvas);
            
            // Set DPI for proper scaling
            Graphics.DeviceDPI = (int)Resources!.DisplayMetrics!.Density * 160;
            
            // Set line width and flags
            g.LineWidth = 2;
            g.Flags = Android.Graphics.PaintFlags.AntiAlias;
            
            // Draw background
            g.FillRectangle(new Brush(Color.White), 0, 0, Width, Height);
            
            // Draw various shapes to demonstrate canvas operations
            
            // Draw a red line
            g.DrawLine(new Pen(Color.Red), 50, 50, 350, 50);
            
            // Draw a blue rectangle
            g.DrawRectangle(new Pen(Color.Blue), 50, 100, 300, 100);
            
            // Fill a green rectangle
            g.FillRectangle(new Brush(Color.Green), 70, 120, 260, 60);
            
            // Draw an orange ellipse
            g.DrawEllipse(new Pen(Color.Orange), 50, 250, 300, 150);
            
            // Fill a purple ellipse
            g.FillEllipse(new Brush(Color.Purple), 100, 280, 200, 90);
            
            // Draw text
            g.DrawString("AndroidDrawing Example", 
                new Font("serif", 20, FontStyle.Bold),
                new Brush(Color.Black), 
                new RectangleF(50, 450, 300, 50));
            
            // Draw multiline text in a rectangle
            g.DrawString("This demonstrates various canvas operations:\n• Lines\n• Rectangles\n• Ellipses\n• Text rendering", 
                new Font("serif", 14, FontStyle.Regular),
                new Brush(Color.DarkBlue), 
                new RectangleF(50, 520, 300, 200));
            
            // Flush to complete drawing
            g.Flush();
        }
    }
}
