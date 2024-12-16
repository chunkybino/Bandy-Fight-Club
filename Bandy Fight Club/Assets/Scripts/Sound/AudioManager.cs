using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public SoundsListSO soundList;

    public List<Sound> sounds = new List<Sound>();

    public string playName;
    public bool play;

    [SerializeField] AudioSource musicSource;
    [SerializeField] float musicVolume = 1;

    [SerializeField] float fadeOutMusicTime = 1;
    float fadeOutMusicTimer = -1;

    public bool musicPlaying;

    public float musicTimer;
    public float musicBPM;
    public float musicBeatCounter;

    void Start()
    {
        if (musicSource)
        {
            musicSource.volume = 0;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (play) 
        {
            play = false;
            Play(playName);
        }

        if (!musicSource) return;

        if (fadeOutMusicTimer > 0)
        {
            UFunc.TickTimer(ref fadeOutMusicTimer);

            float t = fadeOutMusicTimer / fadeOutMusicTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume, t);
        }
        else if (fadeOutMusicTimer == 0)
        {
            musicSource.Stop();
            fadeOutMusicTimer = -1;
            musicPlaying = false;
        }

        if (musicPlaying)
        {
            musicTimer += Time.deltaTime;

            if (musicBeatCounter*60/musicBPM < musicTimer)
            {
                musicBeatCounter++;
            }
        }
        else
        {
            musicTimer = 0;
            musicBeatCounter = 0;
        }
    }

    Sound AddSound(SoundData so)
    {
        GameObject g = new GameObject(so.name);
        g.transform.SetParent(transform);
        Sound s = g.AddComponent<Sound>();
        s.soundList = soundList;
        s.SO = so;
        s.StartFunc();
        sounds.Add(s);
        return s;
    }

    public void Play(string name, float volume = 1, float pitch = 1)
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

    public void PlayList(List<string> names, float volume = 1, float pitch = 1)
    {
        for (int i = 0; i < names.Count; i++)
        {
            Play(names[i], volume, pitch);
        }
    }

    Sound FindSoundByName(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.SO.name == name) return s;
        }
        return null;
    }

    public void StartMusic()
    {
        musicSource.volume = musicVolume;
        if (musicSource.isPlaying) musicSource.Stop();
        musicSource.Play();
        fadeOutMusicTimer = -1;

        musicPlaying = true;

        musicTimer = 0;
        musicBeatCounter = 0;
    }
    public void StopMusic()
    {
        musicSource.Stop();

        musicPlaying = false;
    }
    public void FadeOutMusic()
    {
        fadeOutMusicTimer = fadeOutMusicTime;

        musicPlaying = false;
    }
}