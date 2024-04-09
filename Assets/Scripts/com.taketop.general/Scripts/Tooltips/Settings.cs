using System;
using UnityEngine;


namespace Module.General.Tooltips
{
    [Serializable]
    public struct Settings
    {
        public AutoSizeSettings autosize;
        public float textSize;
        public Vector2 size;
        public Vector2 pivot;
        public float targetOffset;
        public float screenOffset;
    }


    public enum AutoSizeSettings
    {
        None,
        RectSize,
        TextSize,
        Custom
    }


    public enum Orientation
    {
        Above,
        Below,
        Left,
        Right
    }
}
