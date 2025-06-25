using UnityEngine;
public class ToggleIphoneXActivate : MonoBehaviour
{
    public GameObject Other;
    public GameObject IphoneX;

    void Start () 
    {
		if (IphoneXAdapter.IsIphoneX()) 
        {
			this.IphoneX.SetActive(true);
            this.Other.SetActive(false);
		}
        else
        {
            this.Other.SetActive(true);
            this.IphoneX.SetActive(false);
        }
	}
}
