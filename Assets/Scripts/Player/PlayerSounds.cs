using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] playerSoundClips;

    private void PlayWalkSound()
    {
        SoundManager.instance.PlaySoundClip(playerSoundClips[0]);
    }

    public void PlayJumpSound()
    {
        SoundManager.instance.PlaySoundClip(playerSoundClips[1]);
    }

    public void PlayAttackSound(AudioClip audio)
    {
        SoundManager.instance.PlaySoundClip(audio);
    }
}
