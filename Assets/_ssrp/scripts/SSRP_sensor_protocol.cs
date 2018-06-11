using System;
using System.Collections;
using UnityEngine;

public class SSRP_participant
{

    public string Email;
    public string Password;

    public SSRP_participant()
    {
        Email = "";
        Password = "";
    }

    public SSRP_participant(string _email, string _pass)
    {
        Email = _email;
        Password = _pass;
    }



}

public class SSRP_client
{
    public string Clientid;
    public string Clientpassword;

    public SSRP_client()
    {
        Clientid = "";
        Clientpassword = "";
    }

    public SSRP_client(string _id, string _pw)
    {
        Clientid = _id;
        Clientpassword = _pw;
    }




}

public class SSRP_entity
{
    public string Entity_id;
    public string Sensorid;
    public string SensorPassword;

    public SSRP_entity()
    {
        Entity_id = "";
        Sensorid = "";
        SensorPassword = "";
    }

    public SSRP_entity(string ent_id, string sen_id, string sen_pw)
    {
        Entity_id = ent_id;
        Sensorid = sen_id;
        SensorPassword = sen_pw;
    }


}


/* loraSensors_unordered_List
  { 
  contextResponses": [ ] 
  }
*/
public class SSRP_response_raw
{
    public Type ssrp_raw_response;
    public SSRP_contextResponse[] contextResponses;
}

/* an actual sensor
{ 
 "contextElement": { },
 "statusCode": { }
}
*/
public class SSRP_contextResponse
{
    public Type ssrp_contextResponse;
    public SSRP_StatusCode statusCode;
    public SSRP_ContextElement contextElement;
    public Vector3 gpsPos = new Vector3(0f, 0f, 0f);

    public double distToViewer { get; internal set; }
    public string marker_name { get; internal set; }

    public SSRP_contextResponse()
    {
        statusCode = new SSRP_StatusCode();
        contextElement = new SSRP_ContextElement();
        gpsPos = new Vector3(0f, 0f, 0f);
        distToViewer = 0;

    }

    public SSRP_StatusCode getStatus()
    {
        return statusCode;
    }

    public SSRP_ContextElement data_basic()
    {
        SSRP_ContextElement basicInfo = new SSRP_ContextElement();
        basicInfo.id = contextElement.id;
        basicInfo.type = contextElement.type;
        basicInfo.isPattern = contextElement.isPattern;
        return basicInfo;
    }

    public string description()
    {
        string ret = "ContextElement : {";
        ret += "id: " + contextElement.id + ", ";
        ret += "type: " + contextElement.type + ", ";
        ret += "isPattern: " + contextElement.isPattern + ", ";
        ret += contextElement.attributes.Length + "attribute(s) }";
        return ret;
    }

    public String getAttributeValue(String name)
    {

        try
        {
            return contextElement.getAttributeValue(name);
        }
        catch { };
        return "";
    }
    public SSRP_attribute[] attributes()
    {
        return contextElement.attributes;
    }

    public string list_attributes()
    {
        string ret = "";
        foreach (SSRP_attribute att in contextElement.attributes)
        {
            ret += "(" + att.type + ") " + att.name + " = " + att.value;
            try
            {
                if (att.metadatas.Length > 0)
                {
                    ret += " [";
                    foreach (SSRP_Metadata met in att.metadatas)
                    {
                        ret += att.description();
                        ret += met.name + ", ";
                        ret += met.type + ", ";
                        ret += met.value + ", ";

                    }
                    ret += " ]";
                }
            }
            catch
            {

            }
            ret += "\n";
        }
        return ret;
    }

}


/* sensor.status
statusCode": {
   "code": "200",
   "reasonPhrase": "OK"
}
*/
public class SSRP_StatusCode
{
    public Type ssrp_statusCode;
    public String code;
    public String reasonPhrase;

    public SSRP_StatusCode()
    {
        code = "";
        reasonPhrase = "";
    }

    public SSRP_StatusCode(String _code, String _reasonphrase)
    {
        code = _code;
        reasonPhrase = _reasonphrase;
    }
}

/*
 *{"entities": [

{"id": "0004a30b0022a677",

"type": "LORA_Sensor",

"isPattern": false}]

}
*/
public class SSRP_sensor_request
{

    public SSRP_ContextElement[] entities;

    private SSRP_ContextElement sensorObj;

    public SSRP_sensor_request()
    {
        sensorObj = new SSRP_ContextElement();
        entities = new SSRP_ContextElement[] { sensorObj };

    }

    public SSRP_sensor_request(Boolean test)
    {
        if (test)
        {
            sensorObj = new SSRP_ContextElement("0004a30b0022a677", "LORA_Sensor", "false");
        }
        else
        {
            sensorObj = new SSRP_ContextElement();
        }
        entities = new SSRP_ContextElement[1] { sensorObj };
    }

    public SSRP_sensor_request(String _id, String _type, String _isPattern)
    {
        sensorObj = new SSRP_ContextElement(_id, _type, _isPattern);
        entities = new SSRP_ContextElement[1] { sensorObj };
    }

}

/* sensor.info
contextElement:{ 
    "type": "LORA_Sensor",
    "isPattern": "false",
    "id": "0004a30b0022a677",
    "attributes": []
    }
 */

public class SSRP_ContextElement
{

    public Type ssrp_contextElement;
    public String id;
    public String type;
    public String isPattern;
    public SSRP_attribute[] attributes;

    public SSRP_ContextElement()
    {
        id = ""; type = ""; isPattern = "false";
        attributes = new SSRP_attribute[0];
    }

    public SSRP_ContextElement(String _id, String _type, String _isPattern)
    {
        id = _id;
        type = _type;
        isPattern = _isPattern;
        attributes = new SSRP_attribute[0];

    }
    public string getAttributeValue(String _name)
    {
        String returnString = "";
        foreach (SSRP_attribute att in attributes)
        {
            if (att.name == _name)
            {
                return att.value;
            }
        }
        return returnString;
    }
    public SSRP_ContextElement(String _id, String _type, String _isPattern, SSRP_attribute[] _att)
    {
        id = _id;
        type = _type;
        isPattern = _isPattern;
        attributes = _att;

    }

    public String description()
    {
        String returnString = "ContextElement: {";
        try
        {
            returnString += "id = " + id + " type = " + type + " isPattern = " + isPattern;
            returnString += " attributes[";
            foreach (SSRP_attribute att in attributes)
            {
                returnString += att.description();
            }
            returnString += "]";

        }
        catch { }
        returnString += "}";
        return returnString;
    }

}




/* sensor.attributes
 "name": "PM25",
 "type": "float",
 "value": "1.484232", 
 "metadatas": []
 */
public class SSRP_attribute : SSRP_Metadata
{
    public Type ssrp_attribute;
    public SSRP_Metadata[] metadatas;

    public SSRP_attribute()
    {
        name = "";
        type = "";
        value = "";
        metadatas = new SSRP_Metadata[] { };
    }

    public SSRP_attribute(SSRP_Metadata[] list)
    {
        int i = 0;
        int m = list.Length;
        if (m > 1) { metadatas = new SSRP_Metadata[m - 1]; }
        for (i = 0; i < m; i++)
        {
            if (i == 0)
            {
                name = list[i].name;
                type = list[i].type;
                value = list[i].value;
            }
            else
            {

            }
        }
    }

    public SSRP_attribute(String _name, String _type, String _value, SSRP_Metadata[] list)
    {
        name = _name;
        type = _type;
        value = _value;
        metadatas = list;
    }
    public String description()
    {
        String ret = "Attribut : {";
        ret += "name:" + name + ", ";
        ret += "value: " + value + ", ";
        ret += "type:" + type + ", [";
        foreach (SSRP_Metadata meta in metadatas)
        {
            ret += meta.description() + ", ";
        }
        ret += "]}";
        return ret;
    }
}


/*
metadata : 
    "name": "unit",
    "type": "string",
    "value": "ug/m3"
*/
public class SSRP_Metadata : IEnumerable
{
    public Type ssrp_Metadata;
    public String name;
    public String type;
    public String value;

    public SSRP_Metadata()
    {
        name = "";
        type = "";
        value = "";
    }

    public SSRP_Metadata(String _name, String _type, String _value)
    {
        name = _name;
        type = _type;
        value = _value;

    }

    public String description()
    {
        String ret = "metaData:{";
        ret += "name:" + name + ", ";
        ret += "value: " + value + ", ";
        ret += "type:" + type + "}";

        return ret;
    }


    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void import(String _name, String _type, String _value)
    {
        name = _name;
        type = _type;
        value = _value;

    }
}