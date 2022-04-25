using System;
using System.Collections.Generic;
using Core.Events;
using Core.StatResource;

namespace Core.Combat
{
	public class DamageHandler
	{
		public AuthorizableAction<Damage> OnDamageInitiated;
		public AuthorizableAction<Damage> OnDamageDeflected;
		public AuthorizableAction<Damage> OnDamageDealt;
		
		private List<Damage> _damages = new List<Damage>();

		private ResourceContainer _damagableResource;

		public DamageHandler(ResourceContainer damagableResource)
		{
			_damagableResource = damagableResource;
		}

		public void InitiateDamage(Damage damage)
		{
			_damages.Add(damage);
			OnDamageInitiated?.Invoke(damage);
			if (!damage.IsDeflected)
			{
				DealDamage(damage);
			}
		}

		public void DeflectDamage(Damage damage)
		{
			if (!_damages.Contains(damage)) return;
			damage.Deflect();
			OnDamageDeflected?.Invoke(damage);
		}

		private void DealDamage(Damage damage)
		{
			_damagableResource.SubtractValue(damage.Amount);
			OnDamageDealt?.Invoke(damage);
			_damages.Remove(damage);
		}
	}
}