using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AwakeSolutions
{
    public class AwakeMenuScreen : MonoBehaviour
    {
        public string name;
        public GameObject container;
        public string folderPath;

        public UnityEvent onShow;

        public void Init()
        {
            name = gameObject.name;
            container = gameObject;

            gameObject.SetActive(false);
        }

        public void OnShow()
        {
            Debug.Log("[AwakeMenu] Menu screen opened: " + name);

            InitializeChilds();

            onShow?.Invoke();
        }

        private void InitializeChilds()
        {
            foreach (AwakeMediaPlayer player in GetComponentsInChildren<AwakeMediaPlayer>())
                player.Init();

            foreach (AwakeButton button in GetComponentsInChildren<AwakeButton>())
                button.Init();
        }
    }
}