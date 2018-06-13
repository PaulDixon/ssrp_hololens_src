using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class renderMe : MonoBehaviour {
    PersistantManager boss;
    // Use this for initialization
    void Start () {
        boss = PersistantManager.Instance;
        bool gotIssues = false;
        try
        {
            Canvas mycanvas = GetComponent<Canvas>();
            mycanvas.enabled = true;
            
            // testDataGeneration();
        }
        catch
        {
         //   boss.hud.addText("Context Canvas won't initialize?");
        }

        try
        {
            BoxCollider mycolider = GetComponent<BoxCollider>();
            mycolider.enabled = true;
        }
        catch
        {
            gotIssues = true;
        }

        try
        {
            MeshRenderer rendererererer = GetComponent<MeshRenderer>();
            rendererererer.enabled = true;
        }
        catch
        {
           // boss.hud.addText("3dObj won't initialize?");
        }
        if (gotIssues)
        {
            boss.hud.addText("BoxCollider missing");
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        

    }
}
