namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements.Shading
{
    internal enum ShadingType
    {
        /// <summary>
        /// Axial shadings (type 2) define a colour blend along a line between two points
        /// </summary>
        AxialType2 = 2,

        /// <summary>
        /// Radial shadings (type 3) define a blend between two circles
        /// </summary>
        RadialType3 = 3
    }
}