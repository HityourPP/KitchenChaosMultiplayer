using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefs_SO audioClipRefsSo;

    private float volume = 0.5f;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME, 0.5f);//获取该键值中的值，若无默认设为0.5
    }

    private void Start()
    {
        DelieverManager.Instance.OnRecipeSuccess += DelievryManagerOnRecipeSuccess;
        DelieverManager.Instance.OnRecipeFailed += DelievryManagerOnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        BaseCounter.OnAnyObjectPlaceHere += BaseCounterOnAnyObjectPlaceHere;
        TrashCounter.OnAnyObjectDestroyed += TrashCounterOnAnyObjectDestroyed;
        
        PlayerController.OnAnyPickedSomething += PlayerOnPickedSomething;
    }

    private void BaseCounterOnAnyObjectPlaceHere(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSo.objectDrop, baseCounter.transform.position);
    }

    private void TrashCounterOnAnyObjectDestroyed(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSo.trash, trashCounter.transform.position);
    }

    private void PlayerOnPickedSomething(object sender, EventArgs e)
    {
        PlayerController playerController = sender as PlayerController;
        PlaySound(audioClipRefsSo.objectPickUp, playerController.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSo.chop, cuttingCounter.transform.position);
    }

    private void DelievryManagerOnRecipeFailed(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSo.delievryFailed, deliveryCounter.transform.position);
    }

    private void DelievryManagerOnRecipeSuccess(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSo.delievrySuccess, deliveryCounter.transform.position);
    }

    public void PlaySound(AudioClip audioClip,Vector3 position,float volumeMultipler = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultipler);//设置一个音频源，设置其位置与音量大小   
    }   
    public void PlaySound(AudioClip[] audioClipArray,Vector3 position,float volumeMultipler = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)], position,
            volumeMultipler * volume);
    }

    public void PlayFootStepSound(Vector3 position, float volume)
    {
        PlaySound(audioClipRefsSo.footStep,position,volume);
    }    
    public void PlayNumberPopSound()
    {
        PlaySound(audioClipRefsSo.warning, Vector3.zero);
    }

    public void ChangeVolume()
    {
        volume += .1f;
        if (volume > 1f)
        {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_VOLUME, volume);//将音量大小保存在这个字段中
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
 