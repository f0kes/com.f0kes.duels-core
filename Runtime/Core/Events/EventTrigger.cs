using System;
using System.Collections.Generic;
using Core.Character;
using Core.Combat;
using Core.CoreEnums;
using RiptideNetworking;

namespace Core.Events
{
	public struct TriggerKey
	{
		public Entity Entity { get; }
		public ActionType ActionType { get; }

		public TriggerKey(Entity entity, ActionType actionType)
		{
			Entity = entity;
			ActionType = actionType;
		}

		public TriggerKey(Message message)
		{
			Entity = Entity.EntityDict[message.GetUShort()];
			ActionType = (ActionType) message.GetUShort();
		}

		public Message Serialize(Message message)
		{
			message.AddUShort(Entity.Id);
			message.AddUShort((ushort) ActionType);
			return message;
		}
	}

	public class EventTrigger
	{
		public Action<TriggerKey, TriggerEventArgs> AnyActionTriggered;

		public class TriggerAction<TArgs>
		{
			public bool Authorized { get; set; }
			private Action<TArgs> AuthorizedAction { get; set; }
			private Action<TArgs> UnauthorizedAction { get; set; }

			public void Subscribe(Action<TArgs> action, bool needsToBeAuthorised)
			{
				if (needsToBeAuthorised)
				{
					AuthorizedAction += action;
				}
				else
				{
					UnauthorizedAction += action;
				}
			}

			public void Invoke(TArgs action, bool forceAuthorization = false)
			{
				if (Authorized || forceAuthorization)
				{
					AuthorizedAction?.Invoke(action);
				}

				UnauthorizedAction?.Invoke(action);
			}
		}

		private static EventTrigger _instance;
		public static EventTrigger I => _instance ??= new EventTrigger();


		private Dictionary<TriggerKey, TriggerAction<TriggerEventArgs>> _triggers =
			new Dictionary<TriggerKey, TriggerAction<TriggerEventArgs>>();


		public TriggerAction<TriggerEventArgs> this[TriggerKey triggerKey]
		{
			get
			{
				if (!_triggers.ContainsKey(triggerKey))
				{
					_triggers[triggerKey] = new TriggerAction<TriggerEventArgs>();
					_triggers[triggerKey].Subscribe((args) => { AnyActionTriggered?.Invoke(triggerKey, args); }, false);
				}

				return _triggers[triggerKey];
			}
		}

		public TriggerAction<TriggerEventArgs> this[Entity entity, ActionType type]
		{
			get
			{
				var triggerKey = new TriggerKey(entity, type);
				return this[triggerKey];
			}
		}

		public TriggerAction<TriggerEventArgs> this[ushort entityId, ActionType type]
		{
			get
			{
				var entity = Entity.EntityDict[entityId];
				return this[entity, type];
			}
		}
	}

	public abstract class TriggerEventArgs : EventArgs
	{
		public abstract Message Serialize(Message message);
		public abstract void Deserialize(Message message);
	}

	public class EmptyEventArgs : TriggerEventArgs
	{
		public override Message Serialize(Message message)
		{
			return message;
		}

		public override void Deserialize(Message message)
		{
		}
	}

	public class DamageEventArgs : TriggerEventArgs
	{
		public Damage Damage { get; set; }

		public DamageEventArgs(Damage damage)
		{
			Damage = damage;
		}

		public DamageEventArgs()
		{
		}

		public override Message Serialize(Message message)
		{
			message = Damage.Serialize(message);
			return message;
		}

		public override void Deserialize(Message message)
		{
			Damage = new Damage(message);
		}
	}

	public class AttackEventArgs : TriggerEventArgs
	{
		public Attack Attack;

		public AttackEventArgs(Attack attack)
		{
			Attack = attack;
		}

		public AttackEventArgs()
		{
		}

		public override Message Serialize(Message message)
		{
			message = Attack.Serialize(message);
			return message;
		}

		public override void Deserialize(Message message)
		{
			Attack = new Attack(message);
		}
	}

	public class DeathEventArgs : TriggerEventArgs
	{
		public Entity Killer { get; set; }

		public DeathEventArgs()
		{
		}

		public DeathEventArgs(Entity killer)
		{
			Killer = killer;
		}

		public override Message Serialize(Message message)
		{
			message.AddUShort(Killer.Id);
			return message;
		}

		public override void Deserialize(Message message)
		{
			Killer = Entity.EntityDict[message.GetUShort()];
		}
	}
}