﻿using CollectionsManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager.Services
{
	public class CollectionsService : ICollectionsService
	{
		private ObservableCollection<Collection> collections = new ObservableCollection<Collection>();

		public CollectionsService()
		{

		}

		public void AddCollection(Collection collection, bool mergeDiff = false)
		{
			var existingCollection = collections.FirstOrDefault(x => x.CollectionRefId.Equals(collection.CollectionRefId)
				|| x.Name.Equals(collection.Name));

			if(existingCollection != null)
			{
				if(mergeDiff)
				{
					MergeCollections(existingCollection, collection);
				}
				else
				{
					throw new ArgumentException($"Collection with RefId: '{collection.CollectionRefId}' or name: '{collection.Name}' already exists and merging differences has been disabled.");
				}
			}
			else
			{
				collections.Add(collection);
			}
		}

		public Collection CreateCollection(CreateCollection input)
		{
			if(string.IsNullOrWhiteSpace(input.Name))
			{
				throw new ArgumentException($"Collection name cannot be empty.");
			}

			if(collections
				.Where(x => x.Name.Equals(input.Name))
				.Any())
			{
				throw new ArgumentException($"Collection with name '{input.Name}' already exists");
			}

			Collection newCollection = new Collection()
			{
				Name = input.Name
			};

			Collection.DEFAULT_ITEM_STATUSES.ForEach(s => newCollection.ItemStatuses.Add(s));

			collections.Add(newCollection);

			return newCollection;
		}

		public CollectionItem CreateCollectionItem(CreateCollectionItem input)
		{
			Collection? collection = collections.FirstOrDefault(x => x.CollectionRefId.Equals(input.CollectionRefId));

			if(collection == null)
			{
				throw new ArgumentException($"Collection with RefId: '{input.CollectionRefId}' does not exist.");
			}

			CollectionItem newItem = new CollectionItem()
			{
				CollectionRefId = input.CollectionRefId,
				Name = input.Name,
				Comment = input.Comment,
				Quantity = input.Quantity,
				Rating = input.Rating,
				IsForSale = input.IsForSale,
				Image = input.Image
			};

            foreach (var status in input.Statuses)
            {
				newItem.Statuses.Add(status);
            }

			collection.Items.Add(newItem);

			return newItem;
        }

		public void DeleteCollection(Guid collectionId)
		{
			Collection? collection = collections.FirstOrDefault(x => x.CollectionRefId.Equals(collectionId));

			if(collection == null)
			{
				throw new ArgumentException($"Collection with RefId: '{collectionId}' does not exist.");
			}

			foreach(var item in collection.Items)
			{
				DisposeCollectionItem(item);
			}

			collection.Items.Clear();
			collection.ItemStatuses.Clear();

			collections.Remove(collection);
		}

		public void DeleteCollectionItem(Guid collectionId, Guid collectionItemId)
		{
			Collection? collection = collections.FirstOrDefault(x => x.CollectionRefId.Equals(collectionId));

			if(collection == null)
			{
				throw new ArgumentException($"Collection with RefId: '{collectionId}' does not exist.");
			}

			CollectionItem? item = collection.Items.FirstOrDefault(x => x.CollectionItemRefId.Equals(collectionItemId));

			if(item == null)
			{
				throw new ArgumentException($"Collection item with RefId: '{collectionItemId}' does not exist.");
			}

			DisposeCollectionItem(item);
			collection.Items.Remove(item);
		}

		public List<CollectionItem> GetCollectionItems(Collection collection)
		{
			return collection.Items
				.OrderBy(x => x.IsSold)
				.ToList();
		}

		public List<Collection> GetCollections()
		{
			return collections
				.OrderBy(x => x.Name)
				.ToList();
		}

		public Collection LoadCollectionFromFile(string path)
		{
			Debug.WriteLine($"Loading collection from path: '{path}'");

			string collectionFile = Path.Combine(path, "collection.txt");
			string collectionItemsFile = Path.Combine(path, "collectionItems.txt");
			string collectionItemStatusesFile = Path.Combine(path, "collectionItemStatuses.txt");

			Collection? collection = null;

			try
			{
				if(!File.Exists(collectionFile))
				{
					throw new IOException("Collection main file does not exist.");
				}

				using(var collectionFileStream = File.OpenRead(collectionFile))
				{
					using(var sr = new StreamReader(collectionFileStream))
					{
						string? line = sr.ReadLine();

						if(line == null || !line.Equals(FileMarkers.SOF_MARKER))
						{
							throw new IOException("Provided collection file is not valid.");
						}

						line = sr.ReadLine();

						if(line != null && !line.Equals(FileMarkers.EOF_MARKER))
						{
							string[] data = line.Split(FileMarkers.COLUMN_SEPARATOR);

							collection = new Collection()
							{
								CollectionRefId = Guid.Parse(data[0]),
								Name = data[1]
							};
						}
						else
						{
							throw new IOException("Unexpected EOF marker");
						}

						line = sr.ReadLine();

						if(line == null || !line.Equals(FileMarkers.EOF_MARKER))
						{
							throw new IOException("No EOF marker found - file may be corrupted.");
						}
					}
				}

				if(File.Exists(collectionItemStatusesFile))
				{
					using (var collectionItemStatusesStream = File.OpenRead(collectionItemStatusesFile))
					{
						using (var sr = new StreamReader(collectionItemStatusesStream))
						{
							string? line = sr.ReadLine();
							bool eofReached = false;

							if(line == null || !line.Equals(FileMarkers.SOF_MARKER))
							{
								throw new IOException("Provided collection file is not valid.");
							}

							while((line = sr.ReadLine()) != null)
							{
								if(line.Equals(FileMarkers.EOF_MARKER))
								{
									eofReached = true;
									break;
								}

								string[] data = line.Split(FileMarkers.COLUMN_SEPARATOR);

								var itemStatus = new CollectionItemStatus()
								{
									Id = Guid.Parse(data[0]),
									Name = data[1]
								};

								collection.ItemStatuses.Add(itemStatus);
							}

							if(!eofReached)
							{
								throw new IOException("No EOF marker found - file may be corrupted.");
							}
						}
					}
				}

				if(File.Exists(collectionItemsFile))
				{
					using (var collectionItemsStream = File.OpenRead(collectionItemsFile))
					{
						using(var sr = new StreamReader(collectionItemsStream))
						{
							string? line = sr.ReadLine();
							bool eofReached = false;

							if(line == null || !line.Equals(FileMarkers.SOF_MARKER))
							{
								throw new IOException("Provided collection file is not valid.");
							}

							while((line = sr.ReadLine()) != null)
							{
								if(line.Equals(FileMarkers.EOF_MARKER))
								{
									eofReached = true;
									break;
								}

								if(line != null)
								{
									string[] data = line.Split(FileMarkers.COLUMN_SEPARATOR);
									Guid[] itemStatusesGuids = data[8]
										.Split(FileMarkers.ITEM_STATUSES_SEPARATOR)
										.Select(s => Guid.Parse(s))
										.ToArray();

									List<CollectionItemStatus> itemStatuses = new List<CollectionItemStatus>();

									foreach(Guid guid in itemStatusesGuids)
									{
										CollectionItemStatus? status = collection.ItemStatuses
											.FirstOrDefault(x => x.Id.Equals(guid));

										if(status == null)
										{
											throw new IOException("Decoupled item status Guid found.");
										}

										itemStatuses.Add(status);
									}

									byte[]? imageData = null;

									if (data[9].Equals("N/A"))
									{
										imageData = null;
									}
									else
									{
										imageData = CollectionItem.ConvertBase64ToImage(data[9]);
									}

									var item = new CollectionItem()
									{
										CollectionItemRefId = Guid.Parse(data[0]),
										CollectionRefId = collection.CollectionRefId,
										Name = data[2],
										Quantity = int.Parse(data[3]),
										Rating = uint.Parse(data[4]),
										Comment = data[5],
										IsForSale = bool.Parse(data[6]),
										IsSold = bool.Parse(data[7]),
										Image = imageData
									};

									itemStatuses.ForEach(st => item.Statuses.Add(st));

									collection.Items.Add(item);
								}
							}

							if(!eofReached)
							{
								throw new IOException("No EOF marker found - file may be corrupted.");
							}
						}
					}
				}

				return collection;
			}
			catch(Exception)
			{
				if(collection != null)
				{
					collection.Items.Clear();
					collection.ItemStatuses.Clear();

					if(collections.Contains(collection))
					{
						collections.Remove(collection);
					}
				}

				throw;
			}
		}

		public void SaveCollectionToFile(Collection collection, string path)
		{
			Debug.WriteLine($"Saving collection '{collection.CollectionRefId}' to path: '{path}'");

			string collectionFile = Path.Combine(path, "collection.txt");
			string collectionItemsFile = Path.Combine(path, "collectionItems.txt");
			string collectionItemStatusesFile = Path.Combine(path, "collectionItemStatuses.txt");

			try
			{
				using(var collectionFileStream = File.OpenWrite(collectionFile))
				{
					using(var sw = new StreamWriter(collectionFileStream))
					{
						sw.WriteLine(FileMarkers.SOF_MARKER);

						sw.Write(collection.CollectionRefId.ToString());
						sw.Write(FileMarkers.COLUMN_SEPARATOR);
						sw.Write(collection.Name);
						sw.Write(Environment.NewLine);

						sw.WriteLine(FileMarkers.EOF_MARKER);
					}
				}

				if(collection.Items.Count > 0)
				{
					using(var collectionItemsFileStream = File.OpenWrite(collectionItemsFile))
					{
						using(var sw = new StreamWriter(collectionItemsFileStream))
						{
							sw.WriteLine(FileMarkers.SOF_MARKER);

							foreach(var item in collection.Items)
							{
								string itemStatuses = string.Join(
									FileMarkers.ITEM_STATUSES_SEPARATOR,
									item.Statuses
									.Select(s => s.Id.ToString())
									.ToList());

								string? imageB64 = CollectionItem.ConvertImageToBase64(item);

								sw.Write(item.CollectionItemRefId.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.CollectionRefId.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.Name);
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.Quantity.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.Rating.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.Comment);
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.IsForSale.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(item.IsSold.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(itemStatuses);
								sw.Write(FileMarkers.COLUMN_SEPARATOR);

								if(!string.IsNullOrWhiteSpace(imageB64))
								{
									sw.Write(imageB64);
									sw.Write(FileMarkers.COLUMN_SEPARATOR);
								}
								else
								{
									sw.Write("N/A");
									sw.Write(FileMarkers.COLUMN_SEPARATOR);
								}

								sw.Write(Environment.NewLine);
							}

							sw.WriteLine(FileMarkers.EOF_MARKER);
						}
					}
				}

				if(collection.ItemStatuses.Count > 0)
				{
					using (var collectionItemStatusesFileStream = File.OpenWrite(collectionItemStatusesFile))
					{
						using (var sw = new StreamWriter(collectionItemStatusesFileStream))
						{
							sw.WriteLine(FileMarkers.SOF_MARKER);

							foreach(var status in collection.ItemStatuses)
							{
								sw.Write(status.Id.ToString());
								sw.Write(FileMarkers.COLUMN_SEPARATOR);
								sw.Write(status.Name);
								sw.Write(Environment.NewLine);
							}

							sw.WriteLine(FileMarkers.EOF_MARKER);
						}
					}
				}
			}
			catch(Exception)
			{
				if(File.Exists(collectionFile))
				{
					File.Delete(collectionFile);
				}

				if(File.Exists(collectionItemsFile))
				{
					File.Delete(collectionItemsFile);
				}

				if(File.Exists(collectionItemStatusesFile))
				{
					File.Delete(collectionItemStatusesFile);
				}

				throw;
			}
		}

		public void LoadCollectionsFromFile(string? path, bool mergeDiff = false)
		{
			string _path = (!string.IsNullOrWhiteSpace(path))
				? path
				: AppConsts.CollectionsPath;

			Debug.WriteLine($"Loading collections from path: '{_path}'");

			DirectoryInfo pathInfo = new DirectoryInfo(_path);

			if(!pathInfo.Exists)
			{
				Directory.CreateDirectory(_path);
				throw new IOException("Provided path does not exist.");
			}

			List<Collection> loadedCollections = new List<Collection>();

			try
			{
				foreach(var collectionPath in pathInfo.EnumerateDirectories())
				{
					Collection collection = LoadCollectionFromFile(collectionPath.FullName);
					loadedCollections.Add(collection);
				}
			}
			catch(Exception)
			{
				loadedCollections.Clear();
				throw;
			}

			loadedCollections.ForEach(c =>
			{
				var existingCollection = collections
					.FirstOrDefault(x => x.CollectionRefId.Equals(c.CollectionRefId)
						|| x.Name.Equals(c.Name));

				if(existingCollection != null)
				{
					if(mergeDiff)
					{
						AddCollection(c, true);
					}
					else
					{
						Debug.WriteLine($"Collection with RefId: '{c.CollectionRefId}' already loaded. Skipping...");
					}
				}
				else
				{
					collections.Add(c);
				}
			});

			Debug.WriteLine($"Loaded {loadedCollections.Count} collections");
		}

		public void SaveCollectionsToFile(string? path)
		{
			string _path = (!string.IsNullOrWhiteSpace(path))
				? path
				: AppConsts.CollectionsPath;

			Debug.WriteLine($"Saving collections to path: '{_path}'");

			DirectoryInfo pathInfo = new DirectoryInfo(_path);

			if(!pathInfo.Exists)
			{
				Directory.CreateDirectory(_path);
			}
			else
			{
				PurgeDirectory(pathInfo);
			}

			try
			{
				foreach(var collection in collections)
				{
					var collectionPath = Collection.GetCollectionPath(collection, _path);

					if(!Directory.Exists(collectionPath))
					{
						Directory.CreateDirectory(collectionPath);
					}

					SaveCollectionToFile(collection, collectionPath);
				}
			}
			catch(Exception)
			{
				PurgeDirectory(pathInfo);
				throw;
			}
		}

		public Collection UpdateCollection(UpdateCollection input)
		{
			Collection? existingCollection = collections
				.FirstOrDefault(x => x.CollectionRefId.Equals(input.CollectionRefId));

			if(existingCollection == null)
			{
				throw new ArgumentException($"Collection with RefId: '{input.CollectionRefId}' does not exist.");
			}

			if(string.IsNullOrWhiteSpace(input.Name))
			{
				throw new ArgumentException($"Collection name cannot be empty.");
			}

			if(collections
				.Where(x => x.Name.Equals(input.Name)
					&& !x.CollectionRefId.Equals(input.CollectionRefId))
				.Any())
			{
				throw new ArgumentException($"Collection with name '{input.Name}' already exists");
			}

			existingCollection.Name = input.Name;

			return existingCollection;
		}

		public CollectionItem UpdateCollectionItem(UpdateCollectionItem input)
		{
			Collection? oldCollection = collections
				.FirstOrDefault(x => x.CollectionRefId.Equals(input.CollectionRefId));

			Collection? newCollection = (input.NewCollectionRefId.HasValue)
				? collections
					.FirstOrDefault(x => x.CollectionRefId.Equals(input.NewCollectionRefId.Value))
				: null;

			if(oldCollection == null)
			{
				throw new ArgumentException($"Collection with RefId: '{input.CollectionRefId}' does not exist.");
			}

			if(input.NewCollectionRefId.HasValue && newCollection == null)
			{
				throw new ArgumentException($"Collection with RefId: '{input.NewCollectionRefId.Value}' does not exist.");
			}

			CollectionItem? item = oldCollection.Items
				.FirstOrDefault(x => x.CollectionItemRefId.Equals(input.CollectionItemRefId));

			if(item == null)
			{
				throw new ArgumentException($"Collection item with RefId: '{input.CollectionItemRefId}' does not exist.");
			}

			if(newCollection != null)
			{
				oldCollection.Items.Remove(item);
				newCollection.Items.Add(item);
			}

			item.Name = input.Name;
			item.Comment = input.Comment;
			item.Quantity = input.Quantity;
			item.Rating = input.Rating;
			item.IsForSale = input.IsForSale;
			item.IsSold = input.IsSold;
			item.Image = input.Image;

			item.Statuses.Clear();

			input.Statuses
				.ToList()
				.ForEach(s => item.Statuses.Add(s));

			return item;
		}

		public int GetCollisions(Collection collection, string itemName)
		{
			return collection.Items
				.Where(x => x.Name.Equals(itemName))
				.Count();
		}

		private void DisposeCollectionItem(CollectionItem item)
		{
			item.Image = null;
			item.Statuses.Clear();
		}

		private static void PurgeDirectory(DirectoryInfo baseDir)
		{
			if (!baseDir.Exists)
				return;
			
			foreach (var dir in baseDir.EnumerateDirectories())
			{
				RecursiveDelete(dir);
			}
		}

		private static void RecursiveDelete(DirectoryInfo baseDir)
		{
			if (!baseDir.Exists)
				return;

			foreach (var dir in baseDir.EnumerateDirectories())
			{
				RecursiveDelete(dir);
			}
			baseDir.Delete(true);
		}

		private void MergeCollections(Collection oldCollection, Collection newCollection)
		{
			oldCollection.Name = newCollection.Name;

			foreach(var itemStatus in newCollection.ItemStatuses)
			{
				var existingStatus = oldCollection.ItemStatuses
					.FirstOrDefault(x => x.Id.Equals(itemStatus.Id)
						|| x.Name.Equals(itemStatus.Name));

				if(existingStatus != null)
				{
					existingStatus.Name = itemStatus.Name;
				}
				else
				{
					oldCollection.ItemStatuses.Add(itemStatus);
				}
			}

			foreach(var item in newCollection.Items)
			{
				var existingItem = oldCollection.Items
					.FirstOrDefault(x => x.CollectionItemRefId.Equals(item.CollectionItemRefId)
						|| x.Name.Equals(item.Name));

				if(existingItem != null)
				{
					existingItem.Name = item.Name;
					existingItem.Quantity = item.Quantity;
					existingItem.Rating = item.Rating;
					existingItem.Comment = item.Comment;
					existingItem.IsForSale = item.IsForSale;
					existingItem.IsSold = item.IsSold;
					existingItem.Image = item.Image;

					foreach(var itemStatus in item.Statuses)
					{
						var existingStatus = existingItem.Statuses
							.FirstOrDefault(x => x.Id.Equals(itemStatus.Id)
								|| x.Name.Equals(itemStatus.Name));

						if(existingStatus == null)
						{
							oldCollection.ItemStatuses.Add(itemStatus);
						}
					}
				}
				else
				{
					oldCollection.Items.Add(item);
				}
			}
		}
	}
}
