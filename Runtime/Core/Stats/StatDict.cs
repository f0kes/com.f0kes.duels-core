using System;
using System.Collections.Generic;
using System.Linq;
using Core.Stats;

namespace Core.Stats
{
	public class StatDict<T>
	{
		private Dictionary<T, Stat> _stats = new Dictionary<T, Stat>();
		public Stat[] Values => _stats.Values.ToArray();
		public float this[T name] => _stats[name].GetValue();

		public Stat GetStat(T name)
		{
			return _stats[name];
		}

		public void SetStat(T name, Stat stat)
		{
			_stats[name] = stat;
		}
	}
}