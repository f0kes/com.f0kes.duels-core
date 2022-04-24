using System.Collections.Generic;
using Core.Character;
using UnityEngine;

namespace Core.Combat
{
	public class VictimGetter
	{
		public List<Entity> GetVictims(Entity attacker, Attack attack, Vector3 position)
		{
			//OnAttack?.Invoke(damage);
			List<Entity> victims = new List<Entity>();
			Collider[] colliders = Physics.OverlapSphere(position, attack.AttackRange);
			foreach (Collider col in colliders)
			{
				Entity target = col.GetComponent<Entity>();
				if (target != null && IsTargetAttackable(attacker, target))
				{
					victims.Add(target);
				}
			}

			return victims;
		}

		private bool IsTargetAttackable(Entity attacker, Entity target)
		{
			return attacker != target;
		}
	}
}