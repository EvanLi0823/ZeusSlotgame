using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CaptureScreen
{
    public static IEnumerator ShowCaptureBg(GameObject parent)
    {
        yield return new WaitForEndOfFrame();
        Transform captureTransform = GetCaptureBackground(parent);
        GameObject captureBg;
        if (captureTransform != null)
        {
            //make sure only one exists
            yield break;
        }
        else
        {
            captureBg = CreateCaptureBackground();
        }

        captureBg.transform.SetParent(parent.transform, false);
        SetFullSizeInParent(captureBg);
        captureBg.transform.SetSiblingIndex(0);
        Image captureImage = captureBg.GetComponent<Image>();
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        //not well in phone
        //TextureScale.Bilinear (texture, Screen.width / 2, Screen.height / 2); 

        captureImage.sprite =
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
    }

    public static Transform GetCaptureBackground(GameObject parentGameObject = null)
    {
        Transform o = parentGameObject.transform; //parentGameObject? parentGameObject.transform : transform.parent;
        while (o != null)
        {
            Transform t = o.Find(CAPTURE_BACKGROUND);
            if (t != null)
            {
                return t;
            }
            else
            {
                o = o.parent;
            }
        }

        return null;
    }

    static GameObject CreateCaptureBackground()
    {
        GameObject captureBackground = new GameObject(CAPTURE_BACKGROUND, typeof(RectTransform));
        int expectedLayer = LayerMask.NameToLayer(BackgroundLayerName);
        if (expectedLayer >= 0)
        {
            captureBackground.layer = LayerMask.NameToLayer(BackgroundLayerName);
        }

        Image bgImage = captureBackground.AddComponent<Image>();
        Color color = Color.white;
        color.a = 1;
        bgImage.color = color;

        return captureBackground;
    }

    static void SetFullSizeInParent(GameObject gameobject)
    {
        RectTransform tr = gameobject.transform as RectTransform;
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.one;
    }

    public static readonly string BackgroundLayerName = "Background";
    protected readonly static string CAPTURE_BACKGROUND = "CaptureBackground";
}