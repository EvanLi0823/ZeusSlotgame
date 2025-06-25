using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ChangeColor : MonoBehaviour {


    public Text MText;
   
    private Color Normal= new Color(1f,1f,1f,1f);
    private Color Press = new Color(1f,1f,1f,0.6f);
	

    public void OnPress(){
        MText.color = Press;
    }

    public void OnUp(){
        MText.color = Normal;
    }
}
