using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSI_LED_Tool
{
    public class TemperatureColorUtil
    {
        public static int CalculateTemperatureDeltaHunderdBased(int lowerLimit, int upperLimit, int current)
        {
            try
            {
                var difference = upperLimit - lowerLimit;
                var adjustedCurrent = current - lowerLimit;

                if (difference <= 0)
                {
                    return 50;
                }

                if (adjustedCurrent <= 0)
                {
                    return 0;
                }

                if (adjustedCurrent > upperLimit)
                {
                    adjustedCurrent = upperLimit;
                }

                var delta = Convert.ToInt32(1.0f * adjustedCurrent / difference * 100);

                if (delta > 100)
                {
                    delta = 100;
                }

                return delta;
            }
            catch
            {
                return 50;
            }
        }

        public static Color GetColorForDeltaTemperature(int x)
        {
            return Color.FromArgb(GetRedForDeltaTemperature(x), GetGreenForDeltaTemperature(x), 0);
        }

        private static int GetRedForDeltaTemperature(int x)
        {
            var percent = x / 100.0f;
            var value = Convert.ToInt32(percent * 255 * 2);

            if (value > 255)
            {
                return 255;
            }

            return value;
        }

        private static int GetGreenForDeltaTemperature(int x)
        {
            var percent = x / 100.0f;
            var value = Convert.ToInt32((255 - percent * 255) * 2);

            if (value > 255)
            {
                return 255;
            }

            return value;
        }
    }
}
