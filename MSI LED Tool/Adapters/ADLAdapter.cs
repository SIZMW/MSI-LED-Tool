using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MSI_LED_Tool
{
    public class ADLAdapter : AdapterBase
    {
        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_Initialize();

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_GetGPUCounts(out int gpuCounts);

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_GetGraphicsInfo(int iAdapterIndex, out AdlGraphicsInfo graphicsInfo);

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_SetIlluminationParm_RGB(int iAdapterIndex, int cmd, int led1, int led2, int ontime, int offtime, int time, int darktime, int bright, int r, int g, int b, bool one = false);

        public ADLAdapter()
        {
            Manufacturer = Manufacturer.AMD;
            AdapterIndices = new List<int>();
        }

        public override long GetGpuCounts()
        {
            var gotGpuCount = ADL_GetGPUCounts(out int gpuCount);

            return (gotGpuCount) ? gpuCount : 0;
        }

        public override bool Initialize()
        {
            return ADL_Initialize();
        }

        public override bool GetGraphicsInformation(int adapterIndex, out GenericGraphicsInfo info)
        {
            var gotGraphicsInfo = ADL_GetGraphicsInfo(adapterIndex, out AdlGraphicsInfo adlInfo);

            if (gotGraphicsInfo)
            {
                info = new GenericGraphicsInfo(adlInfo);
            }
            else
            {
                info = null;
            }

            return gotGraphicsInfo;
        }

        public override bool InitializeAdapters(long gpuCount, LedSettings settings)
        {
            for (int i = 0; i < gpuCount; i++)
            {
                if (!GetGraphicsInformation(i, out GenericGraphicsInfo graphicsInfo))
                {
                    return false;
                }

                // PCI\VEN_1002&DEV_67DF&SUBSYS_34111462&REV_CF\4&25438C51&0&0008
                var pnpSegments = graphicsInfo.CardPNP.Split('\\');

                if (pnpSegments.Length < 2)
                {
                    continue;
                }

                // VEN_1002&DEV_67DF&SUBSYS_34111462&REV_CF
                var codeSegments = pnpSegments[1].Split('&');

                if (codeSegments.Length < 3)
                {
                    continue;
                }

                string vendorCode = codeSegments[0].Substring(4, 4).ToUpper();
                string deviceCode = codeSegments[1].Substring(4, 4).ToUpper();
                string subVendorCode = codeSegments[2].Substring(11, 4).ToUpper();

                if (settings.OverwriteSecurityChecks)
                {
                    if (vendorCode.Equals(Constants.VendorCodeAmd, StringComparison.OrdinalIgnoreCase))
                    {
                        AdapterIndices.Add(i);
                    }
                }
                else if (vendorCode.Equals(Constants.VendorCodeAmd, StringComparison.OrdinalIgnoreCase)
                    && subVendorCode.Equals(Constants.SubVendorCodeMsi, StringComparison.OrdinalIgnoreCase)
                    && Constants.SupportedDeviceCodes.Any(dc => deviceCode.Equals(dc, StringComparison.OrdinalIgnoreCase)))
                {
                    AdapterIndices.Add(i);
                }
            }

            return true;
        }

        public override void SetIlluminationRGBColor(LedSettings settings, int adapterIndex, int cmd, int ledId, int time, int timeOn = 0, int timeOff = 0, int timeDark = 0, bool doOneCall = false)
        {
            ADL_SetIlluminationParm_RGB(adapterIndex, cmd, ledId, 0, timeOn, timeOff, time, timeDark, 0, settings.R, settings.G, settings.B, doOneCall);
        }
    }
}
