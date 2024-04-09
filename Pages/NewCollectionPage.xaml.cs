using CollectionsManager.Models;
using CollectionsManager.Services;

namespace CollectionsManager.Pages
{
    public partial class NewCollectionPage : ContentPage
    {
        private readonly ICollectionsService _collectionsService;
    
        private CreateCollection CreateCollection = new CreateCollection();
    
        public NewCollectionPage(ICollectionsService collectionsService)
        {
            _collectionsService = collectionsService;

            InitializeComponent();
        }
    
    	protected override void OnAppearing()
    	{
    		base.OnAppearing();
            CreateCollection = new CreateCollection();
            BindingContext = CreateCollection;
    	}
    
    	private async void newCollectionPage_cancelButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }
    
        private async void newCollectionPage_submitButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                _collectionsService.CreateCollection(CreateCollection);
                _collectionsService.SaveCollectionsToFile(null);
                await Shell.Current.GoToAsync("//home");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Błąd dodawania kolekcji", $"Wystąpił błąd podczas dodawania kolekcji. Sprawdź czy wprowadzone dane są poprawne i spróbuj ponownie.\n\n{ex.Message}", "OK");
            }
        }
    }
}
