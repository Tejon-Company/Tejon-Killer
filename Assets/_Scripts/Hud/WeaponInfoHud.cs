using _Scripts.Managers;
using TMPro;
using UnityEngine;

namespace _Scripts.Hud
{
    public class WeaponInfoHud : MonoBehaviour
    {
        private TMP_Text _currentBullets;
        private TMP_Text _totalBullets;

        private void OnEnable()
        {
            EventManager.Current.updateBulletsEvent.AddListener(UpdateBullets);
        }

        private void OnDisable()
        {
            EventManager.Current.updateBulletsEvent.RemoveListener(UpdateBullets);
        }

        public void UpdateBullets(int newCurrentBullets, int newTotalBullets)
        {
            _currentBullets.color = newCurrentBullets switch
            {
                <= 0 => new Color(0.75f, 0, 0),
                <= 3 => new Color(255f, 165f, 0),
                _ => Color.white,
            };
            _currentBullets.text = newCurrentBullets.ToString();
            _totalBullets.text = newTotalBullets.ToString();
        }
    }
}
