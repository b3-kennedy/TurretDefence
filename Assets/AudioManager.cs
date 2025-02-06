using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental.GlobalIllumination;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;

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
    public float effectsVolume;

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
        
    }

    public void OnMusicSliderChange()
    {
        musicVolume = musicSlider.value;
        musicVolText.text = (musicVolume*100).ToString("F1");
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

    // Update is called once per frame
    void Update()
    {

        if (audioSettingsPanel.activeSelf && EnemySpawnManager.Instance.localPlayer != null)
        {
            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = false;
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            musicSource.mute = !musicSource.mute;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            foreach (AudioSource source in effectsSources) 
            {
                source.mute = !source.mute;
            }
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
