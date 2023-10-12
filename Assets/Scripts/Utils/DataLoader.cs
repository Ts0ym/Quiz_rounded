using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataLoader
{
    public static List<T> GetListFromJSONpath<T>(string filename)
    {
        Debug.Log(filename);
        if (!File.Exists(filename))
        {
            throw new System.Exception("Wrong JSON filename");
        }

        string json = File.ReadAllText(filename);

        List<T> itemsList = JsonConvert.DeserializeObject<List<T>>(json);

        return itemsList;
    }

    public static List<T> GetListFromJSONfile<T>(string jsonFile)
    {
        List<T> itemsList = JsonConvert.DeserializeObject<List<T>>(jsonFile);

        return itemsList;
    }
    public static string BuildStreamingAssetPath(string assetName)
    {
        return Path.Combine(Application.streamingAssetsPath, assetName);
    }

    public static Sprite LoadImage(string imageName)
    {
        string imagePath = Path.Combine(Application.streamingAssetsPath, imageName);
        Debug.Log(imageName);
        Debug.Log(imagePath);
        if (!File.Exists(imagePath))
        {
            throw new SystemException("Wrong Image path");
        }

        byte[] loadedBytes = File.ReadAllBytes(imagePath);

        Texture2D imageTexture = new Texture2D(1, 1);
        imageTexture.LoadImage(loadedBytes);

        return Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
    }
}
