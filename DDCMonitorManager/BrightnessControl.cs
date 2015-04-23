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
        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);

        [DllImport("user32.dll")]
        static extern Int32 MonitorFromWindow(IntPtr hwnd, Int32 dwflags);

        [DllImport("Dxva2.dll")]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(Int32 hMonitor,out Int32 num);

        [DllImport("Dxva2.dll")]
        private static extern bool SetMonitorBrightness(Int32 hMonitor, short newBrightness);

        [DllImport("Dxva2.dll")]
        private static extern bool GetMonitorCapabilities(Int32 hMonitor, out Int32 capabilities, out Int32 supportedColorTemperatures);

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

            SetMonitorBrightness(hMonitor, brightness);

            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    int arrayVal = i * (brightness + 128);

                    if (arrayVal > 65535)
                        arrayVal = 65535;

                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            //For some reason, this always returns false?
            //bool retVal = SetDeviceGammaRamp(hdc, gArray);

            //Memory allocated through stackalloc is automatically free'd
            //by the CLR.

            return false;
        }
    }
}
