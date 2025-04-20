using DG.Tweening;
using TMPro;
using UnityEngine;


namespace Arenar.UI
{
    public class MoneyWallet : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text moneyText;
        [SerializeField]
        private float scale = 1.1f;
        [SerializeField]
        private float duration = 0.5f;
        
        private Tween scaleTween;


        public void UpdateText(int money)
        {
            moneyText.text = money.ToString();
            
            this.transform.localScale = new Vector3(scale, scale, 1);
            scaleTween = this.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
            scaleTween.Play();
        }
        
        private void OnDestroy()
        {
            scaleTween?.Kill(false);
        }
    }
}