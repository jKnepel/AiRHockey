using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TableBehaviour : MonoBehaviour
{
    TapToPlace t2Pmanager;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(!t2Pmanager)
        {
            t2Pmanager = GetComponent<TapToPlace>();
        }
    }
    /*

    void OnDisable()
    {
        t2Pmanager.OnPlacingStopped.RemoveListener(PlaceTableHandler);
    }*/

    public void PlaceTableHandler()
    {
        Debug.Log("Placed Table");
        t2Pmanager.enabled = false;
    }

    public void LiftTableHandler()
    {
        t2Pmanager.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
