using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Klak.Hap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Events;

namespace AwakeSolutions
{
    [RequireComponent(typeof(RawImage))]

    public class AwakeMediaPlayer : Localizable
    {
        #region public
        public string folderPath;
        public string fileName;
        public string fileExtension;

        public bool autoload = false;
        public bool autoplay = false;
        public bool loop = false;

        [Range(-10, 10)]
        public float speed = 1f;

        public double time { get { return _time; } }
        public bool isPlaying { get { return _hapPlayer != null ? speed != 0 : _videoPlayer != null ? _videoPlayer.isPlaying : false; } }

        public bool initOnStart = true;
        public bool debug = false;
        #endregion

        #region events
        public UnityEvent onLoaded = new UnityEvent();
        public UnityEvent onFinished = new UnityEvent();
        #endregion

        #region private
        enum PlayerType { NONE, IMAGE, VIDEO, HAP }
        private PlayerType playerType;

        private VideoPlayer _videoPlayer;
        private HapPlayer _hapPlayer;

        private RawImage rawImage;
        private RectTransform rectTransform;

        private double _time;

        private bool isInitialized = false;

        private bool isLocalizable = false;
        private double timeToSeekOnLoad;

        private double length { get { return GetLength(); } }

        #endregion


        private void Start()
        {
            if (initOnStart)
                Init();
        }


        #region STATIC METHODS

        public static string[] Find(string folderPath, string fileName)
        {
            string[] foundFiles = new string[] { };

            folderPath = LocalizeFolderPath(folderPath);

            if (fileName == "")
                Debug.LogWarning("[AwakeMediaPlayer] File name not specified: " + folderPath);

            try
            {
                foundFiles = Directory.GetFiles(Application.streamingAssetsPath + "/" + folderPath, fileName + ".*", SearchOption.TopDirectoryOnly);
                foundFiles = foundFiles.Where(name => FilterFileName(name)).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogError("[AwakeMediaPlayer] Directory not found: " + folderPath + ", " + fileName);
            }

            if (foundFiles.Length == 0)
                Debug.LogWarning("[AwakeMediaPlayer] Files not found: " + folderPath + ", " + fileName);

            if (foundFiles.Length > 1)
                Debug.LogWarning("[AwakeMediaPlayer] Files found more than one: " + folderPath + ", " + fileName);

            return foundFiles;
        }

        public static string[] FindAll(string folderPath)
        {
            string[] foundFiles = new string[] { };

            try
            {
                foundFiles = Directory.GetFiles(Application.streamingAssetsPath + "/" + folderPath, "*", SearchOption.TopDirectoryOnly);
                foundFiles = foundFiles.Where(name => FilterFileName(name)).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogError("[AwakeMediaPlayer] Directory not found: " + folderPath);
            }

            return foundFiles;
        }

        #endregion


        #region PUBLIC METHODS

        public void Open(string _folderPath = null, string _fileName = null, bool play = true, bool isLooping = false)
        {
            if (debug) Debug.Log("[AwakeMediaPlayer] Open(" + _folderPath + ", " + _fileName + ", " + play + ", " + isLooping + ")");

            if (!isInitialized) Init();

            loop = isLooping;
            autoplay = play;

            SetFile(_folderPath, _fileName);
        }

        public void Play()
        {
            if (playerType == PlayerType.VIDEO)
                _videoPlayer.Play();
            else if (playerType == PlayerType.HAP)
            {
                speed = 1;
                _hapPlayer.speed = 1;
            }
            else if (playerType == PlayerType.NONE)
                Open(folderPath, fileName, autoplay, loop);
        }

        public void Pause()
        {
            if (playerType == PlayerType.VIDEO)
                _videoPlayer.Pause();
            else if (playerType == PlayerType.HAP)
            {
                speed = 0;
                _hapPlayer.speed = 0;
            }
        }

        public void Stop(bool clear = true)
        {
            if (playerType == PlayerType.VIDEO)
                Destroy(_videoPlayer);
            else if (playerType == PlayerType.HAP)
                Destroy(_hapPlayer);

            if (clear)
                Clear();

            playerType = PlayerType.NONE;
        }

        public void Seek(float time)
        {
            if (playerType == PlayerType.VIDEO)
                _videoPlayer.time = time;
            else if (playerType == PlayerType.HAP)
                _hapPlayer.time = time;
        }

        public void Clear()
        {
            Texture2D transparentTexture = new Texture2D(1, 1);
            transparentTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
            transparentTexture.Apply();
            rawImage.texture = transparentTexture;
        }

        #endregion


        #region MAIN LOGIC

        void SetFile(string _folderPath, string _fileName)
        {
            #region Получаем путь на файл

            folderPath = _folderPath;
            fileName = _fileName;

            if (folderPath.Contains("%LANG%"))
                isLocalizable = true;

            string[] foundFiles = Find(folderPath, fileName);

            if (foundFiles.Length == 0)
            {
                Clear();
                return;
            }

            string[] foundFileParts = foundFiles[0].Split('.');

            fileExtension = foundFileParts[foundFileParts.Length - 1];

            #endregion

            PlayerType targetPlayerType = GetPlayerTypeByExtension(fileExtension);

            SetPlayer(targetPlayerType);

            if (playerType == PlayerType.VIDEO)
            {
                _videoPlayer.source = VideoSource.Url;
                _videoPlayer.url = @Application.streamingAssetsPath + "/" + LocalizeFolderPath(folderPath) + "/" + fileName + "." + fileExtension;
                _videoPlayer.isLooping = loop;
            }
            else if (playerType == PlayerType.HAP)
            {
                _hapPlayer.Open(LocalizeFolderPath(folderPath) + "/" + fileName + "." + fileExtension);
                _hapPlayer.loop = loop;
            }
            else if (playerType == PlayerType.IMAGE)
            {
                StartCoroutine(nameof(SetTextureAsync));
            }

            if (!autoplay) Pause();

            StartCoroutine(CheckForIsLoaded());
        }

        private PlayerType GetPlayerTypeByExtension(string fileExtension)
        {
            if (fileExtension.ToLower() == "mov")
                return PlayerType.HAP;
            else if (fileExtension.ToLower() == "mp4" || fileExtension.ToLower() == "webm")
                return PlayerType.VIDEO;
            else if (fileExtension.ToLower() == "png" || fileExtension.ToLower() == "jpg")
                return PlayerType.IMAGE;

            Debug.LogError("[AwakeMediaPlayer] File extension not recognized: " + fileExtension);

            return PlayerType.NONE;
        }

        void SetPlayer(PlayerType targetPlayerType)
        {
            if (targetPlayerType != playerType)
            {
                RemoveExistingPlayers();

                if (targetPlayerType == PlayerType.HAP)
                    SetHapPlayer();
                else if (targetPlayerType == PlayerType.VIDEO)
                    SetVideoPlayer();
                else if (targetPlayerType == PlayerType.IMAGE)
                    SetImage();

                playerType = targetPlayerType;
            }
        }

        void RemoveExistingPlayers()
        {
            VideoPlayer _videoPlayer = GetComponent<VideoPlayer>();
            HapPlayer _hapPlayer = GetComponent<HapPlayer>();

            if (_videoPlayer != null) Destroy(_videoPlayer);
            if (_hapPlayer != null) Destroy(_hapPlayer);
        }

        void SetHapPlayer()
        {
            rawImage.texture = CreateRenderTexture(rectTransform);

            _hapPlayer = gameObject.AddComponent<HapPlayer>();
            _hapPlayer.targetTexture = rawImage.texture as RenderTexture;
        }

        void SetVideoPlayer()
        {
            rawImage.texture = CreateRenderTexture(rectTransform);

            _videoPlayer = gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.targetTexture = rawImage.texture as RenderTexture;
            _videoPlayer.aspectRatio = VideoAspectRatio.Stretch;

            _videoPlayer.loopPointReached += (VideoPlayer vp) => { onFinished?.Invoke(); };
        }

        void SetImage()
        {
        }

        #endregion


        #region LOW-LEVEL SERVICE METHODS

        public void Init()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            playerType = PlayerType.NONE;

            rawImage = GetComponent<RawImage>();
            rectTransform = GetComponent<RectTransform>();

            RemoveExistingPlayers();
            Clear();

            // Seek to previous time onLoad
            onLoaded.AddListener(() => {
                if (isLocalizable && timeToSeekOnLoad != 0)
                    Seek((float)timeToSeekOnLoad);
            });

            if (autoload) Open(folderPath, fileName, autoplay, loop);
            if (!autoload && autoplay) throw new Exception("Media is not loaded");
        }

        void Update()
        {
            if (isPlaying)
            {
                CheckEvents();
                UpdateParameters();
            }
        }

        void CheckEvents()
        {
            // OnVideoEnded
            if (isPlaying)
            {
                if (playerType == PlayerType.VIDEO)
                    _time = _videoPlayer.time;
                else if (playerType == PlayerType.HAP)
                {
                    if (_hapPlayer.time > _hapPlayer.streamDuration || _hapPlayer.time < 0)
                    {
                        if (debug)
                            Debug.Log("OnVideoEnded: " + speed + ", " + _hapPlayer.streamDuration + ", " + time);

                        if (loop)
                            Seek(speed > 0 ? 0 : (float)_hapPlayer.streamDuration);
                        else
                        {
                            Seek(speed > 0 ? (float)_hapPlayer.streamDuration : 0);
                            Pause();
                        }

                        onFinished?.Invoke();
                    }
                }
            }
        }

        void UpdateParameters()
        {
            // INTERNAL
            // Time
            if (playerType == PlayerType.VIDEO)
                _time = _videoPlayer.time;
            else if (playerType == PlayerType.HAP)
                _time = _hapPlayer.time;

            // EXTERNAL
            // Speed
            if (playerType == PlayerType.VIDEO)
                _videoPlayer.playbackSpeed = speed;
            else if (playerType == PlayerType.HAP)
                _hapPlayer.speed = speed;

            // Loop
            if (playerType == PlayerType.VIDEO)
                _videoPlayer.isLooping = loop;
            else if (playerType == PlayerType.HAP)
                _hapPlayer.loop = loop;
        }

        IEnumerator CheckForIsLoaded()
        {
            yield return new WaitForEndOfFrame();

            if (playerType == PlayerType.VIDEO || playerType == PlayerType.HAP)
            {
                float timeout = 1f;

                while (length < 0.1f)
                {
                    timeout -= Time.deltaTime;

                    if (timeout <= 0)
                        yield break;

                    yield return new WaitForEndOfFrame();
                }
            }

            onLoaded?.Invoke();
        }

        RenderTexture CreateRenderTexture(RectTransform rectTransform)
        {
            int width = (int)rectTransform.rect.width;
            int height = (int)rectTransform.rect.height;

            return new RenderTexture(width, height, 32);
        }

        IEnumerator SetTextureAsync()
        {
            using (UnityWebRequest loader = UnityWebRequestTexture.GetTexture("file://" + @Application.streamingAssetsPath + "/" + LocalizeFolderPath(folderPath) + "/" + fileName + "." + fileExtension))
            {
                yield return loader.SendWebRequest();

                if (string.IsNullOrEmpty(loader.error))
                {
                    rawImage.texture = DownloadHandlerTexture.GetContent(loader);

                    if (debug) Debug.LogFormat("[AwakeMediaPlayer] Texture loaded: '{0}'", loader.uri);
                }
                else
                    Debug.LogErrorFormat("[AwakeMediaPlayer] Error loading Texture '{0}': {1}", loader.uri, loader.error);
            }
        }

        double GetLength()
        {
            if (playerType == PlayerType.VIDEO)
                return _videoPlayer.length;
            else if (playerType == PlayerType.HAP)
                return _hapPlayer.streamDuration;

            return 0;
        }

        static bool FilterFileName(string name)
        {
            return
                !name.ToLower().EndsWith(".meta") &&
                !name.Contains("DS_Store") &&
                !name.Contains("/.") &&
                !name.Contains("\\.");
        }

        #endregion


        #region LOCALIZATION METHODS

        public override void Localize(Localizator.Language language)
        {
            if (!isLocalizable)
                return;

            timeToSeekOnLoad = time;

            Open(folderPath, fileName, autoplay, loop);
        }

        static string LocalizeFolderPath(string folderPath)
        {
            return folderPath.Replace("%LANG%", Localizator.currentLangCode);
        }

        #endregion
    }
}