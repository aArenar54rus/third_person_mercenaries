using System.Collections.Generic;
using UnityEngine;


namespace Module.General
{
	public class Example : MonoBehaviour
	{
		private ExampleData exampleData;


		private void Awake()
		{
			exampleData = new ExampleData();
			CustomDebug.Log(exampleData.Name);
		}


		private void Start()
		{
			exampleData.Name = "New Name";
		}
	}

	public class ExampleData : DataSaveHolder<ExampleData.Data>
	{
		public class Data : DataSaveHolderData
		{
			public int health = default;
			public string name = default;
			public List<string> listStrings = new List<string>();


			public Data()
			{
				RestoreOldDataValues();
			}


			private void RestoreOldDataValues()
			{
				if (PlayerPrefs.HasKey("Health"))
				{
					health = PlayerPrefs.GetInt("Health");
				}

				if (PlayerPrefs.HasKey("Name"))
				{
					name = PlayerPrefs.GetString("Name");
				}
			}
		}

		protected override string PrefsKey { get; } = "TestKey";

		public int Health
		{
			get => data.health;
			set
			{
				data.health = value;
				SaveData();
			}
		}

		public string Name
		{
			get => data.name;
			set
			{
				data.name = value;
				SaveData();
			}
		}

		public List<string> ListStr
		{
			get => data.listStrings;
			set
			{
				data.listStrings = value;
				SaveData();
			}
		}
	}
}
