﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE
{
    public static class BabyIDEVisualStudioServices
    {
        public static EnvDTE.DTE DTE
        {
            get;
            set;
        }

        public static Microsoft.VisualStudio.OLE.Interop.IServiceProvider OLEServiceProvider
        {
            get;
            set;
        }

        public static System.IServiceProvider ServiceProvider
        {
            get;
            set;
        }
    }
}