using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwakeSolutions
{
    public class AwakeScreenManager : Localizable
    {
        public string folderPath;
        public GameObject mediaPrefab;
        public Transform mediaContainer;

        GameObject prevMedia;

        public void Show(string itemIndex)
        {
            Debug.Log("[AwakeScreenManager] Showing item index: " + itemIndex);

            if (prevMedia != null)
                StartCoroutine(DestroyPrevTouchOverlay(prevMedia));

            prevMedia = Instantiate(mediaPrefab, mediaContainer);
            prevMedia.GetComponent<AwakeMediaPlayer>().Open(folderPath, itemIndex);
        }

        IEnumerator DestroyPrevTouchOverlay(GameObject prevTouchOverlay)
        {
            yield return new WaitForSeconds(1F);

            Destroy(prevTouchOverlay);
        }
    }
}