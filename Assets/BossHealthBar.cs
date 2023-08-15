using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossHealthBar : MonoBehaviour
{
    public Slider activeSlider;
    public Slider reserveSlider;
    public Text timerText;

    public PhasedHealth target;

    public float transitionTime = 0.2f;
    public Color activeTimeColor = Color.white;
    public Color inactiveTimeColor = Color.gray;

    [SerializeField] RectTransform uiRect;
    [SerializeField] float hideHeight = 30;

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        if (activeSlider != null) {
            activeSlider.value = target.ActiveHealthFrac();
        }
        if (reserveSlider != null) {
            reserveSlider.value = target.ReserveHealthFrac();
        }
        if (timerText != null) {
            timerText.text = target.timeLeft.ToString("###.000");
            timerText.color = (target.TimerIsActive() ? activeTimeColor : inactiveTimeColor);
        }
        //Debug.Log($"{target.ActiveHealthFrac()}-{target.ReserveHealthFrac()}");
    }

    void OnEnable() {
        Show();
    }

    void DisableThis() {
        gameObject.SetActive(false);
    }

    public void Hide() {
        LeanTween.moveY(uiRect, hideHeight, transitionTime).setEase(LeanTweenType.easeInQuint).setOnComplete(DisableThis);
    }

    public void Show() {
        uiRect.anchoredPosition = new Vector2(uiRect.anchoredPosition.x, hideHeight);
        //uiRect.localPosition = new Vector3(uiRect.localPosition.x, hideHeight, uiRect.localPosition.z);
        LeanTween.moveY(uiRect, 0, transitionTime).setEase(LeanTweenType.easeOutSine);
        
    }
}
