using System;
using System.Collections;
using System.Collections.Generic;
using AwakeSolutions;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeSolutions
{
    public class AwakeButton : Localizable
    {
        public List<AwakeButton> buttonsGroup = new List<AwakeButton>();

        public float fadeSpeed = 0.3f;

        public bool isActive = false;
        public bool setMediaAutomatically = true;
        public bool replaceDefaultWithActive = false;
        public bool isToggle = false;

        public AwakeMediaPlayer defaultPlayer;
        public AwakeMediaPlayer activePlayer;

        Animator animator;
        Button button;

        AwakeMenuScreen menuScreen;
        private bool isInitialized;

        public bool debug = false;

        public void Init()
        {
            if (isInitialized)
                return;

            animator = GetComponent<Animator>();
            button   = GetComponent<Button>();

            button.onClick.AddListener(Click);

            if (defaultPlayer == null)
                defaultPlayer = transform.Find("Default Media Player").GetComponent<AwakeMediaPlayer>();

            if (activePlayer == null)
                activePlayer  = transform.Find("Active Media Player")?.GetComponent<AwakeMediaPlayer>();

            menuScreen = gameObject.GetComponentInParent<AwakeMenuScreen>();

            isInitialized = true;

            if (setMediaAutomatically)
                SetMediaAutomatically();
        }

        private void SetMediaAutomatically()
        {
            if (!isInitialized)
                Init();

            defaultPlayer.Open(
                "AwakeMenu/" + Localizator.currentLangCode + "/" + menuScreen.folderPath + "/" + menuScreen.name + "/Buttons",
                gameObject.name + "_default"
            );

            activePlayer.Open(
                "AwakeMenu/" + Localizator.currentLangCode + "/" + menuScreen.folderPath + "/" + menuScreen.name + "/Buttons",
                gameObject.name + "_active"
            );
        }

        public void Click()
        {
            SetActive(isToggle ? !isActive : true);

            foreach (AwakeButton button in buttonsGroup)
                if (button != this && button.isActive)
                    button.SetActive(false);
        }

        public void SetActive(bool _isActive)
        {
            isActive = _isActive;

            if (buttonsGroup.Count > 0)
                animator.CrossFade(GetTargetAnimationName(), fadeSpeed);
            else
                animator.Play("Click");
        }

        private string GetTargetAnimationName()
        {
            if (isActive && !replaceDefaultWithActive)
                return "SetActive";

            if (isActive && replaceDefaultWithActive)
                return "ReplaceWithActive";

            if (!isActive)
                return "SetInactive";

            throw new Exception("[AwakeButton] Unknown error");
        }

        public override void Localize(Localizator.Language language) {
            if (debug)
                Debug.Log("[AwakeButton] Localizating object " + gameObject.name);

            StartCoroutine(_Localize());
        }

        IEnumerator _Localize() {
            yield return new WaitForEndOfFrame();

            if (setMediaAutomatically)
                SetMediaAutomatically();
        }
    }
}