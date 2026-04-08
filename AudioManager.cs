using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] playlist;
    public AudioSource audioSource;
    private int musicIndex = 0;
    public AudioMixerGroup soundEffectMixer;


    public static AudioManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de AudioManager dans la scène");
            Destroy(gameObject); // détruit le doublon au lieu de return
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // persiste entre les scènes
    }

    void Start()
    {
        // Démarre la musique uniquement si elle ne joue pas déjà
        if (!audioSource.isPlaying)
        {
            audioSource.clip = playlist[0];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            playNextSong();
        }
    }

    void playNextSong()
    {
        musicIndex = (musicIndex+1) % playlist.Length;
        audioSource.clip = playlist[musicIndex];
        audioSource.Play();
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = soundEffectMixer;
        audioSource.Play();
        Destroy(tempGO, clip.length);
        return audioSource;
    }
}

