using System.Collections.Generic;

namespace MSI_LED_Tool
{
    public abstract class AdapterBase : IAdapter
    {
        protected GenericGraphicsInfo graphicsInfo;

        public AdapterBase()
        {
            graphicsInfo = new GenericGraphicsInfo();
        }

        public Manufacturer Manufacturer { get; protected set; }

        public IList<int> AdapterIndices { get; protected set; }

        public int GetAdapterIndexCount()
        {
            return AdapterIndices.Count;
        }

        public abstract long GetGpuCounts();

        public abstract bool Initialize();

        public abstract bool GetGraphicsInformation(int adapterIndex, out GenericGraphicsInfo info);

        public abstract bool InitializeAdapters(long gpuCount, LedSettings settings);

        public abstract void SetIlluminationRGBColor(LedSettings settings, int adapterIndex, int cmd, int ledId, int time, int timeOn = 0, int timeOff = 0, int timeDark = 0, bool doOneCall = false);
    }
}
