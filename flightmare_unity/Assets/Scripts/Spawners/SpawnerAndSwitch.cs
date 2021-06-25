﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAndSwitch : MonoBehaviour
{
    public string objectClass = "-1";


    // Start is called before the first frame update
    public virtual void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void setPlantScale(float inScale)
    {

    }

    public virtual void Spawn()
    {

        SetAllChildrenStatic(this.transform);

    }

    public virtual void Unspawn()
    {

    }

    void SetAllChildrenStatic(Transform ob)
    {
        foreach (Transform child in ob)//.GetComponentsInChildren<Transform>())
        {
            child.gameObject.isStatic = true;
            SetAllChildrenStatic(child);
        }
    }



    public virtual void SwitchToRGB()
    {

    }


    public virtual void SwitchToNIR()
    {
        /*
        foreach (GameObject leaf in createdPrefabLeaf)
        {


        }
        */       
    }

    public virtual void SwitchToTAG()
    {
        /*
        foreach (GameObject leaf in createdPrefabLeaf)
        {


        }
        */
    }
}
