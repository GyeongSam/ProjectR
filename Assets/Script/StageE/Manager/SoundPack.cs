using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPack : MonoBehaviour
{
    [SerializeField]
    AudioClip[] _audios;

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    void Start()
    {
        string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
        for(int i = 0; i < soundNames.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames[i] };
            _audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = transform;
        }

        _audioSources[(int)Define.Sound.Bgm].loop = true;
        _audioSources[(int)Define.Sound.Effect].volume = 1.0f;
        
        _audioSources[(int)Define.Sound.Bgm].volume = 0.2f;
        _audioSources[(int)Define.Sound.Bgm].clip = _audios[0];
        _audioSources[(int)Define.Sound.Bgm].Play();
    }
    public void Play(int num, float pitch = 1.0f, Define.Sound soundType = Define.Sound.Effect)
    {
        if(soundType == Define.Sound.Bgm)
        {
            if (_audioSources[(int)Define.Sound.Bgm].isPlaying)
                _audioSources[(int)Define.Sound.Bgm].Stop();
            _audioSources[(int)Define.Sound.Bgm].pitch = pitch;
            _audioSources[(int)Define.Sound.Bgm].clip = _audios[num];
            _audioSources[(int)Define.Sound.Bgm].Play();
        }

        else
        {
            _audioSources[(int)Define.Sound.Effect].pitch = pitch;
            _audioSources[(int)Define.Sound.Effect].PlayOneShot(_audios[num]);
        }
    }

}
