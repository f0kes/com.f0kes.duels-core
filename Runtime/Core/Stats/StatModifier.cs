using System.IO;
using Core.Interfaces;
using RiptideNetworking;

namespace Core.Stats
{
	public class StatModifier : ISerializableData
	{
		public float Value { get; set; }

		public string Name => GetType().ToString();
		public float priority;

		public StatModifier()
		{
			
		}
		
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

		public Message Serialize(Message message)
		{
			message.AddFloat(Value);
			message.AddFloat(priority);
			return message;
		}

		public void Deserialize(Message message)
		{
			Value = message.GetFloat();
			priority = message.GetFloat();
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

		public StatModifierMultiplyStat()
		{
			
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += Value * _basedUpon.GetValue();
		}

		public override void Serialize(BinaryWriter bw)
		{
			base.Serialize(bw);
		}

		public override void Deserialize(BinaryReader br)
		{
			base.Deserialize(br);
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