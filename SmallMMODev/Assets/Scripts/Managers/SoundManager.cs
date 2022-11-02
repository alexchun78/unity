
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if(root == null)
        {
            root = new GameObject{ name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for(int i =0; i < soundNames.Length -1; ++i)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
                _audioSources[i].loop = true;
            }            
        }
    }
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip clip = GetOrAddAudioClip(path, type);
        Play(clip, type, pitch);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.BGM)
        {
            AudioSource src = _audioSources[(int)Define.Sound.BGM];
            if (src.isPlaying == true)
                src.Stop();

            src.pitch = pitch;
            src.clip = audioClip;
            src.Play();
        }
        else if (type == Define.Sound.Effect)
        {
            AudioSource src = _audioSources[(int)Define.Sound.Effect];
            src.pitch = pitch;
            src.PlayOneShot(audioClip);
        }
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        // 만약 Path에 Sounds 폴더가 명시되어 있지 않다면, 패쓰를 변경해준다.
        if (path.Contains("Sounds") == false)
            path = $"Sounds/{path}";

        AudioClip clip = null;
        if (type == Define.Sound.BGM)
        {
            clip = Managers.Resource.Load<AudioClip>(path);
        }
        else if (type == Define.Sound.Effect)
        {

            if (_audioClips.TryGetValue(path, out clip) == false)
            {
                clip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, clip);
            }
        }

        if (clip == null)
        {
            Debug.Log($"audio clip is nothing in {path}");
        }

        return clip;
    }

    public void Clear()
    {
        foreach(AudioSource src in _audioSources)
        {
            src.Stop();
            src.clip = null;
        }
        _audioClips.Clear();
    }
}
