using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactionListener : MonoBehaviour {
    private PersistantManager boss;
    // Use this for initialization
    void Start () {
        boss = PersistantManager.Instance;
    }
	
	// Update is called once per frame
	void Update () {
        // input

        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hitDetection(ray, "mouse");

        }
    }

    public void hitDetection(Ray ray,string device = "")
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            string name = hit.transform.name;
            //boss.hud.addText("MouseClick hit");
            SSRP_context_element_controller child_controller = null;
            try
            {
                child_controller = hit.transform.GetComponent<SSRP_context_element_controller>();
                //child_controller.importData(entityData);


            }
            catch
            {
               
                
            }

            if (child_controller == null)
            {
                boss.hud.addText("no SSRP_context_element_controller found on " + name + " from " + device + " iteraction ");
                boss.hud.addText("Go look for A sensesmart logo ");

                return;
            }
            else
            {
                
                child_controller.isClosed = !child_controller.isClosed;
                if (child_controller.isClosed)
                {
                    boss.hud.addText("Closed Sensor Data Panel"  + name);
                }
                else
                {
                    boss.hud.addText("Open Sensor Data Panel "+   name);
                }
            }
            
        }
        else
        {
             boss.hud.addText(" no raycast hit from " + device);
        }
        
    }
}

