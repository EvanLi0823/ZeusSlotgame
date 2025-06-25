using UnityEngine;
using UnityEngine.UI;
namespace Classic.AdaptII
{
    public class AndroidSquarePanelOffset:MonoBehaviour
    {
        private Vector3 LocalV;
        [Header("是否是长屏生效，false宽屏生效")]
        public bool isLand;
        [Header("True:靠下   False:靠上")]
        public bool isBottom;
        [Header("True:靠左   False:靠右")]
        public bool isLeft;
        [Header("True:在竖版中是横版界面旋转   False:在竖版中有竖版界面")]
        public bool isRotatedInPortrait;
        void Awake()
        {
             LocalV = (transform as RectTransform).localPosition;
        }
        void Start()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
			

            if (screenHeight > screenWidth)//可认为是竖屏模式
            {	
                screenWidth = Screen.height;
                screenHeight = Screen.width;
            }

            float realRadio = screenWidth / screenHeight;
            float commonRadio = 16 / 9f;
            float dealWidth = 1920;
            float dealHeight = 1080;
            if (realRadio < commonRadio)
            {
                if (!isLand)
                {
                    float v = (1920 / realRadio - 1080) / 2;
                    if (Screen.width < Screen.height && !isRotatedInPortrait)
                    {
                        float w = isLeft ? LocalV.x - v : LocalV.x + v;
                        (transform as RectTransform).localPosition = new Vector3(w, LocalV.y, LocalV.z);
                    }
                    else
                    {
                        float h = isBottom ? LocalV.y - v : LocalV.y + v;
                        (transform as RectTransform).localPosition = new Vector3(LocalV.x,h,LocalV.z);
                    }
 
                }
            }
            else if(realRadio > commonRadio)
            {
                if (isLand)
                {
                    float v = (1080 * realRadio - 1920) / 2;
                    if (Screen.width>Screen.height || isRotatedInPortrait)
                    {
                        float w = isLeft ? LocalV.x - v : LocalV.x + v;
                        (transform as RectTransform).localPosition = new Vector3(w, LocalV.y, LocalV.z);
                    }
                    else
                    {
                        float h = isBottom ? LocalV.y - v : LocalV.y + v;
                        (transform as RectTransform).localPosition = new Vector3(LocalV.x,h,LocalV.z);
                    }

                    
                }
                
            }


        }
    }
}