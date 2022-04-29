using System;
using System.Collections.Generic;
using Core.Combat;
using Core.CoreEnums;
using Core.Stats;
using Core.Enums;
using Core.Events;
using Core.Interfaces;
using Core.StatResource;
using Core.Types;
using RiptideNetworking;
using UnityEngine;

namespace Core.Character
{
	public class Entity : MonoBehaviour, IIdentifiable
	{
		public EntityState State { get; private set; }

		public Action<float> OnHealthChanged;
		public Action Initialized;
		public Action OnDeath;

		public static Dictionary<ushort, Entity> EntityDict = new Dictionary<ushort, Entity>();
		private Identity _identity;

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

		private ResourceContainer _health;
		private DamageHandler _damageHandler;
		public EntityCombat Combat => _combat;


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
			_identity = Identity.Root.GenerateChild(this, id);
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

			
			_health = new ResourceContainer(Stats.GetStat(BasedStat.Health));
			_damageHandler = new DamageHandler(_health, _identity);
			_combat.Init(this, Attributes, Stats, new CombatStateContainer(),_damageHandler);
			
			_health.OnDepleted += Die;
			_health.OnValueChanged += (percent) => OnHealthChanged?.Invoke(percent);
			Initialized?.Invoke();
		}

		public void TakeDamage(Damage damage)
		{
			_damageHandler.InitiateDamage(damage);
			OnHealthChanged?.Invoke(_health.RemainingPercent);
		}

		private void Die()
		{
			OnDeath?.Invoke();
			EventTrigger.I[this, ActionType.OnDeath].Invoke(new DeathEventArgs(this));
		}

		public Message Serialize(Message message)
		{
			_health.Serialize(message);
			return message;
		}
		public void Deserialize(Message message)
		{
			_health.Deserialize(message);
		}
	}
}