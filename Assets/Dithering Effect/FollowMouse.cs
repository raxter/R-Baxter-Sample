using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour
{
    
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(Input.mousePosition);
        transform.eulerAngles = new Vector3(
            (((float)Input.mousePosition.y / Screen.height) - 0.5f) * -90,
            (((float)Input.mousePosition.x / Screen.width) - 0.5f) * 90, 
            0);
	}
}
