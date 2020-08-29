namespace Fortnite.Model.Responses.WorldInfo
{
    public class Brush
    {
        public ImageSize imageSize { get; set; }
        public Margin margin { get; set; }
        public TintColor tintColor { get; set; }
        public string resourceObject { get; set; }
        public string resourceName { get; set; }
        public UVRegion uVRegion { get; set; }
        public string drawAs { get; set; }
        public string tiling { get; set; }
        public string mirroring { get; set; }
        public string imageType { get; set; }
        public bool bIsDynamicallyLoaded { get; set; }
    }
}