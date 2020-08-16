using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace MSI_LED_Tool
{
    public class BreathingRGBCycleAnimation : BreathingAnimation
    {
        private readonly Queue<Color> colorQueue = new Queue<Color>(new Color[]
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

        public BreathingRGBCycleAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
        }

        public override void AnimateBack(IAdapter adapter, LedSettings ledSettings)
        {
            Thread.Sleep(ColorCycleDelay);
            base.AnimateBack(adapter, ledSettings);
        }

        public override void AnimateFront(IAdapter adapter, LedSettings ledSettings)
        {
            Thread.Sleep(ColorCycleDelay);
            base.AnimateFront(adapter, ledSettings);
        }

        public override void AnimateSide(IAdapter adapter, LedSettings ledSettings)
        {
            Thread.Sleep(ColorCycleDelay);
            ledSettings.Color = CycleNextColor();
            base.AnimateSide(adapter, ledSettings);
        }

        private Color CycleNextColor()
        {
            var nextColor = colorQueue.Dequeue();
            colorQueue.Enqueue(nextColor);
            return nextColor;
        }
    }
}
