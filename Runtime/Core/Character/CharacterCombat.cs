using System.Collections.Generic;
using System.Linq;
using Core.Combat;
using Core.Enums;
using Core.Stats;
using UnityEngine;

namespace Core.Character
{
	public class CharacterCombat : MonoBehaviour
	{
		[SerializeField] private WeaponObject[] _weaponObjects = new WeaponObject[6];
		private Weapon[] _weapons = new Weapon[6];
		private Weapon _currentWeapon;
		private int _currentWeaponIndex = 0;

		private StatDict<AttributeStat> _attributes = new StatDict<AttributeStat>();
		private StatDict<BasedStat> _stats = new StatDict<BasedStat>();

		public void Init(StatDict<AttributeStat> characterAttributes, StatDict<BasedStat> characterStats)
		{
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
			if (_currentWeapon != null)
			{
				_currentWeapon.OnAttackStarted -= OnAttackStarted;
			}

			_currentWeaponIndex = index;
			_currentWeapon = _weapons[index];
			_currentWeapon.OnAttackStarted += OnAttackStarted;
		}

		public void TryAttack()
		{
			if (_currentWeapon == null)
				return;
			Debug.Log("TryAttack");
			_currentWeapon.TryAttack();
		}

		private void OnAttackStarted(Damage damage)
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, damage.Attack.AttackRange);
			foreach (Collider col in colliders)
			{
				CharacterEntity characterEntity = col.GetComponent<CharacterEntity>();
				if (characterEntity != null && IsCharacterAttackable(characterEntity))
				{
					characterEntity.TakeDamage(damage);
				}
			}
		}

		private bool IsCharacterAttackable(CharacterEntity characterEntity)
		{
			return characterEntity.gameObject != gameObject;
		}
	}
}