using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDestroy : MonoBehaviour {
    public float m_timer = 1.0f;
	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, m_timer);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
