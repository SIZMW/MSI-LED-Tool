using System;
using System.Threading;

namespace MSI_LED_Tool
{
    public class NoAnimation : AnimationBase
    {
        public NoAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            cmd = 21;
            time = 4;
            callOnce = false;
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
