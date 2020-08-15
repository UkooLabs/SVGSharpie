namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    /// <summary>
    /// Describes how a child element is horizontally positioned or stretched within a parent elements layout
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// An element aligned to the left of the layout slot for the parent element.
        /// </summary>
        Left,
        /// <summary>
        /// An element aligned to the center of the layout slot for the parent element.
        /// </summary>
        Center,
        /// <summary>
        /// An element aligned to the right of the layout slot for the parent element.
        /// </summary>
        Right,
        /// <summary>
        /// An element stretched to fill the entire layout slot of the parent element.
        /// </summary>
        Stretch
    }
}