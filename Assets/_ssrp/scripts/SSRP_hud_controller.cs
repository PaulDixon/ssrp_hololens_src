using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSRP_hud_controller : MonoBehaviour
{
    PersistantManager boss;


    public List<int> deviceCounter;
    public List<string> info;
    public bool needsTidying = false;
    public int tidyCycleInSeconds = 4;
    public int MaxMessages = 15;
    public Text ui_text;
    public Text UI_sensorslist;
    private bool isUI = false;



    // Use this for initialization
    void Start()
    {
        boss = PersistantManager.Instance;
        info = new List<string>();
        if (ui_text != null)
        {
            isUI = true;
        }
    }

    private IEnumerator houseKeeping()
    {

        while (needsTidying && tidyCycleInSeconds >= 1)
        {
            if (info.Count > 0)
            {
                info.RemoveAt(0);

                yield return new WaitForSeconds(tidyCycleInSeconds);
            }
            else
            {
                needsTidying = false;

            }
        }
        //renderText();
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
            string display = "";
            int i = 0;
            int m = info.Count;
            for (i = 0; i < m; i++)
            {
                // display += info[i] + " [" + i + "]\n";
                display += info[i] + " -\n";
            }
            ui_text.text = display;
        }
    }

    public void addText(string str)
    {


        Debug.Log(str);
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