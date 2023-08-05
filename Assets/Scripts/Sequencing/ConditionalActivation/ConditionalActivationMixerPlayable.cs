using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


class ConditionalActivationMixerPlayable : PlayableBehaviour
{
    ConditionalActivationTrack.PostPlaybackState m_PostPlaybackState;
    bool m_BoundGameObjectInitialStateIsActive;

    private GameObject m_BoundGameObject;


    public static ScriptPlayable<ConditionalActivationMixerPlayable> Create(PlayableGraph graph, int inputCount)
    {
        return ScriptPlayable<ConditionalActivationMixerPlayable>.Create(graph, inputCount);
    }

    public ConditionalActivationTrack.PostPlaybackState postPlaybackState
    {
        get { return m_PostPlaybackState; }
        set { m_PostPlaybackState = value; }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_BoundGameObject == null)
            return;

        switch (m_PostPlaybackState)
        {
            case ConditionalActivationTrack.PostPlaybackState.Active:
                m_BoundGameObject.SetActive(true);
                break;
            case ConditionalActivationTrack.PostPlaybackState.Inactive:
                m_BoundGameObject.SetActive(false);
                break;
            case ConditionalActivationTrack.PostPlaybackState.Revert:
                m_BoundGameObject.SetActive(m_BoundGameObjectInitialStateIsActive);
                break;
            case ConditionalActivationTrack.PostPlaybackState.LeaveAsIs:
            default:
                break;
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (m_BoundGameObject == null)
        {
            m_BoundGameObject = playerData as GameObject;
            m_BoundGameObjectInitialStateIsActive = (m_BoundGameObject != null && m_BoundGameObject.activeSelf);
        }

        if (m_BoundGameObject == null)
            return;

        int inputCount = playable.GetInputCount();
        bool hasInput = false;
        for (int i = 0; i < inputCount; i++)
        {
            if (playable.GetInputWeight(i) > 0) {
                var inputPlayable = (ScriptPlayable<ConditionalActivationControlPlayable>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                if (input.isActive) {
                    hasInput = true;
                    break;
                }
            }
        }

        m_BoundGameObject.SetActive(hasInput);
    }
}
