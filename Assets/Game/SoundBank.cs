using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundBank", menuName = "Custom/SoundBank", order = 0)]
public class SoundBank : ScriptableObject {
    public List<AudioClip> sounds;
}
