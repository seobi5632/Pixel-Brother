using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    private static SoundManager soundManager;
    public static SoundManager instance
    {
        get
        {
            if (soundManager == null)
                soundManager = FindObjectOfType<SoundManager>();
            return soundManager;
        }
    }

    public string[] playSoundName;

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBGM;

    public Sound[] effectSounds;
    public Sound[] BGMSounds;

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("All sound is Playing");
                return;
            }
        }
        Debug.Log(_name + "is not matched");
    }

    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
        Debug.Log("Sound all stop");
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("Sound is stop");
    }

    public void SetMusicVolume(float volume)
    {
        audioSourceBGM.volume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].volume = volume;
        }
    }
}
