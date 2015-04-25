using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Interop;
using System.Diagnostics;

namespace DDCMonitorManager
{
    class BrightnessControl
    {

        private IntPtr hWnd;
        private NativeStructures.PHYSICAL_MONITOR[] pPhysicalMonitorArray;
        private uint pdwNumberOfPhysicalMonitors;

        public BrightnessControl(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            SetupMonitors();
        }

        private void SetupMonitors()
        {
            IntPtr hMonitor = NativeCalls.MonitorFromWindow(hWnd, NativeConstants.MONITOR_DEFAULTTOPRIMARY);
            int lastWin32Error = Marshal.GetLastWin32Error();

            bool numberOfPhysicalMonitorsFromHmonitor = NativeCalls.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors);
            lastWin32Error = Marshal.GetLastWin32Error();

            pPhysicalMonitorArray = new NativeStructures.PHYSICAL_MONITOR[pdwNumberOfPhysicalMonitors];
            bool physicalMonitorsFromHmonitor = NativeCalls.GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray);
            lastWin32Error = Marshal.GetLastWin32Error();
        }

        private void GetMonitorCapabilities(int monitorNumber)
        {
            uint pdwMonitorCapabilities = 0u;
            uint pdwSupportedColorTemperatures = 0u;
            var monitorCapabilities = NativeCalls.GetMonitorCapabilities(pPhysicalMonitorArray[monitorNumber].hPhysicalMonitor, ref pdwMonitorCapabilities, ref pdwSupportedColorTemperatures);
            Debug.WriteLine(pdwMonitorCapabilities);
            Debug.WriteLine(pdwSupportedColorTemperatures);
            int lastWin32Error = Marshal.GetLastWin32Error();
            NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE type = NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE.MC_SHADOW_MASK_CATHODE_RAY_TUBE;
            var monitorTechnologyType = NativeCalls.GetMonitorTechnologyType(pPhysicalMonitorArray[monitorNumber].hPhysicalMonitor, ref type);
            Debug.WriteLine(type);
            lastWin32Error = Marshal.GetLastWin32Error();
        }

        public bool SetBrightness(short brightness,int monitorNumber)
        {
            var brightnessWasSet = NativeCalls.SetMonitorBrightness(pPhysicalMonitorArray[monitorNumber].hPhysicalMonitor, (short)brightness);
            if (brightnessWasSet)
                Debug.WriteLine("Brightness set to " + (short)brightness);
            int lastWin32Error = Marshal.GetLastWin32Error();
            return brightnessWasSet;
        }

        public BrightnessInfo GetBrightnessCapabilities(int monitorNumber)
        {
            short current = -1, minimum = -1, maximum = -1;
            bool getBrightness = NativeCalls.GetMonitorBrightness(pPhysicalMonitorArray[monitorNumber].hPhysicalMonitor,ref minimum,ref current,ref maximum);
            int lastWin32Error = Marshal.GetLastWin32Error();
            return new BrightnessInfo { minimum = minimum, maximum = maximum, current = current};
        }

        public void DestroyMonitors()
        {
            var destroyPhysicalMonitors = NativeCalls.DestroyPhysicalMonitors(pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray);
            int lastWin32Error = Marshal.GetLastWin32Error();
        }

        public uint GetMonitors()
        {
            return pdwNumberOfPhysicalMonitors;
        }
    }
}
