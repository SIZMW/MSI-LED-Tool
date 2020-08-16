using System;

namespace MSI_LED_Tool
{
    public class BreathingAnimation : AnimationBase
    {
        public BreathingAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            cmd = 27;
            time = 7;
        }
    }
}
