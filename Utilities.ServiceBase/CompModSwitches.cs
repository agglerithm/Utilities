using System.Diagnostics;

namespace Utilities.WinMockups
{
    internal static class CompModSwitches
    {
        private static TraceSwitch installerDesign;

        public static TraceSwitch InstallerDesign
        {
            get
            {
                if (CompModSwitches.installerDesign == null)
                    CompModSwitches.installerDesign = new TraceSwitch("InstallerDesign", "Enable tracing for design-time code for installers");
                return CompModSwitches.installerDesign;
            }
        }
    }
}