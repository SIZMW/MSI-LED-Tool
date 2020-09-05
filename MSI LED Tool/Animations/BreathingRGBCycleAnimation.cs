using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace MSI_LED_Tool
{
    public class BreathingRGBCycleAnimation : BreathingAnimation
    {
        /// <summary>
        /// Predefined colors based on visual accuracy to cycle on loop.
        /// </summary>
        private readonly List<Color> ColorList = new List<Color>
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
        };

        private readonly Queue<Color> colorQueueBack = new Queue<Color>();
        private readonly Queue<Color> colorQueueFront = new Queue<Color>();
        private readonly Queue<Color> colorQueueSide = new Queue<Color>();

        public BreathingRGBCycleAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            ColorList.ForEach(c =>
            {
                colorQueueBack.Enqueue(c);
                colorQueueFront.Enqueue(c);
                colorQueueSide.Enqueue(c);
            });
        }

        public override void AnimateBack(IAdapter adapter, LedSettings ledSettings)
        {
            Thread.Sleep(ColorCycleDelay);
            ledSettings.Color = CycleNextColor(colorQueueBack);
            base.AnimateBack(adapter, ledSettings);
        }

        public override void AnimateFront(IAdapter adapter, LedSettings ledSettings)
        {
            Thread.Sleep(ColorCycleDelay);
            ledSettings.Color = CycleNextColor(colorQueueFront);
            base.AnimateFront(adapter, ledSettings);
        }

        public override void AnimateSide(IAdapter adapter, LedSettings ledSettings)
        {
            Thread.Sleep(ColorCycleDelay);
            ledSettings.Color = CycleNextColor(colorQueueSide);
            base.AnimateSide(adapter, ledSettings);
        }

        private Color CycleNextColor(Queue<Color> queue)
        {
            var nextColor = queue.Dequeue();
            queue.Enqueue(nextColor);
            return nextColor;
        }
    }
}
