using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI_LED_Tool
{
    public interface IAdapter
    {
        int GetAdapterIndexCount();

        bool Initialize();

        long GetGpuCounts();

        bool GetGraphicsInformation(int adapterIndex, out GenericGraphicsInfo info);

        bool InitializeAdapters(long gpuCount, LedSettings settings);

        void SetIlluminationRGBColor(LedSettings settings, int adapterIndex, int cmd, int ledId, int time, int timeOn = 0, int timeOff = 0, int timeDark = 0, bool doOneCall = false);
    }
}
