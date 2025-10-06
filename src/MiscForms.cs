using System;
using System.Drawing;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace System.Windows.Forms
{
    public class PaintEventArgs
    {
        public PaintEventArgs(Graphics g, Rectangle r)
        {
            Graphics = g;
            ClipRectangle = r;
        }
        public Graphics Graphics { get; set; }
        public Rectangle ClipRectangle { get; set; }
    }

#if OLD_ANDROID_XAMARIN

    public class Timer : IDisposable
    {
        System.Timers.Timer cdt;
        public event EventHandler Tick;
        private bool enabled;

        public bool Enabled {
            get{
                return enabled;
            }
            set{
                //
                enabled = value;
                if (enabled){
                    cdt = new System.Timers.Timer(Interval);
                    cdt.Elapsed += (sender, e) => {
                        Tick(this, e);
                    };
                    cdt.Enabled = true;
                }else{
                    if (cdt != null)
                        cdt.Enabled = false;
                }

            }
        }

        public int Interval{get; set;}

        public void Dispose()
        {
            if (cdt != null)
                cdt.Enabled = false;
            cdt = null;
        }
    }

#endif

}

