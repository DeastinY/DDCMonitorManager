using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Interop;

namespace DDCMonitorManager
{
    class BrightnessControl
    {
        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow", SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(
            [In] IntPtr hwnd, uint dwFlags);

        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor,
            uint dwPhysicalMonitorArraySize,
            [Out] NativeStructures.PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(
            uint dwPhysicalMonitorArraySize, [Out] NativeStructures.PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorTechnologyType", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorTechnologyType(
            IntPtr hMonitor, ref NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE pdtyDisplayTechnologyType);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorCapabilities", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorCapabilities(
            IntPtr hMonitor, ref uint pdwMonitorCapabilities, ref uint pdwSupportedColorTemperatures);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMonitorBrightness(
            IntPtr hMonitor, short brightness);

        public class NativeConstants
        {
            public const int MONITOR_DEFAULTTOPRIMARY = 1;

            public const int MONITOR_DEFAULTTONEAREST = 2;

            public const int MONITOR_DEFAULTTONULL = 0;
        }

        public class NativeStructures
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct PHYSICAL_MONITOR
            {
                public IntPtr hPhysicalMonitor;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szPhysicalMonitorDescription;
            }

            public enum MC_DISPLAY_TECHNOLOGY_TYPE
            {
                MC_SHADOW_MASK_CATHODE_RAY_TUBE,

                MC_APERTURE_GRILL_CATHODE_RAY_TUBE,

                MC_THIN_FILM_TRANSISTOR,

                MC_LIQUID_CRYSTAL_ON_SILICON,

                MC_PLASMA,

                MC_ORGANIC_LIGHT_EMITTING_DIODE,

                MC_ELECTROLUMINESCENT,

                MC_MICROELECTROMECHANICAL,

                MC_FIELD_EMISSION_DEVICE,
            }
        }

        private struct PHYSICAL_MONITOR
        {
            Int32 hPhysicalMonitor;

        }

        private IntPtr hWnd;

        public BrightnessControl(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public unsafe bool SetBrightness(short brightness)
        {
            IntPtr hMonitor = MonitorFromWindow(hWnd, NativeConstants.MONITOR_DEFAULTTOPRIMARY);
            int lastWin32Error = Marshal.GetLastWin32Error();

            uint pdwNumberOfPhysicalMonitors = 0u;
            bool numberOfPhysicalMonitorsFromHmonitor = GetNumberOfPhysicalMonitorsFromHMONITOR(
                hMonitor, ref pdwNumberOfPhysicalMonitors);
            lastWin32Error = Marshal.GetLastWin32Error();

            NativeStructures.PHYSICAL_MONITOR[] pPhysicalMonitorArray =
                new NativeStructures.PHYSICAL_MONITOR[pdwNumberOfPhysicalMonitors];
            bool physicalMonitorsFromHmonitor = GetPhysicalMonitorsFromHMONITOR(
                hMonitor, pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray);
            lastWin32Error = Marshal.GetLastWin32Error();

            uint pdwMonitorCapabilities = 0u;
            uint pdwSupportedColorTemperatures = 0u;
            var monitorCapabilities = GetMonitorCapabilities(
                pPhysicalMonitorArray[0].hPhysicalMonitor, ref pdwMonitorCapabilities, ref pdwSupportedColorTemperatures);
            lastWin32Error = Marshal.GetLastWin32Error();

            var brightnessWasSet = SetMonitorBrightness(pPhysicalMonitorArray[0].hPhysicalMonitor, (short)brightness);
            if (brightnessWasSet)
                System.Console.WriteLine("Brightness set to " + (short)brightness);
            lastWin32Error = Marshal.GetLastWin32Error();

            NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE type =
                NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE.MC_SHADOW_MASK_CATHODE_RAY_TUBE;
            var monitorTechnologyType = GetMonitorTechnologyType(
                pPhysicalMonitorArray[0].hPhysicalMonitor, ref type);
            lastWin32Error = Marshal.GetLastWin32Error();

            var destroyPhysicalMonitors = DestroyPhysicalMonitors(
                pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray);
            lastWin32Error = Marshal.GetLastWin32Error();

            return true;
        }
    }
}
