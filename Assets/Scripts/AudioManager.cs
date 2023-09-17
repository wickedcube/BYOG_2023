using System.Security.Cryptography;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum ClipTypes
    {
        DoorOpen,
        Crash,
        ChatBoxNext,
        GamePlayBG,
        GamePlayBG_2,
        GamePlayBG_3,
        Translated,
        Click
    }
    
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AudioManager>();

            if (instance == null)
                instance = Instantiate(Resources.Load<GameObject>("AudioManager")).GetComponent<AudioManager>();
            
            return instance;
        }
    }

    [SerializeField] private AudioSource bgSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private ClipTypes startingBgClip;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void LoadAudioManagerIfNotFound()
    {
        var ins = Instance; //just to load the manager :p
    }
    
    private void Awake()
    {
        if(instance != null && instance != this)
            Destroy(this.gameObject);
        
        DontDestroyOnLoad(this.gameObject);
        PlayBG(startingBgClip);
    }

    private AudioClip GetClip(string fileName)
    {
        return Resources.Load<AudioClip>($"Audio/{fileName}");
    }
    
    public void PlayBG(ClipTypes types)
    {
        var clip = GetClip(types.ToString());
        if (clip == null)
            return;

        bgSource.clip = clip;
        bgSource.Play();
    }
    
    public void PlaySFX(ClipTypes types)
    {
        var clip = GetClip(types.ToString());
        if (clip == null)
            return;
       sfxSource.PlayOneShot(clip);
    }
}
