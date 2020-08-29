using FTNPower.Image.Api.Service;

public class ImageProvider
{
    private static ImageService m_imageService;

    public static ImageService ISVC => m_imageService;

    internal static void Configure(ImageService imageService)
    {
        m_imageService = imageService;
    }
}