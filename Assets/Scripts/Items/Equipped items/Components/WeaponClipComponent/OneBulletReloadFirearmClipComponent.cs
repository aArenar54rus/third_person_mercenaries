using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar
{
    public class OneBulletReloadFirearmClipComponent : IClipComponent
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
        
        public bool IsReloadProcess { get; private set; }
        
        public bool IsInfinityClip { get; }
        
        
        public OneBulletReloadFirearmClipComponent(bool isInfinityClip,
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

            IsReloadProcess = true;
            StartReloadOneBulletTween();
            

            void StartReloadOneBulletTween()
            {
                _reloadTween = DOVirtual.DelayedCall(_reloadProcessTime,() =>
                    {
                        ClipSize++;
                    }
                ).OnComplete(
                    () =>
                    {
                        if (ClipSize == ClipSizeMax)
                            IsReloadProcess = false;
                        else
                        {
                            StartReloadOneBulletTween();
                        }
                    });
            }
        }
    }
}