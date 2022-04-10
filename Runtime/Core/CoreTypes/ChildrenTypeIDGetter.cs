using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Types
{
	public static class ChildrenTypeIDGetter<T>
	{
		private static Dictionary<ushort, Type> _ids;

		static ChildrenTypeIDGetter()
		{
			_ids = new Dictionary<ushort, Type>();
			List<Type> objects = new List<Type>();
			foreach (Type type in
			         Assembly.GetAssembly(typeof(T)).GetTypes()
				         .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
			{
				objects.Add(type);
			}

			objects.Sort((type,type1)=> String.CompareOrdinal(type.ToString(),type1.ToString()));
			for (ushort i = 0; i < objects.Count; i++)
			{
				_ids[i] = objects[i];
			}
		}

		public static ushort GetIDByType(Type type)
		{
			if (type.IsSubclassOf(typeof(T)))
			{
				var id = _ids.First((x) => x.Value == type).Key;
				return id;
			}
			else
			{
				throw new Exception($"Type{type} is not a child of type {typeof(T)}");
			}
		}

		public static Type GetTypeById(ushort id)
		{
			return _ids[id];
		}
	}
}