﻿using System;
using System.Collections.Generic;
using Core.Combat;
using Core.CoreEnums;
using Core.Stats;
using Core.Enums;
using Core.Events;
using UnityEngine;

namespace Core.Character
{
	public class CharacterEntity : MonoBehaviour
	{
		public static Dictionary<ushort, CharacterEntity> EntityDict = new Dictionary<ushort, CharacterEntity>();

		public ushort Id { get; protected set; }
		
		
		[Serializable]
		public struct AttributeKeyValue
		{
			public AttributeStat AttributeStat;
			public float Value;
		}

		[SerializeField] private List<AttributeKeyValue> _inspectorAttributes;

		[SerializeField] private CharacterCombat _combat;

		public StatDict<AttributeStat> Attributes = new StatDict<AttributeStat>();
		public StatDict<BasedStat> Stats = new StatDict<BasedStat>();

		public float CurrentHealth => _currentHealthPercent * Stats[BasedStat.Health];

		private float _currentHealthPercent = 1;

		private void Awake()
		{
			EventTrigger<DamageEventArgs>.I[this, ActionType.HitTaken]
				.Subscribe((args) => TakeDamage(args.Damage), true);
		}

		public void SetAttributes(float[] values)
		{
			_inspectorAttributes = new List<AttributeKeyValue>();
			for (int i = 0; i < values.Length; i++)
			{
				AttributeKeyValue attributeKeyValue = new AttributeKeyValue
					{AttributeStat = (AttributeStat) i, Value = values[i]};
				_inspectorAttributes.Add(attributeKeyValue);
			}

			Init(Id);
		}

		public void Init(ushort id)
		{
			EntityDict[id] = this;
			Id = id;
			foreach (AttributeKeyValue kv in _inspectorAttributes)
			{
				Attributes.SetStat(kv.AttributeStat, new Stat(kv.Value));
			}

			foreach (var stat in (BasedStat[]) Enum.GetValues(typeof(BasedStat)))
			{
				Stats.SetStat(stat, BasedStatBook.GetBasedStat(stat, Attributes));
			}

			_combat.Init(id, Attributes, Stats);
		}

		public void TakeDamage(Damage damage)
		{
			float newHealth = CurrentHealth - damage.Amount;
			_currentHealthPercent = newHealth / Stats[BasedStat.Health];
			Debug.Log(damage.Amount);
			if (_currentHealthPercent <= 0)
			{
				_currentHealthPercent = 0;
				Die();
			}
		}

		private void Die()
		{
			Destroy(gameObject);
		}
	}
}