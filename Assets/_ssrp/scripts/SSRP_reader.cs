using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using FullSerializer;


public class SSRP_reader : MonoBehaviour {

    private PersistantManager boss;
    private static readonly fsSerializer _serializer = new fsSerializer();
    // due to creative common agreement stuff
    public List<string> peopleToThank;

    // SSRP client / server connection.

    public SSRP_participant participant_02 = new SSRP_participant("user02ssr@ssr.se", "Password");
    public SSRP_client local_client = new SSRP_client("1e11fb3249344c4a9e0adbb190bfa2e7", "f70e87088f334df4af185bf3ff6f4e98");
    public SSRP_entity ssrp_room_entity = new SSRP_entity("Room62", "iot_sensor_2bd37a1cc7784f93b998ea6feff42915", "789a173df6944eb5829130b3ce447bce");
    public SSRP_sensor_request local_request = new SSRP_sensor_request("0004a30b0022a677", "LORA_Sensor", "false");
    public string loraURL = "www.";

    // SSRP localFileBackup
    public string gameDataProjectFilePath;
    public string ssrp_response_string;
    public Boolean isAuthenticated = false;
    public Boolean isOnline = false;
    public Boolean isBeating = false;
    public int heartbeat_bpm = 30;
    public int wwwTimer = 5;

    
    private SSRP_entity_manager entityManager;
    public SSRP_response_raw response_raw;

   // Vuforia SenseSmart details
    
    public string vApiKey = "AWbv2Br/////AAADmfP5GXalbk/Un7Konr4YTlYJuhC11t5wmTGsILPJ5GP9vVPER2KxzmBAq7Pa+sNgfbsby96rYWi2U7Yue8d4SLdjmxtiB+P19oapvxndRHHnVcTyn1Z6/PVc9apPLs6AdRFy7PR/orE4oEoilC2wKe+lO7tpyL49zP5rSW5sQtCUYINMZdQOkm5kNqPIrXtsG3e7LO7b8Yt8e0BIK4okkZnjPV+ioRlrSpu/9MXeB6Vds8A8bDmV99ISBpRolr/cpskPJikHaCeV5NE5GGfdLOA1CFG9FX0/9Iv6YddqoC0wmG07LJGrz49e/17j5vHcNfR56/7UkhMZ4Dqf1XUTzImeCVIezGaj1JXdmq7fmR6m";
    public string vuforiaMarkerId = "eed385f01b6d4f11a32a43304d6deb2f"; // id of sense_smart_logo_in Vuforia DB.

    // you still have to login and download the existing images that are stored there, THEN import the BD into Unity
    
    



    //  CONNECTION STUFF

    // Use this for initialization
    void Start()
    {
        boss = PersistantManager.Instance;
       
        

        try
        {
            
            entityManager = GetComponent<SSRP_entity_manager>();
           
        }
        catch
        {
            Debug.LogWarning("EntityManager not set up");
        }

        isBeating = true;
        isAuthenticated = false;
        StartCoroutine(checkInternetConnection());
        StartCoroutine(Heartbeat());
        //*/

        // coreFunctionTester();

    }

    public void coreFunctionTester()
    {
        boss = PersistantManager.Instance;
       

        participant_02 = new SSRP_participant("user02ssr@ssr.se", "Password");
        local_client = new SSRP_client("1e11fb3249344c4a9e0adbb190bfa2e7", "f70e87088f334df4af185bf3ff6f4e98");
        ssrp_room_entity = new SSRP_entity("Room62", "iot_sensor_2bd37a1cc7784f93b998ea6feff42915", "789a173df6944eb5829130b3ce447bce");
        local_request = new SSRP_sensor_request("0004a30b0022a677", "LORA_Sensor", "false");
        string markerId = 
        loraURL = "www.";
        string ok_ssrp_handshake = "";
        string malformed_ssrp_handshake = "";
        string ok_ssrp_handshake_response ="";
        string malformed_ssrp_handshake_response = "";
        string ok_ssrp_authenticatedQuery = "";
        string malformed_ssrp_authenticatedQuery = "";
        string ok_ssrp_authenticatedQueryRespnse = "";
        string malformed_ssrp_authenticatedQueryRespnse = "";


        // this is the basic linear flow of the connectionProcess. 
        // TimeOut and Retries are not a part of the code.
        // and I never got around to setting up a SSRP client to actuall test it properly
        checkInternetConnection();
        connectToSSRP();
        sendSSRPHandShake(ok_ssrp_handshake);
        sendSSRPHandShake(malformed_ssrp_handshake);
        CheckSSRPHandShakeResponse(ok_ssrp_handshake_response);
        CheckSSRPHandShakeResponse(malformed_ssrp_handshake_response);
        sendAuthenticatedSSRPQuery(ok_ssrp_authenticatedQuery);
        sendAuthenticatedSSRPQuery(malformed_ssrp_authenticatedQuery);
        CheckSSRPQueryResponse(ok_ssrp_authenticatedQueryRespnse);
        CheckSSRPQueryResponse(malformed_ssrp_authenticatedQueryRespnse);
        SSRP_Connection_Error();
        readStoredResponseFromFile();
   //     entityManager.open(vuforiaMarkerId);


    }
    
    
    

    IEnumerator  checkInternetConnection()
    {
        boss = PersistantManager.Instance;
        while (isBeating == true && heartbeat_bpm >= 1)
        {
            boss.connectIcon.action("checking");
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                boss.connectIcon.Off(); 
                isOnline = false;
                boss.hud.addText("HUD active"); 
            }
            else
            {
                boss.connectIcon.On(); 
                isOnline = true;
            }
            yield return new WaitForSeconds(wwwTimer);
        }


       
    }


    // checks for data at regularIntervalss
    IEnumerator Heartbeat()
    {
        boss = PersistantManager.Instance;
        while (isBeating == true && heartbeat_bpm >= 1)
        {
            boss.hud.addText(" heart_isBeating(" + isBeating + " ) at " + heartbeat_bpm + " bpm and isOnline(" + isOnline + ")"); 
            if (isOnline)
            {
                connectToSSRP();
            }
            else
            {
                SSRP_Connection_Error();
            }
            yield return new WaitForSeconds(heartbeat_bpm);
        }
        SSRP_Connection_Error();
    }

    
    // Action connectToSSRP -> address (send handshake)
    public void connectToSSRP()
    {
        boss = PersistantManager.Instance;
        boss.hud.addText("connectToSSRP (isAuthenticated:" + isAuthenticated +")"); 
        if (!isAuthenticated)
        {
            boss.ssrpIcon.action("Handshaking");
            StartCoroutine(sendSSRPHandShake()); 
        }
        else
        {
            StartCoroutine(sendAuthenticatedSSRPQuery());
        }
    }

    IEnumerator sendSSRPHandShake(string handshake = "")
    {
        boss = PersistantManager.Instance;
        boss.hud.addText("send SSRP HandShake");
        boss.ssrpIcon.action("HandShake"); 
        string url = loraURL + handshake;
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                SSRP_Connection_Error();
            }
            else
            {
                CheckSSRPHandShakeResponse(www.text);
            }
        } 
    }

    

    public void CheckSSRPHandShakeResponse( String response)
    {
        boss = PersistantManager.Instance;
        boss.hud.addText("Verifying SSRP HandShake Response"); 
        //is response in a formate we understand
        isAuthenticated = false;
        boss.ssrpIcon.action("verify"); 
        try
        {
            response_raw = (SSRP_response_raw)Deserialize(typeof(SSRP_response_raw), response);

            // validation process stubb
            bool responseOk = false;
            
            if (responseOk )
            { 
                isAuthenticated = true;
                boss.ssrpIcon.On();
                sendAuthenticatedSSRPQuery();
            }
        }
        catch
        {
            SSRP_Connection_Error();
        }
        
       

        
    }
    IEnumerator sendAuthenticatedSSRPQuery(string query = "")
    {
        boss = PersistantManager.Instance;
        boss.hud.addText("send Authenticated SSRP Query");
        boss.ssrpIcon.action("Query"); 
        string url = loraURL + query;
        
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                SSRP_Connection_Error();
            }
            else
            {
                CheckSSRPQueryResponse(www.text);
            }
        }
    }

    public void CheckSSRPQueryResponse(String response)
    {
        boss = PersistantManager.Instance;
        boss.hud.addText("Check SSRP Query Response");
        boss.ssrpIcon.action("Verify"); 
        try
        {
            boss.ssrpIcon.On(); 
            response_raw = (SSRP_response_raw)Deserialize(typeof(SSRP_response_raw), response);
           
            // validation process stubb
            isAuthenticated = true;
            sendAuthenticatedSSRPQuery();
        }
        catch
        {

            SSRP_Connection_Error();
        }
    }

    public void SSRP_Connection_Error()
    {
        boss = PersistantManager.Instance;
        Debug.Log("SSRP_Connection_Error");
        boss.ssrpIcon.Off(); 
        isAuthenticated = false;

        boss.hud.addText("Internal Warning: SSRP Connection Error");
            
        

      
        readStoredResponseFromFile();
    }

    



    private void readStoredResponseFromFile()
    {
        boss = PersistantManager.Instance;
        string filePath = Application.dataPath + gameDataProjectFilePath;
        boss.hud.addText("Read Stored Response From File()");
        boss.hud.addText("[device]" + filePath);
        
        if (File.Exists(filePath))
        {


            ssrp_response_string = File.ReadAllText(filePath);
           
            response_raw = (SSRP_response_raw)Deserialize(typeof(SSRP_response_raw), ssrp_response_string);
           
           


            boss.entityManager.importEntity(response_raw.contextResponses);
          //  entityManager.open(vuforiaMarkerId);
        }
        else
        {
            Debug.LogError("Internal Warning: Stored Response From File() Error");
       
            boss.hud.addText("Internal Warning: Stored Response From File() Error ??????? ");
        }
    }

    
    public static string Serialize(Type type, object value)
    {
        // serialize the data
        fsData data;
        _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }

    public static object Deserialize(Type type, string serializedState)
    {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }

    // Update is called once per frame
    void Update()
    {
    }
        
}
