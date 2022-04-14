using System;
using System.Collections.Generic;
using Core.Character;
using Core.Combat;
using Core.CoreEnums;
using JetBrains.Annotations;
using PlasticGui.WebApi.Responses;

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
					UnauthorizedAction += action;
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
					UnauthorizedAction?.Invoke(action);
					AuthorizedAction?.Invoke(action);
				}
				else
				{
					UnauthorizedAction?.Invoke(action);
				}
			}
		}

		private static EventTrigger<T> _instance;
		public static EventTrigger<T> I => _instance ??= new EventTrigger<T>();


		private Dictionary<TriggerKey, TriggerAction<T>> _dictionary =
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
				if (!_dictionary.ContainsKey(triggerKey))
				{
					_dictionary[triggerKey] = new TriggerAction<T>();
				}

				return _dictionary[triggerKey];
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

				_dictionary[triggerKey] = value;
			}
		}
	}

	public abstract class TriggerEventArgs : EventArgs
	{
	}

	public class DamageEventArgs : TriggerEventArgs
	{
		public Damage Damage { get; set; }

		public DamageEventArgs(Damage damage)
		{
			Damage = damage;
		}
	}
}