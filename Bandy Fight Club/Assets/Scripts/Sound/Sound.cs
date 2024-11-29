using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public SoundsListSO soundList;
    public int dataIndex;
    public SoundData SO;

    public AudioSource source;

    public void StartFunc()
    {
        if (!source) source = GetComponent<AudioSource>();
        if (!source) source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;

        for (int i = 0; i < soundList.sounds.Length; i++)
        {
            if (soundList.sounds[i].name == SO.name)
            {
                dataIndex = i;
                break;
            }
        }
    }

    public void Play(float volume = 1, float pitch = 1)
    {
        SO = soundList.sounds[dataIndex];

        AudioClip c = SO.clip;
        if (SO.doRandomSound)
        {
            if (SO.clips.Length > 0)
            {
                c = SO.clips[Random.Range(0, SO.clips.Length)];
            } 
        }

        float v = SO.volume;
        if (SO.doRandomVolume) v += Random.Range(SO.randVolumeRange.x, SO.randVolumeRange.y);
        v *= volume;

        float p = SO.pitch;
        if (SO.doRandomPitch) p += Random.Range(SO.randPitchRange.x, SO.randPitchRange.y);
        p *= pitch;

        source.clip = c;
        source.volume = v;
        source.pitch = p;

        source.Play();
    }
}
