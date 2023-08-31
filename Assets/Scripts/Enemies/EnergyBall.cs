using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public float playSpeed = 1;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator animator;

    // Update is called once per frame
    void Update() {
        if (Time.timeScale == 0) {
            audioSource.pitch = 0;
        }
        else {
            audioSource.pitch = playSpeed;
            animator.speed = playSpeed;
        }
    }
}
