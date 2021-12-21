using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private Shield shield;
    [SerializeField] private Slider slider;

    private void UpdateFill() {
        if (shield == null)
            slider.value = 0;
        float fill = shield.CurrentCharge / shield.maximumCharge;
        slider.value = fill;
    }

    void Update()
    {
        UpdateFill();
    }
}
