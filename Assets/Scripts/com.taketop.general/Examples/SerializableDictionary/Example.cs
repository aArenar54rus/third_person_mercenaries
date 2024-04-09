using UnityEngine;
using Module.General;


public class Example : MonoBehaviour
{
	public SerializableDictionary<string, string> names;
	public SerializableDictionary<string, GameObject> gameObjects;
	public SerializableDictionary<string, Color> colors;
	public SerializableDictionary<GameObject, Color> custom;
}
