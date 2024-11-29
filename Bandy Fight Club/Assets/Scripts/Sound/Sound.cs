using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public SoundsListSO soundList;
    public int dataIndex;
    public SoundData SO;

    public AudioSource source;

    void Awake()
    {
        if (!source) source = GetComponent<AudioSource>();
        if (!source) source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.clip = SO.clip;
        source.volume = SO.volume;
        source.pitch = SO.pitch;

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

//[CreateAssetMenu(fileName = "SoundSO", menuName = "ScriptableObjects/Sound")]
[System.Serializable]
public class SoundData
{
    public string name;

    public AudioClip clip;
    [Range(0f,1f)] public float volume = 0.5f;
    [Range(0f,3f)] public float pitch = 1;

    public bool doRandomSound;
    public AudioClip[] clips;

    public bool doRandomVolume;
    public Vector2 randVolumeRange;

    public bool doRandomPitch;
    public Vector2 randPitchRange;
}

[CreateAssetMenu(fileName = "SoundListSO", menuName = "ScriptableObjects/SoundList")]
public class SoundsListSO : ScriptableObject
{
    public SoundData[] sounds;

    public SoundData FindByName(string name)
    {
        foreach (SoundData s in sounds)
        {
            if (s.name == name) return s;
        }
        return null;
    }
}
