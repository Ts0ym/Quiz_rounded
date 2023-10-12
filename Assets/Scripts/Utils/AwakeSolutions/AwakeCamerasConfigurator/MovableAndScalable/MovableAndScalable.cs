using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovableAndScalable : MonoBehaviour
{
    private float scaleSensivity = 50f;

    private void Awake()
    {
        LoadPositionAndScale();
    }

    private void LoadPositionAndScale()
    {
        if (!PlayerPrefs.HasKey(GetFullTransformPath(transform) + "_px"))
            return;

        transform.localPosition = new Vector3(
            float.Parse(PlayerPrefs.GetString(GetFullTransformPath(transform) + "_px")),
            float.Parse(PlayerPrefs.GetString(GetFullTransformPath(transform) + "_py")),
            float.Parse(PlayerPrefs.GetString(GetFullTransformPath(transform) + "_pz"))
        );

        transform.localScale = new Vector3(
            float.Parse(PlayerPrefs.GetString(GetFullTransformPath(transform) + "_sx")),
            float.Parse(PlayerPrefs.GetString(GetFullTransformPath(transform) + "_sy")),
            float.Parse(PlayerPrefs.GetString(GetFullTransformPath(transform) + "_sz"))
        );

        Debug.Log("Position and scale loaded on " + GetFullTransformPath(transform));
    }

    private void SavePositionAndScale()
    {
        var tmp = GetFullTransformPath(transform);
        PlayerPrefs.SetString(GetFullTransformPath(transform) + "_px", transform.localPosition.x.ToString());
        PlayerPrefs.SetString(GetFullTransformPath(transform) + "_py", transform.localPosition.y.ToString());
        PlayerPrefs.SetString(GetFullTransformPath(transform) + "_pz", transform.localPosition.z.ToString());
        PlayerPrefs.SetString(GetFullTransformPath(transform) + "_sx", transform.localScale.x.ToString());
        PlayerPrefs.SetString(GetFullTransformPath(transform) + "_sy", transform.localScale.y.ToString());
        PlayerPrefs.SetString(GetFullTransformPath(transform) + "_sz", transform.localScale.z.ToString());
        PlayerPrefs.Save();
        Debug.Log(transform.position.x);
        Debug.Log(transform.position.y);
        Debug.Log(transform.position.z);
        Debug.Log("Position and scale saved on " + gameObject.name);
    }

    private void OnMouseEvent(AwakeCam.MouseEventManager.MouseEvent mouseEvent)
    {
        if (mouseEvent.type == AwakeCam.MouseEventManager.MouseEvent.Type.DRAG)
            transform.position += mouseEvent.worldDelta;

        if (mouseEvent.type == AwakeCam.MouseEventManager.MouseEvent.Type.OVER)
            transform.localScale = new Vector3(
                transform.localScale.x * (1f - Input.mouseScrollDelta.y / scaleSensivity),
                transform.localScale.y * (1f - Input.mouseScrollDelta.y / scaleSensivity),
                transform.localScale.z * (1f - Input.mouseScrollDelta.y / scaleSensivity)
            );

        if (mouseEvent.type == AwakeCam.MouseEventManager.MouseEvent.Type.EXIT)
            SavePositionAndScale();
    }

    private void OnMouseOver()
    {
        Debug.Log("Mouse is over " + gameObject.name);
    }

    private void OnMouseDown(Vector3 mousePosition)
    {
        Debug.Log("Mouse is down over " + gameObject.name);
    }

    private void OnMouseDrag(Vector3 mousePosition)
    {
        Debug.Log("Mouse drag " + gameObject.name);

    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse up over " + gameObject.name);
    }

    public static string GetFullTransformPath(Transform transform)
    {
        if (!transform.parent) return transform.name;
        return GetFullTransformPath(transform.parent) + "/" + transform.name;
    }

    private void Update()
    {
        // All Objects Positions Clear
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.C))
        {
            PlayerPrefs.DeleteKey(GetFullTransformPath(transform) + "_px");
            PlayerPrefs.DeleteKey(GetFullTransformPath(transform) + "_py");
            PlayerPrefs.DeleteKey(GetFullTransformPath(transform) + "_pz");
            PlayerPrefs.DeleteKey(GetFullTransformPath(transform) + "_sx");
            PlayerPrefs.DeleteKey(GetFullTransformPath(transform) + "_sy");
            PlayerPrefs.DeleteKey(GetFullTransformPath(transform) + "_sz");

            StartCoroutine(Exit());
        }
    }

    IEnumerator Exit()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
