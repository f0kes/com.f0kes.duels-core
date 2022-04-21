using Combat;
using Core.CoreEnums;
using Core.Enums;
using RiptideNetworking;
using UnityEngine;

namespace Core.Combat
{
	[System.Serializable]
	public class Attack 
	{
		public SpellAction SpellAction;
		public DamageType DamageType;
		public AttackType AttackName;
		public int DamageModifier;
		
		public Vector2 Direction;
		
		public float AttackPreparationTime = 0.2f;
		public float AttackCooldownTime =0.1f;
		public float AttackTime;
		
		public float AttackRange;
		public float StaminaCost;

		public Attack(Attack fromAttack)
		{
			SpellAction = fromAttack.SpellAction;
			DamageType = fromAttack.DamageType;
			AttackName = fromAttack.AttackName;
			DamageModifier = fromAttack.DamageModifier;
			AttackTime = fromAttack.AttackTime;
			AttackRange = fromAttack.AttackRange;
			StaminaCost = fromAttack.StaminaCost;
			Direction = fromAttack.Direction;
		}

		public Attack(Message fromMessage)
		{
			DamageType = (DamageType)fromMessage.GetUShort();
			AttackName = (AttackType)fromMessage.GetUShort();
			DamageModifier = fromMessage.GetInt();
			AttackTime = fromMessage.GetFloat();
			AttackRange = fromMessage.GetFloat();
			StaminaCost = fromMessage.GetFloat();
			Direction = fromMessage.GetVector2();
		}

		public Message Serialize(Message message)
		{
			message.AddUShort((ushort)DamageType);
			message.AddUShort((ushort)AttackName);
			message.AddInt(DamageModifier);
			message.AddFloat(AttackTime);
			message.AddFloat(AttackRange);
			message.AddFloat(StaminaCost);
			message.AddVector2(Direction);
			return message;
		}
	}
}