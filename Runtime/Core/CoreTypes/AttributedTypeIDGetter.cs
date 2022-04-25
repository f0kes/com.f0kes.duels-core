using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Types
{
	public class AttributedTypeIDGetter
	{
		private static Dictionary<ushort, Type> _ids;

		static AttributedTypeIDGetter()
		{
			_ids = new Dictionary<ushort, Type>();
			var objects = GetTypesWithHelpAttribute(typeof(AttributedTypeIDGetter).Assembly).ToList();
			objects.Sort((type, type1) => string.CompareOrdinal(type.ToString(), type1.ToString()));
			for (ushort i = 0; i < objects.Count; i++)
			{
				_ids[i] = objects[i];
			}
		}

		static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly)
		{
			return assembly.GetTypes().Where(type =>
				type.GetCustomAttributes(typeof(EnumerableTypeAttribute), true).Length > 0);
		}

		public static ushort GetIDByType(Type type)
		{
			var id = _ids.First((x) => x.Value == type).Key;
			return id;
		}

		public static Type GetTypeById(ushort id)
		{
			return _ids[id];
		}
	}
}