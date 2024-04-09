using CollectionsManager.Models;
using CollectionsManager.Services;

namespace CollectionsManager.Pages;

[QueryProperty(nameof(Collection), nameof(Collection))]
[QueryProperty(nameof(CollectionItem), nameof(CollectionItem))]
public partial class UpdateCollectionItemPage : ContentPage, IQueryAttributable
{
	private readonly ICollectionsService _collectionsService;

	private Collection collection;
	private CollectionItem collectionItem;
	private UpdateCollectionItem updateCollectionItem;

	public Collection Collection
	{
		get => collection;
		set
		{
			collection = value;
			OnPropertyChanged("Collection");
		}
	}

	public CollectionItem CollectionItem
	{
		get => collectionItem;
		set
		{
			collectionItem = value;
			OnPropertyChanged("CollectionItem");
		}
	}

	public UpdateCollectionItem UpdateCollectionItem
	{
		get => updateCollectionItem;
		set
		{
			updateCollectionItem = value;
			OnPropertyChanged("UpdateCollectionItem");
		}
	}

	public UpdateCollectionItemPage(ICollectionsService collectionsService)
	{
		_collectionsService = collectionsService;

		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		UpdateCollectionItem = new UpdateCollectionItem();
	}

	public void ApplyQueryAttributes(IDictionary<string, object> args)
	{
		Collection = args["Collection"] as Collection;
		CollectionItem = args["CollectionItem"] as CollectionItem;

		if(Collection != null && CollectionItem != null)
		{
			UpdateCollectionItem = new UpdateCollectionItem();

			UpdateCollectionItem.CollectionRefId = Collection.CollectionRefId;
			UpdateCollectionItem.CollectionItemRefId = CollectionItem.CollectionItemRefId;
			UpdateCollectionItem.Name = CollectionItem.Name;
			UpdateCollectionItem.Quantity = CollectionItem.Quantity;
			UpdateCollectionItem.Rating = CollectionItem.Rating;
			UpdateCollectionItem.Comment = CollectionItem.Comment;
			UpdateCollectionItem.IsForSale = CollectionItem.IsForSale;
			UpdateCollectionItem.IsSold = CollectionItem.IsSold;

			UpdateCollectionItem.Statuses.Clear();
			CollectionItem.Statuses.ToList().ForEach(s => UpdateCollectionItem.Statuses.Add(s));

			this.updateCollectionItemPage_statusPicker.ItemsSource = Collection.ItemStatuses;
			BindingContext = UpdateCollectionItem;
		}
	}

	private void updateCollectionItemPage_statusPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		Picker picker = (Picker)sender;
		CollectionItemStatus selectedStatus = (CollectionItemStatus) picker.SelectedItem;

		UpdateCollectionItem.Statuses.Clear();
		UpdateCollectionItem.Statuses.Add(selectedStatus);
	}

	private async void updateCollectionItemPage_cancelButton_Clicked(object sender, EventArgs e)
	{
		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection }
		};

		await Shell.Current.GoToAsync("//itemslist", args);
	}

	private async void updateCollectionItemPage_submitButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			bool nameTaken = _collectionsService.CheckIfCollectionItemExists(Collection, UpdateCollectionItem.Name);

			if(nameTaken)
			{
				var result = await DisplayAlert(
					"Konfliktujące nazwy",
					$"Nazwa '{UpdateCollectionItem.Name}' jest już zajęta. Czy aby na pewno chcesz utworzyć przedmiot o takiej nazwie?",
					"Tak", "Nie");

				if(!result)
				{
					return;
				}
			}

			_collectionsService.UpdateCollectionItem(UpdateCollectionItem);
			_collectionsService.SaveCollectionsToFile(null);
		}
		catch(Exception ex)
		{
			await DisplayAlert(
				"Dodawanie przedmiotu",
				$"Wystąpił błąd podczas dodawania przedmiotu.\n\n{ex.Message}",
				"OK");
		}

		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection }
		};

		await Shell.Current.GoToAsync("//itemslist", args);
	}

	private async void updateCollectionItemPage_imageChooseButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			PickOptions options = new()
			{
				PickerTitle = "Wybierz plik graficzny dla przedmiotu",
				FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<String>>
				{
					{ DevicePlatform.WinUI, new[] { ".jpg", ".png" } }
				})
			};

			var result = await FilePicker.Default.PickAsync(options);
			if (result != null)
			{
				if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
					result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
				{
					using var stream = await result.OpenReadAsync();
					using var ms = new MemoryStream();

					stream.CopyTo(ms);
					UpdateCollectionItem.Image = ms.ToArray();
				}
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert(
				"Wybieranie pliku graficznego",
				"Wystąpił błąd, bądź operacja została anulowana.",
				"OK");

			UpdateCollectionItem.Image = null;
		}
	}

	private void updateCollectionItemPage_imageClearButton_Clicked(object sender, EventArgs e)
	{
		UpdateCollectionItem.Image = null;
	}
}