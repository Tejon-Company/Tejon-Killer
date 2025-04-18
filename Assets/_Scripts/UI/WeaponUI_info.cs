using UnityEngine;
using TMPro;

public class WeaponUI_info : MonoBehaviour
{
    public TMP_Text currentBullets;
    public TMP_Text totalBullets;


    void OnEnable()
    {
        EventManager.current.updateBulletsEvent.AddListener(UpdateBullets);
    }
    void OnDisable()
    {
        EventManager.current.updateBulletsEvent.RemoveListener(UpdateBullets);
    }
    public void UpdateBullets(int newCurrentBullets, int newTotalBullets)
    {
        if(newCurrentBullets<=0){
            currentBullets.color = new Color(0.75f,0,0);
        }
        else if(1<=newCurrentBullets && newCurrentBullets<=3){
            currentBullets.color = new Color(255f, 165f, 0);
        }else{
            currentBullets.color = Color.white;
        }
        currentBullets.text=newCurrentBullets.ToString();
        totalBullets.text=newTotalBullets.ToString();
    }
}
