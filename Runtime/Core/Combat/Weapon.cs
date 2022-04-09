using System;
using System.Collections.Generic;
using Core.Enums;
using Core.Stats;
using UnityEngine;

namespace Core.Combat
{
	public class Weapon 
	{
		public Action<Damage> OnAttackStarted;
		
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
						return  ElementType.Blue;
					case AttributeStat.Agility:
					case AttributeStat.Nature:
					default:
						return  ElementType.Green;
				}
			}
		}

		private List<Attack> Attacks;
		private AttributeStat _baseAttribute;
		private Attack _currentAttack;

		private int _currentAttackCount = 0;
		private float _currentAttackTime = 0;

		private StatDict<AttributeStat> _playerAttributes;
		private float PlayerDamage => _playerStats[BasedStat.Damage];
		private StatDict<BasedStat> _playerStats;

		private float _currentStamina = 0;

		public Weapon (AttributeStat baseAttribute, List<Attack> attacks, StatDict<AttributeStat> playerAttributes, StatDict<BasedStat> playerStats)
		{
			_baseAttribute = baseAttribute;
			Attacks = attacks;
			_playerAttributes = playerAttributes;
			_playerStats = playerStats;
		}

		public Weapon(WeaponObject weaponObject, StatDict<AttributeStat> playerAttributes, StatDict<BasedStat> playerStats)
		{
			_baseAttribute = weaponObject.BaseAttribute;
			Attacks = new List<Attack>();
			foreach (var attack in weaponObject.Attacks)
			{
				Attacks.Add(new Attack(attack));
			}
			_playerAttributes = playerAttributes;
			_playerStats = playerStats;
		}
		


		public void Equip( StatDict<AttributeStat> playerAttributes,
			StatDict<BasedStat> playerStats)
		{
			_playerAttributes = playerAttributes;
			_playerStats = playerStats;
			_currentStamina = playerStats.GetStat(BasedStat.Stamina).BaseValue;
		}


		public void TryAttack()
		{
			if (!CanAttack())
			{
				_currentAttackTime += Time.deltaTime;
				return;
			}

			_currentAttack = GetNextAttack();
			_currentStamina -= _currentAttack.StaminaCost;
			_currentAttackTime = 0;
			
			Damage newDamage = GetDamage(_currentAttack);
			newDamage.OnDamageDeflected += OnDamageDeflected;
			OnAttackStarted?.Invoke(newDamage);
		}

		private Attack GetNextAttack()
		{
			_currentAttackCount = _currentAttackCount++ % Attacks.Count;
			return Attacks[_currentAttackCount];
		}

		private bool CanAttack()
		{
			float staminaCost = Attacks[_currentAttackCount + 1].StaminaCost;
			if (_currentStamina < staminaCost)
			{
				return false;
			}

			return _currentAttack == null || _currentAttackTime >= _currentAttack.AttackTime;
		}

		private void OnDamageDeflected(Weapon other)
		{
			float damageToOtherWeaponStamina = PlayerDamage * _currentAttack.DamageModifier;
			float damageMultiplier =1;
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
			other._currentStamina-= damageToOtherWeaponStamina;

		}

		private Damage GetDamage(Attack withAttack)
		{
			var damage = new Damage(CalculateDamage(withAttack), withAttack.DamageType, withAttack);


			return damage;
		}

		private float CalculateDamage(Attack attack)
		{
			return PlayerDamage * attack.DamageModifier;
		}
	}
}