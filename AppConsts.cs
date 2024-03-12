using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager
{
	public static class AppConsts
	{
		public static string AppDataPath { get; private set; } = FileSystem.Current.AppDataDirectory;
		public static string CollectionsPath { get; private set; } = Path.Combine(AppDataPath, "Collections");
	}
}
