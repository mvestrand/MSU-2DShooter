using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LightControlMixerBehaviour : PlayableBehaviour {

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        Light trackBinding = playerData as Light;
        float finalIntensity = 0f;
        Color finalColor = Color.black;
        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<LightControlBehaviour>)playable.GetInput(i);
            var input = inputPlayable.GetBehaviour();

            finalIntensity += input.intensity * inputWeight;
            finalColor += input.color * inputWeight;
        }

        trackBinding.intensity = finalIntensity;
        trackBinding.color = finalColor;
    }
}
