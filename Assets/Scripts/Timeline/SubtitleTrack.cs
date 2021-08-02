using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using TMPro;

[TrackColor(0.2f, 0.4f, 0.6f)]
[TrackClipType(typeof(SubtitleClip))]
[TrackBindingType(typeof(TextMeshProUGUI))]
public class SubtitleTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SubtitleMixerBehaviour>.Create(graph, inputCount);
    }
}
