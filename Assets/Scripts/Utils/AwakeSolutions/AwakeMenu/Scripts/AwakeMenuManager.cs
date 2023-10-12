using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwakeSolutions
{
    public class AwakeMenuManager : MonoBehaviour
    {
        public AwakeMenuScreen mainMenuScreen;

        List<AwakeMenuScreen> menuScreens = new List<AwakeMenuScreen>();

        public AwakeMenuScreen activeMenuScreen;

        public string backgroundFileName;
        public string overlay1FileName;
        public string overlay2FileName;

        public AwakeMediaPlayer backgroundMediaPlayer;
        public AwakeMediaPlayer overlay1MediaPlayer;
        public AwakeMediaPlayer overlay2MediaPlayer;

        bool isNavigationBlocked = false;

        private void Start()
        {
            menuScreens = new List<AwakeMenuScreen>(FindObjectsOfType<AwakeMenuScreen>());

            foreach (AwakeMenuScreen menuScreen in menuScreens)
                menuScreen.Init();

            ShowFirstMenuScreen();
        }

        private void ShowFirstMenuScreen()
        {
            ShowMenuScreen(mainMenuScreen);
        }

        public void ShowMenuScreen(string menuScreenName)
        {
            ShowMenuScreen(GetMenuScreenByName(menuScreenName));
        }

        public void ShowMenuScreen(AwakeMenuScreen menuScreen)
        {
            Debug.Log("[AwakeMenu] ShowMenuScreen " + menuScreen.name);

            StartCoroutine(_ShowMenuScreen(menuScreen));
        }

        private IEnumerator _ShowMenuScreen(AwakeMenuScreen menuScreen)
        {
            if (isNavigationBlocked) {
                Debug.LogWarning("[AwakeMenu] Navigation blocked");
                yield break;
            }

            isNavigationBlocked = true;

            yield return new WaitForSeconds(0.5f); // TODO: AwakeTransition.fullCoverTime

            activeMenuScreen = menuScreen;

            foreach (AwakeMenuScreen _menuScreen in menuScreens)
                _menuScreen.container.SetActive(menuScreen == _menuScreen);

            menuScreen.OnShow();

            backgroundMediaPlayer.Open("AwakeMenu/" + Localizator.currentLangCode + "/" + menuScreen.folderPath + "/" + menuScreen.name, backgroundFileName, true, true);
            overlay1MediaPlayer  .Open("AwakeMenu/" + Localizator.currentLangCode + "/" + menuScreen.folderPath + "/" + menuScreen.name, overlay1FileName,   true, true);
            overlay2MediaPlayer  .Open("AwakeMenu/" + Localizator.currentLangCode + "/" + menuScreen.folderPath + "/" + menuScreen.name, overlay2FileName,   true, true);

            isNavigationBlocked = false;
        }

        public AwakeMenuScreen GetMenuScreenByName(string menuScreenName)
        {
            foreach (AwakeMenuScreen _menuScreen in menuScreens)
                if (_menuScreen.name == menuScreenName)
                    return _menuScreen;

            Debug.LogError("[AwakeMenu] Menu Screen \"" + menuScreenName + "\" not found!");

            return null;
        }
    }
}