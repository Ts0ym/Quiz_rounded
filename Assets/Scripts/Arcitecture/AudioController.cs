using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource _audioSource => GetComponent<AudioSource>();
    [SerializeField] private AudioClip _clickSound;

    public void PlayAudio(AudioClip clip, float volume = 1)
    {
         _audioSource.PlayOneShot(clip, volume);
    }

    public void PlayClickSound(float volume = 1)
    {
        PlayAudio(_clickSound, volume);
    }
}
