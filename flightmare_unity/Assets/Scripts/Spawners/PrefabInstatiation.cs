﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PrefabInstatiation : MonoBehaviour
{

    public GameObject prefab;
    //public int numberOfObjects = 20;
    //public float radius = 5f;
    public bool useSeed = false;
    public int seed = 0;

    public Vector3 overallPositionRandomness = new Vector3(0, 0, 0);
    public Vector3 overallRotationRandomness = new Vector3(0, 0, 0);


    private Vector3 myPosition;// = transform.position;
    private List<GameObject> createdPrefabs = new List<GameObject>();
    public float gridX = 5f;
    public float gridY = 5f;
    public float spacingX = 2f;
    public float spacingY = 2f;
    public Vector3 Rotation; //for rotating the mesh in case the mesh is upside down or something else
    public Vector3 Scale = new Vector3(1,1,1);
    private Quaternion newRotation;

    public Vector3 positionOffset = new Vector3(0, 0, 0);
    public Vector3 positionRandomness = new Vector3(0, 0, 0);

    private Vector3 addRandomRotation;

    [Range(0.0f, 1.0f)]
    public float addRandomRotationX = 0;
    [Range(0.0f, 1.0f)]
    public float addRandomRotationY = 0;
    [Range(0.0f, 1.0f)]
    public float addRandomRotationZ = 0;


    private Vector3 randomRotationValue;


    public float scaleRandomness = 0;// = new Vector3(0, 0, 0);
    /*
    [Range(0.0f, 1.0f)]
    public float scaleRandomnessX = 0;
    [Range(0.0f, 1.0f)]
    public float scaleRandomnessY = 0;
    [Range(0.0f, 1.0f)]
    public float scaleRandomnessZ = 0;
    [Range(0.0f, 100.0f)]
    */

    public float Density = 100;
    public int ForcedAmountLow = -1;
    public int ForcedAmountHigh = -1;
    private int current_forcedAmount = -1;

    public int sub_ForcedAmountLow = -1;
    public int sub_ForcedAmountHigh = -1;
    private int sub_current_forcedAmount = -1;

    public bool updateInstantiation = false;    // Click to update instances
    //private bool canUpdate = true;
    private bool regenerate = true;



    public List<GameObject> procedural_Instantiate(GameObject inGameObject)
    {
        //gameObject.isStatic = true;

        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in transform) //this.gameObject.transform)
        {
            childrenToDestroy.Add(child.gameObject);
            //DestroyImmediate(child.gameObject);
            //GameObject.DestroyImmediate(child.gameObject);
        }
        foreach (GameObject o in childrenToDestroy) //this.gameObject.transform)
        {
            GameObject.DestroyImmediate(o);
        }

        current_forcedAmount = ForcedAmountLow;

        if(ForcedAmountLow > 0 && ForcedAmountHigh > ForcedAmountLow)
        {current_forcedAmount = Random.Range(ForcedAmountLow, ForcedAmountHigh+1);}

        sub_current_forcedAmount = sub_ForcedAmountLow;

        if(sub_ForcedAmountLow > 0 && sub_ForcedAmountHigh > sub_ForcedAmountLow)
        {sub_current_forcedAmount = Random.Range(sub_ForcedAmountLow, sub_ForcedAmountHigh+1);}


        Vector3 newOverallPositionRandomness = new Vector3(Random.Range(-overallPositionRandomness.x, overallPositionRandomness.x),
        Random.Range(-overallPositionRandomness.y, overallPositionRandomness.y), Random.Range(-overallPositionRandomness.z, overallPositionRandomness.z));

        Vector3 newOverallRotationRandomness = new Vector3(Random.Range(-overallRotationRandomness.x, overallRotationRandomness.x),
        Random.Range(-overallRotationRandomness.y, overallRotationRandomness.y), Random.Range(-overallRotationRandomness.z, overallRotationRandomness.z));


        //print("Instatiating prefabs");
        print(inGameObject);
        if (inGameObject != null)
        {
            prefab = inGameObject;
        }
        if (prefab != null)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }

            myPosition = transform.position;
            newRotation = Quaternion.Euler(Rotation);
            //print("Debugging1");
            if(createdPrefabs != null)
            {
                createdPrefabs.Clear();
            }
            //print("Debugging2");

            if (regenerate && current_forcedAmount != 0)
            {

                for (int y = 0; y < gridY; y++)
                {
                    for (int x = 0; x < gridX; x++)
                    {
                        //randomRotationValue = new Vector3(Random.Range(-90.0f, 90.0f)* addRandomRotationX, Random.Range(-90.0f, 90.0f) * addRandomRotationY, Random.Range(-90.0f, 90.0f) * addRandomRotationZ));
                        /*
                        addRandomRotation = new Vector3(addRandomRotationX, addRandomRotationY, addRandomRotationZ);
                        randomRotationValue = addRandomRotation * (Random.Range(-180.0f, 180.0f));
                        newRotation = Quaternion.Euler(Rotation + randomRotationValue);
                        */
                        if (current_forcedAmount>0 || (Density / 100) >= Random.Range(0.0f, 1.0f))
                        {
                            Vector3 newPositionRandomness = new Vector3(Random.Range(-positionRandomness.x, positionRandomness.x),
                            Random.Range(-positionRandomness.y, positionRandomness.y), Random.Range(-positionRandomness.z, positionRandomness.z));

                            Vector3 pos = new Vector3(x * spacingX, 0, y * spacingY) + myPosition + newPositionRandomness + positionOffset + newOverallPositionRandomness;
                            GameObject createdPrefab = Instantiate(prefab, pos, Quaternion.Euler(Vector3.zero));
                            //print("Debugging3");
                            createdPrefabs.Add(createdPrefab);
                            //print("Debugging4");

                            createdPrefab.transform.SetParent(this.gameObject.transform); // = this.transform;
                                                                                          //prefab.transform.parent = transform;



                            //Vector3 newRandomness = new Vector3(Random.Range(0.0f, scaleRandomness.X), Random.Range(0.0f, scaleRandomness.Y), Random.Range(0.0f, scaleRandomness.Z));
                            //Vector3 newScaleRandomness = new Vector3(Random.Range(-scaleRandomnessX, scaleRandomnessX), Random.Range(-scaleRandomnessY, scaleRandomnessY), Random.Range(-scaleRandomnessZ, scaleRandomnessZ));

                            if (createdPrefab.GetComponent<SpawnerAndSwitch>())
                            {
                                createdPrefab.GetComponent<SpawnerAndSwitch>().setPlantScale(Random.Range(-scaleRandomness, scaleRandomness));
                                createdPrefab.GetComponent<SpawnerAndSwitch>().Spawn();

                            }
                            createdPrefab.transform.localScale = Scale;// + newScaleRandomness;
                            
                            addRandomRotation = new Vector3(Random.Range(-addRandomRotationX,addRandomRotationX), Random.Range(-addRandomRotationY,addRandomRotationY), Random.Range(-addRandomRotationZ,addRandomRotationZ));
                            randomRotationValue = addRandomRotation * (180.0f);//(Random.Range(-180.0f, 180.0f));
                            //randomRotationValue = addRandomRotation;
                            //print(randomRotationValue);
                            newRotation = Quaternion.Euler(Rotation + newOverallRotationRandomness + randomRotationValue);

                            createdPrefab.transform.rotation = newRotation;

                        }
                    }
                }
            }
        }
        /*
        foreach (Transform child in transform)
        {
            float newFloatScaleRandomness = Random.Range(-scaleRandomness, scaleRandomness);
            Vector3 newScaleRandomness = new Vector3(newFloatScaleRandomness, newFloatScaleRandomness, newFloatScaleRandomness);

            //Vector3 newScaleRandomness = new Vector3(newFloatScaleRandomness, newFloatScaleRandomness, newFloatScaleRandomness);
            //print(child);
            child.transform.localScale = Scale + newScaleRandomness;
        }
        */

        if(current_forcedAmount > 0)
        {
            while(current_forcedAmount < createdPrefabs.Count)
            {
                //createdPrefabs.RemoveAt(Random.Range(0,createdPrefabs.Count));
                int indexToDestroy = Random.Range(0,createdPrefabs.Count);
                GameObject objectToDestroy = createdPrefabs[indexToDestroy]; 
                createdPrefabs.RemoveAt(indexToDestroy);
                Destroy(objectToDestroy);

                //print("removing object " + createdPrefabs.Count);
            }
        }

        if(sub_current_forcedAmount > 0)
        {
            //List<GameObject> objectsToUnspawn = new List<GameObject>();
            List<GameObject> objectsToUnspawn = createdPrefabs;

            /*
            foreach (GameObject o in createdPrefabs)
            {
                objectsToUnspawn.Add(new GameObject(o));
            }
            */
            while(sub_current_forcedAmount < objectsToUnspawn.Count)
            {
                int indexToUnspwan = Random.Range(0,objectsToUnspawn.Count);
                GameObject objectToUnspawn = objectsToUnspawn[indexToUnspwan]; 
                objectsToUnspawn.RemoveAt(indexToUnspwan);
                objectToUnspawn.GetComponent<SpawnerAndSwitch>().Unspawn();

                //print("unspawning object " + objectsToUnspawn.Count);
                //print("unspawning object " + createdPrefabs.Count);
            }




        }

        print("created prefabs " + createdPrefabs.Count);

        return createdPrefabs;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (useSeed)
        { Random.seed = seed; }
    }

    // Update is called once per frame
    void Update()
    //void OnRenderObject()
    {


        if (updateInstantiation)
        {
            if (useSeed)
            { Random.seed = seed; }

            procedural_Instantiate(prefab);
            updateInstantiation = false;
        }


        /*
        if (updateInstantiation)
        {
            if (canUpdate)
            {
                canUpdate = false;
                foreach (Transform child in this.gameObject.transform)
                {
                    DestroyImmediate(child.gameObject);
                }
                Instantiate();

                updateInstantiation = false;
                canUpdate = true;
            }
        }
        */


        /*
        for (int i = 0; i < numberOfObjects; i++)
        {

            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius) + myPosition;
            Instantiate(prefab, pos, Quaternion.identity);
         }
        */


        //print(this.gameObject.name);
    }
}
