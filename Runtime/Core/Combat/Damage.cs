using System;
using Core.Character;
using Core.Enums;
using RiptideNetworking;

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

		public Damage(Message from)
		{
			Attack = new Attack(from);
			Amount = from.GetFloat();
			Type = (DamageType)from.GetUShort();
			OnDamageInflicted = null;
			OnDamageDeflected = null;
		}

		public Message Serialize(Message message)
		{
			message = Attack.Serialize(message);
			message.AddFloat(Amount);
			message.AddUShort((ushort)Type);
			return message;
		}
	}
}