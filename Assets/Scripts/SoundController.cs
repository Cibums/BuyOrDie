using System;
using UnityEngine;

[Serializable]
public class SoundEffect
{

    public SoundEffectType Effect;
    public AudioClip Clip;
}

public class SoundController : MonoBehaviour
{
    public SoundEffect[] SoundEffects;
    public AudioClip BackgroundMusic;
    public AudioSource MusicSource;

    public static SoundController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        MusicSource.clip = BackgroundMusic;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void PlaySoundEffect(SoundEffectType effectType)
    {
        SoundEffect soundEffect = Array.Find(SoundEffects, s => s.Effect == effectType);
        if (soundEffect != null && soundEffect.Clip != null)
        {
            AudioSource.PlayClipAtPoint(soundEffect.Clip, Camera.main.transform.position, 1.0f * (GameController.Instance.State.Settings.MasterVolume / 100f));
        }
        else
        {
            Debug.LogWarning("Sound effect not found or clip is null: " + effectType);
        }
    }
}

public enum SoundEffectType
{
    CashRegister,
    ItemBought,
    NewCustomer,
    Step,
    Talk_Male,
    Talk_Female,
    GameOver
}
