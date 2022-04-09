using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Stats;
using UnityEngine;

namespace Core.Stats
{
	[System.Serializable]
	public class Stat : ISerializableData
	{
		[SerializeField] private float baseValue;

		public float BaseValue => baseValue;

		private float _lastValue;
		private List<StatModifier> _modifiers;

		public List<StatModifier> Modifiers => _modifiers;

		public Stat()
		{
			_modifiers = new List<StatModifier>();
		}

		public Stat(float baseValue)
		{
			_modifiers = new List<StatModifier>();
			this.baseValue = baseValue;
		}

		public float GetValue()
		{
			float result = baseValue;
			foreach (var mod in _modifiers)
			{
				mod.ApplyMod(ref result, baseValue);
			}

			_lastValue = result;
			return result;
		}

		public void SetBaseValue(float val)
		{
			baseValue = val;
		}

		public float GetLastValue()
		{
			return _lastValue;
		}

		public void AddMod(StatModifier mod)
		{
			_modifiers.Add(mod);
			_modifiers = _modifiers.OrderBy(m => m.priority).ToList();
		}

		public void RemoveMod(StatModifier mod)
		{
			if (_modifiers.Contains(mod))
			{
				_modifiers.Remove(mod);
				_modifiers = _modifiers.OrderBy(m => m.priority).ToList();
			}
		}


		public void Serialize(BinaryWriter bw)
		{
			bw.Write(baseValue);
			bw.Write(_modifiers.Count);
			foreach (var modifier in _modifiers)
			{
				bw.Write(ChildrenTypeIDGetter<StatModifier>.GetIDByType(modifier.GetType()));
				modifier.Serialize(bw);
			}
		}

		public void Deserialize(BinaryReader br)
		{
			float temp = br.ReadSingle();
			baseValue = temp;
			int modCount = br.ReadInt32();
			_modifiers = new List<StatModifier>();
			for (int i = 0; i < modCount; i++)
			{
				ushort modTypeID = br.ReadUInt16();
				Type modType = ChildrenTypeIDGetter<StatModifier>.GetTypeById(modTypeID);
				StatModifier modifier = (StatModifier) Activator.CreateInstance(modType);
				modifier.Deserialize(br);
			}
		}
	}
}