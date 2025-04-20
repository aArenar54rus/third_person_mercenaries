using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
	public class InventoryDebugCanvasLayer : CanvasWindowLayer
	{
		[SerializeField]
		private TMP_Dropdown removeItemDropdown;
		[SerializeField]
		private TMP_InputField removeItemCountField;
		[SerializeField]
		private Button removeItemButton;

		[Space(10)]
		[SerializeField]
		private TMP_Dropdown addItemDropdown;
		[SerializeField]
		private TMP_InputField addItemCountField;
		[SerializeField]
		private Button addItemButton;
		
		[SerializeField]
		private Button logAllItemsInInventoryButton;


		public TMP_Dropdown RemoveItemDropdown => removeItemDropdown;
		public TMP_InputField RemoveItemCountField => removeItemCountField;
		public Button RemoveItemButton => removeItemButton;

		public TMP_Dropdown AddItemDropdown => addItemDropdown;
		public TMP_InputField AddItemCountField => addItemCountField;
		public Button AddItemButton => addItemButton;
		
		public Button LogAllItemsInInventoryButton => logAllItemsInInventoryButton;
	}
}