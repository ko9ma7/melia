﻿using System;
using Melia.Channel.Network;
using Melia.Shared.Const;
using Melia.Shared.Util;
using Melia.Shared.World.ObjectProperties;

namespace Melia.Channel.World.Entities
{
	/// <summary>
	/// A player character's properties.
	/// </summary>
	public class CharacterProperties : Properties
	{
		/// <summary>
		/// Returns the owner of the properties.
		/// </summary>
		public Character Character { get; }

		/// <summary>
		/// Creates new instance for the character.
		/// </summary>
		/// <param name="character"></param>
		public CharacterProperties(Character character)
		{
			this.Character = character;
			this.AddDefaultProperties();
			this.InitEvents();
		}

		/// <summary>
		/// Sets up properties that every character has by default.
		/// </summary>
		public void AddDefaultProperties()
		{
			// We only need to set up properties that are calculated or
			// have min/max or non-default values. All others will be
			// created with default values as needed on demand.

			this.Add(new FloatProperty(PropertyId.PC.Lv, 1, min: 1));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.STR_ADD, this.GetSTR_ADD));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.STR, this.GetSTR));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.CON_ADD, this.GetCON_ADD));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.CON, this.GetCON));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.INT_ADD, this.GetINT_ADD));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.INT, this.GetINT));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.MNA_ADD, this.GetMNA_ADD));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MNA, this.GetMNA));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.DEX_ADD, this.GetDEX_ADD));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.DEX, this.GetDEX));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.MHP, this.GetMHP));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MSP, this.GetMSP));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MaxSta, this.GetMaxSta));

			// Don't set a max value initially, as that could cap the HP
			// during loading.
			this.Add(new FloatProperty(PropertyId.PC.HP, this.GetMHP(), min: 0));
			this.Add(new FloatProperty(PropertyId.PC.SP, this.GetMSP(), min: 0));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.RHP, this.GetRHP));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.RSP, this.GetRSP));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.RHPTIME, this.GetRHPTIME));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.RSPTIME, this.GetRSPTIME));

			this.Add(new FloatProperty(PropertyId.PC.StatByLevel, min: 0));
			this.Add(new FloatProperty(PropertyId.PC.StatByBonus, min: 0));
			this.Add(new FloatProperty(PropertyId.PC.UsedStat, min: 0));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.StatPoint, this.GetStatPoint));
			this.Add(new StringProperty(PropertyId.PC.AbilityPoint, "0")); // Why oh why did they make this a string >_>

			this.Add(new CalculatedFloatProperty(PropertyId.PC.MAXPATK, this.GetMAXPATK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MINPATK, this.GetMINPATK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MAXMATK, this.GetMAXMATK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MINMATK, this.GetMINMATK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MAXPATK_SUB, this.GetMAXPATK_SUB));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MINPATK_SUB, this.GetMINPATK_SUB));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.DEF, this.GetDEF));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.MDEF, this.GetMDEF));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.CRTATK, this.GetCRTATK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.CRTHR, this.GetCRTHR));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.CRTDR, this.GetCRTDR));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.HR, this.GetHR));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.DR, this.GetDR));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.BLK, this.GetBLK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.BLK_BREAK, this.GetBLK_BREAK));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.SR, this.GetSR));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.SDR, this.GetSDR));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.MaxWeight, this.GetMaxWeight));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.NowWeight, this.GetNowWeight));

			this.Add(new CalculatedFloatProperty(PropertyId.PC.MSPD, this.GetMSPD));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.JumpPower, this.GetJumpPower));
			this.Add(new CalculatedFloatProperty(PropertyId.PC.CastingSpeed, this.GetCastingSpeed));

			this.Add(new FloatProperty(PropertyId.PC.MovingShotable, 0));
			this.Add(new FloatProperty(PropertyId.PC.HPDrain, 2));
			this.Add(new FloatProperty(PropertyId.PC.BOOST, 1));
			this.Add(new FloatProperty(PropertyId.PC.Const, 1.909859f));
			this.Add(new FloatProperty(PropertyId.PC.CAST, 1));
			this.Add(new FloatProperty(PropertyId.PC.Sta_Jump, 1000));
		}

		/// <summary>
		/// Sets up auto updates for the default properties.
		/// </summary>
		/// <remarks>
		/// Call after all properties were loaded, as to not trigger
		/// auto-updates before all properties are in place.
		/// </remarks>
		public void InitAutoUpdates()
		{
			this.AutoUpdate(PropertyId.PC.STR, new[] { PropertyId.PC.Lv, PropertyId.PC.STR_ADD, PropertyId.PC.STR_STAT, PropertyId.PC.STR_JOB });
			this.AutoUpdate(PropertyId.PC.CON, new[] { PropertyId.PC.Lv, PropertyId.PC.CON_ADD, PropertyId.PC.CON_STAT, PropertyId.PC.CON_JOB });
			this.AutoUpdate(PropertyId.PC.INT, new[] { PropertyId.PC.Lv, PropertyId.PC.INT_ADD, PropertyId.PC.INT_STAT, PropertyId.PC.INT_JOB });
			this.AutoUpdate(PropertyId.PC.MNA, new[] { PropertyId.PC.Lv, PropertyId.PC.MNA_ADD, PropertyId.PC.MNA_STAT, PropertyId.PC.MNA_JOB });
			this.AutoUpdate(PropertyId.PC.DEX, new[] { PropertyId.PC.Lv, PropertyId.PC.DEX_ADD, PropertyId.PC.DEX_STAT, PropertyId.PC.DEX_JOB });
			this.AutoUpdate(PropertyId.PC.MHP, new[] { PropertyId.PC.Lv, PropertyId.PC.CON, PropertyId.PC.MHP_BM, PropertyId.PC.MHP_Bonus });
			this.AutoUpdate(PropertyId.PC.MSP, new[] { PropertyId.PC.Lv, PropertyId.PC.MNA, PropertyId.PC.MSP_BM, PropertyId.PC.MSP_Bonus });
			this.AutoUpdate(PropertyId.PC.StatPoint, new[] { PropertyId.PC.StatByLevel, PropertyId.PC.StatByBonus, PropertyId.PC.UsedStat });
			this.AutoUpdate(PropertyId.PC.MSPD, new[] { PropertyId.PC.MSPD_BM, PropertyId.PC.MSPD_Bonus });
			this.AutoUpdate(PropertyId.PC.CastingSpeed, new[] { PropertyId.PC.CastingSpeed_BM });
			this.AutoUpdate(PropertyId.PC.DEF, new[] { PropertyId.PC.Lv, PropertyId.PC.DEF_BM, PropertyId.PC.DEF_RATE_BM });
			this.AutoUpdate(PropertyId.PC.MDEF, new[] { PropertyId.PC.Lv, PropertyId.PC.MDEF_BM, PropertyId.PC.MDEF_RATE_BM });
			this.AutoUpdate(PropertyId.PC.CRTATK, new[] { PropertyId.PC.CRTATK_BM });
			this.AutoUpdate(PropertyId.PC.CRTHR, new[] { PropertyId.PC.Lv, PropertyId.PC.CRTHR_BM });
			this.AutoUpdate(PropertyId.PC.CRTDR, new[] { PropertyId.PC.Lv, PropertyId.PC.CRTDR_BM });
			this.AutoUpdate(PropertyId.PC.HR, new[] { PropertyId.PC.Lv, PropertyId.PC.STR, PropertyId.PC.HR_BM, PropertyId.PC.HR_RATE_BM });
			this.AutoUpdate(PropertyId.PC.DR, new[] { PropertyId.PC.Lv, PropertyId.PC.DEX, PropertyId.PC.DR_BM, PropertyId.PC.DR_RATE_BM });
			this.AutoUpdate(PropertyId.PC.BLK, new[] { PropertyId.PC.Lv, PropertyId.PC.CON, PropertyId.PC.BLK_BM, PropertyId.PC.BLK_RATE_BM });
			this.AutoUpdate(PropertyId.PC.BLK_BREAK, new[] { PropertyId.PC.Lv, PropertyId.PC.DEX, PropertyId.PC.BLK_BREAK_BM, PropertyId.PC.BLK_BREAK_RATE_BM });
			this.AutoUpdate(PropertyId.PC.SR, new[] { PropertyId.PC.SR_BM });
			this.AutoUpdate(PropertyId.PC.SDR, new[] { PropertyId.PC.SDR_BM });

			this.AutoUpdateMax(PropertyId.PC.HP, PropertyId.PC.MHP);
			this.AutoUpdateMax(PropertyId.PC.SP, PropertyId.PC.MSP);
		}

		/// <summary>
		/// Sets up event subscriptions, to react to actions of the
		/// character with property updates.
		/// </summary>
		private void InitEvents()
		{
			// Update recovery times when the character sits down,
			// as those properties are affected by the sitting status.
			this.Character.SitStatusChanged += this.UpdateRecoveryTimes;
		}

		/// <summary>
		/// Recalculates and updates HP and SP recovery time properties.
		/// </summary>
		/// <param name="character"></param>
		private void UpdateRecoveryTimes(Character character)
		{
			this.Calculate(PropertyId.PC.RHPTIME);
			this.Calculate(PropertyId.PC.RSPTIME);

			Send.ZC_OBJECT_PROPERTY(this.Character, PropertyId.PC.RHPTIME, PropertyId.PC.RSPTIME);
		}

		/// <summary>
		/// Returns the character's maximum HP.
		/// </summary>
		public float GetMHP()
		{
			var level = this.GetFloat(PropertyId.PC.Lv, 1);
			var stat = this.GetFloat(PropertyId.PC.CON, 1);

			var rateByJob = this.Character.Job?.Data.HpRate ?? 1;
			var byJob = Math.Floor(400 * rateByJob);

			var byLevel = Math.Floor(byJob + ((level - 1) * 80 * rateByJob));
			var byStat = Math.Floor(((stat * 0.003f) + (Math.Floor(stat / 10.0f) * 0.01f)) * byLevel);
			var byItem = this.Character.Inventory.GetEquipProperties(PropertyId.Item.MHP);
			var byItemRatio = (byLevel + byStat) * (this.Character.Inventory.GetEquipProperties(PropertyId.Item.MHPRatio) / 100f);
			var byBonus = this.GetFloat(PropertyId.PC.MHP_Bonus);

			var value = byLevel + byStat + byItem + byItemRatio + byBonus;

			var byBuffs = this.GetFloat(PropertyId.PC.MHP_BM);
			var byBuffRate = Math.Floor(value * this.GetFloat(PropertyId.PC.MHP_RATE_BM));

			value += byBuffs + byBuffRate;

			return (int)Math.Max(1, value);
		}

		/// <summary>
		/// Returns the character's maximum SP.
		/// </summary>
		public float GetMSP()
		{
			var level = this.GetFloat(PropertyId.PC.Lv, 1);
			var stat = this.GetFloat(PropertyId.PC.MNA, 1);

			var rateByJob = this.Character.Job?.Data.SpRate ?? 1;
			var byJob = Math.Floor(200 * rateByJob);

			var byLevel = Math.Floor(byJob + ((level - 1) * 18 * rateByJob));
			var byStat = Math.Floor(((stat * 0.005f) + (Math.Floor(stat / 10.0f) * 0.015f)) * byLevel);
			var byItem = this.Character.Inventory.GetEquipProperties(PropertyId.Item.MSP);
			var byBonus = this.GetFloat(PropertyId.PC.MSP_Bonus);

			var value = byLevel + byStat + byItem + byBonus;

			var byBuffs = this.GetFloat(PropertyId.PC.MSP_BM);
			var byBuffRate = Math.Floor(value * this.GetFloat(PropertyId.PC.MSP_RATE_BM));

			value += byBuffs + byBuffRate;

			return (int)Math.Max(0, value);
		}

		/// <summary>
		/// Gets or set Stamina, clamped between 0 and MaxStamina.
		/// </summary>
		public int Stamina
		{
			get => _stamina;
			set => _stamina = (int)Math2.Clamp(0, this.GetMaxSta(), value);
		}
		private int _stamina;

		/// <summary>
		/// Returns the character's maximum stamina (PC.MaxSta).
		/// </summary>
		public int MaxStamina => (int)this.GetFloat(PropertyId.PC.MaxSta);

		/// <summary>
		/// Returns the character's maximum stamina.
		/// </summary>
		public float GetMaxSta()
		{
			// TODO: Item and buff bonus.
			return this.Character.Job?.Data.Stamina ?? 1;
		}

		/// <summary>
		/// Returns HP recovery.
		/// </summary>
		public float GetRHP()
		{
			var mhp = this.GetFloat(PropertyId.PC.MHP, 1);
			var jobHpRate = this.Character.Job?.Data.RHpRate ?? 1;

			var byDefault = Math.Floor(mhp / 100f * jobHpRate);
			var byItems = this.Character.Inventory.GetEquipProperties(PropertyId.Item.RHP);
			var byBuffs = this.GetFloat(PropertyId.PC.RHP_BM);

			var value = (byDefault + byItems + byBuffs);
			return (float)Math.Max(0, value);
		}

		/// <summary>
		/// Returns HP recovery time.
		/// </summary>
		public float GetRHPTIME()
		{
			// The recovery time is presumably the number of milliseconds
			// between regen ticks, with the default being 20 seconds,
			// which is reduced by items, buffs, sitting, etc.

			var defaultTime = 20000;

			// Item.RHPTIME doesn't exist?
			var byItems = 0; ; // TimeSpan.FromMilliseconds(this.Character.Inventory.GetEquipProperties(PropertyId.Item.RHPTIME));
			var byBuffs = this.GetFloat(PropertyId.PC.RHPTIME_BM);

			var value = defaultTime - byItems - byBuffs;

			if (this.Character.IsSitting)
				value /= 2;

			return (int)Math.Max(1000, value);
		}

		/// <summary>
		/// Returns SP recovery.
		/// </summary>
		public float GetRSP()
		{
			var mhp = this.GetFloat(PropertyId.PC.MSP, 1);
			var jobSpRate = this.Character.Job?.Data.RSpRate ?? 1;

			var byDefault = Math.Floor(mhp * 0.03f * jobSpRate);
			var byItems = this.Character.Inventory.GetEquipProperties(PropertyId.Item.RSP);
			var byBuffs = this.GetFloat(PropertyId.PC.RSP_BM);

			var value = (byDefault + byItems + byBuffs);
			return (float)Math.Max(0, value);
		}

		/// <summary>
		/// Returns SP recovery time.
		/// </summary>
		public float GetRSPTIME()
		{
			var defaultTime = 20000;

			var byItems = 0; ; // TimeSpan.FromMilliseconds(this.Character.Inventory.GetEquipProperties(PropertyId.Item.RSPTIME));
			var byBuffs = this.GetFloat(PropertyId.PC.RSPTIME_BM);

			var value = defaultTime - byItems - byBuffs;

			if (this.Character.IsSitting)
				value /= 2;

			return (int)Math.Max(1000, value);
		}

		/// <summary>
		/// Returns maximum weight the character can carry.
		/// </summary>
		/// <remarks>
		/// At release: Base 5000, plus 5 for each Str/Con.
		/// Now: Base 8000 plus bonuses?
		/// </remarks>
		public float GetMaxWeight()
			=> 8000;

		/// <summary>
		/// Returns combined weight of all items the character is currently carrying.
		/// </summary>
		public float GetNowWeight()
			=> this.Character.Inventory.GetNowWeight();

		/// <summary>
		/// Stat points.
		/// </summary>
		public float GetStatPoint()
		{
			var byLevel = (int)this.GetFloat(PropertyId.PC.StatByLevel);
			var byBonus = (int)this.GetFloat(PropertyId.PC.StatByBonus);
			var usedStat = (int)this.GetFloat(PropertyId.PC.UsedStat);

			return (byLevel + byBonus - usedStat);
		}

		/// <summary>
		/// Returns character's STR bonus from items and buffs.
		/// </summary>
		public float GetSTR_ADD()
		{
			var byItem = 0; // TODO

			// Buffs: "STR_BM"
			var byBuffs = 0;

			// "STR_ITEM_BM" Item Awakening/Enchantment ?
			var byItemBuff = 0;

			var value = byItem + byBuffs + byItemBuff;

			return value;
		}

		/// <summary>
		/// Returns character's total strength.
		/// </summary>
		/// <returns></returns>
		public float GetSTR()
		{
			var defaultStat = this.GetFloat(PropertyId.PC.STR_JOB);

			var byJob = 0f;
			var jobs = this.Character.Jobs.GetList();
			foreach (var job in jobs)
				byJob += job.Data.StrRatio;
			byJob = (float)Math.Floor((this.GetFloat(PropertyId.PC.Lv) - 1) * (byJob / jobs.Length / 100f));

			var byStat = this.GetFloat(PropertyId.PC.STR_STAT);
			var byBonus = this.GetFloat(PropertyId.PC.STR_Bonus);
			var byAdd = this.GetFloat(PropertyId.PC.STR_ADD);
			var byTemp = 0; // this.GetFloat(PropertyId.PC.STR_TEMP);

			var rewardProperty = 0; // GET_REWARD_PROPERTY(self, statString);

			var result = defaultStat + byJob + byStat + byBonus + byAdd + byTemp + rewardProperty;
			return (float)Math.Floor(Math.Max(1, result));
		}

		/// <summary>
		/// Returns character's CON bonus from items and buffs.
		/// </summary>
		public float GetCON_ADD()
		{
			var byItem = 0; // TODO

			// Buffs: "CON_BM"
			var byBuffs = 0;

			// "CON_ITEM_BM" Item Awakening/Enchantment ?
			var byItemBuff = 0;

			var value = byItem + byBuffs + byItemBuff;

			return value;
		}

		/// <summary>
		/// Returns character's total constitution.
		/// </summary>
		public float GetCON()
		{
			var defaultStat = this.GetFloat(PropertyId.PC.CON_JOB);

			var byJob = 0f;
			var jobs = this.Character.Jobs.GetList();
			foreach (var job in jobs)
				byJob += job.Data.ConRatio;
			byJob = (float)Math.Floor((this.GetFloat(PropertyId.PC.Lv) - 1) * (byJob / jobs.Length / 100f));

			var byStat = this.GetFloat(PropertyId.PC.CON_STAT);
			var byBonus = this.GetFloat(PropertyId.PC.CON_Bonus);
			var byAdd = this.GetFloat(PropertyId.PC.CON_ADD);
			var byTemp = 0; // this.GetFloat(PropertyId.PC.CON_TEMP);

			var rewardProperty = 0; // GET_REWARD_PROPERTY(self, statString);

			var result = defaultStat + byJob + byStat + byBonus + byAdd + byTemp + rewardProperty;
			return (float)Math.Floor(Math.Max(1, result));
		}

		/// <summary>
		/// Returns character's INT bonus from items and buffs.
		/// </summary>
		public float GetINT_ADD()
		{
			var byItem = 0; // TODO

			// Buffs: "INT_BM"
			var byBuffs = 0;

			// "INT_ITEM_BM" Item Awakening/Enchantment ?
			var byItemBuff = 0;

			var value = byItem + byBuffs + byItemBuff;

			return value;
		}

		/// <summary>
		/// Returns character's total intelligence.
		/// </summary>
		public float GetINT()
		{
			var defaultStat = this.GetFloat(PropertyId.PC.INT_JOB);

			var byJob = 0f;
			var jobs = this.Character.Jobs.GetList();
			foreach (var job in jobs)
				byJob += job.Data.IntRatio;
			byJob = (float)Math.Floor((this.GetFloat(PropertyId.PC.Lv) - 1) * (byJob / jobs.Length / 100f));

			var byStat = this.GetFloat(PropertyId.PC.INT_STAT);
			var byBonus = this.GetFloat(PropertyId.PC.INT_Bonus);
			var byAdd = this.GetFloat(PropertyId.PC.INT_ADD);
			var byTemp = 0; // this.GetFloat(PropertyId.PC.INT_TEMP);

			var rewardProperty = 0; // GET_REWARD_PROPERTY(self, statString);

			var result = defaultStat + byJob + byStat + byBonus + byAdd + byTemp + rewardProperty;
			return (float)Math.Floor(Math.Max(1, result));
		}

		/// <summary>
		/// Returns character's MNA (SPR) bonus from items and buffs.
		/// </summary>
		public float GetMNA_ADD()
		{
			var byItem = 0; // TODO

			// Buffs: "SPR_BM"
			var byBuffs = 0;

			// "SPR_ITEM_BM" Item Awakening/Enchantment ?
			var byItemBuff = 0;

			var value = byItem + byBuffs + byItemBuff;

			return value;
		}

		/// <summary>
		/// Returns character's total spirit.
		/// </summary>
		public float GetMNA()
		{
			var defaultStat = this.GetFloat(PropertyId.PC.MNA_JOB);

			var byJob = 0f;
			var jobs = this.Character.Jobs.GetList();
			foreach (var job in jobs)
				byJob += job.Data.SprRatio;
			byJob = (float)Math.Floor((this.GetFloat(PropertyId.PC.Lv) - 1) * (byJob / jobs.Length / 100f));

			var byStat = this.GetFloat(PropertyId.PC.MNA_STAT);
			var byBonus = this.GetFloat(PropertyId.PC.MNA_Bonus);
			var byAdd = this.GetFloat(PropertyId.PC.MNA_ADD);
			var byTemp = 0; // this.GetFloat(PropertyId.PC.MNA_TEMP);

			var rewardProperty = 0; // GET_REWARD_PROPERTY(self, statString);

			var result = defaultStat + byJob + byStat + byBonus + byAdd + byTemp + rewardProperty;
			return (float)Math.Floor(Math.Max(1, result));
		}

		/// <summary>
		/// Returns character's DEX bonus from items and buffs.
		/// </summary>
		public float GetDEX_ADD()
		{
			var byItem = 0; // TODO

			// Buffs: "DEX_BM"
			var byBuffs = 0;

			// "DEX_ITEM_BM" Item Awakening/Enchantment ?
			var byItemBuff = 0;

			var value = byItem + byBuffs + byItemBuff;

			return value;
		}

		/// <summary>
		/// Returns character's total dexterity.
		/// </summary>
		public float GetDEX()
		{
			var defaultStat = this.GetFloat(PropertyId.PC.DEX_JOB);

			var byJob = 0f;
			var jobs = this.Character.Jobs.GetList();
			foreach (var job in jobs)
				byJob += job.Data.DexRatio;
			byJob = (float)Math.Floor((this.GetFloat(PropertyId.PC.Lv) - 1) * (byJob / jobs.Length / 100f));

			var byStat = this.GetFloat(PropertyId.PC.DEX_STAT);
			var byBonus = this.GetFloat(PropertyId.PC.DEX_Bonus);
			var byAdd = this.GetFloat(PropertyId.PC.DEX_ADD);
			var byTemp = 0; // this.GetFloat(PropertyId.PC.DEX_TEMP);

			var rewardProperty = 0; // GET_REWARD_PROPERTY(self, statString);

			var result = defaultStat + byJob + byStat + byBonus + byAdd + byTemp + rewardProperty;
			return (float)Math.Floor(Math.Max(1, result));
		}

		/// <summary>
		/// Returns minimum physical ATK.
		/// </summary>
		public float GetMINPATK()
		{
			var level = this.GetFloat(PropertyId.PC.Lv, 1);
			var stat = this.GetFloat(PropertyId.PC.STR, 1);

			var baseValue = 20;
			var byLevel = level;

			var byStat = (stat * 2f) + ((float)Math.Floor(stat / 10f) * (byLevel * 0.05f));

			var byItem = 0f;
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.MINATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.PATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.ADD_MINATK);

			var value = (baseValue + byLevel + byStat + byItem);

			var byBuffs = 0f;
			byBuffs += this.GetFloat(PropertyId.PC.PATK_BM);
			byBuffs += this.GetFloat(PropertyId.PC.MINPATK_BM);
			byBuffs += this.GetFloat(PropertyId.PC.PATK_MAIN_BM);
			byBuffs += this.GetFloat(PropertyId.PC.MINPATK_MAIN_BM);

			var byRateBuffs = 0f;
			byRateBuffs += this.GetFloat(PropertyId.PC.PATK_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.MINPATK_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.PATK_MAIN_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.MINPATK_MAIN_RATE_BM);
			byRateBuffs = (value * byRateBuffs);

			value += byBuffs + byRateBuffs;

			var max = this.GetMAXPATK();
			return (float)Math2.Clamp(1, max, value);
		}

		/// <summary>
		/// Returns maximum physical ATK.
		/// </summary>
		public float GetMAXPATK()
		{
			var level = this.GetFloat(PropertyId.PC.Lv, 1);
			var stat = this.GetFloat(PropertyId.PC.STR, 1);

			var baseValue = 20;
			var byLevel = level;

			var byStat = (stat * 2f) + ((float)Math.Floor(stat / 10f) * (byLevel * 0.05f));

			var byItem = 0f;
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.MAXATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.PATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.ADD_MAXATK);

			var value = (baseValue + byLevel + byStat + byItem);

			var byBuffs = 0f;
			byBuffs += this.GetFloat(PropertyId.PC.PATK_BM);
			byBuffs += this.GetFloat(PropertyId.PC.MAXPATK_BM);
			byBuffs += this.GetFloat(PropertyId.PC.PATK_MAIN_BM);
			byBuffs += this.GetFloat(PropertyId.PC.MAXPATK_MAIN_BM);

			var byRateBuffs = 0f;
			byRateBuffs += this.GetFloat(PropertyId.PC.PATK_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.MAXPATK_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.PATK_MAIN_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.MAXPATK_MAIN_RATE_BM);
			byRateBuffs = (value * byRateBuffs);

			value += byBuffs + byRateBuffs;

			return (float)Math.Max(1, value);
		}

		/// <summary>
		/// Returns minimum physical ATK (for sub-weapon?).
		/// </summary>
		public float GetMINPATK_SUB()
		{
			var baseValue = 20;
			var level = this.GetFloat(PropertyId.PC.Lv);
			var stat = this.GetFloat(PropertyId.PC.STR);

			var byLevel = level / 2f;
			var byStat = (stat * 2f) + ((float)Math.Floor(stat / 10f) * 5f);
			var byItem = 0; // TODO: "MINATK" "PATK" "ADD_MINATK"

			var value = baseValue + byLevel + byStat + byItem;

			// Reducation for shields and stuff?
			//value -= leftHand.MinAtk;
			//if(hasBuff("Warrior_RH_VisibleObject"))
			//	value -= rightHand.MinAtk

			// Buffs: "PATK_BM", "MINPATK_SUB_BM"
			var byBuffs = 0;

			// Rate buffs: "PATK_RATE_BM", "MINPATK_SUB_RATE_BM"
			//if(hasBuff("Guardian"))
			//	rate -= SkillLevel
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			var maxPatk_sub = this.GetMAXPATK_SUB();
			if (value > maxPatk_sub)
				return maxPatk_sub;

			return (int)value;
		}

		/// <summary>
		/// Returns maximum physical ATK (for sub-weapon?).
		/// </summary>
		public float GetMAXPATK_SUB()
		{
			var baseValue = 20;
			var level = this.GetFloat(PropertyId.PC.Lv);
			var stat = this.GetFloat(PropertyId.PC.STR);

			var byLevel = level / 2f;
			var byStat = (stat * 2f) + ((float)Math.Floor(stat / 10f) * 5f);
			var byItem = 0; // TODO: "MAXATK" "PATK" "ADD_MAXATK"

			var value = baseValue + byLevel + byStat + byItem;

			// Reducation for shields and stuff?
			//value -= leftHand.MaxAtk;
			//if(hasBuff("Warrior_RH_VisibleObject"))
			//	value -= rightHand.MaxAtk;

			// Buffs: "PATK_BM", "MAXPATK_SUB_BM"
			var byBuffs = 0;

			// Rate buffs: "PATK_RATE_BM", "MAXPATK_SUB_RATE_BM"
			//if(hasBuff("Guardian"))
			//	rate -= SkillLevel
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns minimum magic ATK.
		/// </summary>
		public float GetMINMATK()
		{
			var level = this.GetFloat(PropertyId.PC.Lv, 1);
			var stat = this.GetFloat(PropertyId.PC.INT, 1);

			var baseValue = 20;
			var byLevel = level;

			var byStat = (stat * 2f) + ((float)Math.Floor(stat / 10f) * (byLevel * 0.05f));

			var byItem = 0f;
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.MATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.ADD_MATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.ADD_MINATK);

			var value = (baseValue + byLevel + byStat + byItem);

			var byBuffs = 0f;
			byBuffs += this.GetFloat(PropertyId.PC.MATK_BM);
			byBuffs += this.GetFloat(PropertyId.PC.MINMATK_BM);

			var byRateBuffs = 0f;
			byRateBuffs += this.GetFloat(PropertyId.PC.MATK_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.MINMATK_RATE_BM);
			byRateBuffs = (value * byRateBuffs);

			value += byBuffs + byRateBuffs;

			var max = this.GetMAXMATK();
			return (float)Math2.Clamp(1, max, value);
		}

		/// <summary>
		/// Returns maximum magic ATK.
		/// </summary>
		public float GetMAXMATK()
		{
			var level = this.GetFloat(PropertyId.PC.Lv, 1);
			var stat = this.GetFloat(PropertyId.PC.INT, 1);

			var baseValue = 20;
			var byLevel = level;

			var byStat = (stat * 2f) + ((float)Math.Floor(stat / 10f) * (byLevel * 0.05f));

			var byItem = 0f;
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.MATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.ADD_MATK);
			byItem += this.Character.Inventory.GetEquipProperties(PropertyId.Item.ADD_MAXATK);

			var value = (baseValue + byLevel + byStat + byItem);

			var byBuffs = 0f;
			byBuffs += this.GetFloat(PropertyId.PC.MATK_BM);
			byBuffs += this.GetFloat(PropertyId.PC.MINMATK_BM);

			var byRateBuffs = 0f;
			byRateBuffs += this.GetFloat(PropertyId.PC.MATK_RATE_BM);
			byRateBuffs += this.GetFloat(PropertyId.PC.MINMATK_RATE_BM);
			byRateBuffs = (value * byRateBuffs);

			value += byBuffs + byRateBuffs;

			return (float)Math.Max(1, value);
		}

		/// <summary>
		/// Returns Physical Defense.
		/// </summary>
		public float GetDEF()
		{
			var baseValue = 20;
			var level = this.GetFloat(PropertyId.PC.Lv);

			var byLevel = level;
			var byItem = 0f; // TODO: "DEF" "DEF_Rate"

			var value = baseValue + byLevel + byItem;

			// Buffs: "DEF_BM"
			var byBuffs = 0;

			// Rate buffs: "DEF_RATE_BM"
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns Magic Defense.
		/// </summary>
		public float GetMDEF()
		{
			var baseValue = 20;
			var level = this.GetFloat(PropertyId.PC.Lv);

			var byLevel = level;
			var byItem = 0f; // TODO: "MDEF" "MDEF_Rate"

			var value = baseValue + byLevel + byItem;

			// Buffs: "MDEF_BM"
			var byBuffs = 0;

			// Rate buffs: "MDEF_RATE_BM"
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns critical attack.
		/// </summary>
		public float GetCRTATK()
		{
			var stat = this.GetFloat(PropertyId.PC.DEX);

			var byStat = (stat * 4f) + ((float)Math.Floor(stat / 10f) * 10f);
			var byItem = 0; // TODO

			var value = byStat + byItem;

			// Buffs: "CRTATK_BM"
			var byBuffs = 0;

			// Rate buffs: Does Tos have something like CritATK +x%?
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns critical hit rate (crit chance).
		/// </summary>
		public float GetCRTHR()
		{
			var level = this.GetFloat(PropertyId.PC.Lv);

			var byLevel = level / 2f;
			var byItem = 0; // TODO

			var value = byLevel + byItem;

			// Buffs: "CRTHR_BM"
			var byBuffs = 0;

			// Rate buffs:
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns critical dodge rate.
		/// </summary>
		public float GetCRTDR()
		{
			var level = this.GetFloat(PropertyId.PC.Lv);

			var byLevel = level / 2f;
			var byItem = 0; // TODO

			var value = byLevel + byItem;

			// Buffs: "CRTDR_BM"
			var byBuffs = 0;

			// Rate buffs:
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns hit rate.
		/// </summary>
		public float GetHR()
		{
			var level = this.GetFloat(PropertyId.PC.Lv);
			var stat = this.GetFloat(PropertyId.PC.STR);

			var byLevel = level / 4f;
			var byStat = (stat / 2f) + ((float)Math.Floor(stat / 15f) * 3f);
			var byItem = 0; // HR, ADD_HR

			var value = byLevel + byStat + byItem;

			// Buffs: "HR_BM"
			var byBuffs = 0;

			// Rate buffs: HR_RATE_BM
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns dodge rate.
		/// </summary>
		public float GetDR()
		{
			var level = this.GetFloat(PropertyId.PC.Lv);
			var stat = this.GetFloat(PropertyId.PC.DEX);

			var byLevel = level / 4f;
			var byStat = (stat / 2f) + ((float)Math.Floor(stat / 15f) * 3f);
			var byItem = 0; // TODO

			var value = byLevel + byStat + byItem;

			// Buffs: "DR_BM"
			var byBuffs = 0;

			// Rate buffs: "ADD_DR"
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns block.
		/// </summary>
		public float GetBLK()
		{
			// TODO: Update it after equipment change.
			// Shield/Dagger = Right hand.
			if (this.Character.Inventory.GetItem(EquipSlot.LeftHand).Data.EquipType1 != EquipType.Shield)
				return 0;

			var Level = this.GetFloat(PropertyId.PC.Lv);
			var stat = this.GetFloat(PropertyId.PC.CON);

			var byLevel = Level / 4f;
			var byStat = (stat / 2f) + ((float)Math.Floor(stat / 15f) * 3f);
			var byItem = 0f; // TODO

			var value = byLevel + byStat + byItem;

			// Buffs: "BLK_BM"
			var byBuffs = 0;

			// Rate buffs: BlockRate
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns block break (penetration).
		/// </summary>
		public float GetBLK_BREAK()
		{
			var level = this.GetFloat(PropertyId.PC.Lv);
			var stat = this.GetFloat(PropertyId.PC.DEX);

			var byLevel = level / 4f;
			var byStat = (stat / 2f) + ((float)Math.Floor(stat / 15f) * 3f);
			var byItem = 0; // TODO

			var value = byLevel + byStat + byItem;

			// Buffs: "BLK_BREAK_BM"
			var byBuffs = 0;

			// Rate buffs: 
			var rate = 0;
			var byRateBuffs = (float)Math.Floor(value * rate);

			value += byBuffs + byRateBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns the character's splash rate?
		/// </summary>
		public float GetSR()
		{
			var baseValue = 3;

			if (this.Character.Jobs.Has(JobId.Swordsman, Circle.First))
				baseValue = 4;
			else if (this.Character.Jobs.Has(JobId.Archer, Circle.First))
				baseValue = 0;

			var byItem = 0f; // TODO

			var value = baseValue + byItem;

			// Buffs: "SR_BM"
			var byBuffs = 0;

			value += byBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns splash dodge rate.
		/// </summary>
		public float GetSDR()
		{
			var baseValue = 1;
			var byItem = 0f; // TODO

			var value = baseValue + byItem;

			// Buffs: "SDR_BM"
			var byBuffs = 0;

			value += byBuffs;

			return (int)value;
		}

		/// <summary>
		/// Returns character's movement speed.
		/// </summary>
		/// <returns></returns>
		private float GetMSPD()
		{
			var byDefault = 30;
			var byBuff = this.GetFloat(PropertyId.PC.MSPD_BM);
			var byBonus = this.GetFloat(PropertyId.PC.MSPD_Bonus);

			return (byDefault + byBuff + byBonus);
		}

		/// <summary>
		/// Returns character's current jump power, which dictates how
		/// high they can jump.
		/// </summary>
		/// <returns></returns>
		public float GetJumpPower()
		{
			return 350;
		}

		/// <summary>
		/// Returns character's current casting speed.
		/// </summary>
		/// <returns></returns>
		public float GetCastingSpeed()
		{
			var byDefault = 100;
			var byBuff = this.GetFloat(PropertyId.PC.CastingSpeed_BM);

			var result = byDefault + byBuff;
			return (float)Math.Floor(Math2.Clamp(10, 200, result));
		}
	}
}
