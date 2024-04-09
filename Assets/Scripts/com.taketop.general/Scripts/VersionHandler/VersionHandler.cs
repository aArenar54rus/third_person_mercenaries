using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Module.General.VersionHandler
{


	public class VersionHandler : MonoSingleton<VersionHandler>
	{
		private static string VersionPrefsKey => $"{Application.productName}_VERSION";

		private const int POWER_FACTOR = 3;
		private const string ZERO_VERSION = "0.0.0";

		public static VersionChangeInfo VersionChangeInfo { get; private set; }
		public static int CurrentVersionNumber => VersionChangeInfo.CurrentVersionNumber;
		public static int LastVersionNumber => VersionChangeInfo.LastVersionNumber;
		public static VersionComparisonResult VersionComparisonResult => VersionChangeInfo.ComparisonResult;
		private static IEnumerable<IVersionChangeHandler> VersionChangeHandlers =>
			VersionHandlerSettings.Instance.Handlers;

		private static string LastVersion
		{
			get => PlayerPrefs.GetString(VersionPrefsKey, ZERO_VERSION);
			set => PlayerPrefs.SetString(VersionPrefsKey, value);
		}

		private static readonly Dictionary<VersionComparer, Func<bool>> CompareFunction = new Dictionary<VersionComparer, Func<bool>>
		{
			{ VersionComparer.NotEqual, () => CurrentVersionNumber != LastVersionNumber },
			{ VersionComparer.Less, () => CurrentVersionNumber < LastVersionNumber },
			{ VersionComparer.Greater, () => CurrentVersionNumber > LastVersionNumber },
			{ VersionComparer.Always, () => true },
		};



		public static bool TryCompare(VersionComparer comparer) =>
			CompareFunction.TryGetValue(comparer, out Func<bool> function)
			&& function.Invoke();


		public static int AppVersionToInt(string version)
		{
			const int VERSION_SPLIT_NUMBERS_COUNT = 3;
			const char ZERO_CHAR = '0';
			string result = string.Empty;

			if (string.IsNullOrEmpty(version))
				return default;

			string[] versionDigits = version.Split('.');

			for (int i = 0; i < VERSION_SPLIT_NUMBERS_COUNT; i++)
			{
				string numberStr = versionDigits[i];
				int.TryParse(numberStr, out int number);
				numberStr = number.ToString();

				int count = Mathf.Clamp(POWER_FACTOR - numberStr.Length, 0, POWER_FACTOR);
				for (int j = 0; j < count; j++)
					result += ZERO_CHAR;

				result += number;
			}

			return int.Parse(result);
		}


		protected override void Initialize()
		{
			base.Initialize();

			HandleVersion();
			LastVersion = Application.version;
		}

		private static void HandleVersion()
		{
			int currentVersionNumber = AppVersionToInt(Application.version);
			int lastVersionNumber = AppVersionToInt(LastVersion);
			VersionChangeInfo = new VersionChangeInfo
			{
				CurrentVersionNumber = currentVersionNumber,
				LastVersionNumber = lastVersionNumber,
				ComparisonResult = currentVersionNumber > lastVersionNumber
					? VersionComparisonResult.Upgrade
					: (currentVersionNumber == lastVersionNumber
						? VersionComparisonResult.Same
						: VersionComparisonResult.Downgrade),
			};


			if (VersionComparisonResult == VersionComparisonResult.Same)
				return;

			foreach (IVersionChangeHandler handler in VersionChangeHandlers)
			{
				if (handler.VersionComparer == VersionComparer.None)
				{
					Debug.LogError($"Game Version Processor <b>{handler.GetType()}</b> has <b>{VersionComparer.None}</b> comparer!");
					continue;
				}

				if (TryCompare(handler.VersionComparer))
					handler.Execute(VersionChangeInfo);
			}
		}


		#if UNITY_EDITOR

		[InitializeOnLoadMethod]
		private static void CreateSettings() =>
			_ = VersionHandlerSettings.Instance;

		[ContextMenu("Reset Version")]
		private void DowngradeVersion() =>
			LastVersion = ZERO_VERSION;

		#endif
	}



	#region Additionals

	public enum VersionComparer : byte
	{
		None = 0,
		NotEqual = 1,
		Greater = 2,
		Less = 3,
		Always = 4,
	}


	public enum VersionComparisonResult : byte
	{
		Same = 0,
		Upgrade = 1,
		Downgrade = 2,
	}


	public struct VersionChangeInfo
	{
		public VersionComparisonResult ComparisonResult;
		public int CurrentVersionNumber;
		public int LastVersionNumber;
	}


	public interface IVersionChangeHandler
	{
		public abstract VersionComparer VersionComparer { get; }

		public abstract void Execute(VersionChangeInfo info);
	}

	#endregion
}
