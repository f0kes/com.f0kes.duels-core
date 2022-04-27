using System.Collections.Generic;

namespace Core.Types
{
	public static class IDGetter<T>
	{
		private static Dictionary<T,ushort> _ids = new Dictionary<T,ushort>();
		private static ushort _nextId = 0;
		public static ushort GetId(T obj)
		{
			ushort id;
			if(!_ids.TryGetValue(obj, out id))
			{
				id = _nextId++;
				_ids.Add(obj, id);
			}
			return id;
		}
		public static T GetObject(ushort id)
		{
			foreach(var pair in _ids)
			{
				if(pair.Value == id)
					return pair.Key;
			}
			return default(T);
		}

	}
}