using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChargeBar : MonoBehaviour
{
    [SerializeField] private MVest.FloatReference ChargeLevel = new MVest.FloatReference();
    [SerializeField] private MVest.IntReference CurrentlyCharged = new MVest.IntReference();
    [SerializeField] private List<Slider> sliders;


    private void UpdateFill() {

        int fullMeters = CurrentlyCharged;
        for (int i = 0; i < fullMeters && i < sliders.Count; i++) {
            sliders[i].value = 1;
        }
        if (fullMeters >= sliders.Count)
            return;
        sliders[fullMeters].value = ChargeLevel;
        for (int i = fullMeters + 1; i < sliders.Count; i++) {
            sliders[i].value = 0;
        }
    }

    void Update()
    {
        UpdateFill();
    }
}
