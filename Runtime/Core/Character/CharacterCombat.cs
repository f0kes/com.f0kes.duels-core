using System;
using System.Collections.Generic;
using System.Linq;
using Core.Combat;
using Core.CoreEnums;
using Core.Enums;
using Core.Events;
using Core.Stats;
using UnityEngine;

namespace Core.Character
{
	public class CharacterCombat : MonoBehaviour
	{
		public Action<Damage> OnAttack;
		public Action<Weapon> OnWeaponChange;

		[SerializeField] private WeaponObject[] _weaponObjects = new WeaponObject[6];
		private Weapon[] _weapons = new Weapon[6];
		private Weapon _currentWeapon;
		private int _currentWeaponIndex = 0;

		private StatDict<AttributeStat> _attributes = new StatDict<AttributeStat>();
		private StatDict<BasedStat> _stats = new StatDict<BasedStat>();

		private ushort _entityId;

		public void Init(ushort entityId, StatDict<AttributeStat> characterAttributes,
			StatDict<BasedStat> characterStats)
		{
			_entityId = entityId;
			_attributes = characterAttributes;
			_stats = characterStats;
			int i = 0;
			foreach (var weaponObject in _weaponObjects)
			{
				if (weaponObject == null)
				{
					continue;
				}

				Weapon weapon = new Weapon(weaponObject, characterAttributes, characterStats);
				AddWeapon(weapon, i);
				i++;
			}

			if (i > 0)
				ChangeWeapon(0);

			
			EventTrigger.I[_entityId, ActionType.OnAttackStarted].Subscribe(Attack, true);
		}

		public Weapon GetWeapon()
		{
			return _currentWeapon;
		}

		public void AddWeapon(Weapon weapon, int index)
		{
			_weapons[index] = weapon;
			weapon.Equip(_attributes, _stats);
		}

		public void ChangeWeapon(int index)
		{
			Debug.Log("ChangeWeapon: " + index);
			if (_weapons[index] == null || _currentWeapon == _weapons[index])
				return;

			_currentWeaponIndex = index;
			_currentWeapon = _weapons[index];
			OnWeaponChange?.Invoke(_currentWeapon);
		}

		

		public void Attack(TriggerEventArgs args)
		{
			if (_currentWeapon == null)
				return;
			Damage damage = _currentWeapon.GetAttackDamage();
			OnAttackStarted(damage);
		}

		private void OnAttackStarted(Damage damage)
		{
			if(damage.Attack==null)
				return;
			OnAttack?.Invoke(damage);
			Collider[] colliders = Physics.OverlapSphere(transform.position, damage.Attack.AttackRange);
			foreach (Collider col in colliders)
			{
				CharacterEntity target = col.GetComponent<CharacterEntity>();
				if (target != null && IsCharacterAttackable(target))
				{
					EventTrigger.I[target, ActionType.HitTaken]
						.Invoke(new DamageEventArgs(damage));
				}
			}
		}

		private bool IsCharacterAttackable(CharacterEntity characterEntity)
		{
			return characterEntity.gameObject != gameObject;
		}
	}
}