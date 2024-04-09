using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Module.General
{
	public class GUILogs : MonoBehaviour
	{
		const float P_DEL = 50f;
		const float L_DEL = 25f;
		const float LEN_KOEF = 2f;
		private int size;
		private int maxLenght;

		private List<string> messages;
		private Vector2 scrollPosition;


		public void SetMessages(List<string> messages)
		{
			this.messages = messages;
		}


		public void AddMessage(string message)
		{
			messages.Add(message);
		}


		private void Start()
		{
			float ratio = (float)Screen.width / Screen.height;
			float sizeKoef = Mathf.Lerp(P_DEL, L_DEL, Mathf.InverseLerp(0.5f, 2f, ratio));
			size = (int)(Screen.height / sizeKoef);
			maxLenght = (int)(LEN_KOEF * Screen.width / size) - 1;
		}


		private void OnGUI()
		{
			if (messages == null || messages.Count == 0)
			{
				return;
			}

			GUI.skin.textField.fontSize = size;
			GUI.skin.button.fontSize = size;

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Copy all to Clipboard"))
			{
				var sb = new StringBuilder();
				foreach (string message in messages)
				{
					sb.AppendLine(message);
					sb.AppendLine();
				}
				sb.ToString().CopyToClipboard();
			}
			if (GUILayout.Button("Close"))
			{
				Destroy(gameObject);
				return;
			}
			GUILayout.EndHorizontal();


			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();

			foreach (string message in messages)
			{
				string msg = message;
				int nextBreakIndex = maxLenght;
				for (var k = 0; k < msg.Length; k++)
				{
					if (msg[k] == '\n')
					{
						nextBreakIndex = k + maxLenght;
						continue;
					}

					if (k == nextBreakIndex)
					{
						msg = msg.Insert(k, " \n");
						k += 2;
						nextBreakIndex = k + maxLenght + 1;
					}
				}
				GUILayout.TextField(msg);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}
}
