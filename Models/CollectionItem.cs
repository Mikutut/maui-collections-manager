using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private ObservableCollection<CollectionItemStatus> statuses = new ObservableCollection<CollectionItemStatus>();
		private string name = string.Empty;
		private byte[]? image;
		private ImageSource? imageSource;
		private int quantity = 0;
		private uint rating = 0;
		private string? comment = null;
		private bool isForSale = false;
		private bool isSold = false;

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

		public ObservableCollection<CollectionItemStatus> Statuses
		{
			get => statuses;
			set
			{
				statuses = value;
				OnPropertyChanged("Statuses");
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

				if(value != null)
				{
					ImageSource = ImageSource.FromStream(() => CollectionItem.GetImageStream(value));
				}
				else
				{
					ImageSource = null;
				}

				OnPropertyChanged("Image");
			}
		}

		public ImageSource? ImageSource
		{
			get => imageSource;
			set
			{
				imageSource = value;
				OnPropertyChanged("ImageSource");
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

		public bool IsSold
		{
			get => isSold;
			set
			{
				isSold = value;
				OnPropertyChanged("IsSold");
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

		public static MemoryStream? GetImageStream(byte[]? image)
		{
			if(image == null)
			{
				return null;
			}

			return new MemoryStream(image);
		}

		public static string? ConvertImageToBase64(CollectionItem item)
		{
			if(item.Image == null)
			{
				return null;
			}

			string b64 = Convert.ToBase64String(item.Image);

			return b64;
		}

		public static byte[] ConvertBase64ToImage(string b64)
		{
			byte[] image = Convert.FromBase64String(b64);

			using (MemoryStream ms = new MemoryStream(image))
			{
				var imgSrc = ImageSource.FromStream(() => ms);

				if(imgSrc.IsEmpty)
				{
					throw new ArgumentException("Provided Base64 string is not a valid image.");
				}
			}

			return image;
		}

		private void OnPropertyChanged(string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
