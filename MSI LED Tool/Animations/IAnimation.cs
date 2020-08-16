namespace MSI_LED_Tool
{
    public interface IAnimation
    {
        void AnimateBack(IAdapter adapter, LedSettings ledSettings);

        void AnimateFront(IAdapter adapter, LedSettings ledSettings);

        void AnimateSide(IAdapter adapter, LedSettings ledSettings);
    }
}
