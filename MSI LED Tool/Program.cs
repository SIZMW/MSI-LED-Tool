﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MSI_LED_Tool
{
    class Program
    {
        private const string SettingsFileName = "Settings.json";

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        public static extern bool NDA_Initialize();

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_Initialize();

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        private static extern bool NDA_GetGPUCounts(out long gpuCounts);

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        private static extern bool NDA_GetGraphicsInfo(int iAdapterIndex, out NdaGraphicsInfo graphicsInfo);

        [DllImport("Lib\\NDA.dll", CharSet = CharSet.Unicode)]
        private static extern bool NDA_SetIlluminationParmColor_RGB(int iAdapterIndex, int cmd, int led1, int led2, int ontime, int offtime, int time, int darktime, int bright, int r, int g, int b, bool one = false);

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_GetGPUCounts(out int gpuCounts);

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_GetGraphicsInfo(int iAdapterIndex, out AdlGraphicsInfo graphicsInfo);

        [DllImport("Lib\\ADL.dll", CharSet = CharSet.Unicode)]
        public static extern bool ADL_SetIlluminationParm_RGB(int iAdapterIndex, int cmd, int led1, int led2, int ontime, int offtime, int time, int darktime, int bright, int r, int g, int b, bool one = false);

        private const int NoAnimationDelay = 60000;

        private static Thread updateThreadFront;
        private static Thread updateThreadBack;
        private static Thread updateThreadSide;

        private static List<int> adapterIndexes;

        private static bool vgaMutex;
        private static Manufacturer manufacturer;
        private static LedSettings ledSettings;

        private static Mutex mutex;
        private static NdaGraphicsInfo ndaGraphicsInfo;
        private static AdlGraphicsInfo adlGraphicsInfo;

        static void Main(string[] args)
        {
            mutex = new Mutex();

            string settingsFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\{SettingsFileName}";
            InitializeFromSettings(settingsFile);

            adapterIndexes = new List<int>();

            long gpuCountNda = 0;

            if (NDA_Initialize())
            {
                bool canGetGpuCount = NDA_GetGPUCounts(out gpuCountNda);
                if (canGetGpuCount == false)
                {
                    return;
                }

                if (gpuCountNda > 0 && InitializeNvidiaAdapters(gpuCountNda))
                {
                    manufacturer = Manufacturer.Nvidia;
                }
            }

            if (gpuCountNda == 0 && ADL_Initialize())
            {
                int gpuCountAdl;
                bool canGetGpuCount = ADL_GetGPUCounts(out gpuCountAdl);
                if (canGetGpuCount == false)
                {
                    return;
                }

                if (gpuCountAdl > 0 && InitializeAmdAdapters(gpuCountAdl))
                {
                    manufacturer = Manufacturer.AMD;
                }
            }

            if (adapterIndexes.Count > 0)
            {
                updateThreadFront = new Thread(UpdateLedsFront);
                updateThreadSide = new Thread(UpdateLedsSide);
                updateThreadBack = new Thread(UpdateLedsBack);
                updateThreadFront.Start();
                updateThreadSide.Start();
                updateThreadBack.Start();
            }
            else
            {
                MessageBox.Show(
                    "No adapters found that are supported by this tool. Report a new issue with a GPU-Z screenshot if you want your card added.",
                    "No supported adapter(s) found.");
            }

            while (true)
            {
                Thread.CurrentThread.Join(new TimeSpan(1, 0, 0));
            }
        }

        private static void InitializeFromSettings(string settingsFile)
        {
            if (File.Exists(settingsFile))
            {
                using (var sr = new StreamReader(settingsFile))
                {
                    ledSettings = JsonSerializer<LedSettings>.DeSerialize(sr.ReadToEnd()) ?? new LedSettings();
                }
            }
            else
            {
                ledSettings = new LedSettings();

                using (var sw = new StreamWriter(settingsFile, false))
                {
                    sw.WriteLine(JsonSerializer<LedSettings>.Serialize(ledSettings));
                }
            }
        }

        #region Adapter Initializations

        private static bool InitializeNvidiaAdapters(long gpuCount)
        {
            for (int i = 0; i < gpuCount; i++)
            {
                NdaGraphicsInfo graphicsInfo; 
                if (NDA_GetGraphicsInfo(i, out graphicsInfo) == false)
                {
                    return false;
                }

                string vendorCode = graphicsInfo.Card_pDeviceId.Substring(4, 4).ToUpper();
                string deviceCode = graphicsInfo.Card_pDeviceId.Substring(0, 4).ToUpper();
                string subVendorCode = graphicsInfo.Card_pSubSystemId.Substring(4, 4).ToUpper();

                if (ledSettings.OverwriteSecurityChecks)
                {
                    if (vendorCode.Equals(Constants.VendorCodeNvidia, StringComparison.OrdinalIgnoreCase))
                    {
                        adapterIndexes.Add(i);
                    }
                }
                else if (vendorCode.Equals(Constants.VendorCodeNvidia, StringComparison.OrdinalIgnoreCase)
                    && subVendorCode.Equals(Constants.SubVendorCodeMsi, StringComparison.OrdinalIgnoreCase)
                    && Constants.SupportedDeviceCodes.Any(dc => deviceCode.Equals(dc, StringComparison.OrdinalIgnoreCase)))
                {
                    adapterIndexes.Add(i);
                }
            }

            return true;
        }

        private static bool InitializeAmdAdapters(int gpuCount)
        {
            for (int i = 0; i < gpuCount; i++)
            {
                AdlGraphicsInfo graphicsInfo;
                if (ADL_GetGraphicsInfo(i, out graphicsInfo) == false)
                {
                    return false;
                }
                
                // PCI\VEN_1002&DEV_67DF&SUBSYS_34111462&REV_CF\4&25438C51&0&0008
                var pnpSegments = graphicsInfo.Card_PNP.Split('\\');
                
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

                if (ledSettings.OverwriteSecurityChecks)
                {
                    if (vendorCode.Equals(Constants.VendorCodeAmd, StringComparison.OrdinalIgnoreCase))
                    {
                        adapterIndexes.Add(i);
                    }
                }
                else if (vendorCode.Equals(Constants.VendorCodeAmd, StringComparison.OrdinalIgnoreCase)
                    && subVendorCode.Equals(Constants.SubVendorCodeMsi, StringComparison.OrdinalIgnoreCase)
                    && Constants.SupportedDeviceCodes.Any(dc => deviceCode.Equals(dc, StringComparison.OrdinalIgnoreCase)))
                {
                    adapterIndexes.Add(i);
                }
            }

            return true;
        }

        #endregion

        private static void UpdateLedsFront()
        {
            while (true)
            {
                switch (ledSettings.AnimationType)
                {
                    case AnimationType.NoAnimation:
                        UpdateLeds(21, 4, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
                    case AnimationType.Breathing:
                        UpdateLeds(27, 4, 7);
                        break;
                    case AnimationType.Flashing:
                        UpdateLeds(28, 4, 0, 25, 100);
                        break;
                    case AnimationType.DoubleFlashing:
                        UpdateLeds(30, 4, 0, 10, 10, 91);
                        break;
                    case AnimationType.Off:
                        UpdateLeds(24, 4, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
                    case AnimationType.TemperatureBased:
                        switch (manufacturer)
                        {
                            case Manufacturer.Nvidia:
                                mutex.WaitOne();
                                if (NDA_GetGraphicsInfo(0, out ndaGraphicsInfo))
                                {
                                    int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                        ledSettings.TemperatureUpperLimit, ndaGraphicsInfo.GPU_Temperature_Current);
                                    ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                                    UpdateLeds(21, 4, 4);
                                }
                                mutex.ReleaseMutex();
                                break;
                            case Manufacturer.AMD:
                                mutex.WaitOne();
                                if (ADL_GetGraphicsInfo(0, out adlGraphicsInfo))
                                {
                                    int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                        ledSettings.TemperatureUpperLimit, adlGraphicsInfo.GPU_Temperature_Current);
                                    ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                                    UpdateLeds(21, 4, 4);
                                }
                                mutex.ReleaseMutex();
                                break;
                        }
                        break;
                }
            }
        }


        private static void UpdateLedsSide()
        {
            while (true)
            {
                switch (ledSettings.AnimationType)
                {
                    case AnimationType.NoAnimation:
                        UpdateLeds(21, 1, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
                    case AnimationType.Breathing:
                        UpdateLeds(27, 1, 7);
                        break;
                    case AnimationType.Flashing:
                        UpdateLeds(28, 1, 0, 25, 100);
                        break;
                    case AnimationType.DoubleFlashing:
                        UpdateLeds(30, 1, 0, 10, 10, 91);
                        break;
                    case AnimationType.Off:
                        UpdateLeds(24, 1, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
                    case AnimationType.TemperatureBased:
                        switch (manufacturer)
                        {
                            case Manufacturer.Nvidia:
                                mutex.WaitOne();
                                if (NDA_GetGraphicsInfo(0, out ndaGraphicsInfo))
                                {
                                    int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                        ledSettings.TemperatureUpperLimit, ndaGraphicsInfo.GPU_Temperature_Current);
                                    ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                                    UpdateLeds(21, 1, 4);
                                }
                                mutex.ReleaseMutex();
                                break;
                            case Manufacturer.AMD:
                                mutex.WaitOne();
                                if (ADL_GetGraphicsInfo(0, out adlGraphicsInfo))
                                {
                                    int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                        ledSettings.TemperatureUpperLimit, adlGraphicsInfo.GPU_Temperature_Current);
                                    ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                                    UpdateLeds(21, 1, 4);
                                }
                                mutex.ReleaseMutex();
                                break;
                        }
                        break;
                }
            }
        }

        private static void UpdateLedsBack()
        {
            while (true)
            {
                switch (ledSettings.AnimationType)
                {
                    case AnimationType.NoAnimation:
                        UpdateLeds(21, 2, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
                    case AnimationType.Breathing:
                        UpdateLeds(27, 2, 7);
                        break;
                    case AnimationType.Flashing:
                        UpdateLeds(28, 2, 0, 25, 100);
                        break;
                    case AnimationType.DoubleFlashing:
                        UpdateLeds(30, 2, 0, 10, 10, 91);
                        break;
                    case AnimationType.Off:
                        UpdateLeds(24, 2, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
                    case AnimationType.TemperatureBased:
                        switch (manufacturer)
                        {
                            case Manufacturer.Nvidia:
                                mutex.WaitOne();
                                if (NDA_GetGraphicsInfo(0, out ndaGraphicsInfo))
                                {
                                    int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                        ledSettings.TemperatureUpperLimit, ndaGraphicsInfo.GPU_Temperature_Current);
                                    ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                                    UpdateLeds(21, 2, 4);
                                }
                                mutex.ReleaseMutex();
                                break;
                            case Manufacturer.AMD:
                                mutex.WaitOne();
                                if (ADL_GetGraphicsInfo(0, out adlGraphicsInfo))
                                {
                                    int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                        ledSettings.TemperatureUpperLimit, adlGraphicsInfo.GPU_Temperature_Current);
                                    ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                                    UpdateLeds(21, 2, 4);
                                }
                                mutex.ReleaseMutex();
                                break;
                        }
                        break;
                }
            }
        }

        private static void UpdateLeds(int cmd, int ledId, int time, int ontime = 0, int offtime = 0, int darkTime = 0)
        {
            for (int i = 0; i < adapterIndexes.Count; i++)
            {
                Thread.CurrentThread.Join(10);
                for (int index = 0; vgaMutex && index < 100; ++index)
                {
                    Thread.CurrentThread.Join(5);
                }
                vgaMutex = true;
                Thread.CurrentThread.Join(20);

                bool oneCall = ledSettings.AnimationType != AnimationType.NoAnimation;

                if (manufacturer == Manufacturer.Nvidia)
                {
                    NDA_SetIlluminationParmColor_RGB(i, cmd, ledId, 0, ontime, offtime, time, darkTime, 0, ledSettings.R, ledSettings.G, ledSettings.B, oneCall);
                }

                if (manufacturer == Manufacturer.AMD)
                {
                    ADL_SetIlluminationParm_RGB(i, cmd, ledId, 0, ontime, offtime, time, darkTime, 0, ledSettings.R, ledSettings.G, ledSettings.B, oneCall);
                }

                vgaMutex = false;
            }

            Thread.CurrentThread.Join(2000);
        }
    }
}