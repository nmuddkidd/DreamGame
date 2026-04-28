using UnityEngine;
using System.Collections.Generic;
using System;

public class sfxlogic : MonoBehaviour
{
    public AudioSource audioSource;
    [Serializable]
    public struct MyDictionaryEntry
    {
        public string key;
        public AudioClip value;
    }
    public List<MyDictionaryEntry> inspectorList;

    private Dictionary<string, AudioClip> music = new Dictionary<string, AudioClip>();
    [Header("Common Ambiance")]
    public AudioClip[] cambiance;
    [Header("Rare Ambiance")]
    public AudioClip[] rambiance;


    void Start(){
        foreach (var entry in inspectorList)
        {
            /*if (string.IsNullOrWhiteSpace(entry.key) || entry.value == null)
            {
                continue;
            }*/
            music[entry.key] = entry.value;
            Debug.Log(entry.key+ " " +entry.value);
            Debug.Log(music[entry.key]);
        }

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    void Update(){

    }

    public void changeBackground(string song){
        /*(if (audioSource == null)
        {
            Debug.LogWarning("sfxlogic.changeBackground: AudioSource is not assigned.");
            return;
        }

        if (string.IsNullOrWhiteSpace(song))
        {
            Debug.LogWarning("sfxlogic.changeBackground: requested track key was empty.");
            return;
        }

        AudioClip clip;
        if (!music.TryGetValue(song, out clip) || clip == null)
        {
            Debug.LogWarning("sfxlogic.changeBackground: missing track key '" + song + "'. Add it to inspectorList.");
            return;
        }

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }*/
        foreach(var key in music){
            Debug.Log(key.Key + " "+key.Value);
        }
        audioSource.clip = music[song];
        audioSource.Play();
    }

    public void ambiant(){

    }
}
