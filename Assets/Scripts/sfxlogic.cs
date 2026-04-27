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
        audioSource.Play();
        foreach (var entry in inspectorList)
        {
            music[entry.key] = entry.value;
        }
    }

    void Update(){

    }

    public void changeBackground(string song){
        if(music[song]!=null){
            audioSource.clip = music[song];
            audioSource.Play();
        }
    }

    public void ambiant(){

    }
}
