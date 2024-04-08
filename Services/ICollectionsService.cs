using CollectionsManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager.Services
{
	public interface ICollectionsService
	{
		public List<Collection> GetCollections();
		public Collection CreateCollection(CreateCollection input);
		public Collection UpdateCollection(UpdateCollection input);
		public void DeleteCollection(Guid collectionId);

		public List<CollectionItem> GetCollectionItems(Collection collection);
		public CollectionItem CreateCollectionItem(CreateCollectionItem input);
		public CollectionItem UpdateCollectionItem(UpdateCollectionItem input);
		public void DeleteCollectionItem(Guid collectionId, Guid collectionItemId);

		public void SaveCollectionToFile(Collection collection, string path);
		public void SaveCollectionsToFile(string? path);
		public Collection LoadCollectionFromFile(string path);
		public void LoadCollectionsFromFile(string? path);
	}
}
