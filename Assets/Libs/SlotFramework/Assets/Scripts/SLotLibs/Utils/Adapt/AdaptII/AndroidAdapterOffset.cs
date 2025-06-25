using System;
using UnityEngine;
using UnityEngine.UI;

namespace Classic.AdaptII
{
    public class AndroidAdapterOffset : MonoBehaviour
    {
        private Vector3 LocalV;
        [Header("靠上")]
        public bool isUp;
        [Header("靠下")]
        public bool isBottom;
        [Header("靠左")]
        public bool isLeft;
        [Header("靠右")]
        public bool isRight;
        [Header("初始化竖版Iphone的位置")]
        public Vector3 portraitIphonePosition;
        

        void Start()
        {
           
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            LocalV = (transform as RectTransform).localPosition;
            
            if (screenHeight > screenWidth) //可认为是竖屏模式
            {
                screenWidth = Screen.height;
                screenHeight = Screen.width;
                if (portraitIphonePosition!=null)
                {
                    (transform as RectTransform).localPosition = LocalV = portraitIphonePosition;
                }
            }

            float realRadio = screenWidth / screenHeight;
            float commonRadio = 16 / 9f;
            float dealWidth = 1920;
            float dealHeight = 1080;
            if (realRadio==commonRadio)
            {
                return;
            }

            if (isUp)
            {
                float v = (1920 / realRadio - 1080) / 2 ;
                (transform as RectTransform).localPosition = new Vector3(LocalV.x, LocalV.y - v, LocalV.z);
            }
            if (isBottom)
            {
                float v = (1920 / realRadio - 1080) / 2 ;
                (transform as RectTransform).localPosition = new Vector3(LocalV.x, LocalV.y + v, LocalV.z);
            }
            if (isLeft)
            {
                float v = (1080 * realRadio - 1920) / 2 ;
                (transform as RectTransform).localPosition = new Vector3(LocalV.x - v , LocalV.y, LocalV.z);
            }
            if (isRight)
            {
                float v = (1080 * realRadio - 1920) / 2 ;
                (transform as RectTransform).localPosition = new Vector3(LocalV.x + v , LocalV.y, LocalV.z);
            }
        }

    }

}

