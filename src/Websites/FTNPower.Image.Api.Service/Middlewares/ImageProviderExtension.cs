using FTNPower.Image.Api.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

public static class ImageProviderExtension
{
    public static void AddImageProvider(this IServiceCollection services, Uri loadFromRemoteUrl = null)
    {
        services.AddSingleton<ImageService>(new ImageService().Init(loadFromRemoteUrl));
    }

    public static IApplicationBuilder UseImageProvider(this IApplicationBuilder app)
    {
        ImageProvider.Configure(app.ApplicationServices.GetRequiredService<ImageService>());
        return app;
    }
}