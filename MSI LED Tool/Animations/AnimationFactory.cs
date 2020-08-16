using System;

namespace MSI_LED_Tool
{
    public class AnimationFactory
    {
        public AnimationFactory()
        {
        }

        public IAnimation BuildAnimator(AnimationType type, Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction)
        {
            switch (type)
            {
                case AnimationType.Off:
                    return new OffAnimation(updateLedsAction);
                case AnimationType.NoAnimation:
                    return new NoAnimation(updateLedsAction);
                case AnimationType.Breathing:
                    return new BreathingAnimation(updateLedsAction);
                case AnimationType.Flashing:
                    return new FlashingAnimation(updateLedsAction);
                case AnimationType.DoubleFlashing:
                    return new DoubleFlashingAnimation(updateLedsAction);
                case AnimationType.TemperatureBased:
                    return new TemperatureBasedAnimation(updateLedsAction);
                case AnimationType.BreathingRgbCycle:
                    return new BreathingRGBCycleAnimation(updateLedsAction);
                default:
                    return null;
            }
        }
    }
}
