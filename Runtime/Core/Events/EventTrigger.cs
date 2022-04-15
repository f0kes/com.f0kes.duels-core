﻿using System;
using System.Collections.Generic;
using Core.Character;
using Core.Combat;
using Core.CoreEnums;
using JetBrains.Annotations;
using PlasticGui.WebApi.Responses;
using RiptideNetworking;

namespace Core.Events
{
	public struct TriggerKey
	{
		public CharacterEntity Entity { get; }
		public ActionType ActionType { get; }

		public TriggerKey(CharacterEntity entity, ActionType actionType)
		{
			Entity = entity;
			ActionType = actionType;
		}
	}

	public class EventTrigger<T> where T : TriggerEventArgs
	{
		public class TriggerAction<TAction>
		{
			public bool Authorized { get; set; }
			private Action<TAction> AuthorizedAction { get; set; }
			private Action<TAction> UnauthorizedAction { get; set; }

			public void Subscribe(Action<TAction> action, bool needsToBeAuthorised)
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

			public void Invoke(TAction action, bool forceAuthorization = false)
			{
				if (Authorized || forceAuthorization)
				{
					AuthorizedAction?.Invoke(action);
				}

				UnauthorizedAction?.Invoke(action);
			}
		}

		private static EventTrigger<T> _instance;
		public static EventTrigger<T> I => _instance ??= new EventTrigger<T>();


		private Dictionary<TriggerKey, TriggerAction<T>> _triggers =
			new Dictionary<TriggerKey, TriggerAction<T>>();

		private List<TriggerKey> _workingEvents = new List<TriggerKey>();

		public List<TriggerKey> GetWorkingEvents()
		{
			return _workingEvents;
		}

		public TriggerAction<T> this[CharacterEntity entity, ActionType type]
		{
			get
			{
				TriggerKey triggerKey = new TriggerKey(entity, type);
				if (!_triggers.ContainsKey(triggerKey))
				{
					_triggers[triggerKey] = new TriggerAction<T>();
				}

				return _triggers[triggerKey];
			}
			set
			{
				TriggerKey triggerKey = new TriggerKey(entity, type);
				if (value != null)
				{
					_workingEvents.Add(triggerKey);
				}
				else if (_workingEvents.Contains(triggerKey))
				{
					_workingEvents.Remove(triggerKey);
				}

				_triggers[triggerKey] = value;
			}
		}

		public TriggerAction<T> this[ushort entityId, ActionType type]
		{
			get
			{
				CharacterEntity entity = CharacterEntity.EntityDict[entityId];
				return this[entity, type];
			}
			set
			{
				CharacterEntity entity = CharacterEntity.EntityDict[entityId];
				this[entity, type] = value;
			}
		}
	}

	public abstract class TriggerEventArgs : EventArgs
	{
		public abstract Message Serialize(Message initial);
		public abstract void Deserialize(Message message);
	}

	public class DamageEventArgs : TriggerEventArgs
	{
		public Damage Damage { get; set; }

		public DamageEventArgs(Damage damage)
		{
			Damage = damage;
		}

		public override Message Serialize(Message initial)
		{
			throw new NotImplementedException();
		}

		public override void Deserialize(Message message)
		{
			throw new NotImplementedException();
		}
	}

	public class AttackEventArgs : TriggerEventArgs
	{
		public Weapon Weapon { get; set; }
		public AttackEventArgs(Weapon weapon)
		{
			Weapon = weapon;
		}

		public override Message Serialize(Message initial)
		{
			throw new NotImplementedException();
		}

		public override void Deserialize(Message message)
		{
			throw new NotImplementedException();
		}
	}
}