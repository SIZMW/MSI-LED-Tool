using System;

namespace MSI_LED_Tool
{
    public class FlashingAnimation : AnimationBase
    {
        public FlashingAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            cmd = 28;
            time = 0;
            onTime = 25;
            offTime = 100;
        }
    }
}
