using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DamageScreen : MonoBehaviour
{
    [SerializeField] Image bloodScreenImg;
    [SerializeField] float fadeOutDuration = 2f;
    [SerializeField] float fadeOutValue = -.01f;


    public void FadeIn(float value, float duration)
    {
        bloodScreenImg.DOFade(value, duration);
    }
    public void FadeOut(float value, float duration)
    {
        bloodScreenImg.DOFade(value, duration);
    }

    void Update()
    {
        FadeOut(fadeOutValue, fadeOutDuration);
    }
}
