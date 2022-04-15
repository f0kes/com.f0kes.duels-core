﻿using Core.CoreEnums;
using Core.Enums;
using RiptideNetworking;

namespace Core.Combat
{
	[System.Serializable]
	public class Attack
	{
		public DamageType DamageType;
		public AttackType AttackName;
		public int DamageModifier;
		public float AttackTime;
		public float AttackRange;
		public float StaminaCost;

		public Attack(Attack fromAttack)
		{
			DamageType = fromAttack.DamageType;
			AttackName = fromAttack.AttackName;
			DamageModifier = fromAttack.DamageModifier;
			AttackTime = fromAttack.AttackTime;
			AttackRange = fromAttack.AttackRange;
			StaminaCost = fromAttack.StaminaCost;
		}

		public Attack(Message fromMessage)
		{
			DamageType = (DamageType)fromMessage.GetUShort();
			AttackName = (AttackType)fromMessage.GetUShort();
			DamageModifier = fromMessage.GetInt();
			AttackTime = fromMessage.GetFloat();
			AttackRange = fromMessage.GetFloat();
			StaminaCost = fromMessage.GetFloat();
		}

		public Message Serialize(Message message)
		{
			message.AddUShort((ushort)DamageType);
			message.AddUShort((ushort)AttackName);
			message.AddInt(DamageModifier);
			message.AddFloat(AttackTime);
			message.AddFloat(AttackRange);
			message.AddFloat(StaminaCost);
			return message;
		}
	}
}