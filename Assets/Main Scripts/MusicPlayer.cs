 using UnityEngine;
 using System.Collections;
 
 public class MusicPlayer : MonoBehaviour //A singleton class to play music.
 {
    private AudioClip[] musicTracks; //Preload all music tracks to prevent lag when loading.
    private static MusicPlayer instance = null;
    public static MusicPlayer Instance
    {
        get 
        {
        if(!instance) //No instance... Must've skipped the splash screen (editor). Create a new music player object.
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/MusicPlayer")); 
        }

        return instance; 
    }
    }

    private AudioSource audioSource;

    void Awake()
    {
        musicTracks = Resources.LoadAll<AudioClip>("Audios");
        
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            audioSource.enabled = true; //Enable the object. It is disabled on start just to be safe.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string theme)
    {
        if(audioSource.clip.name.Equals(theme)) //Theme already playing. Do not play again
            return;

        foreach(AudioClip clip in musicTracks) //Can also convert it to a hashmap later.
        {
            if (clip.name.Equals(theme))
            {
                audioSource.clip = Resources.Load<AudioClip>("Audios/" + theme);
                audioSource.Play();
                break;
            }
        }

    }
 }