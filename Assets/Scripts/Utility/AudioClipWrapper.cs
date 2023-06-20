using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// [CreateAssetMenu(menuName ="AudioClipWrapper")]
// public class AudioClipWrapper : ScriptableObject {

//     [SerializeField]
//     AudioClip audioClip = null;
//     [SerializeField]
//     float timeDelay = 0.01f;

//     float lastPlayed = float.NegativeInfinity;

//     public void PlayAudio(AudioSource source, float volume) {
//         if (lastPlayed + timeDelay < Time.realtimeSinceStartup && audioClip != null && source != null) {
//             lastPlayed = Time.realtimeSinceStartup;
//             source.PlayOneShot(audioClip, volume);
//         }
//     }

//     void OnEnable() {
//         lastPlayed = float.NegativeInfinity;
//     }

// }
