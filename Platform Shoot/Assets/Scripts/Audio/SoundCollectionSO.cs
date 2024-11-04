using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class SoundCollectionSO : ScriptableObject
{
    [Header("Muisic")]
    public SoundSO[] FightMusic;
    public SoundSO[] DiscoParty;

    [Header("SFX")]
    public SoundSO[] GunShoot;
    public SoundSO[] Jump;
    public SoundSO[] Splat;
}
