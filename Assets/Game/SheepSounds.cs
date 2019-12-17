using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheepSounds", menuName = "Custom/SheepSounds", order = 0)]
public class SheepSounds : ScriptableObject {
    public List<AudioClip> sounds;
}
