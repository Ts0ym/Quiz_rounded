using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace AwakeSolutions
{
    public class AwakeSoundManager : MonoBehaviour
    {
        [Header("Управление звуком")]
        [Tooltip("Эта переменная просто хранит описание работы и не используется в коде")]
        [TextArea(1, 50)]
        public string Инструкция = "" +
            "В папке StreamingAssets создаем папку Sounds, и накидываем туда mp3 и wav звуки.\n\n" +
            "Этот скрипт может даже не висеть на сцене и не требует AudioSource.\n\n" +
            "В коде вызываем звук так: SoundManager.Play(\"file_name_without_extension\");\n\n" +
            "Залупленные звуки не поддерживаются (пока?)\n\n" +
            "Версия от 21.07.22 20:15";

        private static Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

        private static bool isLoaded = false;

        public static void Play(string name)
        {
            if (!isLoaded)
            {
                Start();
                return;
            }

            if (sounds.ContainsKey(name))
                PlayClip(sounds[name]);
            else
                Debug.LogError("Sound \"" + name + "\" isn't loaded yet");
        }

        private static void PlayClip(AudioClip audioClip)
        {
            PlayClip(audioClip, Vector3.zero);
        }

        private static void PlayClip(AudioClip audioClip, Vector3 worldPosition)
        {
            GameObject tempGO = new GameObject("One shot audio");
            tempGO.transform.position = worldPosition;

            AudioSource audioSource = tempGO.AddComponent<AudioSource>();

            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            audioSource.mute = audioSource.mute;
            audioSource.bypassEffects = audioSource.bypassEffects;
            audioSource.bypassListenerEffects = audioSource.bypassListenerEffects;
            audioSource.bypassReverbZones = audioSource.bypassReverbZones;
            audioSource.playOnAwake = audioSource.playOnAwake;
            audioSource.loop = audioSource.loop;
            audioSource.priority = audioSource.priority;
            audioSource.volume = audioSource.volume;
            audioSource.pitch = audioSource.pitch;
            audioSource.panStereo = audioSource.panStereo;
            audioSource.spatialBlend = 0;
            audioSource.reverbZoneMix = audioSource.reverbZoneMix;
            audioSource.dopplerLevel = audioSource.dopplerLevel;
            audioSource.rolloffMode = audioSource.rolloffMode;
            audioSource.spread = audioSource.spread;

            audioSource.minDistance = 10000f;
            audioSource.maxDistance = 10000f;

            audioSource.Play();
            MonoBehaviour.Destroy(tempGO, audioSource.clip.length);
        }

        private static void Start()
        {
            LoadAll();
        }

        private static async void LoadAll()
        {
            Debug.Log("Sounds loading started!");

            string[] allMp3 = Directory.GetFiles(Application.streamingAssetsPath + "/Sounds", "*.mp3", SearchOption.AllDirectories);
            string[] allWav = Directory.GetFiles(Application.streamingAssetsPath + "/Sounds", "*.wav", SearchOption.AllDirectories);

            Debug.Log("MP3 count: " + allMp3.Length);
            Debug.Log("WAV count: " + allWav.Length);

            for (int i = 0; i < allMp3.Length; i++)
                await Load(allMp3[i], AudioType.MPEG);

            for (int i = 0; i < allWav.Length; i++)
                await Load(allWav[i], AudioType.WAV);

            isLoaded = true;
        }

        private static async Task Load(string path, AudioType audioType)
        {
#if UNITY_EDITOR_OSX
            path = "file://" + path;
#endif
            string key = path.Split('/')[path.Split('/').Length - 1];
            key = key.Split('\\')[key.Split('\\').Length - 1];
            key = key.Split('.')[0];

            Debug.Log("Sound loading: " + key);

            using (UnityWebRequest audioLoadRequest = UnityWebRequestMultimedia.GetAudioClip(@path, audioType))
            {
                var operation = audioLoadRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (audioLoadRequest.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(audioLoadRequest);

                    sounds.Add(key, clip);

                    Debug.Log("Sound loaded: " + key);
                }
                else
                {
                    Debug.LogError("Sound loading error: " + key);
                }
            }
        }
    }
}