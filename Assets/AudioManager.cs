using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental.GlobalIllumination;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;

    
    public TextMeshProUGUI musicText;

    public AudioSource musicSource;
    AudioLowPassFilter musicLowPass;

    public AudioSource normalImpactSource;
    public AudioSource explosionSource;
    public AudioSource boomSource;
    public AudioSource demonSource;
    public AudioSource angelSource;
    public AudioSource wallHitSource;

    public List<AudioClip> musicList;

    public List<AudioSource> effectsSources;

    public float musicVolume;

    bool isMuffled;

    public bool toMax;

    int index;

    public GameObject audioSettingsPanel;
    public Button audioSettingsButton;
    public Button menuSettingsButton;

    public Slider effectsSlider;
    public Slider musicSlider;

    public TextMeshProUGUI musicVolText;
    public TextMeshProUGUI effectsVolText;

    private Dictionary<AudioSource, float> maxVolumes = new Dictionary<AudioSource, float>();

    bool demonicFound = false;
    bool divineFound = false;

    float effectSliderPercent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        musicLowPass = musicSource.GetComponent<AudioLowPassFilter>();
        index = 0;
        musicSource.clip = musicList[index];
        musicSource.volume = musicVolume;
        AddToSourceDict();
        OnEffectsSliderChange();
        audioSettingsButton.onClick.AddListener(ShowSettingsPanel);
        LoadSettings();
        ShowMusicText();
    }

    public void AddToSourceDict()
    {
        foreach (AudioSource source in effectsSources)
        {
            maxVolumes[source] = source.volume;
        }
    }

    public void ShowSettingsPanel()
    {
        audioSettingsButton.onClick.RemoveAllListeners();
        audioSettingsButton.onClick.AddListener(HideSettingsPanel);
        menuSettingsButton.onClick.RemoveAllListeners();
        menuSettingsButton.onClick.AddListener(HideSettingsPanel);
        audioSettingsPanel.SetActive(true);
        if(EnemySpawnManager.Instance.localPlayer != null)
        {
            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = false;
        }
        
    }

    public void HideSettingsPanel()
    {
        audioSettingsButton.onClick.RemoveAllListeners();
        audioSettingsButton.onClick.AddListener(ShowSettingsPanel);
        menuSettingsButton.onClick.RemoveAllListeners();
        menuSettingsButton.onClick.AddListener(ShowSettingsPanel);
        audioSettingsPanel.SetActive(false);
        if (EnemySpawnManager.Instance.localPlayer != null)
        {
            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = true;
        }
        SaveSettings();
        
    }

    public void OnMusicSliderChange()
    {
        musicVolume = musicSlider.value;
        musicVolText.text = (musicVolume*100).ToString("F1") + "%";
        musicSource.volume = musicVolume;

    }

    public void OnEffectsSliderChange()
    {
        effectSliderPercent = effectsSlider.value;
        effectsVolText.text = (effectSliderPercent * 100).ToString("F1") + "%";

        foreach (AudioSource source in effectsSources)
        {
            if (maxVolumes.ContainsKey(source))
            {
                source.volume = maxVolumes[source] * effectSliderPercent;
            }
        }
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", effectSliderPercent);
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        }

        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            effectSliderPercent = PlayerPrefs.GetFloat("EffectsVolume", 1);
        }

        Debug.Log(musicVolume);
        Debug.Log(effectSliderPercent);
        SetSettings();
    }

    void SetSettings()
    {
        effectsSlider.value = effectSliderPercent;
        musicSlider.value = musicVolume;

        foreach (AudioSource source in effectsSources)
        {
            if (maxVolumes.ContainsKey(source))
            {
                source.volume = maxVolumes[source] * effectSliderPercent;
            }
        }

        musicSource.volume = musicVolume;
    }

    public void ActivateMusicLowPass()
    {
        isMuffled = true;
    }

    public void DeactivateMusicLowPass()
    {
        isMuffled = false;
    }

    public void MusicSmoothToMaxVolume()
    {
        toMax = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SmoothToMaxServerRpc()
    {
        SmoothToMaxClientRpc();
    }

    [ClientRpc]
    void SmoothToMaxClientRpc()
    {
        musicSource.volume = 0;
        toMax = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void MusicToZeroServerRpc()
    {
        MusicToZeroClientRpc();
    }

    [ClientRpc]
    void MusicToZeroClientRpc()
    {
        musicSource.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (demonicFound)
        {
            if(demonSource.time >= demonSource.clip.length)
            {
                SmoothToMaxServerRpc();
                demonicFound = false;
            }
        }

        if (divineFound)
        {
            if (angelSource.time >= angelSource.clip.length)
            {
                SmoothToMaxServerRpc();
                divineFound = false;
            }
        }


        if (audioSettingsPanel.activeSelf && EnemySpawnManager.Instance.localPlayer != null)
        {
            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = false;
        }

        if (toMax)
        {
            
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, musicVolume, Time.deltaTime*0.1f);
            if(musicSource.volume >= musicVolume)
            {
                toMax = false;
            }
        }


        if(musicSource.time >= musicSource.clip.length)
        {
            index++;
            if(index >= musicList.Count)
            {
                index = 0;
            }
            musicSource.clip = musicList[index];
            Debug.Log("music switch");
            ShowMusicText();
            musicSource.Play();
        }

        if (isMuffled)
        {
            musicLowPass.cutoffFrequency = Mathf.MoveTowards(musicLowPass.cutoffFrequency, 1308f, 5000f * Time.deltaTime);
        }
        else
        {
            musicLowPass.cutoffFrequency = Mathf.MoveTowards(musicLowPass.cutoffFrequency, 11000f, 5000f * Time.deltaTime);
        }
    }

    void ShowMusicText()
    {
        musicText.text = UnicodeHolder.MUSIC_NOTE + " Now Playing: " + musicSource.clip.name + " " + UnicodeHolder.MUSIC_NOTE;
        musicText.GetComponent<HideAfterTime>().ShowText(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayExplosionServerRpc(ulong clientId)
    {
        PlayExposionClientRpc(clientId);
    }

    [ClientRpc]
    void PlayExposionClientRpc(ulong clientId)
    {
        if(NetworkManager.Singleton.LocalClientId != clientId)
        {
            explosionSource.PlayOneShot(explosionSource.clip);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayHitSoundServerRpc(ulong clientId, bool isDeath)
    {
        PlayHitSoundClientRpc(clientId, isDeath);
    }

    [ClientRpc]
    void PlayHitSoundClientRpc(ulong clientId, bool isDeath)
    {
        if (!isDeath)
        {
            normalImpactSource.volume = effectSliderPercent * 0.5f;
            normalImpactSource.pitch = Random.Range(1.25f, 1.75f);
            normalImpactSource.PlayOneShot(normalImpactSource.clip);
        }
        else
        {
            normalImpactSource.volume = effectSliderPercent * 0.8f;
            normalImpactSource.pitch = Random.Range(0.75f, 1.25f);
            normalImpactSource.PlayOneShot(normalImpactSource.clip);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayDemonServerRpc()
    {
        PlayDemonClientRpc();
    }

    [ClientRpc]
    void PlayDemonClientRpc()
    {
        demonicFound = true;
        demonSource.Play();
        Debug.Log("Player has found demonic card");
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayAngelServerRpc()
    {
        PlayAngelClientRpc();
    }

    [ClientRpc]
    void PlayAngelClientRpc()
    {
        divineFound = true;
        angelSource.Play();
        Debug.Log("Player has found divine card");
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayWallHitServerRpc()
    {
        PlayWallHitClientRpc();
    }

    [ClientRpc]
    void PlayWallHitClientRpc()
    {
        wallHitSource.pitch = Random.Range(0.5f, 1f);
        wallHitSource.Play();
    }


}
