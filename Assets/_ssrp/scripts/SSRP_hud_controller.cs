using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSRP_hud_controller : MonoBehaviour
{
    PersistantManager boss;


    public List<int> deviceCounter;
    public List<string> info = new List<string>();
    public bool needsTidying = false;
    public int tidyCycleInSeconds;
    public int MaxMessages;
    public Text ui_text;
    public Text UI_sensorslist;
    private bool isUI = false;



    // Use this for initialization
    void Start()
    {
        boss = PersistantManager.Instance;
       
        if (ui_text != null)
        {
            isUI = true;
        }
        StartCoroutine(houseKeeping());
    }

    private IEnumerator houseKeeping()
    {
        yield return new WaitForSeconds(tidyCycleInSeconds);
                
        if (info.Count > 0)
        {
            info.RemoveAt(0);
        }
        else
        {
            needsTidying = false;
        }
        
        if (needsTidying)
        { 
            StartCoroutine(houseKeeping());
        }


    }


    // Update is called once per frame
    void Update()
    {
        renderText();

    }

    public void renderText()
    {

        if (isUI)
        {
            List<string> temp = info;
            string display = "";
            int i = 0;
            int m = temp.Count;
            for (i = 0; i < m; i++)
            {
                // display += info[i] + " [" + i + "]\n";
                display += temp[i] + " -\n";
            }
            ui_text.text = display;
        }
    }

    public void addText(string str)
    {


        Debug.Log(str);
        if(info.Count == MaxMessages)
        {
            info.RemoveAt(0);

        }
        info.Add(str);
        renderText();
        
        if (!needsTidying)
        { 
            needsTidying = true;
            StartCoroutine(houseKeeping());
        }



    }

    public void sensorBreakDown(string str)
    {
        if (UI_sensorslist != null)
        {
            UI_sensorslist.text = "[SensorList]\n" + str;
        }
        //mock data
    }

}