using System;

namespace MSI_LED_Tool
{
    public class DoubleFlashingAnimation : AnimationBase
    {
        public DoubleFlashingAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            cmd = 30;
            time = 0;
            onTime = 10;
            offTime = 10;
            darkTime = 91;
        }
    }
}
