using System;
using System.Collections.Generic;
using Core.Events;
using Core.Interfaces;
using Core.StatResource;
using Core.Types;
using UnityEngine;

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

			OnDamageDeflected.Subscribe(DeflectDamage, true);
			OnDamageInitiated.Subscribe(InitiateDamage, true);
			OnDamageDealt.Subscribe(DealDamage, true);
		}

		private void InitiateDamage(TriggerEventArgs args)
		{
			if (args is DamageEventArgs dArgs)
			{
				Damage damage = dArgs.Damage;
				_damages.Add(damage);
				if (!damage.IsDeflected)
				{
					OnDamageDealt.Invoke(dArgs);
				}
			}
		}

		private void DeflectDamage(TriggerEventArgs args)
		{
			Debug.Log("Deflecting damage");
			if (args is DamageEventArgs dArgs)
			{
				Debug.Log("Dargs");
				Damage damage = dArgs.Damage;
				if (!_damages.Contains(damage)) return;
				Debug.Log("Damage Deflected");
				damage.Deflect();
			}
		}

		private void DealDamage(TriggerEventArgs args)
		{
			if (args is DamageEventArgs dArgs)
			{
				Damage damage = dArgs.Damage;
				_damagableResource.SubtractValue(dArgs.Damage.Amount);
				_damages.Remove(damage);
			}
		}
	}
}