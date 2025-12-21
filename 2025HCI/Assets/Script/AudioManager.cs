using UnityEngine;
using UnityEngine.Audio; // 必须引用：用于控制 Mixer
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // 单例模式，方便全局调用
    public static AudioManager Instance;

    [Header("Mixer 分组")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("背景音乐播放器")]
    private AudioSource musicSource;

    void Awake()
    {
        // 确保全局只有一个 AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景时不销毁，保证音乐连续
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化背景音乐播放器
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
    }

    // ================== BGM 逻辑 ==================

    /// <summary>
    /// 播放或切换背景音乐
    /// </summary>
    /// <param name="clip">音乐文件</param>
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return; // 如果已经是这首歌，跳过

        musicSource.clip = clip;
        musicSource.Play();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ================== SFX 逻辑 ==================

    /// <summary>
    /// 播放一次性音效
    /// </summary>
    /// <param name="clip">音效文件</param>
    /// <param name="volume">音量大小(0-1)</param>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // 创建临时音效播放器（播放完后自动销毁）
        // 这种方式简单直接，适合初排
        GameObject sfxObj = new GameObject("TempSFX_" + clip.name);
        AudioSource source = sfxObj.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.outputAudioMixerGroup = sfxGroup; // 绑定 Mixer 分组

        source.Play();

        // 播放完后销毁物体
        Destroy(sfxObj, clip.length);
    }

    // ================== 进阶：通过名称播放 (可选) ==================
    // 如果你不想在每个脚本里都拖拽 AudioClip，可以使用一个简单的 List
    [System.Serializable]
    public struct AudioData
    {
        public string name;
        public AudioClip clip;
    }
    public List<AudioData> audioLibrary;

    public void PlaySFXByName(string audioName)
    {
        AudioData data = audioLibrary.Find(x => x.name == audioName);
        if (data.clip != null) PlaySFX(data.clip);
        else Debug.LogWarning("未找到音效：" + audioName);
    }

    [System.Serializable]
    public struct bgmData
    {
        public string name;
        public AudioClip clip;
    }
    public List<bgmData> bgmLibrary;

    public void PlayBGMByName(string audioName)
    {
        bgmData data = bgmLibrary.Find(x => x.name == audioName);
        if (data.clip != null) PlayMusic(data.clip);
        else Debug.LogWarning("未找到音效：" + audioName);
    }
}