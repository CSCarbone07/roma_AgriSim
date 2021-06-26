using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markerBox_spawner : SpawnerAndSwitch
{

    public GameObject[] spawnPoints;
    public GameObject[] markers;

    public GameObject current_box;
    public GameObject box_rgb;
    public GameObject box_tag;
    public GameObject box_tag_unspawned;

    private int randomOverallRotation = -1;
    private GameObject spawnedMarker;
    public GameObject boudningBox;



    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Spawn()
    {
        /*
        float Density = 75;
        if ((Density / 100) >= Random.Range(0.0f, 1.0f))
        {

        }
        */

        //print("spawning box");

        if(randomOverallRotation == -1)
        {randomOverallRotation = Random.Range(0, 4);}

        SpawnMarker();

        //box.transform.Rotate(0, 90 * randomOverallRotation, 0, Space.Self);
        //print("rotating box " + randomOverallRotation);
        transform.Rotate(0, 90 * randomOverallRotation, 0, Space.Self);


        hasBeenSpawned = true;
        
    }

    public void SpawnMarker()
    {
        spawnedMarker = Instantiate(markers[0], spawnPoints[randomOverallRotation].transform.position, spawnPoints[randomOverallRotation].transform.rotation, this.gameObject.transform);
        spawnedMarker.transform.localScale = spawnPoints[randomOverallRotation].transform.localScale;
        //createdPrefab.transform.SetParent(this.gameObject.transform); // = this.transform;

        spawnedMarker.transform.Rotate(0, 90 * Random.Range(0, 4), 0, Space.Self);

    }



    public override void Unspawn()
    {
        hasBeenSpawned = false;
        if(spawnedMarker != null)
        {
            Destroy(spawnedMarker);
        }
        if(boudningBox != null)
        {
            Destroy(boudningBox);
        }
    }







    public override void SwitchToRGB()
    {
        base.SwitchToRGB();

        GameObject spawnedInstance = Instantiate(box_rgb, current_box.transform.position, current_box.transform.rotation, this.transform);
        //spawnedInstance.transform.SetParent(this.transform);
        spawnedInstance.transform.localScale = current_box.transform.localScale;
        Destroy(current_box);
        current_box = spawnedInstance;
        //Debug.Log(createdPrefabLeavesType[x]);

        SpawnMarker();

    }

    public override void SwitchToNIR()
    {
        base.SwitchToNIR();



    }

    public override void SwitchToTAG()
    {
        base.SwitchToTAG();

        GameObject spawnedInstance;
        GameObject objectToSpawn;

        if(hasBeenSpawned)
        {
            objectToSpawn = box_tag;
        }
        else
        {
            objectToSpawn = box_tag_unspawned;
        }

        spawnedInstance = Instantiate(objectToSpawn, current_box.transform.position, current_box.transform.rotation, this.transform);
        
        //spawnedInstance.transform.SetParent(this.transform);
        spawnedInstance.transform.localScale = current_box.transform.localScale;
        Destroy(current_box);
        Destroy(spawnedMarker);
        current_box = spawnedInstance;

    }



}
