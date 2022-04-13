using System;
using System.Collections.Generic;
using Core.Character;
using Core.Combat;
using Core.CoreEnums;

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

	public class EventTrigger<T> where T : EventArgs
	{
		private static EventTrigger<T> _instance;
		public static EventTrigger<T> Instance => _instance ??= new EventTrigger<T>();
		

		private Dictionary<TriggerKey, Action<T>> _dictionary =
			new Dictionary<TriggerKey, Action<T>>();

		private List<TriggerKey> _workingEvents = new List<TriggerKey>();

		public List<TriggerKey> GetWorkingEvents()
		{
			return _workingEvents;
		}

		public Action<T> this[TriggerKey triggerKey]
		{
			get
			{
				if (!_dictionary.ContainsKey(triggerKey))
				{
					_dictionary[triggerKey] = null;
				}

				return _dictionary[triggerKey];
			}
			set
			{
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

	public class DamageEventArgs : EventArgs
	{
		public Damage Damage { get; set; }

		public DamageEventArgs(Damage damage)
		{
			Damage = damage;
		}
	}
}