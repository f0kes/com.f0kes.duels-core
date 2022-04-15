using System.Collections.Generic;

namespace Core.Types
{
	public static class IDGetter<T>
	{
		public static Dictionary<ushort, T> _IDDictionary = new Dictionary<ushort, T>();
	}
}