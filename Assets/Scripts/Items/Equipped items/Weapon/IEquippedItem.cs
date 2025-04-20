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

		ItemData ItemData { get; }

		
		void InitializeItem(ItemData itemData, Dictionary<System.Type, IEquippedItemComponent> components);
		
		void PickUpItem(ICharacterEntity characterOwner);

		void DropItem();
	}
}