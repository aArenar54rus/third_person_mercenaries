using System;
using UnityEngine;


namespace Module.General
{
    public class InheritorTypeAttribute : PropertyAttribute
    {
        public readonly Type type;


        public InheritorTypeAttribute(Type type)
        {
            this.type = type;
        }
    }
}
