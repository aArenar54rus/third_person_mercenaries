using System.Collections.Generic;
using System.Text;
using Module.General.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


namespace Module.General.Analytics.Performance
{
	/// <summary>
	/// Reports system specifications and environment information. Information that can
	/// be used to identify a user is marked as private, and won't be included in any generated
	/// reports.
	/// </summary>
	public class StandardSystemInformationService
	{
		private readonly Dictionary<string, IList<InfoEntry>> info =
			new Dictionary<string, IList<InfoEntry>>();


		public StandardSystemInformationService()
		{
			CreateDefaultSet();
		}


		public static string InfoBlockToString(
				IList<InfoEntry> info,
				string nameColor = "#BCBCBC",
				char tickChar = '\u2713',
				char crossChar = '\u00D7'
			)
		{
			var sb = new StringBuilder();
			var maxTitleLength = 0;

			foreach (InfoEntry systemInfo in info)
			{
				if (systemInfo.Title.Length > maxTitleLength)
				{
					maxTitleLength = systemInfo.Title.Length;
				}
			}

			maxTitleLength += 2;

			var first = true;
			foreach (var infoEntry in info)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					sb.AppendLine();
				}

				sb.Append("<color=");
				sb.Append(nameColor);
				sb.Append(">");

				sb.Append(infoEntry.Title);
				sb.Append(": ");

				sb.Append("</color>");

				for (int i = infoEntry.Title.Length; i <= maxTitleLength; ++i)
				{
					sb.Append(' ');
				}

				if (infoEntry.Value is bool value)
				{
					sb.Append(value ? tickChar : crossChar);
				}
				else
				{
					sb.Append(infoEntry.Value);
				}
			}

			return sb.ToString();
		}


		public IEnumerable<string> GetCategories()
		{
			return info.Keys;
		}


		public IList<InfoEntry> GetInfo(string category)
		{
			if (info.TryGetValue(category, out IList<InfoEntry> list))
				return list;

			Debug.LogError("[SystemInformationService] Category not found: {0}".Format(category));

			return new InfoEntry[0];
		}


		public Dictionary<string, Dictionary<string, object>> CreateReport(
				bool includePrivate = false
			)
		{
			var dict = new Dictionary<string, Dictionary<string, object>>();

			foreach (KeyValuePair<string, IList<InfoEntry>> keyValuePair in info)
			{
				var category = new Dictionary<string, object>();

				foreach (InfoEntry systemInfo in keyValuePair.Value)
				{
					if (systemInfo.IsPrivate
						&& !includePrivate)
					{
						continue;
					}

					category.Add(systemInfo.Title, systemInfo.Value);
				}

				dict.Add(keyValuePair.Key, category);
			}

			return dict;
		}


		public string GetDefaultSetString(
				string category,
				string nameColor = "#BCBCBC",
				char tickChar = '\u2713',
				char crossChar = '\u00D7'
			)
		{
			if (info.TryGetValue(category, out IList<InfoEntry> list))
				return InfoBlockToString(list, nameColor, tickChar, crossChar);

			Debug.LogError("[SystemInformationService] Category not found: {0}".Format(category));

			return string.Empty;
		}


		#region Sets

		public IList<InfoEntry> GetSystemSet()
		{
			return new[]
			{
				InfoEntry.Create("Operating System", SystemInfo.operatingSystem),
				InfoEntry.Create("Device Name", SystemInfo.deviceName, true),
				InfoEntry.Create("Device Type", SystemInfo.deviceType),
				InfoEntry.Create("Device Model", SystemInfo.deviceModel),
				InfoEntry.Create("CPU Type", SystemInfo.processorType),
				InfoEntry.Create("CPU Count", SystemInfo.processorCount),
				InfoEntry.Create("System Memory",
					FileUtility.GetBytesReadable((long)SystemInfo.systemMemorySize
						* 1024 * 1024)),
				// InfoEntry.Create("Process Name", () => Process.GetCurrentProcess().ProcessName),
			};
		}


		public IList<InfoEntry> GetBatterySet()
		{
			return SystemInfo.batteryStatus == BatteryStatus.Unknown
				? default
				: new[]
				{
					InfoEntry.Create("Status", SystemInfo.batteryStatus),
					InfoEntry.Create("Battery Level", SystemInfo.batteryLevel)
				};
		}


		public IList<InfoEntry> GetUnitySet()
		{
			#if ENABLE_IL2CPP
            const string IL2CPP = "Yes";
			#else
			const string IL2CPP = "No";
			#endif

			return new[]
			{
				InfoEntry.Create("Version", Application.unityVersion),
				InfoEntry.Create("Debug", Debug.isDebugBuild),
				InfoEntry.Create("Unity Pro", Application.HasProLicense()),
				InfoEntry.Create("Genuine",
					"{0} ({1})".Format(Application.genuine ? "Yes" : "No",
						Application.genuineCheckAvailable ? "Trusted" : "Untrusted")),
				InfoEntry.Create("System Language", Application.systemLanguage),
				InfoEntry.Create("Platform", Application.platform),
				InfoEntry.Create("Install Mode", Application.installMode),
				InfoEntry.Create("Sandbox", Application.sandboxType),
				InfoEntry.Create("IL2CPP", IL2CPP),
				InfoEntry.Create("Application Version", Application.version),
			};
		}


		public IList<InfoEntry> GetDisplaySet()
		{
			return new[]
			{
				InfoEntry.Create("Resolution", () => Screen.width + "x" + Screen.height),
				InfoEntry.Create("DPI", () => Screen.dpi),
				InfoEntry.Create("Fullscreen", () => Screen.fullScreen),
				InfoEntry.Create("Fullscreen Mode", () => Screen.fullScreenMode),
				InfoEntry.Create("Orientation", () => Screen.orientation),
			};
		}


		public IList<InfoEntry> GetRuntimeSet()
		{
			return new[]
			{
				InfoEntry.Create("Play Time", () => Time.unscaledTime),
				InfoEntry.Create("Level Play Time", () => Time.timeSinceLevelLoad),
				#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                InfoEntry.Create("Current Level", () => Application.loadedLevelName),
				#else
				InfoEntry.Create("Current Level", () =>
				{
					Scene activeScene = SceneManager.GetActiveScene();

					return "{0} (Index: {1})".Format(activeScene.name, activeScene.buildIndex);
				}),
				#endif
				InfoEntry.Create("Quality Level", () =>
					QualitySettings.names[QualitySettings.GetQualityLevel()]
					+ " ("
					+ QualitySettings.GetQualityLevel()
					+ ")")
			};
		}


		public IList<InfoEntry> GetBuildSet()
		{
			// Check for cloud build manifest
			var cloudBuildManifest = (TextAsset)Resources.Load("UnityCloudBuildManifest.json");
			Dictionary<string, object> manifestDict = cloudBuildManifest != null
				? JsonConvert.DeserializeObject<Dictionary<string, object>>(cloudBuildManifest.text)
				: null;

			if (manifestDict == null)
			{
				return default;
			}

			var manifestList = new List<InfoEntry>(manifestDict.Count);

			foreach (KeyValuePair<string, object> kvp in manifestDict)
			{
				if (kvp.Value == null)
				{
					continue;
				}

				var value = kvp.Value.ToString();
				manifestList.Add(InfoEntry.Create(GetCloudManifestPrettyName(kvp.Key), value));
			}

			return manifestList;



			string GetCloudManifestPrettyName(string name)
			{
				switch (name)
				{
					case "scmCommitId":
						return "Commit";

					case "scmBranch":
						return "Branch";

					case "cloudBuildTargetName":
						return "Build Target";

					case "buildStartTime":
						return "Build Date";
				}

				// Return name with first letter capitalised.
				return name.Substring(0, 1).ToUpper() + name.Substring(1);
			}
		}


		public IList<InfoEntry> GetFeaturesSet()
		{
			return new[]
			{
				InfoEntry.Create("Location", SystemInfo.supportsLocationService),
				InfoEntry.Create("Accelerometer", SystemInfo.supportsAccelerometer),
				InfoEntry.Create("Gyroscope", SystemInfo.supportsGyroscope),
				InfoEntry.Create("Vibration", SystemInfo.supportsVibration),
				InfoEntry.Create("Audio", SystemInfo.supportsAudio)
			};
		}


		public IList<InfoEntry> GetIOsSet()
		{
			#if UNITY_IOS
			return new[]
			{
				InfoEntry.Create("Generation", UnityEngine.iOS.Device.generation),
				InfoEntry.Create("Ad Tracking", UnityEngine.iOS.Device.advertisingTrackingEnabled),
			};
			#endif
			return default;
		}


		public IList<InfoEntry> GetGraphicsDeviceSet()
		{
			#pragma warning disable 618
			return new[]
			{
				InfoEntry.Create("Device Name", SystemInfo.graphicsDeviceName),
				InfoEntry.Create("Device Vendor", SystemInfo.graphicsDeviceVendor),
				InfoEntry.Create("Device Version", SystemInfo.graphicsDeviceVersion),
				InfoEntry.Create("Graphics Memory",
					FileUtility.GetBytesReadable(((long)SystemInfo.graphicsMemorySize)
						* 1024
						* 1024)),
				InfoEntry.Create("Max Tex Size", SystemInfo.maxTextureSize),
			};
			#pragma warning restore 618
		}


		public IList<InfoEntry> GetGraphicsFeaturesSet()
		{
			#pragma warning disable 618
			return new[]
			{
				InfoEntry.Create("UV Starts at top", SystemInfo.graphicsUVStartsAtTop),
				InfoEntry.Create("Shader Level", SystemInfo.graphicsShaderLevel),
				InfoEntry.Create("Multi Threaded", SystemInfo.graphicsMultiThreaded),
				InfoEntry.Create("Hidden Service Removal (GPU)",
					SystemInfo.hasHiddenSurfaceRemovalOnGPU),
				InfoEntry.Create("Uniform Array Indexing (Fragment Shaders)",
					SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders),
				InfoEntry.Create("Shadows", SystemInfo.supportsShadows),
				InfoEntry.Create("Raw Depth Sampling (Shadows)",
					SystemInfo.supportsRawShadowDepthSampling),
				InfoEntry.Create("Motion Vectors", SystemInfo.supportsMotionVectors),
				InfoEntry.Create("Cubemaps", SystemInfo.supportsRenderToCubemap),
				InfoEntry.Create("Image Effects", SystemInfo.supportsImageEffects),
				InfoEntry.Create("3D Textures", SystemInfo.supports3DTextures),
				InfoEntry.Create("2D Array Textures", SystemInfo.supports2DArrayTextures),
				InfoEntry.Create("3D Render Textures", SystemInfo.supports3DRenderTextures),
				InfoEntry.Create("Cubemap Array Textures", SystemInfo.supportsCubemapArrayTextures),
				InfoEntry.Create("Copy Texture Support", SystemInfo.copyTextureSupport),
				InfoEntry.Create("Compute Shaders", SystemInfo.supportsComputeShaders),
				InfoEntry.Create("Instancing", SystemInfo.supportsInstancing),
				InfoEntry.Create("Hardware Quad Topology", SystemInfo.supportsHardwareQuadTopology),
				InfoEntry.Create("32-bit index buffer", SystemInfo.supports32bitsIndexBuffer),
				InfoEntry.Create("Sparse Textures", SystemInfo.supportsSparseTextures),
				InfoEntry.Create("Render Target Count", SystemInfo.supportedRenderTargetCount),
				InfoEntry.Create("Separated Render Targets Blend",
					SystemInfo.supportsSeparatedRenderTargetsBlend),
				InfoEntry.Create("Multisampled Textures", SystemInfo.supportsMultisampledTextures),
				InfoEntry.Create("Texture Wrap Mirror Once",
					SystemInfo.supportsTextureWrapMirrorOnce),
				InfoEntry.Create("Reversed Z Buffer", SystemInfo.usesReversedZBuffer)
			};
			#pragma warning disable 618
		}


		public IList<InfoEntry> GetStatisticsSet()
		{
			return new[]
			{
				InfoEntry.Create("Static Batches Count",
					PerformanceAnalytics.Instance.RenderMetrics.Batches.CurrentValue),
				InfoEntry.Create("Vertices Count",
					PerformanceAnalytics.Instance.RenderMetrics.Vertices.CurrentValue),
				InfoEntry.Create("Draw Calls Count",
					PerformanceAnalytics.Instance.RenderMetrics.DrawCalls.CurrentValue),
				InfoEntry.Create("Shadow Casters Count",
					PerformanceAnalytics.Instance.RenderMetrics.ShadowCaster.CurrentValue),
				InfoEntry.Create("SetPass Calls Count",
					PerformanceAnalytics.Instance.RenderMetrics.SetPassCalls.CurrentValue),
			};
		}

		#endregion


		private void CreateDefaultSet()
		{
			info.Add("System", GetSystemSet());
			if (SystemInfo.batteryStatus != BatteryStatus.Unknown)
			{
				info.Add("Battery", GetBatterySet());
			}
			info.Add("Unity", GetUnitySet());
			info.Add("Display", GetDisplaySet());
			info.Add("Runtime", GetRuntimeSet());
			IList<InfoEntry> buildSet = GetBuildSet();
			if (buildSet != default)
			{
				info.Add("Build", buildSet);
			}
			info.Add("Features", GetFeaturesSet());
			#if UNITY_IOS
			info.Add("iOS", GetIOsSet());
			#endif
			info.Add("Graphics - Device", GetGraphicsDeviceSet());
			info.Add("Graphics - Features", GetGraphicsFeaturesSet());
		}
	}
}
