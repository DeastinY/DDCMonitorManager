using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DDCMonitorManager
{
    class BrightnessControl
    {
        [DllImport("user32.dll")]
        private extern Int32 MonitorFromWindow(IntPtr hwnd, Int32 dwflags);

        [DllImport("Dxva2.dll")]
        private extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(Int32 hMonitor,out Int32 num);

        [DllImport("Dxva2.dll")]
        private extern bool SetMonitorBrightness(Int32 hMonitor, short newBrightness);

        [DllImport("Dxva2.dll")]
        private extern bool GetMonitorCapabilities(Int32 hMonitor, out Int32 capabilities, out Int32 supportedColorTemperatures);

        private IntPtr hWnd;

        public BrightnessControl(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public unsafe bool SetBrightness(short brightness)
        {
            Int32 hMonitor = MonitorFromWindow(hWnd, 1); //MONITOR_DEFAULTTOPRIMARY
            Int32 num = 0;
            GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor,out num);
            System.Console.WriteLine(num + " physical monitors detected.");
            Int32 capabilities = 0, supportedColorTemperatures = 0;
            if (!GetMonitorCapabilities(hMonitor, out capabilities, out supportedColorTemperatures))
            { 
                //DDC/CI not supported
                System.Console.WriteLine("DDC/CI not supported by monitor.");
            }

            if (brightness > 255)
                brightness = 255;

            if (brightness < 0)
                brightness = 0;

            return SetMonitorBrightness(hMonitor, brightness);
        }
    }
}
