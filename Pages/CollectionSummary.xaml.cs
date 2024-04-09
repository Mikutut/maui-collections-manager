using CollectionsManager.Models;

namespace CollectionsManager.Pages;

public partial class CollectionSummary : ContentPage, IQueryAttributable
{
	private Collection collection;
	private int totalCount = 0, 
		forSaleCount = 0, 
		soldCount = 0;

	public Collection Collection
	{
		get => collection;
		set
		{
			collection = value;
			OnPropertyChanged("Collection");
		}
	}

	public int TotalCount
	{
		get => totalCount;
		set
		{
			totalCount = value;
			OnPropertyChanged("TotalCount");
		}
	}

	public int ForSaleCount
	{
		get => forSaleCount;
		set
		{
			forSaleCount = value;
			OnPropertyChanged("ForSaleCount");
		}
	}

	public int SoldCount
	{
		get => soldCount;
		set
		{
			soldCount = value;
			OnPropertyChanged("SoldCount");
		}
	}

	public CollectionSummary()
	{
		InitializeComponent();

		BindingContext = this;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		Collection = query["Collection"] as Collection;

		if(Collection != null)
		{
			TotalCount = Collection.Items.Count;
			ForSaleCount = Collection.Items
				.Where(x => x.IsForSale && !x.IsSold)
				.Count();
			SoldCount = Collection.Items
				.Where(x => x.IsSold)
				.Count();
		}
	}

	private async void collectionSummary_goBackButton_Clicked(object sender, EventArgs e)
	{
		var args = new Dictionary<string, object>
		{
			{ "Collection", Collection }
		};

		await Shell.Current.GoToAsync("//itemslist", args);
    }
}