using System;
using System.Collections.Generic;
using Core.Events;
using Core.Interfaces;
using Core.StatResource;
using Core.Types;

namespace Core.Combat
{
	public class DamageHandler : IIdentifiable
	{
		private Identity _identity;

		public AuthorizableActionSync OnDamageInitiated;
		public AuthorizableActionSync OnDamageDeflected;
		public AuthorizableActionSync OnDamageDealt;

		private List<Damage> _damages = new List<Damage>();

		private ResourceContainer _damagableResource;

		public DamageHandler(ResourceContainer damagableResource, Identity parentIdentity)
		{
			_damagableResource = damagableResource;
			_identity = parentIdentity.GenerateChild(this);

			OnDamageInitiated = new AuthorizableActionSync(_identity);
			OnDamageDeflected = new AuthorizableActionSync(_identity);
			OnDamageDealt = new AuthorizableActionSync(_identity);

			OnDamageDealt.Subscribe((dArgs) =>
			{
				if (dArgs is DamageEventArgs dArgs1)
					_damagableResource.SubtractValue(dArgs1.Damage.Amount);
			}, true);
		}

		public void InitiateDamage(Damage damage)
		{
			_damages.Add(damage);
			OnDamageInitiated?.Invoke(new DamageEventArgs(damage));
			if (!damage.IsDeflected)
			{
				DealDamage(damage);
			}
		}

		public void DeflectDamage(Damage damage)
		{
			if (!_damages.Contains(damage)) return;
			damage.Deflect();
			OnDamageDeflected?.Invoke(new DamageEventArgs(damage));
		}

		private void DealDamage(Damage damage)
		{
			OnDamageDealt?.Invoke(new DamageEventArgs(damage));
			_damages.Remove(damage);
		}
	}
}