using CollectionsManager.Models;

namespace CollectionsManager.Views;

public partial class CollectionEntryView : ContentView
{
	public static readonly BindableProperty CollectionProperty = BindableProperty.Create(
		"Collection",
		typeof(Collection),
		typeof(CollectionEntryView),
		null);

	public Collection Collection
	{
		get => (Collection)GetValue(CollectionProperty);
		set
		{
			SetValue(CollectionProperty, value);
			OnPropertyChanged("Collection");
		}
	}

	public event Action<Collection>? OnClick;
	public event Action<Collection>? OnDelete;

	public CollectionEntryView()
	{
		InitializeComponent();
		BindingContext = Collection;
	}

	private void collectionEntryView_button_Clicked(object sender, EventArgs e)
	{
		OnClick?.Invoke(Collection);
    }

	private void collectionEntryView_deleteButton_Clicked(object sender, EventArgs e)
	{
		OnDelete?.Invoke(Collection);
	}
}