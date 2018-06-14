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
    private string prev_dataSetName;
    private string dataSetName;
    public GameObject  augmentationObject;
    public GameObject augmentationAnchor;

    private List<SSRP_contextResponse> contextElement_currentList;
    private List<SSRP_contextResponse> contextElement_previousList;
    private string markerid;
    private Boolean importlock = false;
    private Boolean hasDataLoaded = false;
    private IEnumerable<TrackableBehaviour> dataset_With_Trackables = null;
    






    // Use this for initialization
    void Start()

    {
        dataset_With_Trackables = null;
        boss = PersistantManager.Instance;
        contextElement_currentList = null;
        contextElement_previousList = null;
      

    }
   
    public void dynamicLoadofVuforiaDataBase(string _datasetName)
    {
       
        prev_dataSetName = dataSetName;
        dataSetName = _datasetName;
        if (string.Compare(prev_dataSetName, dataSetName) != 0 )
        {
            boss.hud.addText("dynamicLoadofVuforiaDataBase(" + _datasetName + " )");
            // LoadDataSet can only be a max of 100 so with regards to the long-term planning, this is not a viable solution
            // Vuforia 6.2+ 
            VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
        }
    }

  
    public void import(List<SSRP_contextResponse> _list = null)
    {
        boss = PersistantManager.Instance;
        if (importlock != true && _list != null && _list.Count > 0 )
        {
            contextElement_previousList = contextElement_currentList;
            contextElement_currentList = _list;
            
            if (!contextElement_currentList.Equals(contextElement_previousList))
            {
                importlock = true;
                boss.hud.addText(" Imported targets to render : " + contextElement_currentList.Count);
                contextSearchInTrackables();
                
            }
            else
            {
                boss.hud.addText("contextElement_currentList unaltered");
            }
            importlock = false;
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

        if (dataSetName == "")
        {
            boss.hud.addText("error :  dataSetName not set");
            return;
        }
        hasDataLoaded = false;
        dataset_With_Trackables = null;
        if (dataSet.Load(dataSetName))
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

            
            try
            {
                dataset_With_Trackables = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
                hasDataLoaded = true;
            }
            catch
            {
                boss.hud.addText(" collect trackables in " + dataSetName + ": Fail");
                return;
            }

        }
        else
        {
            boss.hud.addText("Failed to load dataset: '" + dataSetName + "'");
        }
        boss.workingIcon.Off();
        
        
    }

    public void contextSearchInTrackables()
    {
        boss.hud.addText(" context Search In Trackables list ()");
        int overalCounter = 0;
        int foundCounter = 0;
        if (dataset_With_Trackables == null)
        {
            boss.hud.addText(" dataset_With_Trackables is empty");
        }

        foreach (TrackableBehaviour tb in dataset_With_Trackables)
        {
            overalCounter++;
            boss.hud.addText(" trackable no:[ " + ++overalCounter + "]" + tb.TrackableName);
            boss.hud.addText("test against " + contextElement_currentList.Count + " local sensors" ) ;
            foreach (SSRP_contextResponse cr in contextElement_currentList)
            {
                boss.hud.addText("sensor = [" + (string)cr.marker_name+"]");
            }

                SSRP_contextResponse hit_response = null;

                
            foreach (SSRP_contextResponse cr in contextElement_currentList)
            {
                    
                if (string.Compare((string) tb.TrackableName, (string)cr.marker_name) == 0)
                {
                    boss.hud.addText(cr.marker_name +": hit");
                    hit_response = cr;
                        
                }
            }

            if (hit_response == null)
            {
                boss.hud.addText(tb.TrackableName + " not found in contextResponse List");
                return;
            }
            if (tb.name != "New Game Object")
            {
                boss.hud.addText("tb.name = " + tb.name);
                return;
            }


            if (augmentationObject == null)
            {
                boss.hud.addText("No augmentation object specified for: " + tb.TrackableName);
                return;

            }
            
                
            //importData    
            //SSRP_ContextElement el = 
            // change generic name to include trackable name
            tb.gameObject.name = ++foundCounter + ":DynamicImageTarget-" + tb.TrackableName;

            // add additional script components for trackable
            tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
            tb.gameObject.AddComponent<TurnOffBehaviour>();
            if (spawnTransform != null)
            {
                tb.gameObject.transform.SetParent(spawnTransform.transform);
                //  aug_anchor.transform.SetParent(tb.gameObject.transform);
            }



            // instantiate augmentation object and parent to trackable
            GameObject aug_anchor = (GameObject)GameObject.Instantiate(augmentationAnchor);
            aug_anchor.transform.SetParent(tb.gameObject.transform);
            //aug_anchor.transform.localPosition = Vector3.zero;
            //aug_anchor.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            //aug_anchor.transform.localScale = new Vector3(0.275f, 0.275f, 0.275f);

            // instantiate augmentation object and parent to trackable
            GameObject aug_obj = (GameObject)GameObject.Instantiate(augmentationObject);
            aug_obj.transform.SetParent(aug_anchor.transform);

            //                            aug_obj.transform.localPosition = new Vector3(0f, 1.75f, 0f);
            //                           aug_obj.transform.localRotation = Quaternion.identity;
            //                          aug_obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            SSRP_context_element_controller aug_obj_script = aug_obj.GetComponent<SSRP_context_element_controller>();
            if (aug_obj_script != null)
            {
                aug_obj_script.importData(hit_response);
            }

            aug_obj.gameObject.SetActive(true);

               
                
        }
    }
     
        

    
    // */
}
