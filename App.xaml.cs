
using CollectionsManager.Services;
using System.Diagnostics;

namespace CollectionsManager
{
	public partial class App : Application
	{
		private readonly ICollectionsService _collectionsService;

		public App(ICollectionsService collectionsService)
		{
			_collectionsService = collectionsService;

			InitializeComponent();

			MainPage = new AppShell();
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			Window window = base.CreateWindow(activationState);

			window.Created += (s, e) =>
			{
				Debug.WriteLine($"App data path: '{AppConsts.AppDataPath}'");
			};

			return window;
		}
	}
}
