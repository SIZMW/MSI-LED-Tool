using System.Drawing;
using System.Runtime.Serialization;

namespace MSI_LED_Tool
{
    [DataContract]
    public class LedSettings
    {
        private const int MinimumTemperature = 0;
        private const int MaximumTemperature = 100;

        private int temperatureLowerLimit;
        private int temperatureUpperLimit;

        /// <summary>
        /// Default constructor with default values.
        /// </summary>
        public LedSettings()
        {
            R = 255;
            G = 0;
            B = 0;
            AnimationType = AnimationType.NoAnimation;
            TemperatureUpperLimit = 85;
            TemperatureLowerLimit = 45;
            OverwriteSecurityChecks = false;
        }

        [DataMember]
        public AnimationType AnimationType { get; set; }

        [DataMember]
        public int R { get; set; }

        [DataMember]
        public int G { get; set; }

        [DataMember]
        public int B { get; set; }

        [DataMember]
        public int TemperatureUpperLimit
        {
            get
            {
                return temperatureUpperLimit;
            }

            set
            {
                temperatureUpperLimit = AdjustTemperatureWithinBounds(value);
                AdjustTemperatureRange();
            }
        }

        [DataMember]
        public int TemperatureLowerLimit {
            get
            {
                return temperatureLowerLimit;
            }

            set
            {
                temperatureLowerLimit = AdjustTemperatureWithinBounds(value);
                AdjustTemperatureRange();
            }
        }

        [DataMember]
        public bool OverwriteSecurityChecks { get; set; }

        /// <summary>
        /// Accessor to modify RGB values through <see cref="System.Drawing.Color"/> object.
        /// </summary>
        public Color Color
        {
            get
            {
                return Color.FromArgb(255, R, G, B);
            }

            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        /// <summary>
        /// Cap the temperature value within acceptable bounds.
        /// </summary>
        /// <param name="input">The value to adjust</param>
        /// <returns>An adjusted value.</returns>
        private int AdjustTemperatureWithinBounds(int input)
        {
            if (input > MaximumTemperature)
            {
                input = MaximumTemperature;
            }

            if (input < MinimumTemperature)
            {
                input = MinimumTemperature;
            }

            return input;
        }

        /// <summary>
        /// Adjust lower and upper limit with respect to each other and bounds.
        /// </summary>
        private void AdjustTemperatureRange()
        {
            if (temperatureUpperLimit <= temperatureLowerLimit)
            {
                if (temperatureLowerLimit >= MaximumTemperature)
                {
                    temperatureLowerLimit = MaximumTemperature - 1;
                }

                temperatureUpperLimit = temperatureLowerLimit + 1;
            }
        }
    }
}
