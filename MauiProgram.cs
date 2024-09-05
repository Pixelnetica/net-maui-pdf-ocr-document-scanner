using CommunityToolkit.Maui;
using ImageSdkWrapper.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Handlers.Compatibility;

namespace MauiDemoApplication
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UsePixelnetica()
                
                ////.UseMauiCompatibility()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}