using Arenar.Character;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Items
{
	public interface IEquippedItem
	{
		Transform transform { get; }
		
		int ItemLevel { get; }

		ICharacterEntity ItemOwner { get; }

		ItemInventoryData ItemInventoryData { get; }

		
		void InitializeItem(ItemInventoryData itemInventoryData, Dictionary<System.Type, IEquippedItemComponent> components);
		
		void PickUpItem(ICharacterEntity characterOwner);

		void DropItem();
	}
}