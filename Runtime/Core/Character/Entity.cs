using System;
using System.Collections.Generic;
using Core.Combat;
using Core.CoreEnums;
using Core.Stats;
using Core.Enums;
using Core.Events;
using UnityEngine;

namespace Core.Character
{
	public class Entity : MonoBehaviour
	{
		public EntityState State { get; private set; }
		
		public Action<float> OnHealthChanged;
		public Action Initialized;
		public Action OnDeath;
		public static Dictionary<ushort, Entity> EntityDict = new Dictionary<ushort, Entity>();

		public ushort Id { get; protected set; }


		[Serializable]
		public struct AttributeKeyValue
		{
			public AttributeStat AttributeStat;
			public float Value;
		}

		[SerializeField] private List<AttributeKeyValue> _inspectorAttributes;

		[SerializeField] private EntityCombat _combat;

		public StatDict<AttributeStat> Attributes = new StatDict<AttributeStat>();
		public StatDict<BasedStat> Stats = new StatDict<BasedStat>();

		public float CurrentHealth => _currentHealthPercent * Stats[BasedStat.Health];
		public EntityCombat Combat => _combat;

		private float _currentHealthPercent = 1;
		
		public static implicit operator ushort(Entity entity)
		{
			return entity == null ? ushort.MinValue : entity.Id;
		}
		public static implicit operator Entity(ushort id)
		{
			return EntityDict.ContainsKey(id) ? EntityDict[id] : null;
		}

		private void Awake()
		{
			EventTrigger.I[this, ActionType.HitTaken]
				.Subscribe((args) =>
				{
					if (args is DamageEventArgs args1)
					{
						TakeDamage(args1.Damage);
					}
				}, true);
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

			State |= EntityState.CanMove;
			State |= EntityState.CanAttack;
			State |= EntityState.Idle;
			State |= EntityState.CanBeAttacked;
			
			_combat.Init(this, Attributes, Stats, new CombatStateContainer());
			Initialized?.Invoke();
		}

		public void TakeDamage(Damage damage)
		{
			float newHealth = CurrentHealth - damage.Amount;
			_currentHealthPercent = newHealth / Stats[BasedStat.Health];
			OnHealthChanged?.Invoke(_currentHealthPercent);
			if (_currentHealthPercent <= 0)
			{
				_currentHealthPercent = 0;
				Die();
			}
		}

		private void Die()
		{
			OnDeath?.Invoke();
			EventTrigger.I[this, ActionType.OnDeath].Invoke( new DeathEventArgs(this));
		}
	}
}