using System;
using Core.Stats;
using UnityEngine;

namespace Core.StatResource
{
	public class ResourceContainer
	{
		public Action<float> OnValueChanged;
		public Action OnDepleted;
		private Stat _capacity;
		private float _remainingPercent;

		public float CurrentValue => _remainingPercent * _capacity.GetValue();

		public ResourceContainer(Stat capacity, float initialValue = 1)
		{
			_capacity = capacity;
			_remainingPercent = initialValue;
		}


		public void SubtractValue(float value)
		{
			float newValue = CurrentValue - value;
			if (newValue < 0)
			{
				newValue = 0;
			}

			_remainingPercent = newValue / _capacity.GetValue();
			OnValueChanged?.Invoke(_remainingPercent);
			if (_remainingPercent <= 0)
			{
				OnDepleted?.Invoke();
			}
		}

		public void AddValue(float value)
		{
			float newValue = CurrentValue + value;
			newValue = Mathf.Min(newValue, _capacity.GetValue());
			_remainingPercent = newValue / _capacity.GetValue();
			OnValueChanged?.Invoke(_remainingPercent);
		}
	}
}