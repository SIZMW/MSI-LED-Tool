using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MSI_LED_Tool
{
    public class NDAAdapter : AdapterBase
    {
        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        public static extern bool NDA_Initialize();

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        public static extern bool NDA_GetGPUCounts(out long gpuCounts);

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        public static extern bool NDA_GetGraphicsInfo(int iAdapterIndex, out NdaGraphicsInfo graphicsInfo);

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        public static extern bool NDA_SetIlluminationParmColor_RGB(int iAdapterIndex, int cmd, int led1, int led2, int ontime, int offtime, int time, int darktime, int bright, int r, int g, int b, bool one = false);

        public NDAAdapter()
        {
            Manufacturer = Manufacturer.Nvidia;
            AdapterIndices = new List<int>();
        }

        public override long GetGpuCounts()
        {
            var gotGpuCount = NDA_GetGPUCounts(out long gpuCount);

            return (gotGpuCount) ? gpuCount : 0;
        }

        public override bool Initialize()
        {
            return NDA_Initialize();
        }

        public override bool GetGraphicsInformation(int adapterIndex, out GenericGraphicsInfo info)
        {
            var gotGraphicsInfo = NDA_GetGraphicsInfo(adapterIndex, out NdaGraphicsInfo ndaInfo);

            if (gotGraphicsInfo)
            {
                info = new GenericGraphicsInfo(ndaInfo);
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

                string vendorCode = graphicsInfo.CardDeviceId.Substring(4, 4).ToUpper();
                string deviceCode = graphicsInfo.CardDeviceId.Substring(0, 4).ToUpper();
                string subVendorCode = graphicsInfo.CardSubsystemId.Substring(4, 4).ToUpper();

                if (settings.OverwriteSecurityChecks)
                {
                    if (vendorCode.Equals(Constants.VendorCodeNvidia, StringComparison.OrdinalIgnoreCase))
                    {
                        AdapterIndices.Add(i);
                    }
                }
                else if (vendorCode.Equals(Constants.VendorCodeNvidia, StringComparison.OrdinalIgnoreCase)
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
            NDA_SetIlluminationParmColor_RGB(adapterIndex, cmd, ledId, 0, timeOn, timeOff, time, timeDark, 0, settings.R, settings.G, settings.B, doOneCall);
        }
    }
}
