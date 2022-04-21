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
		public Action<float> OnStaminaChanged;

		public ElementType ElementType
		{
			get
			{
				switch (_baseAttribute)
				{
					case AttributeStat.Strength:
					case AttributeStat.Darkness:
						return ElementType.Red;
					case AttributeStat.Intellect:
					case AttributeStat.Light:
						return ElementType.Blue;
					case AttributeStat.Agility:
					case AttributeStat.Nature:
					default:
						return ElementType.Green;
				}
			}
		}

		public WeaponType WeaponType { get; private set; }
		public WeaponName WeaponName { get; private set; }


		private List<Attack> Attacks;
		private AttributeStat _baseAttribute;
		private Attack _currentAttack;

		private int _currentAttackCount = 0;
		private float _currentAttackTime = 0;

		private StatDict<AttributeStat> _playerAttributes;
		private float PlayerDamage => _playerStats[BasedStat.Damage];
		private StatDict<BasedStat> _playerStats;

		public Stat MaxStamina { get; private set; }
		private float _currentStaminaPercent = 0;
		public float CurrentStamina => _currentStaminaPercent * MaxStamina.GetValue();


		public Weapon(WeaponObject weaponObject, StatDict<AttributeStat> playerAttributes,
			StatDict<BasedStat> playerStats)
		{
			_playerAttributes = playerAttributes;
			_playerStats = playerStats;
			WeaponType = weaponObject.Type;
			_baseAttribute = weaponObject.BaseAttribute;
			Attacks = new List<Attack>();
			WeaponName = weaponObject.WeaponName;
			foreach (var attack in weaponObject.Attacks)
			{
				Attacks.Add(new Attack(attack));
			}

			Init();
		}


		public void Init()
		{
			Ticker.OnTick += OnTick;
		}

		private void OnTick(ushort obj)
		{
			_currentAttackTime += Time.fixedDeltaTime;
		}

		public void Equip(StatDict<AttributeStat> playerAttributes,
			StatDict<BasedStat> playerStats)
		{
			_playerAttributes = playerAttributes;
			_playerStats = playerStats;
			_currentStaminaPercent = 1;
			MaxStamina = _playerStats.GetStat(BasedStat.Stamina);
		}


		public void Attack(Entity attacker, Vector3 position)
		{
			if (!CanAttack())
			{
				return;
			}

			_currentAttack = GetNextAttack();
			SubtractStamina(_currentAttack.StaminaCost);
			_currentAttackTime = 0;
			
			EventTrigger.I[attacker, ActionType.OnAttackStarted].Invoke(new EmptyEventArgs());
			List<Entity> victims = GetVictims(attacker, _currentAttack, position);
			_currentAttack.SpellAction.Perform(victims, attacker, _currentAttack);
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
			float staminaCost = GetNextAttack(false).StaminaCost;
			if (CurrentStamina <= 0)
			{
				return false;
			}

			return _currentAttack == null || _currentAttackTime >= _currentAttack.AttackTime;
		}

		private void OnDamageDeflected(Weapon other)
		{
			float damageToOtherWeaponStamina = PlayerDamage * _currentAttack.DamageModifier;
			float damageMultiplier = 1;
			//blue stronger than red, red stronger than green, green stronger than blue
			if (ElementType == ElementType.Red && other.ElementType == ElementType.Green)
			{
				damageMultiplier = 2;
			}
			else if (ElementType == ElementType.Blue && other.ElementType == ElementType.Red)
			{
				damageMultiplier = 2;
			}
			else if (ElementType == ElementType.Green && other.ElementType == ElementType.Blue)
			{
				damageMultiplier = 2;
			}

			damageToOtherWeaponStamina *= damageMultiplier;
			other.SubtractStamina(damageToOtherWeaponStamina);
		}

		public Message Serialize(Message message)
		{
			message.AddUShort((ushort) WeaponType);
			return message;
		}
	}
}