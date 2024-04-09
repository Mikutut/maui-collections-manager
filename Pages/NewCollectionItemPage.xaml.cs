using CollectionsManager.Models;
using CollectionsManager.Services;

namespace CollectionsManager.Pages;

[QueryProperty(nameof(Collection), nameof(Collection))]
public partial class NewCollectionItemPage : ContentPage, IQueryAttributable
{
	private readonly ICollectionsService _collectionsService;

	private Collection collection;
	private CreateCollectionItem createCollectionItem;

	public Collection Collection
	{
		get => collection;
		set
		{
			collection = value;
			OnPropertyChanged("Collection");
		}
	}

	public CreateCollectionItem CreateCollectionItem
	{
		get => createCollectionItem;
		set
		{
			createCollectionItem = value;
			OnPropertyChanged("CreateCollectionItem");
		}
	}

	public NewCollectionItemPage(ICollectionsService collectionsService)
	{
		_collectionsService = collectionsService;

		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if(Collection != null)
		{
			CreateCollectionItem = new CreateCollectionItem();
			CreateCollectionItem.CollectionRefId = Collection.CollectionRefId;
			this.newCollectionItemPage_statusPicker.ItemsSource = Collection.ItemStatuses;
			BindingContext = CreateCollectionItem;
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		CreateCollectionItem = new CreateCollectionItem();
	}

	public void ApplyQueryAttributes(IDictionary<string, object> args)
	{
		Collection = args["Collection"] as Collection;

		if(Collection != null)
		{
			CreateCollectionItem = new CreateCollectionItem();
			CreateCollectionItem.CollectionRefId = Collection.CollectionRefId;
			this.newCollectionItemPage_statusPicker.ItemsSource = Collection.ItemStatuses;
			BindingContext = CreateCollectionItem;
		}
	}

	private void newCollectionItemPage_statusPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		Picker picker = (Picker)sender;
		CollectionItemStatus selectedStatus = (CollectionItemStatus) picker.SelectedItem;

		CreateCollectionItem.Statuses.Clear();
		CreateCollectionItem.Statuses.Add(selectedStatus);
	}

	private async void newCollectionItemPage_cancelButton_Clicked(object sender, EventArgs e)
	{
		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection }
		};

		await Shell.Current.GoToAsync("//itemslist", args);
	}

	private async void newCollectionItemPage_submitButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			bool nameTaken = _collectionsService.GetCollisions(Collection, CreateCollectionItem.Name) >= 1;

			if(nameTaken)
			{
				var result = await DisplayAlert(
					"Konfliktujące nazwy",
					$"Nazwa '{CreateCollectionItem.Name}' jest już zajęta. Czy aby na pewno chcesz utworzyć przedmiot o takiej nazwie?",
					"Tak", "Nie");

				if(!result)
				{
					return;
				}
			}

			_collectionsService.CreateCollectionItem(CreateCollectionItem);
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

	private async void newCollectionItemPage_imageChooseButton_Clicked(object sender, EventArgs e)
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
					List<byte> imageData = new List<byte>();
					using (var stream = await result.OpenReadAsync())
					{
						var data = stream.ReadByte();

						while(data != -1)
						{
							imageData.Add((byte)data);
							data = stream.ReadByte();
						}
					}

					CreateCollectionItem.Image = imageData.ToArray();
					await DisplayAlert(
						"Wybieranie pliku graficznego",
						"Pomyślnie zaimportowano plik graficzny",
						"OK");
				}
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert(
				"Wybieranie pliku graficznego",
				"Wystąpił błąd, bądź operacja została anulowana.",
				"OK");

			CreateCollectionItem.Image = null;
		}
	}

	private async void newCollectionItemPage_imageClearButton_Clicked(object sender, EventArgs e)
	{
		CreateCollectionItem.Image = null;
		await DisplayAlert(
			"Usuwanie grafiki",
			"Pomyślnie usunięto grafikę z przedmiotu",
			"OK");
	}
}