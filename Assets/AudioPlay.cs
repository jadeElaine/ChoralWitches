using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    public int maxVoices=1;
    public AudioClip AudioClip_A;

    private List<AudioSource> voices = new List<AudioSource>();
    
    private AudioSource GetBestAudioSource()
    {
        int furthestPlaybackSource = -1;
        for (int i = 0; i < voices.Count; ++i)
        {
            if (!voices[i].isPlaying)
            {
                return voices[i];
            }
            if (furthestPlaybackSource == -1 ||
                voices[i].time > voices[furthestPlaybackSource].time)
            {
                furthestPlaybackSource = i;
            } 
        }

        if (voices.Count < maxVoices)
        {
            GameObject voiceObj = new GameObject("Voice");
            AudioSource voice = voiceObj.AddComponent<AudioSource>();
            voiceObj.transform.SetParent(transform);
            voices.Add(voice);
            return voice;
        }

        return voices[furthestPlaybackSource];
    }

    public AudioSource PlaySemitone(int semitonesFromA, int tempermentWidth)
    {
        AudioSource voice = GetBestAudioSource();
        voice.clip = AudioClip_A;
        voice.pitch = Mathf.Pow(2.0f, (float) semitonesFromA/(float)tempermentWidth); 
        voice.Play();
        return voice;
    }

    public void KillVoice(AudioSource voice)
    {
        voice.Stop();
    }
}
