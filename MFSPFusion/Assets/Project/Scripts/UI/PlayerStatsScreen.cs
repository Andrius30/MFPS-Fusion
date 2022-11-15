using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsScreen : MonoBehaviour
{
    [SerializeField] Image gunImg;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI healthPercentText;


    public void SetAmmo(int currentAmount, int maxAmount)
    {
        ammoText.text = $"{currentAmount}/{maxAmount}";
    }
    public void SetHealthStats(float currentHealth, float maxhealth)
    {
        healthSlider.value = currentHealth / maxhealth;
        healthPercentText.text = $"{currentHealth / maxhealth * 100f:00}%";
    }
    public void SetGunIcon(Sprite gunIcon) => gunImg.sprite = gunIcon;
}
