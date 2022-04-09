using System.IO;

namespace Core.Stats
{
	public class StatModifier : ISerializableData
	{
		public float Value { get; set; }

		public string Name => GetType().ToString();
		public float priority;
		
		public StatModifier(float value, float priority)
		{
			this.Value = value;
			this.priority = priority;
		}

		public virtual void ApplyMod(ref float finalStat, float baseValue)
		{
			//	finalStat = finalStat;
		}

		public virtual void Serialize(BinaryWriter bw)
		{
			bw.Write(Value);
			bw.Write(priority);
		}

		public virtual void Deserialize(BinaryReader br)
		{
			Value = br.ReadSingle();
			priority = br.ReadSingle();
		}
	}

	public class StatModifierAdd : StatModifier
	{
		public StatModifierAdd(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += Value;
		}
	}

	public class StatModifierMultiplyStat : StatModifier
	{
		private Stat _basedUpon;

		public StatModifierMultiplyStat(float value, float priority, Stat basedUpon) : base(value, priority)
		{
			_basedUpon = basedUpon;
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += Value * _basedUpon.GetValue();
		}
	}

	public class StatModifierMultiply : StatModifier
	{
		public StatModifierMultiply(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat *= Value;
		}
	}

	public class StatMultiplyBase : StatModifier
	{
		public StatMultiplyBase(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += (baseValue * Value) - baseValue;
		}
	}

	public class StatMultiplyBasePositive : StatModifier
	{
		public StatMultiplyBasePositive(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += baseValue * (1 + Value) - baseValue;
		}
	}
}