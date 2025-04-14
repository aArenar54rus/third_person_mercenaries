using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar
{
    public class FirearmWeaponClipComponent : IClipComponent
    {
        private int _clipSize;
        private float _reloadProcessTime;
        private Tween _reloadTween;

        
        public int ClipSizeMax { get; }
        
        public int ClipSize
        {
            get => _clipSize;
            set => _clipSize = IsInfinityClip ? ClipSizeMax : Mathf.Clamp(value, 0, ClipSizeMax);
        }
        
        public bool IsReloadProcess { get; private set; } = false;
        
        public bool IsInfinityClip { get; }


        public FirearmWeaponClipComponent(bool isInfinityClip,
                                          int clipSizeMax,
                                          int clipSize,
                                          float reloadProcessTime)
        {
            ClipSizeMax = clipSizeMax;
            ClipSize = clipSize;
            IsInfinityClip = isInfinityClip;
            _reloadProcessTime = reloadProcessTime;
        }


        public void Reload()
        {
            if (IsInfinityClip)
                return;
            
            if (IsReloadProcess)
                return;

            ClipSize = ClipSizeMax;
            IsReloadProcess = false;
        }
    }
}