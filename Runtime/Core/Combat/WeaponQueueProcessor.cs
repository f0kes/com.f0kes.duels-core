using System;
using System.Collections.Generic;
using System.Linq;
using Core.Character;
using Core.CoreEnums;
using Core.StatResource;
using UnityEngine;

namespace Core.Combat
{
	public class WeaponQueueProcessor
	{
		private readonly VictimGetter _victimGetter;
		private readonly Queue<Attack> _attackQueue = new Queue<Attack>();
		private readonly List<Entity> _pendingVictims = new List<Entity>();
		private readonly CombatStateContainer _combatStateContainer;
		private readonly Entity _wielder;
		private readonly ResourceContainer _stamina;

		private float CurrentAttackTime => _combatStateContainer.CurrentAttackTime;

		public WeaponQueueProcessor(VictimGetter victimGetter, Entity wielder, ResourceContainer stamina,
			CombatStateContainer container)
		{
			_victimGetter = victimGetter;
			_wielder = wielder;
			_stamina = stamina;
			_combatStateContainer = container;
		}

		public void EnqueueAttack(Attack toEnqueue, Attack lastAttack)
		{
			if (_attackQueue.Contains(lastAttack))
			{
				return;
			}

			_attackQueue.Enqueue(toEnqueue);
		}

		public void Tick()
		{
			_combatStateContainer.Tick();

			ProcessAttackQueue();
		}

		private void ProcessAttackQueue()
		{
			if (_attackQueue.Count == 0)
				return;

			switch (_combatStateContainer.CurrentState)
			{
				case CombatState.PreparingAttack:
					PrepareAttack();
					break;
				case CombatState.Attacking:
					PerformAttack();
					break;
				case CombatState.Cooldown:
					Cooldown();
					break;
				case CombatState.Idle:
					Idle();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void PrepareAttack()
		{
			var prepareTime = _combatStateContainer.CurrentAttack.PreparationTime;
			if (CurrentAttackTime >= prepareTime)
			{
				_pendingVictims.Clear();
				_stamina.SubtractValue(_combatStateContainer.CurrentAttack.StaminaCost);
				_combatStateContainer.ChangeState(CombatState.Attacking);
			}
		}

		private void PerformAttack()
		{
			var attackTime = _combatStateContainer.CurrentAttack.AttackTime;
			if (CurrentAttackTime >= attackTime)
			{
				//_combatStateContainer.CurrentAttack.SpellAction.Perform(_pendingVictims, _wielder, _combatStateContainer.CurrentAttack);
				_combatStateContainer.ChangeState(CombatState.Cooldown);
			}
			else
			{
				var victims = _victimGetter.GetVictims(_wielder, _combatStateContainer.CurrentAttack,
					_wielder.transform.position);
				foreach (var victim in victims.Where(victim => !_pendingVictims.Contains(victim)))
				{
					_combatStateContainer.CurrentAttack.SpellAction.Perform(victim, _wielder,
						_combatStateContainer.CurrentAttack);
					_pendingVictims.Add(victim);
				}
			}
		}

		private void Cooldown()
		{
			float cooldownTime = _combatStateContainer.CurrentAttack.CooldownTime;
			if (CurrentAttackTime >= cooldownTime)
			{
				_combatStateContainer.ChangeState(CombatState.Idle);
			}
		}

		private void Idle()
		{
			_combatStateContainer.ChangeState(CombatState.PreparingAttack, _attackQueue.Dequeue());
		}
	}
}