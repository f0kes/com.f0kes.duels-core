using Core.Enums;

namespace Core.Combat
{
	[System.Serializable]
	public class Attack
	{
		public DamageType DamageType;
		public string AttackName;
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
	}
}