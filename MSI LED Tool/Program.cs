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

        private static LedSettings ledSettings;
        private static IAdapter graphicsAdapter;
        private static IAnimation animation;

        public static void Main(string[] args)
        {
            // Load user settings
            string settingsFile = $"{AppDomain.CurrentDomain.BaseDirectory}\\{SettingsFileName}";
            ledSettings = InitializeFromSettings(settingsFile);

            // Build graphics card adapter based on cards detected
            var didCreateGraphicsAdapter = ConstructGraphicsAdapter(ledSettings);
            if (didCreateGraphicsAdapter && graphicsAdapter.GetAdapterIndexCount() > 0)
            {
                // Build animation type and initialize animation logic
                AnimationFactory factory = new AnimationFactory();
                animation = factory.BuildAnimator(ledSettings.AnimationType, UpdateLeds);

                // Start threads per LED set
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

            // Try ADL if NDA didn't work
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

            // Nothing found
            return false;
        }

        private static LedSettings InitializeFromSettings(string settingsFile)
        {
            LedSettings settings;

            // From file
            if (File.Exists(settingsFile))
            {
                using (var sr = new StreamReader(settingsFile))
                {
                    settings = JsonSerializer<LedSettings>.DeSerialize(sr.ReadToEnd()) ?? new LedSettings();
                }
            }
            else // Save and use default values
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

        #region Update LED functions

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

        /// <summary>
        /// Generic LED update function that also manages threading.
        /// </summary>
        /// <param name="ledSettings">The input settings for colors.</param>
        /// <param name="cmd">The lighting command.</param>
        /// <param name="ledId">The ID of the LED to be set.</param>
        /// <param name="time">The time of the illumination command.</param>
        /// <param name="ontime">The time for the LED to remain on.</param>
        /// <param name="offtime">The time for the LED to remain off.</param>
        /// <param name="darkTime">The time for the LED to remain darkened.</param>
        /// <param name="callOnce">If the LED command should be called only once.</param>
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

        #endregion
    }
}