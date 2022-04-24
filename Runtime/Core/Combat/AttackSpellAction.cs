using System.Collections.Generic;
using Combat;
using Core.Character;
using Core.CoreEnums;
using Core.Enums;
using Core.Events;
using UnityEngine;

namespace Core.Combat
{
	[CreateAssetMenu(fileName = "New EnqueueAttack Spell Action", menuName = "SpellActions/AttackSpellAction")]
	public class AttackSpellAction : SpellAction
	{
		public DamageType Type = DamageType.Physical;


		public override void Perform(List<Entity> victims, Entity caster, Attack attack)
		{
			if (victims.Count != 0)
			{
				foreach (var victim in victims)
				{
					DealDamage(victim, caster, attack);
				}

				if (Type == DamageType.Physical)
				{
					EventTrigger.I[new TriggerKey(caster, ActionType.HitLanded)]?.Invoke(new EmptyEventArgs());
				}
			}
			else if (Type == DamageType.Physical)
			{
				EventTrigger.I[new TriggerKey(caster, ActionType.HitMissed)]?.Invoke(new EmptyEventArgs());
			}


			base.Perform(victims, caster, attack);
		}

		protected virtual Damage CalculateDamage(Entity victim, Entity actor, Attack attack)
		{
			var damage = new Damage(actor.Stats[BasedStat.Damage] * attack.DamageModifier, attack.DamageType, attack);
			return damage;;
		}

		protected virtual void DealDamage(Entity victim, Entity actor, Attack attack)
		{
			var damage = CalculateDamage(victim, actor, attack);
			EventTrigger.I[victim, ActionType.HitTaken]
				.Invoke(new DamageEventArgs(damage));
		}
	}
}