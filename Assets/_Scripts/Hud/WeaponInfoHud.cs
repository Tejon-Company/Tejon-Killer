using System.Collections;
using _Scripts.Managers;
using TMPro;
using UnityEngine;

namespace _Scripts.Hud
{
    public class WeaponInfoHud : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text currentBullets;

        [SerializeField]
        private TMP_Text totalBullets;

        private void OnEnable()
        {
            StartCoroutine(SubscribeToBulletUpdate());
        }

        private IEnumerator SubscribeToBulletUpdate()
        {
            yield return null;
            EventManager.Instance.updateBulletsEvent.AddListener(UpdateBullets);
        }

        private void OnDisable()
        {
            EventManager.Instance.updateBulletsEvent.RemoveListener(UpdateBullets);
        }

        public void UpdateBullets(int newCurrentBullets, int newTotalBullets)
        {
            currentBullets.color = newCurrentBullets switch
            {
                <= 0 => new Color(0.75f, 0, 0),
                <= 3 => new Color(1f, 0.647f, 0),
                _ => Color.white,
            };
            currentBullets.text = newCurrentBullets.ToString();
            totalBullets.text = newTotalBullets.ToString();
        }
    }
}
