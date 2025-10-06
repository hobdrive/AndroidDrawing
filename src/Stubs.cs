using System;
using System.Collections.Generic;

namespace System
{
#if OLDMFA
    namespace ComponentModel
    {
    public class BindingList<T> : List<T>
    {
        public BindingList ()
        {
        }
    }
    }
#endif

#if OLD_ANDROID_XAMARIN
    namespace Windows.Forms
    {
        public class Application
        {
            public static void DoEvents()
            {
                //throw new NotSupportedException("DoEvents not supported");
                //DOES NOTHING
            }
            public static void Run(object form)
            {
                throw new NotSupportedException("Not supported");
            }
        }

    }
#endif

}

