using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager.Models
{
	public class Collection : INotifyPropertyChanged
	{
		private Guid collectionRefId = Guid.NewGuid();
		private string name = string.Empty;
		private ObservableCollection<CollectionItem> items = new ObservableCollection<CollectionItem>();
		private ObservableCollection<CollectionItemStatus> itemStatuses = new ObservableCollection<CollectionItemStatus>(DEFAULT_ITEM_STATUSES);

		public Guid CollectionRefId
		{
			get => collectionRefId;
			set
			{
				collectionRefId = value;
				OnPropertyChanged("CollectionRefId");
			}
		}

		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}

		public ObservableCollection<CollectionItem> Items
		{
			get => items;
		}

		public ObservableCollection<CollectionItemStatus> ItemStatuses
		{
			get => itemStatuses;
		}

		public static List<CollectionItemStatus> DEFAULT_ITEM_STATUSES = new List<CollectionItemStatus>
		{
			new CollectionItemStatus()
			{
				Name = "Nowy"
			},
			new CollectionItemStatus()
			{
				Name = "Używany"
			}
		};

		public event PropertyChangedEventHandler? PropertyChanged;

		public static string GetCollectionPath(Collection collection, string? basePath)
		{
			string _basePath = (!string.IsNullOrWhiteSpace(basePath))
				? basePath
				: AppConsts.CollectionsPath;

			return Path.Combine(_basePath, collection.CollectionRefId.ToString());
		}

		private void OnPropertyChanged(string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
