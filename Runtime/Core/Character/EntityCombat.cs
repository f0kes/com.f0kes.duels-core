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
	public class EntityCombat : MonoBehaviour
	{
		public Action<Damage> OnAttack;
		public Action<Weapon[]> OnWeaponsChanged;
		public Action<ushort, byte> OnWeaponChange;

		[SerializeField] private WeaponObject[] _weaponObjects = new WeaponObject[6];
		[SerializeField] private WeaponObject _bareHands;
		private Weapon[] _weapons = new Weapon[6];
		private Weapon _bareHandsWeapon;
		private Weapon _currentWeapon;


		private StatDict<AttributeStat> _attributes = new StatDict<AttributeStat>();
		private StatDict<BasedStat> _stats = new StatDict<BasedStat>();

		private int _weaponChangeTokens = 5;

		private ushort _entityId;

		public void Init(Entity entity, StatDict<AttributeStat> characterAttributes,
			StatDict<BasedStat> characterStats)
		{
			_entityId = entity;
			_attributes = characterAttributes;
			_stats = characterStats;
			int i = 0;
			foreach (var weaponObject in _weaponObjects)
			{
				if (weaponObject == null)
				{
					continue;
				}

				Weapon weapon = new Weapon(weaponObject, characterStats);
				AddWeapon(weapon, i);
				i++;
			}

			_bareHandsWeapon = new Weapon(_bareHands, characterStats);

			if (i > 0)
				ChangeWeapon(0);
			else
			{
				SetWeapon(_bareHandsWeapon);
			}


			EventTrigger.I[_entityId, ActionType.OnAttackStarted].Subscribe(Attack, true);
		}

		public Weapon GetWeapon()
		{
			return _currentWeapon;
		}

		public Weapon[] GetWeapons()
		{
			return new List<Weapon>(_weapons).ToArray();
		}

		public void AddWeapon(Weapon weapon, int index)
		{
			_weapons[index] = weapon;
			weapon.Equip(_stats);
			OnWeaponsChanged?.Invoke(GetWeapons());
		}

		public void RemoveWeapon(int index)
		{
			_weapons[index] = null;
			OnWeaponsChanged?.Invoke(GetWeapons());
		}

		public void ChangeWeapon(byte index, bool useToken = false)
		{
			if (index == byte.MaxValue)
			{
				OnWeaponBreak();
			}

			if (_weapons[index] == null || _currentWeapon == _weapons[index])
				return;

			if (useToken)
			{
				if (_weaponChangeTokens <= 0)
				{
					_currentWeapon.Break();
				}

				_weaponChangeTokens--;
			}

			Weapon toSet = _weapons[index];
			SetWeapon(toSet);

			EventTrigger.I[_entityId, ActionType.OnWeaponChanged].Invoke(new WeaponChangeEventArgs(index));
			OnWeaponChange?.Invoke(_entityId, index);
		}

		private void SetWeapon(Weapon weapon)
		{
			if (_currentWeapon != null)
			{
				_currentWeapon.OnBreak -= OnWeaponBreak;
			}

			_currentWeapon = weapon;
			_currentWeapon.OnBreak += OnWeaponBreak;
		}

		private void OnWeaponBreak()
		{
			SetWeapon(_bareHandsWeapon);
			OnWeaponChange?.Invoke(_entityId, byte.MaxValue);
			EventTrigger.I[_entityId, ActionType.OnWeaponChanged].Invoke(new WeaponChangeEventArgs(byte.MaxValue));
			_weaponChangeTokens++;
		}


		public void Attack(TriggerEventArgs args)
		{
			if (_currentWeapon == null)
				return;
			_currentWeapon.Attack(_entityId, transform.position);
		}


		private bool IsCharacterAttackable(Entity entity)
		{
			return entity.gameObject != gameObject;
		}
	}
}