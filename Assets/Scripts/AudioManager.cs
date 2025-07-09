using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public AudioSource audioSource;
    public List<AudioClip> audioClipList;

    public AudioClip GetSound(string name) {
        if (audioClipList.Count > 0) {
            foreach (AudioClip clip in audioClipList) {
                if (clip.name.Equals(name)) {
                    return clip;
                }
            }
        }

        return null;
    }

    public AudioClip GetRandomSound(string name) {
        List<AudioClip> soundList = new List<AudioClip>();

        if (audioClipList.Count > 0) {
            foreach (AudioClip clip in audioClipList) {
                if (clip.name.StartsWith(name)) {
                    soundList.Add(clip);
                }
            }
        }

        if (soundList.Count > 0) {
            AudioClip selectedClip = soundList[Random.Range(0, soundList.Count)];
            return selectedClip;
        }

        return null;
    }

    public void RandomPlaySound(string name) {
        var clip = GetRandomSound(name);

        if (clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySound(string name) {
        if (GetSound(name) != null) {

            AudioClip clip = GetSound(name);

            if (clip != null) {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}