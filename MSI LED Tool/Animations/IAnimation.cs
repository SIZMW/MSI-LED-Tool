namespace MSI_LED_Tool
{
    /// <summary>
    /// Animation interface to define how colors and lights will react for specific animations.
    /// </summary>
    public interface IAnimation
    {
        /// <summary>
        /// Animates the card's back lights.
        /// </summary>
        /// <param name="adapter">Adapter interface to interact with NDA and ADL drivers/DLLs.</param>
        /// <param name="ledSettings">The input settings for colors.</param>
        void AnimateBack(IAdapter adapter, LedSettings ledSettings);

        /// <summary>
        /// Animates the card's front lights.
        /// </summary>
        /// <param name="adapter">Adapter interface to interact with NDA and ADL drivers/DLLs.</param>
        /// <param name="ledSettings">The input settings for colors.</param>
        void AnimateFront(IAdapter adapter, LedSettings ledSettings);

        /// <summary>
        /// Animates the card's side lights.
        /// </summary>
        /// <param name="adapter">Adapter interface to interact with NDA and ADL drivers/DLLs.</param>
        /// <param name="ledSettings">The input settings for colors.</param>
        void AnimateSide(IAdapter adapter, LedSettings ledSettings);
    }
}
