using System;
using UnityEngine;


public enum SoundType
{
    Example, Example2,

}
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        public AudioClip[] sounds;

    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {


        int index = (int)sound;



        AudioClip[] clips = instance.soundlist[index].sounds;



        instance.audioSource.PlayOneShot(clips[0], Mathf.Clamp01(volume));
    }

   
}
