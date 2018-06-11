using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinaryStateIcon : MonoBehaviour
{
    private Boolean isUILinked = false;
    private Boolean prev_isOn = false;

    public Boolean isOn = false;
    public Text ui_state;
    public Text ui_action;
    public RawImage ui_icon;


    private string state = "";
    public string state_0 = "";
    public string state_1 = "";


    private Texture2D icon;
    public Texture2D icon_0;
    public Texture2D icon_1;
    private string resource_name0 = "";
    private string resource_name1 = "";

    private string stateTransition = "";

    public string stateTransition_0_0 = "";
    public string stateTransition_0_1 = "";
    public string stateTransition_1_0 = "";
    public string stateTransition_1_1 = "";
    List<string> stateTransitionList;

    //public BinaryStateIcon get;


    void Start()
    {
        stateTransitionList = new List<string> { stateTransition_0_0, stateTransition_0_1, stateTransition_1_0, stateTransition_1_1 };
        isUILinked = UI_elementsLinkCheck();
        //init_empty();
        
    }

    void Update()
    {
        if (isOn)
        {
            state = state_1;
            icon = icon_1;
            
             // stateTransitionList[2] || stateTransition_1_1
        }
        else
        {
            state = state_0;
            icon = icon_0;
            // stateTransitionList[0]  ||  stateTransition_0_0; 
        }

        
        if (isUILinked)
        {
            
    
            ui_state.text = state;
            ui_action.text = stateTransition;
            ui_icon.texture = (Texture) icon;
        }

    }
    public void action(int _int)
    {
        int m = stateTransitionList.Count;
       
        if (_int >= 0 && _int < m)
        {
            stateTransition = stateTransitionList[_int];
        }
    }

    public void action(string action)
    {
        stateTransition = action;
    }

    private bool UI_elementsLinkCheck()
    {
        bool isOk = true;
        if (ui_state == null) { return false; }
        if (ui_action == null) { return false; }
        if (ui_icon == null) { return false; }

        return isOk;
    }

    // Use this for initialization


    public void On()
    {
        prev_isOn = isOn;
        isOn = true;
        if (hasChanged())
        {
            stateTransition = stateTransition_0_1;
        }
        else
        { 
            stateTransition = stateTransition_1_1;
        }
    }

    public void Off()
    {
        prev_isOn = isOn;
        isOn = false;
        if (hasChanged())
        {
            stateTransition = stateTransition_1_0;
        }
        else
        {
            stateTransition = stateTransition_0_0;
        }
    }

    public void Toggle()
    {
        prev_isOn = isOn;
        isOn = !isOn;
    }
    public Boolean hasChanged()
    {
        if (isOn != prev_isOn)
        {
            return true;
        }
        
        return false;
        
    }

    
    // Update is called once per frame
    

    public void import_Icon(Boolean state, string stateName, string resourcename, string toState = "", string fromState = "", string remain = "")
    {
        if (state)
        {
            resource_name1 = resourcename;
            icon_1 = Resources.Load(resource_name1) as Texture2D;
            setupOn(stateName, toState, fromState, remain);
        }
        else
        {
            resource_name0 = resourcename;
            icon_0 = Resources.Load(resource_name0) as Texture2D;
            setupOff(stateName, toState, fromState, remain);
        }
    }
    public void import_Icon(Boolean state, string stateName, Texture2D textureResource, string toState = "", string fromState = "", string remain = "")
    {

        if (state)
        {
            icon_1 = Resources.Load(resource_name1) as Texture2D;
            setupOn(stateName, toState, fromState, remain);
        }
        else
        {
            icon_0 = Resources.Load(resource_name0) as Texture2D;
            setupOff(stateName, toState, fromState, remain);
        }
    }

    private void setupOn(string stateName, string toState = "", string fromState = "", string remain = "")
    {
        
        if (stateName == "") { state_1 = "ON"; }
        if (stateTransition_1_1 == "" && remain != "") { stateTransition_1_1 = remain; }
        if (stateTransition_0_1 == "" && toState != "") { stateTransition_0_1 = toState; }
        if (stateTransition_1_0 == "" && fromState != "") { stateTransition_1_0 = fromState; }
    }
    private void setupOff(string stateName, string toState = "", string fromState = "", string remain = "")
    {
        
        if (stateName == "") { state_0 = "OFF";}
        if (stateTransition_0_0 == "" && remain != "") { stateTransition_0_0 = remain; }
        if (stateTransition_1_0 == "" && toState != "") { stateTransition_1_0 = toState; }
        if (stateTransition_0_1 == "" && fromState != "") { stateTransition_0_1 = fromState; }
    }




    /*
     * public void init_empty()
    {
        isOn = false;
        state = "";
        stateTransition = "";

        icon_0 = null;
        icon_1 = null;
        state_0 = "";
        state_1 = "";
        resource_name0 = "";
        resource_name1 = "";
        stateTransition_0_0 = "";
        stateTransition_0_1 = "";
        stateTransition_1_0 = "";
        stateTransition_1_1 = "";
    }

    public void init_desc()
    {
        isOn = false;
        state = "status";
        stateTransition = "action";

        icon_0 = null;
        icon_1 = null;
        state_0 = "state_0";
        state_1 = "state_1";
        resource_name0 = "resource_name0";
        resource_name1 = "resource_name1";
        stateTransition_0_0 = "stateTransition_0_0";
        stateTransition_0_1 = "stateTransition_0_1";
        stateTransition_1_0 = "stateTransition_1_0";
        stateTransition_1_1 = "stateTransition_1_1";
    }
*/
   
}
