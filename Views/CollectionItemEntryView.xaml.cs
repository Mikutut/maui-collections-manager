using CollectionsManager.Models;

namespace CollectionsManager.Views;

public partial class CollectionItemEntryView : ContentView
{
	public static readonly BindableProperty CollectionItemProperty = BindableProperty.Create(
		"CollectionItem",
		typeof(CollectionItem),
		typeof(CollectionItemEntryView),
		null);

	public CollectionItem CollectionItem
	{
		get => (CollectionItem)GetValue(CollectionItemProperty);
		set
		{
			SetValue(CollectionItemProperty, value);
			OnPropertyChanged("CollectionItem");
		}
	}

	public event Action<CollectionItem>? OnUpdate;
	public event Action<CollectionItem>? OnDelete;

	public CollectionItemEntryView()
	{
		InitializeComponent();
		BindingContext = CollectionItem;
	}

	private void collectionItemEntryView_updateButton_Clicked(object sender, EventArgs e)
	{
		OnUpdate?.Invoke(CollectionItem);
    }

	private void collectionItemEntryView_deleteButton_Clicked(object sender, EventArgs e)
	{
		OnDelete?.Invoke(CollectionItem);
    }
}