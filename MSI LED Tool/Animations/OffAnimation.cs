using System;
using System.Threading;

namespace MSI_LED_Tool
{
    public class OffAnimation : AnimationBase
    {
        public OffAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            cmd = 24;
            time = 4;
        }

        public override void AnimateBack(IAdapter adapter, LedSettings ledSettings)
        {
            base.AnimateBack(adapter, ledSettings);
            Thread.Sleep(NoAnimationDelay);
        }

        public override void AnimateFront(IAdapter adapter, LedSettings ledSettings)
        {
            base.AnimateFront(adapter, ledSettings);
            Thread.Sleep(NoAnimationDelay);
        }

        public override void AnimateSide(IAdapter adapter, LedSettings ledSettings)
        {
            base.AnimateSide(adapter, ledSettings);
            Thread.Sleep(NoAnimationDelay);
        }
    }
}
