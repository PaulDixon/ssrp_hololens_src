using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class helloWorld : MonoBehaviour {
    public Text guiText;
    public string someString;
	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		if (guiText != null)
        {
            guiText.text = someString;
        }

    }
}
