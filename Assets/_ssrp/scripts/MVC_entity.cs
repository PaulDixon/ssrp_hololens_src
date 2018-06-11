using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVC_entity {
    public GameObject go;
    public List<Type> scripts;

    private bool prefabCheck(List<MVC_entity> prefabList)
    {

        foreach (MVC_entity entity in prefabList)
        {
            if (entity.go == null)
            {
                return false;
            }

            if (entity.scripts == null)
            {
                return false;
            }
        }
        return true;
    }

    public MVC_entity(GameObject _go )
    {
        go = _go;
        scripts = new List<Type> { };

    }

    public MVC_entity(GameObject _go, List<Type> _scripts)
    {
        go = _go;
        scripts = _scripts;

    }

}
