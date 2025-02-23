﻿using System;
using System.Threading;
using Melia.Channel.Network;
using Melia.Shared.Const;
using Melia.Shared.Data.Database;
using Melia.Shared.Util;
using Melia.Shared.World;
using Melia.Shared.World.ObjectProperties;

namespace Melia.Channel.World.Entities
{
	/// <summary>
	/// An item, that might be lying around in the world or is owned by
	/// some entity.
	/// </summary>
	public class Item : IPropertyObject
	{
		private static long _worldId = 0x0050000000000000;

		private int _amount;

		/// <summary>
		/// Returns the item's class id.
		/// </summary>
		public int Id { get; private set; }

		/// <summary>
		/// Returns a reference to the item's data from the item data file.
		/// </summary>
		public ItemData Data { get; private set; }

		/// <summary>
		/// Gets or sets the item's amount (clamped to 1~max),
		/// does not update the client.
		/// </summary>
		public int Amount
		{
			get { return _amount; }
			set
			{
				var max = (this.Data != null ? this.Data.MaxStack : 1);
				_amount = Math2.Clamp(1, max, value);
			}
		}

		/// <summary>
		/// Returns true if item's MaxStack is higher than 1, indicating
		/// that it can contain more than one item of its type.
		/// </summary>
		public bool IsStackable => this.Data.MaxStack > 1;

		/// <summary>
		/// Gets or sets item's globally unique object id.
		/// </summary>
		public long ObjectId { get; set; }

		/// <summary>
		/// Returns item's buy price.
		/// </summary>
		public int Price { get; private set; }

		/// <summary>
		/// Gets or sets whether the item is locked.
		/// </summary>
		/// <remarks>
		/// XXX: Should this be saved? If so, we have to figure out where
		///   that goes in the item data.
		/// </remarks>
		public bool IsLocked { get; set; }

		/// <summary>
		/// Returns the handle of the entity who originally dropped the item.
		/// </summary>
		public int OriginalOwnerHandle { get; private set; } = -1;

		/// <summary>
		/// Returns the time at which the owner can pick the item back up.
		/// </summary>
		public DateTime RePickUpTime { get; private set; }

		/// <summary>
		/// Gets or sets the owner of the item, who is the only one able
		/// to pick it up while the loot protection is active.
		/// </summary>
		public int OwnerHandle { get; private set; } = -1;

		/// <summary>
		/// Returns the time at which the item is free to be picked up
		/// by anyone.
		/// </summary>
		public DateTime LootProtectionEnd { get; private set; } = DateTime.MinValue;

		/// <summary>
		/// Returns the item's property collection.
		/// </summary>
		public Properties Properties { get; } = new Properties();

		/// <summary>
		/// Creates new item.
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="amount"></param>
		public Item(int itemId, int amount = 1)
		{
			// Set id and load data, so we have data to work with.
			this.Id = itemId;
			this.LoadData();

			this.ObjectId = Interlocked.Increment(ref _worldId);
			this.Amount = amount;
		}

		/// <summary>
		/// Loads item data from data files.
		/// </summary>
		private void LoadData()
		{
			if (this.Id == 0)
				throw new InvalidOperationException("Item id wasn't set before calling LoadData.");

			this.Data = ChannelServer.Instance.Data.ItemDb.Find(this.Id);
			if (this.Data == null)
				throw new NullReferenceException("No item data found for '" + this.Id + "'.");

			if (this.Data.MinAtk != 0) this.Properties.Set(PropertyId.Item.MINATK, this.Data.MinAtk);
			if (this.Data.MaxAtk != 0) this.Properties.Set(PropertyId.Item.MAXATK, this.Data.MaxAtk);
			if (this.Data.MAtk != 0) this.Properties.Set(PropertyId.Item.MATK, this.Data.MAtk);
			if (this.Data.PAtk != 0) this.Properties.Set(PropertyId.Item.PATK, this.Data.PAtk);
			if (this.Data.AddMinAtk != 0) this.Properties.Set(PropertyId.Item.ADD_MINATK, this.Data.AddMinAtk);
			if (this.Data.AddMaxAtk != 0) this.Properties.Set(PropertyId.Item.ADD_MAXATK, this.Data.AddMaxAtk);
			if (this.Data.AddMAtk != 0) this.Properties.Set(PropertyId.Item.ADD_MATK, this.Data.AddMAtk);
			if (this.Data.Def != 0) this.Properties.Set(PropertyId.Item.DEF, this.Data.Def);
			if (this.Data.MDef != 0) this.Properties.Set(PropertyId.Item.MDEF, this.Data.MDef);
			if (this.Data.AddDef != 0) this.Properties.Set(PropertyId.Item.ADD_DEF, this.Data.AddDef);
			if (this.Data.AddMDef != 0) this.Properties.Set(PropertyId.Item.ADD_MDEF, this.Data.AddMDef);
		}

		/// <summary>
		/// Returns the item's index in the inventory, using the given
		/// index as an offset for the category the item belongs to.
		/// </summary>
		/// <example>
		/// item = Drug_HP1
		/// item.GetInventoryIndex(5) => 265001 + 5 = 265006
		/// </example>
		/// <remarks>
		/// The client uses index ranges for categorizing the items.
		/// For example:
		/// 45000~109999:  Equipment/MainWeapon
		/// 265000~274999: Item/Consumable
		/// 
		/// The server needs to put the items indices into the correct
		/// ranges for them to appear where they belong, otherwise a
		/// potion might be put into the equip category.
		/// </remarks>
		/// <param name="index"></param>
		/// <returns></returns>
		public int GetInventoryIndex(int index)
		{
			// If the category is none, use the index. This will put
			// the item well below the first category at index 5000
			// and effectively hide it.
			if (this.Data.Category == InventoryCategory.None)
				return index;

			// Get the base id from the database, add the index and return it.
			if (!ChannelServer.Instance.Data.InvBaseIdDb.TryFind(this.Data.Category, out var invBaseData))
				throw new MissingFieldException($"Category '{this.Data.Category}' on item '{this.Id}' not found in base id database.");

			return invBaseData.BaseId + index;
		}

		/// <summary>
		/// Drops item to the map as an ItemMonster.
		/// </summary>
		/// <remarks>
		/// Items are typically dropped by "tossing" them from the source,
		/// such as a killed monster. The given position is the initial
		/// position, and the item is then tossed in the given direction,
		/// by the distance.
		/// </remarks>
		/// <param name="map">Map to drop to the item on.</param>
		/// <param name="position">Initial position of the drop item.</param>
		/// <param name="direction">Direction to toss the item in.</param>
		/// <param name="distance">Distance to toss the item.</param>
		public ItemMonster Drop(Map map, Position position, Direction direction, float distance)
		{
			// ZC_NORMAL_ItemDrop animates the item flying from its
			// initial drop position to its final position. To keep
			// everything in sync, we use the monster's position as
			// the drop position, then add the item to the map,
			// and then make it fly and set the final position.
			// the direction of the item becomes the direction
			// it flies in.
			// FromGround is necessary for the client to attempt to
			// pick up the item. Might act as "IsYourDrop" for items.

			var itemMonster = ItemMonster.Create(this);
			var flyDropPos = position.GetRelative(direction, distance);

			itemMonster.Position = position;
			itemMonster.Direction = direction;
			itemMonster.FromGround = true;
			itemMonster.DisappearTime = DateTime.Now.AddSeconds(ChannelServer.Instance.Conf.World.DropDisappearSeconds);

			map.AddMonster(itemMonster);

			itemMonster.Position = flyDropPos;
			Send.ZC_NORMAL_ItemDrop(itemMonster, direction, distance);

			return itemMonster;
		}

		/// <summary>
		/// Drops item to the map as an ItemMonster.
		/// </summary>
		/// <param name="map">Map to drop to the item on.</param>
		/// <param name="fromPosition">Initial position of the drop item.</param>
		/// <param name="toPosition">Position the item gets tossed to.</param>
		public ItemMonster Drop(Map map, Position fromPosition, Position toPosition)
		{
			var direction = fromPosition.GetDirection(toPosition);
			var distance = (float)fromPosition.Get2DDistance(toPosition);

			return this.Drop(map, fromPosition, direction, distance);
		}

		/// <summary>
		/// Activates the loot protection for the item if entity is set.
		/// Deactivates it if entity is null.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="protectionTime"></param>
		public void SetLootProtection(IEntity entity, TimeSpan protectionTime)
		{
			if (entity == null)
			{
				this.OwnerHandle = -1;
				this.LootProtectionEnd = DateTime.MinValue;
			}
			else
			{
				this.OwnerHandle = entity.Handle;
				this.LootProtectionEnd = DateTime.Now.Add(protectionTime);
			}
		}

		/// <summary>
		/// Sets up a protection, so that the entity won't pick the item
		/// right back up.
		/// </summary>
		/// <param name="entity"></param>
		public void SetRePickUpProtection(IEntity entity)
		{
			if (entity == null)
			{
				this.OriginalOwnerHandle = -1;
				this.RePickUpTime = DateTime.MinValue;
			}
			else
			{
				this.OriginalOwnerHandle = entity.Handle;
				this.RePickUpTime = DateTime.Now.AddSeconds(2);
			}
		}

		/// <summary>
		/// Clears protections, so the item can be picked up by anyone.
		/// </summary>
		/// <param name="entity"></param>
		public void ClearProtections()
		{
			this.SetLootProtection(null, TimeSpan.Zero);
			this.SetRePickUpProtection(null);
		}
	}
}
