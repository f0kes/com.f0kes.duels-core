using System.Collections.Generic;
using Core.Enums;

namespace Core.Stats
{
	public static class BasedStatBook
	{
		private struct AttributeDependency
		{
			public float BaseValue;
			public float BaseMultiplier;
			public Dictionary<AttributeStat, float> SpecificDependencies;

			public AttributeDependency(Dictionary<AttributeStat, float> specificDependencies, float baseMultiplier,
				float baseValue)
			{
				SpecificDependencies = specificDependencies;
				BaseMultiplier = baseMultiplier;
				BaseValue = baseValue;
			}
		}

		private static Dictionary<BasedStat, AttributeDependency> _dependencies =
			new Dictionary<BasedStat, AttributeDependency>();

		static BasedStatBook()
		{
			var armorDependencies = new Dictionary<AttributeStat, float>
			{
				[AttributeStat.Strength] = 1,
				[AttributeStat.Darkness] = 1,
				[AttributeStat.Nature] = 2
			};
			_dependencies[BasedStat.Armor] = new AttributeDependency(armorDependencies, 1, 5);

			var damageDependencies = new Dictionary<AttributeStat, float>
			{
				[AttributeStat.Intellect] = 1,
				[AttributeStat.Darkness] = 2,
				[AttributeStat.Agility] = 1
			};
			_dependencies[BasedStat.Damage] = new AttributeDependency(damageDependencies, 2, 10);

			var healthDependencies = new Dictionary<AttributeStat, float>
			{
				[AttributeStat.Strength] = 2,
				[AttributeStat.Light] = 1,
				[AttributeStat.Nature] = 1
			};
			_dependencies[BasedStat.Health] = new AttributeDependency(healthDependencies, 10, 100);

			var speedDependencies = new Dictionary<AttributeStat, float>
			{
				[AttributeStat.Intellect] = 1,
				[AttributeStat.Light] = 1,
				[AttributeStat.Agility] = 2
			};
			_dependencies[BasedStat.Speed] = new AttributeDependency(speedDependencies, 2, 1);

			var staminaDependencies = new Dictionary<AttributeStat, float>
			{
				[AttributeStat.Intellect] = 2,
				[AttributeStat.Strength] = 1,
				[AttributeStat.Darkness] = 1
			};
			_dependencies[BasedStat.Stamina] = new AttributeDependency(staminaDependencies, 10, 100);

			var attackSpeedDependencies = new Dictionary<AttributeStat, float>
			{
				[AttributeStat.Light] = 2,
				[AttributeStat.Agility] = 1,
				[AttributeStat.Nature] = 1
			};
			_dependencies[BasedStat.AttackSpeed] = new AttributeDependency(attackSpeedDependencies, 1, 5);
		}

		public static Stat GetBasedStat(BasedStat basedStatType, StatDict<AttributeStat> attributes)
		{
			AttributeDependency dependency = _dependencies[basedStatType];
			Stat basedStat = new Stat(dependency.BaseValue);
			foreach (var kv in dependency.SpecificDependencies)
			{
				Stat attributeStat = attributes.GetStat(kv.Key);
				StatModifierMultiplyStat specificDependency = new StatModifierMultiplyStat(kv.Value, 1, attributeStat);
				basedStat.AddMod(specificDependency);
			}

			StatModifierMultiply modifierMultiply = new StatModifierMultiply(dependency.BaseMultiplier, int.MaxValue);

			return basedStat;
		}
	}
}