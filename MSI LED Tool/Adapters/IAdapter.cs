namespace MSI_LED_Tool
{
    /// <summary>
    /// Adapter interface to interact with NDA and ADL drivers/DLLs.
    /// </summary>
    public interface IAdapter
    {
        /// <summary>
        /// Returns the count of graphics indices detected.
        /// </summary>
        /// <returns>An integer</returns>
        int GetAdapterIndexCount();

        /// <summary>
        /// Attempts to initialize the graphics adapter.
        /// </summary>
        /// <returns>true if driver responds positively; false otherwise</returns>
        bool Initialize();

        /// <summary>
        /// Returns the number of detected graphics cards.
        /// </summary>
        /// <returns>An integer</returns>
        long GetGpuCounts();

        /// <summary>
        /// Reads the graphics card sensors to return live graphics card data.
        /// </summary>
        /// <param name="adapterIndex">Graphis index to read.</param>
        /// <param name="info">Output object to store graphics card data.</param>
        /// <returns>true if read was successful; false otherwise</returns>
        bool GetGraphicsInformation(int adapterIndex, out GenericGraphicsInfo info);

        /// <summary>
        /// Attempts to read and load the graphics card adapter(s).
        /// </summary>
        /// <param name="gpuCount">The number of graphics cards detected that should be loaded.</param>
        /// <param name="settings">The input settings for how to load the adapters.</param>
        /// <returns>true if loaded; false otherwise</returns>
        bool InitializeAdapters(long gpuCount, LedSettings settings);

        /// <summary>
        /// Sends the illumination command to the graphics adapter to set the colors on the card.
        /// </summary>
        /// <param name="settings">The input settings for colors.</param>
        /// <param name="adapterIndex">The graphics adapter to set colors on.</param>
        /// <param name="cmd">The lighting command.</param>
        /// <param name="ledId">The ID of the LED to be set.</param>
        /// <param name="time">The time of the illumination command.</param>
        /// <param name="timeOn">The time for the LED to remain on.</param>
        /// <param name="timeOff">The time for the LED to remain off.</param>
        /// <param name="timeDark">The time for the LED to remain darkened.</param>
        /// <param name="doOneCall">If the LED command should be called only once.</param>
        void SetIlluminationRGBColor(LedSettings settings, int adapterIndex, int cmd, int ledId, int time, int timeOn = 0, int timeOff = 0, int timeDark = 0, bool doOneCall = false);
    }
}
