using CollectionsManager.Services;
using CollectionsManager.Models;
using System.Collections.ObjectModel;

namespace CollectionsManager.Pages
{
	public partial class MainPage : ContentPage
	{
		private readonly ICollectionsService _collectionsService;
		private ObservableCollection<Collection> collections = new ObservableCollection<Collection>();

		public ObservableCollection<Collection> Collections
		{
			get => collections;
			set
			{
				collections = value;
				OnPropertyChanged("Collections");
			}
		}

		public MainPage(ICollectionsService collectionsService)
		{
			_collectionsService = collectionsService;
			_collectionsService.LoadCollectionsFromFile(null);

			InitializeComponent();

			BindingContext = this;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			RefreshCollections();
		}

		private void RefreshCollections()
		{
			Collections.Clear();
			
			_collectionsService.GetCollections()
				.ForEach(c => Collections.Add(c));
			OnPropertyChanged("Collections");
		}

		private async void mainPage_newCollectionButton_Clicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//newcollection");
		}

		private async void CollectionEntryView_OnClick(Collection obj)
		{
			var args = new Dictionary<string, object>()
			{
				{ "Collection", obj }
			};
			await Shell.Current.GoToAsync("//itemslist", args);
		}

		private async void CollectionEntryView_OnDelete(Collection obj)
		{
			try
			{
				_collectionsService.DeleteCollection(obj.CollectionRefId);
				_collectionsService.SaveCollectionsToFile(null);
				RefreshCollections();
			}
			catch(Exception ex)
			{
				await DisplayAlert(
					"Błąd usuwania kolekcji",
					$"Nie udało się usunąć kolekcji.\n\n{ex.Message}",
					"OK");
			}
		}
	}
}
