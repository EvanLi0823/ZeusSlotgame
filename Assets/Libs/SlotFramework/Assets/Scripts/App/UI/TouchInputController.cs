using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Touch input controller.
/// 这个组件脚本必须在指定的UI的rect的区域内才会有效，否则手势无效
/// </summary>
public class TouchInputController : MonoBehaviour {
	public Camera targetCamera;
	public RectTransform TouchZone;
	public string TouchMessage = "LobbyPromotionSwipe";
	private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
	private Vector2 offsetMin = Vector2.zero;
	private Vector2 offsetMax = new Vector2(Screen.width,Screen.height);
	void Start(){
		if (targetCamera==null) {
			targetCamera = Camera.main;
		}
		if (TouchZone==null) {
			TouchZone = transform as RectTransform;
		}
	}
	// Update is called once per frame
	private void Update () {
		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			Vector2 position = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			bool inzone = RectTransformUtility.RectangleContainsScreenPoint (TouchZone, position,targetCamera);
			if (inzone) {
				touchOrigin = position;
			}
		}
		else if (Input.GetMouseButtonUp(0)&&touchOrigin.x >= 0) {
			Vector2 touchEnd = new Vector2(Input.mousePosition.x,Input.mousePosition.y);

			//Calculate the difference between the beginning and end of the touch on the x axis.
			float x = touchEnd.x - touchOrigin.x;

			//Calculate the difference between the beginning and end of the touch on the y axis.
			float y = touchEnd.y - touchOrigin.y;

			//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
			touchOrigin.x = -1;
			touchOrigin.y = -1;
			//Check if the difference along the x axis is greater than the difference along the y axis.
			if (Mathf.Abs(x) > Mathf.Abs(y))
				//If x is greater than zero, set horizontal to 1, otherwise set it to -1
				horizontal = x > 0 ? 1 : -1;
			else
				//If y is greater than zero, set horizontal to 1, otherwise set it to -1
				vertical = y > 0 ? 1 : -1;
		}
		#endif


		//Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
		vertical = 0;
		}
		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		//Check if Input has registered more than zero touches
		if (Input.touchCount > 0)
		{
			//Store the first touch detected.
			Touch myTouch = Input.touches[0];
			//Debug.Log("touch point is valid:"+RectTransformUtility.RectangleContainsScreenPoint(TouchZone,myTouch.position));
			//Check if the phase of that touch equals Began
			if (myTouch.phase == TouchPhase.Began&&RectTransformUtility.RectangleContainsScreenPoint(TouchZone,myTouch.position,targetCamera))
			{
				//If so, set touchOrigin to the position of that touch
				touchOrigin = myTouch.position;
			}

			//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
			else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			{
				//Set touchEnd to equal the position of this touch
				Vector2 touchEnd = myTouch.position;

				//Calculate the difference between the beginning and end of the touch on the x axis.
				float x = touchEnd.x - touchOrigin.x;

				//Calculate the difference between the beginning and end of the touch on the y axis.
				float y = touchEnd.y - touchOrigin.y;

				//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
				touchOrigin.x = -1;
				touchOrigin.y = -1;
				//Check if the difference along the x axis is greater than the difference along the y axis.
				if (Mathf.Abs(x) > Mathf.Abs(y))
					//If x is greater than zero, set horizontal to 1, otherwise set it to -1
					horizontal = x > 0 ? 1 : -1;
				else
					//If y is greater than zero, set horizontal to 1, otherwise set it to -1
					vertical = y > 0 ? 1 : -1;
			}
		}

		#endif //End of mobile platform dependendent compilation section started above with #elif
		//Check if we have a non-zero value for horizontal or vertical
		if(horizontal != 0 || vertical != 0)
		{
			Messenger.Broadcast (TouchMessage, horizontal, vertical);
		}
	}
}
