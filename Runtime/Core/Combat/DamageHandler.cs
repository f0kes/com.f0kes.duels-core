﻿using System;
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
		}

		public void InitiateDamage(Damage damage)
		{
			_damages.Add(damage);
			OnDamageInitiated.Invoke(new DamageEventArgs(damage));
			if (!damage.IsDeflected)
			{
				DealDamage(damage);
			}
			else
			{
				Debug.Log("Damage deflected");
			}
		}

		public void DeflectDamage(Damage damage)
		{
			if (!_damages.Exists(damage1 => damage1.Id == damage.Id)) return;
			OnDamageDeflected.Invoke(new DamageEventArgs(damage));
			damage.Deflect();
		}

		public void DealDamage(Damage damage)
		{
			_damagableResource.SubtractValue(damage.Amount);
			OnDamageDealt.Invoke(new DamageEventArgs(damage));
			_damages.Remove(damage);
		}
	}
}