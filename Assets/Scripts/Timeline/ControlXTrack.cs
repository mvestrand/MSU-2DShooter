// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEngine.Timeline;

// namespace MVest.Unity
// {
//     /// <summary>
//     /// A Track whose clips control time-related elements on a GameObject.
//     /// </summary>
//     [TrackClipType(typeof(ControlXPlayableAsset), false)]
//     [ExcludeFromPreset]
//     //[TimelineHelpURL(typeof(ControlXTrack))]
//     public class ControlXTrack : TrackAsset
//     {
// #if UNITY_EDITOR
//         private static readonly HashSet<PlayableDirector> s_ProcessedDirectors = new HashSet<PlayableDirector>();

//         /// <inheritdoc/>
//         public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
//         {
//             if (director == null)
//                 return;

//             // avoid recursion
//             if (s_ProcessedDirectors.Contains(director))
//                 return;

//             s_ProcessedDirectors.Add(director);

//             var particlesToPreview = new HashSet<ParticleSystem>();
//             var activationToPreview = new HashSet<GameObject>();
//             var timeControlToPreview = new HashSet<MonoBehaviour>();
//             var subDirectorsToPreview = new HashSet<PlayableDirector>();

//             foreach (var clip in GetClips())
//             {
//                 var controlPlayableAsset = clip.asset as ControlXPlayableAsset;
//                 if (controlPlayableAsset == null)
//                     continue;

//                 var gameObject = controlPlayableAsset.sourceGameObject.Resolve(director);
//                 if (gameObject == null)
//                     continue;

//                 if (controlPlayableAsset.updateParticle)
//                     particlesToPreview.UnionWith(gameObject.GetComponentsInChildren<ParticleSystem>(true));
//                 if (controlPlayableAsset.active)
//                     activationToPreview.Add(gameObject);
//                 if (controlPlayableAsset.updateITimeControl)
//                     timeControlToPreview.UnionWith(ControlXPlayableAsset.GetControlableScripts(gameObject));
//                 if (controlPlayableAsset.updateDirector)
//                     subDirectorsToPreview.UnionWith(controlPlayableAsset.GetComponent<PlayableDirector>(gameObject));
//             }

//             ControlXPlayableAsset.PreviewParticles(driver, particlesToPreview);
//             ControlXPlayableAsset.PreviewActivation(driver, activationToPreview);
//             ControlXPlayableAsset.PreviewTimeControl(driver, director, timeControlToPreview);
//             ControlXPlayableAsset.PreviewDirectors(driver, subDirectorsToPreview);

//             s_ProcessedDirectors.Remove(director);

//             particlesToPreview.Clear();
//             activationToPreview.Clear();
//             timeControlToPreview.Clear();
//             subDirectorsToPreview.Clear();
//         }

// #endif
//     }
// }
