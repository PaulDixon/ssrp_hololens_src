using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;


public class SSRP_target_controller : MonoBehaviour
{


    PersistantManager boss;// = PersistantManager.Instance;

    // when the Sensor is within Marker distances (within 15m)
    // we allow the the sensorManager to swap over from using icons as visual tracking to an actual marker.
    // we spawn in the market using this. Prefab with the specific Marker ID given to us from the sensor data itself.
    // we can compar this to the Marker DB and generate the expected marker to track.
    // This also provides and anchor point for adding the Sensor Information.



    public  GameObject spawnTransform;
    private SSRP_context_element_controller contextElement;
    public string dataSetName;
    public GameObject  augmentationObject;
    public GameObject augmentationAnchor;

    private List<SSRP_contextResponse> contextElement_currentList;
    private List<SSRP_contextResponse> contextElement_previousList;
    private string markerid;

    /*
    private List<MVC_entity> prefabList;
    bool hasPrefabs = false;
   */







    // Use this for initialization
    void Start()

    {
        boss = PersistantManager.Instance;
        contextElement_currentList = null;
        contextElement_previousList = null;
      

    }

   

  
    public void import(List<SSRP_contextResponse> _list)
    {
        if (_list.Count > 0)
        { 
            boss = PersistantManager.Instance;

            contextElement_previousList = contextElement_currentList;
            contextElement_currentList = _list;

            if (contextElement_previousList != contextElement_currentList)
            {
                if (contextElement_currentList != null)
                {

                    boss.hud.addText("rendering Imported targets : " + contextElement_currentList.Count);
                    // Vuforia 6.2+
                    VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
                }

            }
        }
    }





    // Update is called once per frame
    void Update()
    {

    }



    

    private void LoadDataSet()
    {
        boss = PersistantManager.Instance;
        //Vuforia.ImageTarget;
        boss.hud.addText("LoadDataSet ()" + dataSetName);

        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        
        
        
        if (objectTracker == null)
        {
            boss.hud.addText("error : TrackerManager.Instance.GetTracker<ObjectTracker>() == null");
            return;
        }
        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSetName != "" && dataSet.Load(dataSetName))
        {

            objectTracker.Stop();  // stop tracker so that we can add new dataset

            if (!objectTracker.ActivateDataSet(dataSet))
            {
                // Note: ImageTracker cannot have more than 100 total targets activated
                //Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
                boss.hud.addText("Failed to Activate DataSet: " + dataSetName);
            }

            if (!objectTracker.Start())
            {
                //Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
                boss.hud.addText("Tracker Failed to Start. " + dataSetName);
            }

            int counter = 0;

            foreach (Trackable trackObj in dataSet.GetTrackables())
            {
                Debug.Log("dataSet [" + dataSetName + "]:" + trackObj.Name);
                //boss.hud.addText(trackObj.Name);
            }
            if (contextElement_currentList != null && contextElement_currentList.Count > 0)
            {
                foreach (SSRP_contextResponse cr in contextElement_currentList)
                {
                    Debug.Log("cr:" + cr.marker_name);
                }
            
            
            
                IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
                foreach (TrackableBehaviour tb in tbs)
                {
                    
                        
                    SSRP_contextResponse hit_response = null;
                    if (contextElement_currentList != null && contextElement_currentList.Count > 0)
                    {
                        foreach (SSRP_contextResponse cr in contextElement_currentList)
                        {
                            if (tb.TrackableName == cr.marker_name)
                            {
                                hit_response = cr;
                            }
                        }

                    }

                    if (hit_response != null && tb.name == "New Game Object")
                    {
                        if (spawnTransform != null)
                        {
                            tb.gameObject.transform.SetParent(spawnTransform.transform);
                              //  aug_anchor.transform.SetParent(tb.gameObject.transform);
                        }
                        boss = PersistantManager.Instance;
                        //importData    
                        //SSRP_ContextElement el = 
                        // change generic name to include trackable name
                        tb.gameObject.name = ++counter + ":DynamicImageTarget-" + tb.TrackableName;

                        // add additional script components for trackable
                        tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                        tb.gameObject.AddComponent<TurnOffBehaviour>();

                        if (augmentationObject != null)
                        {

                            // instantiate augmentation object and parent to trackable
                            GameObject aug_anchor = (GameObject)GameObject.Instantiate(augmentationAnchor);
                            aug_anchor.transform.SetParent(tb.gameObject.transform);
                            aug_anchor.transform.localPosition = Vector3.zero;
                            aug_anchor.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                            aug_anchor.transform.localScale = new Vector3(0.275f, 0.275f, 0.275f);

                            // instantiate augmentation object and parent to trackable
                            GameObject aug_obj = (GameObject)GameObject.Instantiate(augmentationObject);
                            aug_obj.transform.SetParent(aug_anchor.transform);

                            aug_obj.transform.localPosition = new Vector3(0f, 1.75f, 0f);
                            aug_obj.transform.localRotation = Quaternion.identity;
                            aug_obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                            SSRP_context_element_controller aug_obj_script = aug_obj.GetComponent<SSRP_context_element_controller>();
                            if (aug_obj_script != null)
                            {
                                aug_obj_script.importData(hit_response);
                            }

                            aug_obj.gameObject.SetActive(true);
                        }
                        else
                        {

                            //Debug.Log("<color=yellow>Warning: No augmentation object specified for: " + tb.TrackableName + "</color>");
                            boss.hud.addText("No augmentation object specified for: " + tb.TrackableName);
                        }
                    }
                    else
                    {

                        boss.hud.addText(tb.TrackableName + " not found in contextResponse List");
                    }
                }
            }
            else
                {
                boss.hud.addText("contextResponse List was empty");
            }
        }
        else
        {
            //Debug.LogError("<color=yellow>Failed to load dataset: '" + dataSetName + "'</color>");
            boss.hud.addText("Failed to load dataset: '" + dataSetName + "'");
        }
    }
    // */
}
