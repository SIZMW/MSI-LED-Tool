using System;

namespace MSI_LED_Tool
{
    public class TemperatureBasedAnimation : AnimationBase
    {
        public TemperatureBasedAnimation(Action<LedSettings, int, int, int, int, int, int, bool> updateLedsAction) : base(updateLedsAction)
        {
            cmd = 21;
            time = 4;
        }

        public override void AnimateBack(IAdapter adapter, LedSettings ledSettings)
        {
            mutex.WaitOne();
            if (adapter.GetGraphicsInformation(0, out GenericGraphicsInfo info))
            {
                int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                    ledSettings.TemperatureUpperLimit, info.GpuCurrentTemperature);
                ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                base.AnimateBack(adapter, ledSettings);
            }
            mutex.ReleaseMutex();
        }

        public override void AnimateFront(IAdapter adapter, LedSettings ledSettings)
        {
            mutex.WaitOne();
            if (adapter.GetGraphicsInformation(0, out GenericGraphicsInfo info))
            {
                int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                    ledSettings.TemperatureUpperLimit, info.GpuCurrentTemperature);
                ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                base.AnimateFront(adapter, ledSettings);
            }
            mutex.ReleaseMutex();
        }

        public override void AnimateSide(IAdapter adapter, LedSettings ledSettings)
        {
            mutex.WaitOne();
            if (adapter.GetGraphicsInformation(0, out GenericGraphicsInfo info))
            {
                int temperatureDelta = TemperatureColorUtil.CalculateTemperatureDeltaHunderdBased(ledSettings.TemperatureLowerLimit,
                    ledSettings.TemperatureUpperLimit, info.GpuCurrentTemperature);
                ledSettings.Color = TemperatureColorUtil.GetColorForDeltaTemperature(temperatureDelta);
                base.AnimateSide(adapter, ledSettings);
            }
            mutex.ReleaseMutex();
        }
    }
}
