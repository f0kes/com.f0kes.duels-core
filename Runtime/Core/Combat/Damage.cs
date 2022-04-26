using System;
using Core.Character;
using Core.Enums;
using Core.Interfaces;
using Core.Types;
using RiptideNetworking;

namespace Core.Combat
{
	public class Damage 
	{
		public Attack Attack { get; private set; }
		public float Amount { get; private set; }
		public DamageType Type { get; private set; }
		public bool IsDeflected { get; private set; }

		public Damage(float amount, DamageType type, Attack attack)
		{
			this.Amount = amount;
			this.Type = type;
			this.Attack = attack;
		}

		public Damage(Message from)
		{
			Attack = new Attack(from);
			Amount = from.GetFloat();
			Type = (DamageType) from.GetUShort();
		}

		public Message Serialize(Message message)
		{
			message = Attack.Serialize(message);
			message.AddFloat(Amount);
			message.AddUShort((ushort) Type);
			return message;
		}

		public void Deflect()
		{
			IsDeflected = true;
		}
		
	}
}