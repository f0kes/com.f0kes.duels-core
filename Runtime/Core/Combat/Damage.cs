using System;
using Core.Character;
using Core.Enums;

namespace Core.Combat
{
	public struct Damage
	{
		public Action<CharacterEntity> OnDamageInflicted;
		public Action<Weapon> OnDamageDeflected;
		public Attack Attack{ get; private set; }
		public  float Amount{ get; private set; }
		public  DamageType Type{ get; private set; }
		public Damage(float amount, DamageType type, Attack attack)
		{
			this.Amount = amount;
			this.Type = type;
			this.Attack = attack;
			OnDamageInflicted = null;
			OnDamageDeflected = null;
		}
	}
}