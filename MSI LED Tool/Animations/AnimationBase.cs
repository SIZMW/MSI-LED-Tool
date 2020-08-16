using System;
using System.Threading;

namespace MSI_LED_Tool
{
    public abstract class AnimationBase : IAnimation
    {
        protected const int NoAnimationDelay = 60000;
        protected const int ColorCycleDelay = 3000;

        protected static Mutex mutex;

        protected Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction;
        protected int cmd = 0;
        protected int time = 0;
        protected int onTime = 0;
        protected int offTime = 0;
        protected int darkTime = 0;
        protected bool callOnce = true;

        public AnimationBase(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction)
        {
            this.updateLedsAction = updateLedsAction;
            mutex = new Mutex();
        }

        public virtual void AnimateBack(IAdapter adapter, LedSettings ledSettings)
        {
            updateLedsAction(ledSettings, cmd, (int)LedId.Back, time, onTime, offTime, darkTime, callOnce);
        }

        public virtual void AnimateFront(IAdapter adapter, LedSettings ledSettings)
        {
            updateLedsAction(ledSettings, cmd, (int)LedId.Front, time, onTime, offTime, darkTime, callOnce);
        }

        public virtual void AnimateSide(IAdapter adapter, LedSettings ledSettings)
        {
            updateLedsAction(ledSettings, cmd, (int)LedId.Side, time, onTime, offTime, darkTime, callOnce);
        }
    }
}
