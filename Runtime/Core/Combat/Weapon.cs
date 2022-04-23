using System;
using System.Collections.Generic;
using Core.Character;
using Core.CoreEnums;
using Core.Enums;
using Core.Events;
using Core.Interfaces;
using Core.Stats;
using RiptideNetworking;
using UnityEngine;

namespace Core.Combat
{
	public class Weapon
	{
		public enum WeaponState : ushort
		{
			PreparingAttack,
			Attacking,
			Cooldown,
			Idle
		}

		public Action<float> OnStaminaChanged;
		public Action OnBreak;

		private Queue<Attack> _attackQueue = new Queue<Attack>();


		public WeaponType WeaponType { get; private set; }
		public WeaponName WeaponName { get; private set; }


		private List<Attack> Attacks;
		private Attack _currentAttack;
		private WeaponState _state;

		private int _currentAttackCount = 0;
		private float _currentAttackTime = 0;

		private List<Entity> _pendingVictims = new List<Entity>();


		private bool _isBroken;


		private StatDict<BasedStat> _playerStats;
		private Entity _wielder;

		public Stat MaxStamina { get; private set; }
		private float _currentStaminaPercent = 0;
		public float CurrentStamina => _currentStaminaPercent * MaxStamina.GetValue();
		public bool IsBroken => _isBroken;


		public Weapon(WeaponObject weaponObject,
			StatDict<BasedStat> playerStats)
		{
			_playerStats = playerStats;
			WeaponType = weaponObject.Type;

			Attacks = new List<Attack>();
			WeaponName = weaponObject.WeaponName;
			foreach (var attack in weaponObject.Attacks)
			{
				Attacks.Add(new Attack(attack));
			}

			Init();
		}

		public void Equip(
			StatDict<BasedStat> playerStats, Entity wielder)
		{
			_wielder = wielder;
			_playerStats = playerStats;
			_currentStaminaPercent = 1;
			MaxStamina = _playerStats.GetStat(BasedStat.Stamina);
		}


		public void Init()
		{
			Ticker.OnTick += OnTick;
		}

		private void OnTick(ushort obj)
		{
			_currentAttackTime += Time.fixedDeltaTime;
			ProcessAttackQueue();
		}


		public void EnqueueAttack()
		{
			if (!CanAttack())
			{
				return;
			}

			Attack toEnqueue = GetNextAttack();
			EventTrigger.I[_wielder, ActionType.OnAttackStarted].Invoke(new AttackEventArgs(toEnqueue));
			_attackQueue.Enqueue(toEnqueue);
		}

		private void ProcessAttackQueue()
		{
			if (_attackQueue.Count == 0)
				return;

			switch (_state)
			{
				case WeaponState.PreparingAttack:
					PrepareAttack();
					break;
				case WeaponState.Attacking:
					PerformAttack();
					break;
				case WeaponState.Cooldown:
					Cooldown();
					break;
				case WeaponState.Idle:
					Idle();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void PrepareAttack()
		{
			var prepareTime = _currentAttack.PreparationTime;
			if (_currentAttackTime >= prepareTime)
			{
				_pendingVictims.Clear();
				_state = WeaponState.Attacking;
				_currentAttackTime = 0;
				SubtractStamina(_currentAttack.StaminaCost);
			}
		}

		private void PerformAttack()
		{
			var attackTime = _currentAttack.AttackTime;
			if (_currentAttackTime >= attackTime)
			{
				_state = WeaponState.Cooldown;
				_currentAttackTime = 0;
				_currentAttack.SpellAction.Perform(_pendingVictims, _wielder, _currentAttack);
			}
			else
			{
				List<Entity> victims = GetVictims(_wielder, _currentAttack, _wielder.transform.position);
				_pendingVictims.AddRange(victims);
			}
		}

		private void Cooldown()
		{
			float cooldownTime = _currentAttack.CooldownTime;
			if (_currentAttackTime >= cooldownTime)
			{
				_state = WeaponState.Idle;
				_currentAttackTime = 0;
				_attackQueue.Dequeue();
			}
		}

		private void Idle()
		{
			_currentAttack = _attackQueue.Peek();
			_state = WeaponState.PreparingAttack;
		}

		private List<Entity> GetVictims(Entity attacker, Attack attack, Vector3 position)
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

		private bool IsTargetAttackable(ushort attacker, ushort target)
		{
			return attacker != target;
		}

		public void SubtractStamina(float stamina)
		{
			float newStamina = CurrentStamina - stamina;
			_currentStaminaPercent = newStamina / MaxStamina.GetValue();
			OnStaminaChanged?.Invoke(_currentStaminaPercent);
			if (_currentStaminaPercent <= 0)
			{
				OnBreak?.Invoke();
			}
		}


		private Attack GetNextAttack(bool setCurrentCount = true)
		{
			int nextAttackCount = (_currentAttackCount + 1) % Attacks.Count;
			Attack result = Attacks[nextAttackCount];
			if (setCurrentCount)
			{
				_currentAttackCount = nextAttackCount;
			}

			return result;
		}

		private bool CanAttack()
		{
			var staminaCost = GetNextAttack(false).StaminaCost;
			if (CurrentStamina <= 0)
			{
				return false;
			}

			return !_isBroken;
		}


		public Message Serialize(Message message)
		{
			message.AddUShort((ushort) WeaponType);
			return message;
		}

		public void Break()
		{
			_isBroken = true;
			OnBreak?.Invoke();
		}
	}
}