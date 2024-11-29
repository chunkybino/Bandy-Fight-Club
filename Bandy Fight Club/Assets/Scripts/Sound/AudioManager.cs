using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public SoundsListSO soundList;

    public List<Sound> sounds = new List<Sound>();

    public string playName;
    public bool play;

    void Update()
    {
        if (play)
        {
            play = false;
            PlaySound(playName);
        }
    }

    Sound AddSound(SoundData so)
    {
        GameObject g = new GameObject(so.name);
        g.transform.SetParent(transform);
        Sound s = g.AddComponent<Sound>();
        s.soundList = soundList;
        s.SO = so;
        sounds.Add(s);
        return s;
    }

    public void PlaySound(string name, float volume = 1, float pitch = 1)
    {
        Sound s = FindSoundByName(name);

        if (!s) //if we dont already have a sound obhjcet made, searhc teh big sound list and make a new object
        {
            SoundData so = soundList.FindByName(name);
            if (so != null)
            {
                s = AddSound(so);
            }
        }

        if (!s)
        {
            Debug.LogWarning("sound aint exists");
            return;
        }

        s.Play(volume, pitch);
    }

    Sound FindSoundByName(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.SO.name == name) return s;
        }
        return null;
    }
}