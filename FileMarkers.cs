using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager
{
	public static class FileMarkers
	{
		public const string SOF_MARKER = "#@#";
		public const string EOF_MARKER = "@#@";
		public const char COLUMN_SEPARATOR = '|';
		public const char ITEM_STATUSES_SEPARATOR = '^';
	}
}
