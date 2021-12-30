using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private MVest.FloatVariable shieldFill;
    // [SerializeField] private ShieldVariable shield;
    [SerializeField] private Slider slider;

    private void UpdateFill() {


        if (shieldFill != null) {
            slider.value = shieldFill;
        }
    }

    void Update()
    {
        UpdateFill();
    }
}
