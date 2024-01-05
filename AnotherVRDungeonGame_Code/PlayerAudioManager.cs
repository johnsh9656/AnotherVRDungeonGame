using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public static PlayerAudioManager Instance;

    // UI
    [Header("UI")]
    public AudioClip uiHover;
    public AudioClip uiSelect;

    // Player
    [Header("Player")]
    public AudioClip footsteps;
    public AudioClip climbGrab;
    public AudioClip weaponGrab;
    public AudioClip slowTime;
    public AudioClip jump;
    public AudioClip falling;
    public AudioClip spawn;
    public AudioClip death;
    public AudioClip hookReelIn;
    public AudioClip hookContact;
    public AudioClip hookRelease;

    // pickups
    [Header("Pickups")]
    public AudioClip healthPickup;
    public AudioClip speedPickup;
    public AudioClip deathPickup;

    AudioSource source;

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound);
    }
}
