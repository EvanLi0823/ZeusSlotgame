using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//TODO: can't find the foot cause that lobb_background sprite can't be released. And slot_hall assets can't be 
// released either when entring a new slot.
public class AutoClearSprite : MonoBehaviour
{
    #region Lifecycle

    // Use this for initialization
    void Start()
    {
    }

    void OnDestroy()
    {
        FixMemoryLeak();
    }

    #endregion

    private void FixMemoryLeak()
    {
        ClearBackgroundImage();
    }

    private void ClearBackgroundImage()
    {
        Image backgroundImage = GetComponent<Image>();
        if (backgroundImage.sprite != null)
        {
            backgroundImage.sprite = null;
        }
    }
}