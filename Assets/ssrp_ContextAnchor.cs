using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ssrp_ContextAnchor : MonoBehaviour {
    PersistantManager boss;
    public GameObject ContextCanvasGameObject;
    public GameObject prefabGoesHere_stub;
    private SSRP_context_element_controller context_controller;
   
	// Use this for initialization
	void Start ()
    {
        boss = PersistantManager.Instance;
        context_controller = null;
        if (ContextCanvasGameObject != null)
        {
            context_controller = ContextCanvasGameObject.GetComponent<SSRP_context_element_controller>();
        }
		
	}
    public void importData(SSRP_contextResponse response)
    {
        boss = PersistantManager.Instance;
        if (ContextCanvasGameObject != null)
        {
            context_controller = ContextCanvasGameObject.GetComponent<SSRP_context_element_controller>();
        }
        if (context_controller != null)
        {
            context_controller.importData(response);
        }
        else
        {
            boss.hud.addText("context response import is not set to entity canvas");
        }


    }
        // Update is called once per frame
        void Update () {
		
	}
}
