using System.Runtime.InteropServices;

namespace MSI_LED_Tool
{
    public class GenericGraphicsInfo
    {
        public GenericGraphicsInfo()
        {

        }

        public GenericGraphicsInfo(AdlGraphicsInfo info)
        {
            AdapterIndex = info.iAdapterIndex;
            IsPrimaryDisplay = info.IsPrimaryDisplay;
            DisplayName = info.DisplayName;
            CardPNP = info.Card_PNP;
            CardDeviceId = info.Card_pDeviceId;
            CardSubsystemId = info.Card_pSubSystemId;
            CardRevisionId = info.Card_pRevisionId;
            CardFullName = info.Card_FullName;
            CardBiosDate = info.Card_BIOS_Date;
            CardBiosPartNumber = info.Card_BIOS_PartNumber;
            CardBiosVersion = info.Card_BIOS_Version;
            GpuUsage = info.GPU_Usage;
            GpuCurrentClock = info.GPU_Clock_Current;
            GpuBaseClock = -1;
            GpuClockSet = -1;
            GpuMaxClock = info.GPU_Clock_Max;
            GpuMinClock = -1;
            VRAMUsage = -1;
            VRAMCurrentClock = -1;
            VRAMBaseClock = -1;
            VRAMMaxClock = -1;
            VRAMMinClock = -1;
            GpuCurrentTemperature = info.GPU_Temperature_Current;
            GpuCurrentVoltage = info.GPU_Voltage_Current;
            GpuCurrentFanPercent = info.GPU_FanPercent_Current;
            TotalMemorySize = info.Memory_TotalSize;
            MemoryClock = info.Memory_Clock;
        }

        public GenericGraphicsInfo(NdaGraphicsInfo info)
        {
            AdapterIndex = info.iAdapterIndex;
            IsPrimaryDisplay = info.IsPrimaryDisplay;
            DisplayName = info.DisplayName;
            CardPNP = info.Card_PNP;
            CardDeviceId = info.Card_pDeviceId;
            CardSubsystemId = info.Card_pSubSystemId;
            CardRevisionId = info.Card_pRevisionId;
            CardFullName = info.Card_FullName;
            CardBiosDate = info.Card_BIOS_Date;
            CardBiosPartNumber = info.Card_BIOS_PartNumber;
            CardBiosVersion = info.Card_BIOS_Version;
            GpuUsage = info.GPU_Usage;
            GpuCurrentClock = info.GPU_Clock_Current;
            GpuBaseClock = info.GPU_Clock_Base;
            GpuClockSet = info.GPU_Clock_Set;
            GpuMaxClock = info.GPU_Clock_Max;
            GpuMinClock = info.GPU_Clock_Min;
            VRAMUsage = info.VRAM_Usage;
            VRAMCurrentClock = info.VRAM_Clock_Current;
            VRAMBaseClock = info.VRAM_Clock_Base;
            VRAMMaxClock = info.VRAM_Clock_Max;
            VRAMMinClock = info.VRAM_Clock_Min;
            GpuCurrentTemperature = info.GPU_Temperature_Current;
            GpuCurrentVoltage = info.GPU_Voltage_Current;
            GpuCurrentFanPercent = info.GPU_FanPercent_Current;
            TotalMemorySize = info.Memory_TotalSize;
            MemoryClock = -1;
        }

        public int AdapterIndex;

        public bool IsPrimaryDisplay;

        [MarshalAs(UnmanagedType.BStr)]
        public string DisplayName;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardPNP;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardDeviceId;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardSubsystemId;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardRevisionId;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardFullName;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardBiosDate;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardBiosPartNumber;

        [MarshalAs(UnmanagedType.BStr)]
        public string CardBiosVersion;

        public int GpuUsage;

        public int GpuCurrentClock;

        public int GpuBaseClock;

        public int GpuClockSet;

        public int GpuMaxClock;

        public int GpuMinClock;

        public int VRAMUsage;

        public int VRAMCurrentClock;

        public int VRAMBaseClock;

        public int VRAMMaxClock;

        public int VRAMMinClock;

        public int GpuCurrentTemperature;

        public float GpuCurrentVoltage;

        public int GpuCurrentFanPercent;

        public int TotalMemorySize;

        public int MemoryClock;

        public void CopyFrom(AdlGraphicsInfo info)
        {
            AdapterIndex = info.iAdapterIndex;
            IsPrimaryDisplay = info.IsPrimaryDisplay;
            DisplayName = info.DisplayName;
            CardPNP = info.Card_PNP;
            CardDeviceId = info.Card_pDeviceId;
            CardSubsystemId = info.Card_pSubSystemId;
            CardRevisionId = info.Card_pRevisionId;
            CardFullName = info.Card_FullName;
            CardBiosDate = info.Card_BIOS_Date;
            CardBiosPartNumber = info.Card_BIOS_PartNumber;
            CardBiosVersion = info.Card_BIOS_Version;
            GpuUsage = info.GPU_Usage;
            GpuCurrentClock = info.GPU_Clock_Current;
            GpuBaseClock = -1;
            GpuClockSet = -1;
            GpuMaxClock = info.GPU_Clock_Max;
            GpuMinClock = -1;
            VRAMUsage = -1;
            VRAMCurrentClock = -1;
            VRAMBaseClock = -1;
            VRAMMaxClock = -1;
            VRAMMinClock = -1;
            GpuCurrentTemperature = info.GPU_Temperature_Current;
            GpuCurrentVoltage = info.GPU_Voltage_Current;
            GpuCurrentFanPercent = info.GPU_FanPercent_Current;
            TotalMemorySize = info.Memory_TotalSize;
            MemoryClock = info.Memory_Clock;
        }

        public void CopyFrom(NdaGraphicsInfo info)
        {
            AdapterIndex = info.iAdapterIndex;
            IsPrimaryDisplay = info.IsPrimaryDisplay;
            DisplayName = info.DisplayName;
            CardPNP = info.Card_PNP;
            CardDeviceId = info.Card_pDeviceId;
            CardSubsystemId = info.Card_pSubSystemId;
            CardRevisionId = info.Card_pRevisionId;
            CardFullName = info.Card_FullName;
            CardBiosDate = info.Card_BIOS_Date;
            CardBiosPartNumber = info.Card_BIOS_PartNumber;
            CardBiosVersion = info.Card_BIOS_Version;
            GpuUsage = info.GPU_Usage;
            GpuCurrentClock = info.GPU_Clock_Current;
            GpuBaseClock = info.GPU_Clock_Base;
            GpuClockSet = info.GPU_Clock_Set;
            GpuMaxClock = info.GPU_Clock_Max;
            GpuMinClock = info.GPU_Clock_Min;
            VRAMUsage = info.VRAM_Usage;
            VRAMCurrentClock = info.VRAM_Clock_Current;
            VRAMBaseClock = info.VRAM_Clock_Base;
            VRAMMaxClock = info.VRAM_Clock_Max;
            VRAMMinClock = info.VRAM_Clock_Min;
            GpuCurrentTemperature = info.GPU_Temperature_Current;
            GpuCurrentVoltage = info.GPU_Voltage_Current;
            GpuCurrentFanPercent = info.GPU_FanPercent_Current;
            TotalMemorySize = info.Memory_TotalSize;
            MemoryClock = -1;
        }
    }
}
