using System;
using System.Collections.Generic;
using Core.Combat;
using Core.CoreEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
	public class CoreGameAssets : MonoBehaviour
	{
		[Serializable]
		public struct WeaponObjectAlias
		{
			public WeaponName WeaponName;
			public WeaponObject WeaponObject;
		}
		
		private static CoreGameAssets _singleton;
		public static CoreGameAssets Singleton
		{
			get => _singleton;
			private set
			{
				if (_singleton == null)
					_singleton = value;
				else if (_singleton != value)
				{
					Debug.Log($"{nameof(CoreGameAssets)} instance already exists, destroying duplicate!");
					Destroy(value);
				}
			}
		}

		public Dictionary<WeaponName,WeaponObject> WeaponObjects = new Dictionary<WeaponName, WeaponObject>();

		[FormerlySerializedAs("playerPrefab")]
		[Header("Prefabs")]
		[SerializeField] private GameObject PlayerPrefab;
		[SerializeField] private List<WeaponObjectAlias> WeaponObjectAliases = new List<WeaponObjectAlias>();
		
		

		private void Awake()
		{
			Singleton = this;
			foreach (var objectAlias in WeaponObjectAliases)
			{
				WeaponObjects.Add(objectAlias.WeaponName, objectAlias.WeaponObject);
			}
		}
	}
}