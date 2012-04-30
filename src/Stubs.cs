using System;
using System.Collections.Generic;

namespace System
{
    namespace ComponentModel
    {
    public class BindingList<T> : List<T>
    {
        public BindingList ()
        {
        }
    }
    }

    namespace Windows.Forms
    {
        public class Application
        {
            public static void DoEvents(){
                //throw new NotSupportedException("DoEvents not supported");
                //DOES NOTHING
            }
            public static void Run(object form)
            {
                throw new NotSupportedException("Not supported");
            }
        }

    }
}

