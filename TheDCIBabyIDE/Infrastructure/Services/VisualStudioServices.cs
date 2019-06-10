using Microsoft.VisualStudio.ComponentModelHost;

namespace KimHaiQuang.TheDCIBabyIDE.Services
{
    
    public static class VisualStudioServices
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

        public static IComponentModel ComponentModel { get; set; }
    }
}
