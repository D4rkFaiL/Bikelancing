using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("Music Playing")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _audioClips;
    [SerializeField] string[] _musicName;
    [SerializeField] int _index;
    [SerializeField] TextMeshProUGUI _musicText;

    [Header("Slide shi")]
    [SerializeField] RectTransform _musicCard;
    [SerializeField] AnimationCurve _animCurve;
    [SerializeField] float _animSpeed;
    [SerializeField] Vector2 _animPos;

    [Header("AudioMixer")]
    [SerializeField] AudioMixer _audioMain;

    void Update()
    {
        if(!_audioSource.isPlaying){
            StartCoroutine(ChangeMusic());
        }
    }

    IEnumerator ChangeMusic(){
        _audioSource.clip = _audioClips[_index];
        _musicText.text = "- " + _musicName[_index];
        _musicText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "- " + _musicName[_index];
        _index++;
        
        if(_index == _audioClips.Length){
            _index = 0;
        }

        _audioSource.Play();
        StartCoroutine(UIManager.Instance.SlideYPosition(_musicCard,_animPos.y,_animSpeed,_animCurve));
        yield return new WaitForSeconds(5f);
        StartCoroutine(UIManager.Instance.SlideYPosition(_musicCard,_animPos.x,_animSpeed,_animCurve));
    }

    public void SetVolumeMain(float volume)
    {
        _audioMain.SetFloat("main",LinearToDecibel(volume));
    }

    private float LinearToDecibel(float linear)
    {
        float dB;
        
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        
        return dB;
    }
}
