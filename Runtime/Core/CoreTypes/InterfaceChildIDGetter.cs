using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Types
{
	public static class InterfaceChildIDGetter<T>
	{
		private static readonly Dictionary<ushort, Type> _ids;

		static InterfaceChildIDGetter()
		{
			_ids = new Dictionary<ushort, Type>();
			var type = typeof(T);
			var objects = Assembly.GetAssembly(typeof(T)).GetTypes()
				.Where(p => type.IsAssignableFrom(p)).ToList();

			objects.Sort((t, t1) => string.CompareOrdinal(t.ToString(), t1.ToString()));
			for (ushort i = 0; i < objects.Count; i++)
			{
				_ids[i] = objects[i];
			}
		}

		public static ushort GetIDByType(Type type)
		{
			if (typeof(T).IsAssignableFrom(type))
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