using System;
using UnityEngine;
using UnityEngine.UI;

namespace Classic.AdaptII
{
    public class AndroidDialogBG:MonoBehaviour//调整大小
    {
        [Header("True:在竖版中是横版界面旋转   False:在竖版中有竖版界面")]
        public bool isRotatedInPortrait;
        private float ImgWidth,ImgHeight; 
        private void Awake()
        {
            Image img = this.transform.GetComponent<Image>();
            ImgWidth = (this.transform as RectTransform).rect.width;
            ImgHeight= (this.transform as RectTransform).rect.height;
//            ImgWidth = img.
        }

        void Start()
        {
            bool isPortrait = false;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            if (screenHeight > screenWidth)//可认为是竖屏模式
            {	
                screenWidth = Screen.height;
                screenHeight = Screen.width;
                isPortrait = true;
            }
            float realRadio = 1;

            float screenRadio = screenWidth / screenHeight;
            float commonRadio = 16 / 9f;
            if (screenRadio > commonRadio)
            {
                if (isPortrait && !isRotatedInPortrait)
                {
                    realRadio = commonRadio / screenRadio;
                }
                else
                {
                    
                    //屏幕相对于1080高度缩放的比率
                    float heightRadio = screenHeight / 1080;
                    //screenWidth /heightRadio ：真实铺满屏幕图片的宽度，也就是要把当前图片设置成这个宽度
                    //（screenWidth /heightRadio） / ImgWidth 预设的宽度/图片的真实宽度=图片的缩放比例    
                    realRadio = screenWidth /heightRadio / ImgWidth;     
                }
               
            }
            else if(screenRadio<commonRadio)
            {
                
                float widthRadio = screenWidth / 1920;
                realRadio = screenHeight /widthRadio / ImgHeight;
                //realRadio = screenHeight / ImgHeight;
            }
           
            (this.transform as RectTransform).sizeDelta = new Vector2(ImgWidth * realRadio, ImgHeight * realRadio);

        }
    }
}