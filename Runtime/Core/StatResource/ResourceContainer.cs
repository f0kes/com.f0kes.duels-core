using System;
using Core.Interfaces;
using Core.Stats;
using RiptideNetworking;
using UnityEngine;

namespace Core.StatResource
{
	public class ResourceContainer : ISerializableData
	{
		public Stat Capacity { get; private set; }
		public Action<float> OnValueChanged;
		public Action OnDepleted;
		
		private float _remainingPercent;

		public float CurrentValue => _remainingPercent * Capacity.GetValue();
		public float RemainingPercent => _remainingPercent;
		

		public ResourceContainer(Stat capacity, float initialValue = 1)
		{
			Capacity = capacity;
			_remainingPercent = initialValue;
		}


		public void SubtractValue(float value)
		{
			float newValue = CurrentValue - value;
			if (newValue < 0)
			{
				newValue = 0;
			}

			_remainingPercent = newValue / Capacity.GetValue();
			OnValueChanged?.Invoke(_remainingPercent);
			if (_remainingPercent <= 0)
			{
				OnDepleted?.Invoke();
			}
		}

		public void AddValue(float value)
		{
			float newValue = CurrentValue + value;
			newValue = Mathf.Min(newValue, Capacity.GetValue());
			_remainingPercent = newValue / Capacity.GetValue();
			OnValueChanged?.Invoke(_remainingPercent);
		}

		public Message Serialize(Message message)
		{
			//Capacity.Serialize(message);
			message.AddFloat(_remainingPercent);
			return message;
		}

		public void Deserialize(Message message)
		{
			//Capacity.Deserialize(message);
			_remainingPercent = message.GetFloat();
		}
	}
}