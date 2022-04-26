using System;
using System.Collections.Generic;
using Core.Character;
using Core.CoreEnums;
using RiptideNetworking;
using UnityEngine;

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

		private static EventTrigger _instance;
		public static EventTrigger I => _instance ??= new EventTrigger();


		private Dictionary<TriggerKey, AuthorizableAction> _triggers =
			new Dictionary<TriggerKey, AuthorizableAction>();


		public AuthorizableAction this[TriggerKey triggerKey]
		{
			get
			{
				if (!_triggers.ContainsKey(triggerKey))
				{
					_triggers[triggerKey] = new AuthorizableAction();
					_triggers[triggerKey].Subscribe((args) => { AnyActionTriggered?.Invoke(triggerKey, args); }, false);
				}

				return _triggers[triggerKey];
			}
		}

		public AuthorizableAction this[Entity entity, ActionType type]
		{
			get
			{
				var triggerKey = new TriggerKey(entity, type);
				return this[triggerKey];
			}
		}

		public AuthorizableAction this[ushort entityId, ActionType type]
		{
			get
			{
				var entity = Entity.EntityDict[entityId];
				return this[entity, type];
			}
		}
	}
}