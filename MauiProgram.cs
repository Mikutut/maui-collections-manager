using CollectionsManager.Pages;
using CollectionsManager.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CollectionsManager
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit()
				.EnableLogging()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FontAwesomeBrands");
					fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FontAwesome");
					fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FontAwesomeSolid");
				})
				.RegisterAppClass()
				.RegisterServices()
				.RegisterPages();

			var app = builder.Build();

			return app;
		}

		private static MauiAppBuilder RegisterAppClass(this MauiAppBuilder builder)
		{
			builder.Services
				.AddSingleton<App>();

			return builder;
		}

		private static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
		{
			builder.Services
				.AddSingleton<AppShell>()
				.AddSingleton<NewCollectionPage>()
				.AddSingleton<MainPage>();

			return builder;
		}

		private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
		{
			builder.Services
				.AddSingleton<ICollectionsService, CollectionsService>();

			return builder;
		}

		private static MauiAppBuilder EnableLogging(this MauiAppBuilder builder)
		{
			builder.Logging.AddConsole();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder;
		}
	}
}
