using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager.Models
{
	public class UpdateCollectionItem : INotifyPropertyChanged
	{
		private Guid collectionItemRefId;
		private Guid collectionRefId;
		private Guid? newCollectionRefId;
		private ObservableCollection<CollectionItemStatus> statuses = new ObservableCollection<CollectionItemStatus>();
		private string name = string.Empty;
		private byte[]? image;
		private int quantity = 0;
		private uint rating = 0;
		private string? comment = null;
		private bool isForSale = false;

		public Guid CollectionItemRefId
		{
			get => collectionItemRefId;
			set
			{
				collectionItemRefId = value;
				OnPropertyChanged("CollectionItemRefId");
			}
		}

		public ObservableCollection<CollectionItemStatus> Statuses
		{
			get => statuses;
		}

		public Guid CollectionRefId
		{
			get => collectionRefId;
			set
			{
				collectionRefId = value;
				OnPropertyChanged("CollectionRefId");
			}
		}

		public Guid? NewCollectionRefId
		{
			get => newCollectionRefId;
			set
			{
				newCollectionRefId = value;
				OnPropertyChanged("NewCollectionRefId");
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

		public byte[]? Image
		{
			get => image;
			set
			{
				image = value;
				OnPropertyChanged("Image");
			}
		}

		public int Quantity
		{
			get => quantity;
			set
			{
				quantity = value;
				OnPropertyChanged("Quantity");
			}
		}

		public uint Rating
		{
			get => rating;
			set
			{
				rating = value;
				OnPropertyChanged("Rating");
			}
		}

		public string? Comment
		{
			get => comment;
			set
			{
				comment = value;
				OnPropertyChanged("Comment");
			}
		}

		public bool IsForSale
		{
			get => isForSale;
			set
			{
				isForSale = value;
				OnPropertyChanged("IsForSale");
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		void OnPropertyChanged(string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
