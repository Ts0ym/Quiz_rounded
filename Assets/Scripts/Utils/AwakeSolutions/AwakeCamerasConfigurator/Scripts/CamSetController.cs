using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwakeCam
{
    public class CamSetController : MonoBehaviour
    {

        public int targetDisplay;

        public Camera textureCamera;
        public Camera renderCamera;

        public QuickCorner quickCorner;

        public const float QUICK_CORNER_SENSIVITY_MULTIPLIER = 0.01f;

        RenderTexture renderTexture;

        public void Init()
        {
            int width = targetDisplay < Display.displays.Length ? Display.displays[targetDisplay].renderingWidth : 1920;
            int height = targetDisplay < Display.displays.Length ? Display.displays[targetDisplay].renderingHeight : 1080;

            renderTexture = new RenderTexture(width, height, 1);

            textureCamera.forceIntoRenderTexture = true;
            textureCamera.targetTexture = renderTexture;

            renderCamera.targetDisplay = targetDisplay;

            quickCorner.targetDisplay = targetDisplay;
            quickCorner._tex = renderTexture;

            Load();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat("Display " + targetDisplay + " orthographicSize", textureCamera.orthographicSize);

            PlayerPrefs.SetFloat("Display " + targetDisplay + " position X", textureCamera.transform.localPosition.x);
            PlayerPrefs.SetFloat("Display " + targetDisplay + " position Y", textureCamera.transform.localPosition.y);

            PlayerPrefs.SetFloat("Display " + targetDisplay + " bottom left X", quickCorner._vertixes[0].x);
            PlayerPrefs.SetFloat("Display " + targetDisplay + " bottom left Y", quickCorner._vertixes[0].y);

            PlayerPrefs.SetFloat("Display " + targetDisplay + " top left X", quickCorner._vertixes[1].x);
            PlayerPrefs.SetFloat("Display " + targetDisplay + " top left Y", quickCorner._vertixes[1].y);

            PlayerPrefs.SetFloat("Display " + targetDisplay + " top right X", quickCorner._vertixes[2].x);
            PlayerPrefs.SetFloat("Display " + targetDisplay + " top right Y", quickCorner._vertixes[2].y);

            PlayerPrefs.SetFloat("Display " + targetDisplay + " bottom right X", quickCorner._vertixes[3].x);
            PlayerPrefs.SetFloat("Display " + targetDisplay + " bottom right Y", quickCorner._vertixes[3].y);
        }

        public void Load()
        {
            if (PlayerPrefs.GetFloat("Display " + targetDisplay + " orthographicSize") == 0f)
            {
                Save();
            }

            textureCamera.orthographicSize = PlayerPrefs.GetFloat("Display " + targetDisplay + " orthographicSize");

            textureCamera.transform.localPosition = new Vector3(
                PlayerPrefs.GetFloat("Display " + targetDisplay + " position X"),
                PlayerPrefs.GetFloat("Display " + targetDisplay + " position Y"),
                textureCamera.transform.localPosition.z
            );

            quickCorner._vertixes[0] = new Vector2(PlayerPrefs.GetFloat("Display " + targetDisplay + " bottom left X"), PlayerPrefs.GetFloat("Display " + targetDisplay + " bottom left Y"));
            quickCorner._vertixes[1] = new Vector2(PlayerPrefs.GetFloat("Display " + targetDisplay + " top left X"), PlayerPrefs.GetFloat("Display " + targetDisplay + " top left Y"));
            quickCorner._vertixes[2] = new Vector2(PlayerPrefs.GetFloat("Display " + targetDisplay + " top right X"), PlayerPrefs.GetFloat("Display " + targetDisplay + " top right Y"));
            quickCorner._vertixes[3] = new Vector2(PlayerPrefs.GetFloat("Display " + targetDisplay + " bottom right X"), PlayerPrefs.GetFloat("Display " + targetDisplay + " bottom right Y"));
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey("Display " + targetDisplay + " orthographicSize");

            PlayerPrefs.DeleteKey("Display " + targetDisplay + " position X");
            PlayerPrefs.DeleteKey("Display " + targetDisplay + " position Y");

            PlayerPrefs.DeleteKey("Display " + targetDisplay + " bottom left X");
            PlayerPrefs.DeleteKey("Display " + targetDisplay + " bottom left Y");

            PlayerPrefs.DeleteKey("Display " + targetDisplay + " top left X");
            PlayerPrefs.DeleteKey("Display " + targetDisplay + " top left Y");

            PlayerPrefs.DeleteKey("Display " + targetDisplay + " top right X");
            PlayerPrefs.DeleteKey("Display " + targetDisplay + " top right Y");

            PlayerPrefs.DeleteKey("Display " + targetDisplay + " bottom right X");
            PlayerPrefs.DeleteKey("Display " + targetDisplay + " bottom right Y");
        }

        private void Update()
        {
            if (AwakeCam.Manager.instance.currentConfiguratingDisplay != targetDisplay)
                return;

            if (AwakeCam.Manager.instance.currentConfigurationMode == AwakeCam.Manager.ConfigurationMode.POSITION)
                ConfigurePosition();

            if (AwakeCam.Manager.instance.currentConfigurationMode == AwakeCam.Manager.ConfigurationMode.QUICK_CORNER)
                ConfigureQuickCorner();
        }

        private void ConfigurePosition()
        {
            if (Input.GetMouseButton(0))
                textureCamera.transform.localPosition = textureCamera.transform.localPosition + new Vector3(-Input.GetAxis("Mouse X") * textureCamera.orthographicSize / 100F, -Input.GetAxis("Mouse Y") * textureCamera.orthographicSize / 100F, 0f);

            textureCamera.orthographicSize -= Input.mouseScrollDelta.y * textureCamera.orthographicSize / 100F;
        }

        private void ConfigureQuickCorner()
        {
            if (!Input.GetMouseButton(0) || AwakeCam.Manager.instance.currentQuickCornerMode == AwakeCam.Manager.QuickCornerMode.NONE)
                return;

            if (AwakeCam.Manager.instance.currentQuickCornerMode == AwakeCam.Manager.QuickCornerMode.BOTTOM_LEFT)
                quickCorner._vertixes[0] += new Vector2(Input.GetAxis("Mouse X") * QUICK_CORNER_SENSIVITY_MULTIPLIER, Input.GetAxis("Mouse Y") * QUICK_CORNER_SENSIVITY_MULTIPLIER);

            if (AwakeCam.Manager.instance.currentQuickCornerMode == AwakeCam.Manager.QuickCornerMode.TOP_LEFT)
                quickCorner._vertixes[1] += new Vector2(Input.GetAxis("Mouse X") * QUICK_CORNER_SENSIVITY_MULTIPLIER, Input.GetAxis("Mouse Y") * QUICK_CORNER_SENSIVITY_MULTIPLIER);

            if (AwakeCam.Manager.instance.currentQuickCornerMode == AwakeCam.Manager.QuickCornerMode.TOP_RIGHT)
                quickCorner._vertixes[2] += new Vector2(Input.GetAxis("Mouse X") * QUICK_CORNER_SENSIVITY_MULTIPLIER, Input.GetAxis("Mouse Y") * QUICK_CORNER_SENSIVITY_MULTIPLIER);

            if (AwakeCam.Manager.instance.currentQuickCornerMode == AwakeCam.Manager.QuickCornerMode.BOTTOM_RIGHT)
                quickCorner._vertixes[3] += new Vector2(Input.GetAxis("Mouse X") * QUICK_CORNER_SENSIVITY_MULTIPLIER, Input.GetAxis("Mouse Y") * QUICK_CORNER_SENSIVITY_MULTIPLIER);
        }
    }
}