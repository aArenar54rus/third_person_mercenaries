using System;
using UnityEngine;

namespace Module.General.Editor
{
    public partial class ScriptableListDrawer
    {
        private static class TypeController<T> where T : ScriptableObject
        {
            private static readonly Type ProcessingType = typeof(T);

            private static string[] _names;
            private static Type[] _types;

            public static string[] Names
            {
                get
                {
                    if (_names == null)
                    {
                        int count = Types.Length;
                        _names = new string[count];
                        for (var i = 0; i < count; i++)
                        {
                            _names[i] = Types[i].Name;
                        }
                    }

                    return _names;
                }
            }

            private static Type[] Types
            {
                get
                {
                    if (_types == null)
                    {
                        _types = ProcessingType.GetAssignableTypes();
                    }

                    return _types;
                }
            }

            public static T CreateInstance(string typeName)
            {
                foreach (Type type in Types)
                {
                    if (type.Name == typeName)
                    {
                        ScriptableObject asset = ScriptableObject.CreateInstance(type);
                        return asset as T;
                    }
                }

                return null;
            }

            private static bool Validate(Type type)
            {
                return !type.IsAbstract && ProcessingType.IsAssignableFrom(type);
            }
        }
    }
}