using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using FMODUnity;

public enum SoundType
{
    FishPickup,
     RunningWater, 
     FishStart,
}

//[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] SoundList[] soundlist;

    private void OnEnable()
    {
        string[] audionames = Enum.GetNames(typeof(SoundType));

        if (soundlist == null || soundlist.Length != audionames.Length)
        {
            Array.Resize(ref soundlist, audionames.Length);
        }

        for (int i = 0; i < audionames.Length; i++)
        {
            soundlist[i].name = audionames[i];
        }
    }

    void Awake()
    {
        instance = this;
    }

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        public EventReference[] sounds;
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        int index = (int)sound;

        EventReference[] events = instance.soundlist[index].sounds;

        if (events.Length == 0)
            return;

        RuntimeManager.PlayOneShot(events[0]);
    }
}
