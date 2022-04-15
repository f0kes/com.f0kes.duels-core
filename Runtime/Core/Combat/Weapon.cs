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

		private float _currentStamina = 0;


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
			_currentStamina = playerStats.GetStat(BasedStat.Stamina).BaseValue;
		}

		

		public Damage GetAttackDamage()
		{
			Damage damage = new Damage();
			if (!CanAttack())
			{
				return damage;
			}

			_currentAttack = GetNextAttack();
			_currentStamina -= _currentAttack.StaminaCost;
			_currentAttackTime = 0;


			damage = GetDamage(_currentAttack);
			damage.OnDamageDeflected += OnDamageDeflected;

			return damage;
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
			if (_currentStamina < staminaCost)
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
			other._currentStamina -= damageToOtherWeaponStamina;
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

		public Message Serialize(Message message)
		{
			message.AddUShort((ushort) WeaponType);
			return message;
		}
	}
}