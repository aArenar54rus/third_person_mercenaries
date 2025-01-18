using UnityEngine;

namespace Arenar
{
    public class LaserAimComponent : IEquippedItemAimComponent
    {
        private LineRenderer _lineRendererEffect;


        public LaserAimComponent(LineRenderer lineRendererEffect)
        {
            _lineRendererEffect = lineRendererEffect;
        }
        
        
        public void SetAimStatus(bool isAiming)
        {
            _lineRendererEffect.gameObject.SetActive(isAiming);
        }
        
        public void OnUpdate(Vector3 position)
        { 
            _lineRendererEffect.SetPosition(1, position);
        }
    }
}