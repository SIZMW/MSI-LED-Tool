using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MSI_LED_Tool
{
    public class Program
    {
        private const string SettingsFileName = "Settings.json";

        private static Thread updateThreadFront;
        private static Thread updateThreadBack;
        private static Thread updateThreadSide;

        private static bool vgaMutex;
        private static IAnimation animation;
        private static LedSettings ledSettings;

        private static IAdapter graphicsAdapter;

        public static void Main(string[] args)
        {
            string settingsFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\{SettingsFileName}";
            ledSettings = InitializeFromSettings(settingsFile);

            var didCreateGraphicsAdapter = ConstructGraphicsAdapter(ledSettings);
            if (didCreateGraphicsAdapter && graphicsAdapter.GetAdapterIndexCount() > 0)
            {
                AnimationFactory factory = new AnimationFactory();
                animation = factory.BuildAnimator(ledSettings.AnimationType, UpdateLeds);

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
                animation.AnimateFront(graphicsAdapter, ledSettings);
            }
        }


        private static void UpdateLedsSide()
        {
            while (true)
            {
                animation.AnimateSide(graphicsAdapter, ledSettings);
            }
        }

        private static void UpdateLedsBack()
        {
            while (true)
            {
                animation.AnimateBack(graphicsAdapter, ledSettings);
            }
        }

        private static void UpdateLeds(LedSettings ledSettings, int cmd, int ledId, int time, int ontime = 0, int offtime = 0, int darkTime = 0, bool callOnce = true)
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

                graphicsAdapter.SetIlluminationRGBColor(ledSettings, i, cmd, ledId, time, ontime, offtime, darkTime, callOnce);

                vgaMutex = false;
            }

            Thread.CurrentThread.Join(2000);
        }
    }
}