using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //get ratio
        float ratio = (float)Screen.width / (float)Screen.height;
        //forever rect!
        this.GetComponent<Camera>().rect = new Rect((1 - 0.2f), (1 - 0.2f * ratio), 0.2f, 0.2f * ratio);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
