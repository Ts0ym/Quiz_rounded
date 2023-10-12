using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeSolutions
{
    public class LocalizableMediaPlayer : Localizable
    {
        public string folderPath;
        public string fileName;
        AwakeMediaPlayer mediaPlayer;

        public bool autoPlay = true;
        public bool loop = false;
        
        void Start()
        {
            mediaPlayer = GetComponent<AwakeMediaPlayer>();
        }
        
        public override void Localize(Localizator.Language language)
        {
            if (mediaPlayer == null)
                mediaPlayer = GetComponent<AwakeMediaPlayer>();
            
            string _folderPath = folderPath.Replace("%LANG%", language == Localizator.Language.RU ? "ru" : "en");
            string _fileName   = fileName  .Replace("%LANG%", language == Localizator.Language.RU ? "ru" : "en");
            
            mediaPlayer.Open(_folderPath, _fileName, autoPlay, loop);
        }
    }
}