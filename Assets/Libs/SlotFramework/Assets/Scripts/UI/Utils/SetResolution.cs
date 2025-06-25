using UnityEngine;
using System.Collections;

namespace UI.Utils
{
    public class SetResolution : MonoBehaviour
    {
        public int width = 800;
        public int height = 600;

        void Start ()
        {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        //Application.targetFrameRate = 60;
        if (Screen.width != width || Screen.height != height)
            Screen.SetResolution(width, height, true);
#endif
            Destroy (this);
        }
    
    }
}
