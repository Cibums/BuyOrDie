using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundEffect
{

    public SoundEffectType Effect;
    public AudioClip[] Clips;

    public AudioClip GetRandomClip()
    {
        if (Clips == null || Clips.Length == 0)
        {
            return null;
        }
        int index = UnityEngine.Random.Range(0, Clips.Length);
        return Clips[index];
    }
}

public class SoundController : MonoBehaviour
{
    public SoundEffect[] SoundEffects;
    public AudioClip BackgroundMusic;
    public AudioSource MusicSource;

    [SerializeField] private AudioMixer sfxMixer;

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

    public void PlayTalkSound()
    {
        float pitch = GameController.Instance.State.CurrentCustomer?.VoicePitch ?? 1f;
        PlaySoundEffectWithPitch(SoundEffectType.Talk, pitch);
    }

    public void PlaySoundEffectWithPitch(SoundEffectType effectType, float semitones = 1f)
    {
        SoundEffect soundEffect = Array.Find(SoundEffects, s => s.Effect == effectType);
        if (soundEffect == null || soundEffect.Clips == null) return;

        AudioClip clip = soundEffect.GetRandomClip();
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio_" + effectType);
        tempGO.transform.position = Camera.main.transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.outputAudioMixerGroup = sfxMixer.FindMatchingGroups("SFX")[0];
        aSource.clip = clip;
        aSource.volume = GameController.Instance.State.Settings.MasterVolume / 100f;
        aSource.Play();

        sfxMixer.SetFloat("MyExposedParam", semitones);

        Destroy(tempGO, clip.length);
    }

    public void PlaySoundEffect(SoundEffectType soundEffect)
    {
        SoundEffect soundEffectObj = Array.Find(SoundEffects, s => s.Effect == soundEffect);
        
        if (soundEffectObj == null || soundEffectObj.Clips == null) return;
        if (soundEffectObj.Clips.Length == 0) return;

        AudioSource.PlayClipAtPoint(soundEffectObj.GetRandomClip(), Camera.main.transform.position, 1.0f * (GameController.Instance.State.Settings.MasterVolume / 100f));
    }
}

public enum SoundEffectType
{
    CashRegister,
    ItemBought,
    NewCustomer,
    Step,
    Talk,
    GameOver
}
