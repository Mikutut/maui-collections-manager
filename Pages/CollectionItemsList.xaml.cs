using CollectionsManager.Models;
using CollectionsManager.Services;
using System.Collections.ObjectModel;

namespace CollectionsManager.Pages;

[QueryProperty(nameof(Collection), nameof(Collection))]
public partial class CollectionItemsList : ContentPage, IQueryAttributable
{
	private readonly ICollectionsService _collectionsService;

	private Collection collection;
	private ObservableCollection<CollectionItem> itemsList = new ObservableCollection<CollectionItem>();

	public Collection Collection
	{
		get => collection;
		set
		{
			collection = value;
			OnPropertyChanged("Collection");
		}
	}

	public ObservableCollection<CollectionItem> ItemsList
	{
		get => itemsList;
		set
		{
			itemsList = value;
			OnPropertyChanged("ItemsList");
		}
	}

	public CollectionItemsList(ICollectionsService collectionsService)
	{
		_collectionsService = collectionsService;
		InitializeComponent();

		BindingContext = this;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if(Collection != null)
		{
			ReloadItemsList();
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		ItemsList.Clear();
	}

	private async void collectionItemsList_newCollectionButton_Clicked(object sender, EventArgs e)
	{
		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection }
		};

		await Shell.Current.GoToAsync("//newitem", args);
	}

	private async void collectionItemsList_goBackButton_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//home");
	}

	private void CollectionItemEntryView_OnDelete(CollectionItem obj)
	{
		_collectionsService.DeleteCollectionItem(Collection.CollectionRefId, obj.CollectionItemRefId);
		_collectionsService.SaveCollectionsToFile(null);
		ReloadItemsList();
	}

	private async void CollectionItemEntryView_OnUpdate(CollectionItem obj)
	{
		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection },
			{ "CollectionItem", obj }
		};

		await Shell.Current.GoToAsync("//updateitem", args);
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		Collection = query["Collection"] as Collection;

		if(Collection != null)
		{
			ReloadItemsList();
		}
	}

	private void ReloadItemsList()
	{
		ItemsList.Clear();

		_collectionsService.GetCollectionItems(Collection)
			.ForEach(c => ItemsList.Add(c));
	}

	private async void collectionItemsList_summaryButton_Clicked(object sender, EventArgs e)
	{
		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection }
		};

		await Shell.Current.GoToAsync("//summary", args);
	}

	private void collectionItemsList_exportButton_Clicked(object sender, EventArgs e)
	{

	}
}