using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MSI_LED_Tool
{
    public class Program
    {
        private const string SettingsFileName = "Settings.json";

        private const int NoAnimationDelay = 60000;
        private const int ColorCycleDelay = 3000;

        private static Thread updateThreadFront;
        private static Thread updateThreadBack;
        private static Thread updateThreadSide;

        private static Mutex mutex;
        private static bool vgaMutex;
        private static LedSettings ledSettings;

        private static IAdapter graphicsAdapter;

        private static readonly Queue<Color> colorQueue = new Queue<Color>(new Color[]
        {
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(255, 5, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(0, 127, 255),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(5, 0, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(255, 255, 255),
        });

        public static void Main(string[] args)
        {
            mutex = new Mutex();

            string settingsFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\{SettingsFileName}";
            ledSettings = InitializeFromSettings(settingsFile);

            var didCreateGraphicsAdapter = ConstructGraphicsAdapter(ledSettings);
            if (didCreateGraphicsAdapter && graphicsAdapter.GetAdapterIndexCount() > 0)
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

        #region Initialization

        private static bool ConstructGraphicsAdapter(LedSettings settings)
        {
            // Try NDA
            var ndaAdapter = new NDAAdapter();
            long ndaCount = 0;
            
            if (ndaAdapter.Initialize())
            {
                ndaCount = ndaAdapter.GetGpuCounts();

                if (ndaCount > 0 && ndaAdapter.InitializeAdapters(ndaCount, settings))
                {
                    graphicsAdapter = ndaAdapter;
                    return true;
                }
            }

            // Try ADL
            var adlAdapter = new ADLAdapter();
            if (ndaCount == 0 && adlAdapter.Initialize())
            {
                long adlCount = adlAdapter.GetGpuCounts();
                if (adlCount > 0 && adlAdapter.InitializeAdapters(adlCount, settings))
                {
                    graphicsAdapter = adlAdapter;
                    return true;
                }
            }

            // Not found
            return false;
        }

        private static LedSettings InitializeFromSettings(string settingsFile)
        {
            LedSettings settings;

            // File
            if (File.Exists(settingsFile))
            {
                using (var sr = new StreamReader(settingsFile))
                {
                    settings = JsonSerializer<LedSettings>.DeSerialize(sr.ReadToEnd()) ?? new LedSettings();
                }
            }
            else // Default
            {
                settings = new LedSettings();

                using (var sw = new StreamWriter(settingsFile, false))
                {
                    sw.WriteLine(JsonSerializer<LedSettings>.Serialize(ledSettings));
                }
            }

            return settings;
        }

        #endregion

        private static void UpdateLedsFront()
        {
            while (true)
            {
                switch (ledSettings.AnimationType)
                {
                    case AnimationType.Off:
                        UpdateLeds(24, 4, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
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
                    case AnimationType.TemperatureBased:
                        mutex.WaitOne();
                        if (graphicsAdapter.GetGraphicsInformation(0, out GenericGraphicsInfo info))
                        {
                            int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                ledSettings.TemperatureUpperLimit, info.GpuCurrentTemperature);
                            ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                            UpdateLeds(21, 4, 4);
                        }
                        mutex.ReleaseMutex();
                        break;
                    case AnimationType.BreathingRgbCycle:
                        Thread.Sleep(ColorCycleDelay);
                        UpdateLeds(27, 4, 7);
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
                    case AnimationType.Off:
                        UpdateLeds(24, 1, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
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
                    case AnimationType.TemperatureBased:
                        mutex.WaitOne();
                        if (graphicsAdapter.GetGraphicsInformation(0, out GenericGraphicsInfo info))
                        {
                            int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                ledSettings.TemperatureUpperLimit, info.GpuCurrentTemperature);
                            ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                            UpdateLeds(21, 1, 4);
                        }
                        mutex.ReleaseMutex();
                        break;
                    case AnimationType.BreathingRgbCycle:
                        Thread.Sleep(ColorCycleDelay);
                        CycleNextColor(); // Cycles colors for everything
                        UpdateLeds(27, 1, 7);
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
                    case AnimationType.Off:
                        UpdateLeds(24, 2, 4);
                        Thread.Sleep(NoAnimationDelay);
                        break;
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
                    case AnimationType.TemperatureBased:
                        mutex.WaitOne();
                        if (graphicsAdapter.GetGraphicsInformation(0, out GenericGraphicsInfo info))
                        {
                            int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                                ledSettings.TemperatureUpperLimit, info.GpuCurrentTemperature);
                            ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                            UpdateLeds(21, 2, 4);
                        }
                        mutex.ReleaseMutex();
                        break;
                    case AnimationType.BreathingRgbCycle:
                        Thread.Sleep(ColorCycleDelay);
                        UpdateLeds(27, 2, 7);
                        break;
                }
            }
        }

        private static void UpdateLeds(int cmd, int ledId, int time, int ontime = 0, int offtime = 0, int darkTime = 0)
        {
            for (int i = 0; i < graphicsAdapter.GetAdapterIndexCount(); i++)
            {
                Thread.CurrentThread.Join(10);
                for (int index = 0; vgaMutex && index < 100; ++index)
                {
                    Thread.CurrentThread.Join(5);
                }
                vgaMutex = true;
                Thread.CurrentThread.Join(20);

                bool oneCall = ledSettings.AnimationType != AnimationType.NoAnimation;

                graphicsAdapter.SetIlluminationRGBColor(ledSettings, i, cmd, ledId, time, ontime, offtime, darkTime, oneCall);

                vgaMutex = false;
            }

            Thread.CurrentThread.Join(2000);
        }

        private static void CycleNextColor()
        {
            var nextColor = colorQueue.Dequeue();
            ledSettings.Color = nextColor;
            colorQueue.Enqueue(nextColor);
        }
    }
}