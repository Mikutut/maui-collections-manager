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

		public event PropertyChangedEventHandler? PropertyChanged;

		public static string GetCollectionPath(Collection collection)
		{
			return Path.Combine(AppConsts.CollectionsPath, collection.CollectionRefId.ToString());
		}

		public static string GetCollectionImagesPath(Collection collection)
		{
			return Path.Combine(GetCollectionPath(collection), "Images");
		}

		private void OnPropertyChanged(string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
