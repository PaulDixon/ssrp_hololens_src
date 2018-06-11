using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSRP_metaData_controller : MonoBehaviour {
    PersistantManager boss;

    public Text ui_name;
    public Text ui_type;
    public Text ui_value;
    private bool canRenderText = false;

    private SSRP_Metadata _data = new SSRP_Metadata();

    // Use this for initialization
    void Start()
    {
        boss = PersistantManager.Instance;
        testUIRender();
        try
        {
            Canvas mycanvas = GetComponent<Canvas>();
            mycanvas.enabled = true;
            BoxCollider mycolider = GetComponent<BoxCollider>();
            mycolider.enabled = true;
            // testDataGeneration();
        }
        catch
        {
            boss.hud.addText("Context Canvas won't initialize?");
        }
        //importData(new SSRP_Metadata("foo", "ooh", "bar"));
    }

    public void importData(SSRP_Metadata meta)
    {
        _data = meta;
        
        refresh();
    }
    // Update is called once per frame
    void refresh()
    {
        testUIRender();
        if (_data != null && canRenderText)
        {
            ui_name.text = _data.name;
            ui_value.text = _data.value;
            ui_type.text = _data.type;
        }
    }

    private void testUIRender()
    {
        try
        {
            canRenderText = false;
            if (ui_name != null && ui_type != null && ui_value != null)
            {
                canRenderText = true;
                //  Debug.LogFormat("canRenderText:{0}", canRenderText);

            }
            else { Debug.LogWarningFormat("UI_404 canRenderText:{0}", canRenderText); }
        }
        catch { Debug.LogWarningFormat("UI_404 canRenderText:{0}", canRenderText); }
    }

    void fetchGameObjectsFromParent()
    {
        ui_name = gameObject.GetComponent("name") as Text;
        ui_value = gameObject.GetComponent("value") as Text;
        ui_type = gameObject.GetComponent("type") as Text;


    }


}
