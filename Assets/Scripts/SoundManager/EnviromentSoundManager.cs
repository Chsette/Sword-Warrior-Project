using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnviromentSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] enviromentSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = enviromentSound[0];
        audioSource.Play();
    }
}
