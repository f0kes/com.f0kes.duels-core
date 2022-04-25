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
		private float _currentAttackTime = 0;
		private readonly Entity _wielder;
		private readonly ResourceContainer _stamina;

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
			_currentAttackTime += Time.fixedDeltaTime;
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
			if (_currentAttackTime >= prepareTime)
			{
				_pendingVictims.Clear();
				_currentAttackTime = 0;
				_stamina.SubtractValue(_combatStateContainer.CurrentAttack.StaminaCost);
				_combatStateContainer.ChangeState(CombatState.Attacking);
			}
		}

		private void PerformAttack()
		{
			var attackTime = _combatStateContainer.CurrentAttack.AttackTime;
			if (_currentAttackTime >= attackTime)
			{
				_currentAttackTime = 0;
				//_combatStateContainer.CurrentAttack.SpellAction.Perform(_pendingVictims, _wielder, _combatStateContainer.CurrentAttack);
				_combatStateContainer.ChangeState(CombatState.Cooldown);
			}
			else
			{
				var victims = _victimGetter.GetVictims(_wielder, _combatStateContainer.CurrentAttack, _wielder.transform.position);
				foreach (var victim in victims.Where(victim => !_pendingVictims.Contains(victim)))
				{
					_combatStateContainer.CurrentAttack.SpellAction.Perform(victim, _wielder, _combatStateContainer.CurrentAttack);
					_pendingVictims.Add(victim);
				}
			}
		}

		private void Cooldown()
		{
			float cooldownTime = _combatStateContainer.CurrentAttack.CooldownTime;
			if (_currentAttackTime >= cooldownTime)
			{
				_currentAttackTime = 0;
				_attackQueue.Dequeue();
				_combatStateContainer.ChangeState(CombatState.Idle);
			}
		}

		private void Idle()
		{
			_currentAttackTime = 0;
			_combatStateContainer.ChangeState(CombatState.PreparingAttack,_attackQueue.Peek());
		}
	}
}