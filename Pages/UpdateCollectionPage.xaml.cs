using CollectionsManager.Models;
using CollectionsManager.Services;

namespace CollectionsManager.Pages
{
    [QueryProperty(nameof(Collection), nameof(Collection))]
    public partial class UpdateCollectionPage : ContentPage, IQueryAttributable
    {
        private readonly ICollectionsService _collectionsService;

        private UpdateCollection updateCollection = new UpdateCollection();
        private Collection collection;
    
        public UpdateCollection UpdateCollection
        {
            get => updateCollection;
            set
            {
                updateCollection = value;
                OnPropertyChanged("UpdateCollection");
            }
        }

        public Collection Collection
        {
            get => collection;
            set
            {
                collection = value;
                OnPropertyChanged("Collection");
            }
        }

        public UpdateCollectionPage(ICollectionsService collectionsService)
        {
            _collectionsService = collectionsService;

            InitializeComponent();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Collection = query["Collection"] as Collection;

            if(Collection != null)
            {
                UpdateCollection = new UpdateCollection();

                UpdateCollection.CollectionRefId = Collection.CollectionRefId;
                UpdateCollection.Name = Collection.Name;

                BindingContext = UpdateCollection;
            }
        }

    	protected override void OnDisappearing()
    	{
    		base.OnDisappearing();
            UpdateCollection = new UpdateCollection();
    	}
    
    	private async void updateCollectionPage_cancelButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }
    
        private async void updateCollectionPage_submitButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                _collectionsService.UpdateCollection(UpdateCollection);
                _collectionsService.SaveCollectionsToFile(null);
                await Shell.Current.GoToAsync("//home");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Błąd edycji kolekcji", $"Wystąpił błąd podczas edytowania kolekcji. Sprawdź czy wprowadzone dane są poprawne i spróbuj ponownie.\n\n{ex.Message}", "OK");
            }
        }
    }
}
