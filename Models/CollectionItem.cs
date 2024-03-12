using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager.Models
{
	public class CollectionItem : INotifyPropertyChanged
	{
		private Guid collectionItemRefId = Guid.NewGuid();
		private Guid collectionRefId = Guid.Empty;
		private string name = string.Empty;
		private byte[]? image;
		private int quantity = 0;
		private uint rating = 0;
		private string? comment = null;

		public Guid CollectionItemRefId
		{
			get => collectionItemRefId;
			set
			{
				collectionItemRefId = value;
				OnPropertyChanged("CollectionItemRefId");
			}
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

		public Stream? GetImageStream()
		{
			if(image != null)
			{
				return new MemoryStream(image);
			}
			else
			{
				return null;
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged(string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
