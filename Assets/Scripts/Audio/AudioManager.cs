using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds, winJingles, loseJingles;
    public AudioSource musicSource,  sfxSource, jingleSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    public void PlayMusic(string soundName)
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == soundName);
        if (s != null) {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySfx(string soundName)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == soundName);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
    
    public void RandomJingle(Sound[] jingleArray)
    {
        int random = UnityEngine.Random.Range(0, jingleArray.Length);
        Sound jingle = jingleArray[random];

        StartCoroutine(PlayJingleCoroutine(jingle.clip));
    }
    
    public void PlayWinJingle() => RandomJingle(winJingles);
    
    public void PlayLoseJingle() => RandomJingle(loseJingles);


    private IEnumerator PlayJingleCoroutine(AudioClip jingleClip)
    {
        musicSource.Pause();
        jingleSource.PlayOneShot(jingleClip);
        yield return new WaitForSeconds(jingleClip.length);
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void GunshotSFX(string SFX, Vector3 position)
    {
        Sound gunshot = Array.Find(sfxSounds, sound => sound.name == SFX);
        if (gunshot != null)
            AudioSource.PlayClipAtPoint(gunshot.clip, position);
    }
    
    public void PauseAudio()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in sources)
            source.Pause();
    }

    public void ResumeAudio()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in sources)
            source.UnPause();
    }
}