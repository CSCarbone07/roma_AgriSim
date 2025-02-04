﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

public class SwarmDatasetGeneration_Spawner : MonoBehaviour
{
    private bool DEBUG_ALL = false;

    private Vector3 cameraInitialPosition;

    public int seed = 1;
    public float delayToSave = 1;
    public float delayToSwitch = 2;
    public float delayToRespawn = 1;
    private float accumulatedDelay = 0;

    public bool varyField = true;
    public bool varyCamera_position = false;
    public Vector3 noise_camera_position = new Vector3(0, 0, 0);
    private Vector3 currentNoisePosition = new Vector3(0, 0, 0);
    public bool varyCamera_rotation = false;
    public Vector3 noise_camera_rotation = new Vector3(0, 0, 0);
    public bool varyIllumination_intensity = false;
    //public float noise_illumination_instensity = 0;
    public bool varyIllumination_orientation = false;
    //public Vector3 noise_Illumination_rotation = new Vector3(0, 0, 0);
    public bool altitudeTest = false;
    public float[] altitudes;
    private float currentTestAltitude = 0;
    private int altitudeId = 0;
    private int altitudesSize = 0;

    public bool takeOverallScreenshot = false;
    public bool overlapTest = false;
    private bool overlapTestStart = true;
    public bool takeOnlyOnePOV = false;
    public bool randomPOV = false;
    public int overlapId_Distance = -1; //-1 for it to be ignored in other tests. (NEED DOUBLE CHECKING)
    private int overlapNum_selectedBasedOnDistance = -2; // -1 to select central POV, -2 to ignore. Ignored by default in overlap test
    private List<int> distancesInPOVsIds = new List<int>();
    private int overlapNum_forImageSaving = -2;
    private int overlapNum = 0;

    public float forcedFOV = 0;
    public float forcedShiftDistance = 0;
    public int forcedWidth = 0;
    public int forcedHeight = 0;
    
    public float altitude = 10;
    private int overlapColumn = -1;
    private int overlapRow = -1;
    private int overlapWidth_0 = -1;
    private int overlapWidth_1 = -1;
    private int overlapHeight_0 = -1;
    private int overlapHeight_1 = -1;


    public bool useForcedAmounts_goodPlant = false;
    
    public bool iterateUp_goodPlant = true; //true to spawn boxes with targets up to the "ForcedAmountLow_goodPlant" and the "sub_ForcedAmountLow_goodPlant". If true these values must be above 0 as well as the "ForcedAmountHigh_goodPlant" and the "sub_ForcedAmountHigh_goodPlant"
    private bool iterateUp_goodPlant_finished = false;

    public int resetImgId_duringForcedAmounts = 0;

    public int ForcedAmountLow_goodPlant = -1;
    public int ForcedAmountHigh_goodPlant = -1;
    private int current_ForcedAmountLow_goodPlant = -1;

    public int sub_ForcedAmountLow_goodPlant = -1;
    public int sub_ForcedAmountHigh_goodPlant = -1;
    private int current_sub_ForcedAmountLow_goodPlant = -1;


    private bool useForcedAmounts_badPlant = false;
    private bool iterateUp_badPlant = true;

    public int resetImgId_duringForcedSubAmounts = 0;
    
    public int ForcedAmountLow_badPlant = -1;
    public int ForcedAmountHigh_badPlant = -1;

    public int sub_ForcedAmountLow_badPlant = -1;
    public int sub_ForcedAmountHigh_badPlant = -1;

    

    public bool Include_NIR = false;
    public bool Include_TAG = false;

    private bool firstSpawn = true;

    public GameObject goodPlantSpawner;
    private GameObject spawned_goodPlantSpawner;
    public GameObject goodPlant;
    public Vector3 goodPlant_Offset = new Vector3(0f, 0.1f, 0f);



    /*
    public SwarmDatasetGeneration_Spawner ()
    {
        beetLeaf = Instantiate(Resources.Load("Assets/Plants/2", typeof(GameObject))) as GameObject;
    }
    */



    public Vector3 goodPlantScale = new Vector3(1, 1, 1);

    public GameObject textWeedSpawner;
    private GameObject spawned_textWeedSpawner;
    public Vector3 textWeed_Offset = new Vector3(0f, 0.1f, 0f);
    private List<GameObject> newTextWeeds;


    //public static int WeedInit = 14;//5;//20;
    public int WeedNumber = 14;

    public GameObject weedPlantSpawner;
    private GameObject spawned_weedPlantSpawner;
    public GameObject[] weedPlants;
    public Vector3[] weedPlants_Offset;
    public Vector3[] weedPlants_Rotation_Min;
    public Vector3[] weedPlants_Rotation_Max;
    public Vector3[] weedPlants_Scale;



    public GameObject terrain;

    public Vector3 terrainOffset = new Vector3(0, 0, 0);

    private Material red;
    private Material black;
    // to select the kind of annotation to be generated
    public bool TakeScreenshots = true;
    public bool SaveBoundingBoxes = true;
    public bool SaveBoxes = false;

    protected Quaternion newRotation;
    protected Vector3 randomRotationValue;

    protected static int plantNumber = 84;//9;//65;

    //private static int CapsellaNumber = 8;
    //private static int GalliumNumber = 8;


    string[] boxes; //= new string[plantNumber + WeedNumber];

    // object instances 
    protected List<GameObject> allPlants = new List<GameObject>();
    protected GameObject newPlantField;
    //protected GameObject[] newPlant;// = new GameObject[plantNumber];
    //protected GameObject[] newWeed; //= new GameObject[WeedInit];
    protected List<GameObject> newPlant;
    protected List<GameObject> newWeed;

    protected GameObject newTerrain;

    //public string classLabel = "0";

    protected int beetLeafAmount = 7;
    protected int galliumLeafAmount = 5;
    protected int capsellaLeafAmount = 8;

    // control the spawing ratio, remember to check also Invoke() functions
    //protected float clearDelay = 2f;
    //protected float spawnDelay = 3f;
    //protected float NIRswitchDelay = 1f;
    //protected float TAGswitchDelay = 1f;
    //protected float nextSpawnTime = 0f;
    //protected float nextNIRswitch = 1f;


    // range for spawing objects
    protected float minScaleValue = 0.2f;
    protected float maxScaleValue = 1f;

    // control missing beet ratio
    protected float missBeet = 10f;
    // control gallium/capsella ratio
    protected float weedType = 7.5f;

    // counter to save pictures incrementally
    protected static int imgPerWeedNumber = 5;
    protected int counter;// = WeedNumber * imgPerWeedNumber;
    public int minImageIndex = 1;
    public int maxImageIndex = 10;

    public int varyFieldInterval = 10;

    // camera resolution
    public int width = 1024;
    public int height = 1024;

    Vector3 spawnPoint;
    Vector3 scaleFactor;

    protected Vector3 zeroPos = new Vector3(0f, 0.4f, 0f);
    protected Vector3 cameraPos = new Vector3(-1f, 7f, -0.7f);
    protected Vector3 cameraRot = new Vector3(90, 0, 0);
    protected Vector3 defaultScale = new Vector3(25f, 1f, 25f);
    protected Vector3 defaultPos = new Vector3(0f, 0.5f, 0f); //default terrain dimensions
    protected Vector3 beetLeafRotation = new Vector3(200f, 0f, -90f);
    protected Vector3 galliumLeafRotation = new Vector3(-90, 0f, 0f);
    protected Vector3 capsellaLeafRotation = new Vector3(0f, 0f, 0f);

    Quaternion zeroRot = Quaternion.Euler(0, 0, 0);
    Quaternion rotation;

    // control where to save pictures
    public enum type { Image, Mask, Box };
    public enum cls { Crop, Weed };
    public enum field { A, B, C, D, E, F, G, H, I, L };
    public enum species { Beet, Gall, Caps };

    protected species[] specs; // = new species[plantNumber + WeedInit];
    public ArrayList positions = new ArrayList();
    public ArrayList rotations = new ArrayList();

    // Start is called before the first frame update
    public void Start()
    {

        if(!overlapTest)
        {
            overlapRow = 0;
            overlapColumn = 0;
        }


        //Random.seed = seed;
        altitudesSize = altitudes.Length;
    

        cameraInitialPosition = this.transform.position;
        //newWeed = new GameObject[WeedNumber];
        boxes = new string[plantNumber + WeedNumber];
        //counter = WeedNumber * imgPerWeedNumber;

        counter = minImageIndex-1; // so its incremented again in the dataloop
        specs = new species[plantNumber + WeedNumber];

        /*
        if(Include_NIR)
        {        
            spawnDelay = 8;
            clearDelay = 6;
            TAGswitchDelay = 4;
            NIRswitchDelay = 2;
        }
        else
        {
            spawnDelay = 6;
            clearDelay = 4;
            TAGswitchDelay = 2;
        }
        */
        if(!varyField)
        {
            SpawnTerrain();
            Spawn(); 
        }
        dataLoop();


    }

    // Update is called once per frame
    public void Update()
    {
        //Debug.Log(nextSpawnTime);

    }
     
    Vector3 myRotateY(Vector3 v, float angle )
    {
        Vector3 outVector = new Vector3();
        
        float sin = Mathf.Sin( angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos( angle * Mathf.Deg2Rad);
   
        float tx = v.x;
        float tz = v.z;

        outVector.x = (cos * tx) - (sin * tz);
        outVector.y = 0;
        outVector.z = (sin * tx) + (cos * tz);

        return outVector;
    }
    
    void dataLoop()
    {
	#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate();
	#endif
        GC.Collect();


	if(DEBUG_ALL) 
	{Debug.Log("spawning");}
        CounterUpdate();
        //Random.seed = counter;
        seed++;
        Random.seed = seed;

        //int altitudesSize = altitudes.Length;

        //print(Application.persistentDataPath);
        if (firstSpawn || (overlapTestStart && 
        (varyField && (varyFieldInterval > 0 && counter > 1 && (counter-1) % varyFieldInterval == 0)) 
        && (!altitudeTest || altitudeId == altitudesSize)))
        {

	    if(DEBUG_ALL)
	    {
            print("respawning on counter " + counter);
            print("Varation interval " + varyFieldInterval);
	    }
            SpawnTerrain();
            Spawn();
        }
        else
        {
            if (!firstSpawn)
            {
                SwitchToRGB();
		if(DEBUG_ALL)
		{
                print("switched to RGB on counter " + counter);
		}
            }

        }

        if (overlapId_Distance >= 0)
        {
            setOverlapId_basedOnOverallRotation();
        }
        if(overlapId_Distance == -1)
        {
            overlapNum_selectedBasedOnDistance = 5;
        }

        if (varyCamera_rotation && (firstSpawn || 
        (overlapTestStart && (!altitudeTest || altitudeId == altitudesSize)) ))
        {

            float randomRotX = Random.Range(-noise_camera_rotation.x, noise_camera_rotation.x);
            float randomRotY = Random.Range(-noise_camera_rotation.y, noise_camera_rotation.y);
            float randomRotZ = Random.Range(-noise_camera_rotation.z, noise_camera_rotation.z);


	    if(DEBUG_ALL) 
	    {
	      print("varying rotation " + randomRotY);
	    }


            this.transform.rotation = Quaternion.Euler(90 + randomRotX, randomRotY, randomRotZ);
            //this.transform.rotation = Quaternion.Euler(90, 90, 0);
        }

        if ((varyIllumination_intensity || varyIllumination_orientation))// && overlapTestStart && (!altitudeTest || altitudeId == 0))
        {
            RandomLightAndPosition();
        }



        float randInitX = 0;
        float randInitY = 0;
        float randInitZ = 0;

        if (varyCamera_position && (firstSpawn || 
        (overlapTestStart && (!altitudeTest || altitudeId==altitudesSize)) ))
        { 
	    if(DEBUG_ALL) 
	    {
	      print("Varying position");
	    }

            randInitX = Random.Range(-noise_camera_position.x, noise_camera_position.x);
            randInitY = Random.Range(-noise_camera_position.y, noise_camera_position.y);
            randInitZ = Random.Range(-noise_camera_position.z, noise_camera_position.z);

            currentNoisePosition = new Vector3(randInitX, randInitY, randInitZ);
        }

        if(altitudeTest && overlapTestStart)
        {
	    if(DEBUG_ALL) 
	    {
	      print("Increasing altitude id from " + altitudeId);
	    }

            if(altitudeId == altitudesSize)
            {
                altitudeId = 0;
            }

            currentTestAltitude = altitudes[altitudeId];
            //altitude = currentTestAltitude;
            cameraInitialPosition = new Vector3(cameraInitialPosition.x, currentTestAltitude, cameraInitialPosition.z) ;
            /*
            if(altitudeId>0)
            {
                counter--;
            }
            */
            altitudeId++;
        }
        this.transform.position = cameraInitialPosition + currentNoisePosition;
        altitude = this.transform.position.y;

        float fov;
        float shiftDistance;

        if(forcedFOV>0)
        {
            fov = forcedFOV;
        }
        else
        {
            fov = GetComponent<Camera>().fieldOfView;
        }

        if(forcedShiftDistance>0)
        {
            shiftDistance = forcedShiftDistance;
        }
        else
        {
            shiftDistance = 2 * altitude * Mathf.Tan((fov/2)* Mathf.Deg2Rad);
        }


        if (overlapTest)
        {
	    if(DEBUG_ALL) 
	    {
	      print("Increasing overlap id from " + overlapNum);
	    }

            overlapNum++;
            if (overlapTestStart)
            {
                //UnityEditor.EditorApplication.isPlaying = false;
                overlapRow = -1;
                overlapNum = 1;
            }
            

            overlapTestStart = false;

            Vector3 movingVector = new Vector3((shiftDistance / 3.0f) * overlapColumn, 0, (shiftDistance / 3.0f) * overlapRow);
            movingVector = myRotateY(movingVector, -this.transform.eulerAngles.y);
            this.transform.position = cameraInitialPosition + currentNoisePosition + movingVector;
            //-1 0 1
            //2 1 0


            if(forcedWidth == 0 && forcedHeight ==0)
            {
                overlapWidth_0 =((width / 3) * (1-overlapColumn));
                overlapWidth_1 =((width / 3) * (2-overlapColumn));

                overlapHeight_0 = ((height / 3) * (1-overlapRow));
                overlapHeight_1 = ((height / 3) * (2-overlapRow));
            }
            else
            {
                int trimWidth = width - forcedWidth;
                int trimHeight = height - forcedHeight;

                overlapWidth_0 =((forcedWidth / 3) * (1-overlapColumn))+trimWidth/2;
                overlapWidth_1 =((forcedWidth / 3) * (2-overlapColumn))+trimWidth/2;

                overlapHeight_0 = ((forcedHeight / 3) * (1-overlapRow))+trimHeight/2;
                overlapHeight_1 = ((forcedHeight / 3) * (2-overlapRow))+trimHeight/2;
                /*
                print("altitude: " + altitude);
                print("width: " + width);
                print("height: " + height);
                print("forcedWidth: " + forcedWidth);
                print("forcedHeight: " + forcedHeight);
                print("trimWidth: " + trimWidth);
                print("trimHeight: " + trimHeight);
                print("overlapColumn: " + overlapColumn);
                print("overlapRow: " + overlapRow); 
                print("overlapWidth_0: " + overlapWidth_0);
                print("overlapWidth_1: " + overlapWidth_1);
                print("overlapHeight_0: " + overlapHeight_0);
                print("overlapHeight_1: " + overlapHeight_1);
                */

            }
            

            //print("Camera moved to position: " + this.transform.position);
            //print("Camera moved by: " + movingVector);

            overlapColumn++;
            if (overlapColumn > 1)
            {
                overlapColumn = -1;
                overlapRow++;
                if(overlapRow>1)
                {
                    overlapTestStart = true;
                }
            }
        }

        if(!overlapTest && takeOnlyOnePOV)
        {
            if(randomPOV)
            {
                overlapColumn = UnityEngine.Random.Range(-1, 2);
                overlapRow = UnityEngine.Random.Range(-1, 2);
            }
            else
            {
                if(overlapNum_selectedBasedOnDistance >= -1)
                {
                    overlapColumn = (overlapNum_selectedBasedOnDistance-1 - ((overlapNum_selectedBasedOnDistance-1)/3)*3)-1;
                    overlapRow = ((overlapNum_selectedBasedOnDistance-1)/3) - 1;
                }
                else
                {
                    overlapColumn = 0;
                    overlapRow = 0;
                }
            }



            Vector3 movingVector = new Vector3((shiftDistance / 3.0f) * overlapColumn, 0, (shiftDistance / 3.0f) * overlapRow);
            movingVector = myRotateY(movingVector, -this.transform.eulerAngles.y);
            this.transform.position = cameraInitialPosition + currentNoisePosition + movingVector;

            overlapNum = (overlapColumn + 2) + (overlapRow+1)*3;
            if (DEBUG_ALL) {print("used overlap id " + overlapNum);}

            if(forcedWidth == 0 && forcedHeight ==0)
            {
                overlapWidth_0 =((width / 3) * (1-overlapColumn));
                overlapWidth_1 =((width / 3) * (2-overlapColumn));

                overlapHeight_0 = ((height / 3) * (1-overlapRow));
                overlapHeight_1 = ((height / 3) * (2-overlapRow));
            }
            else
            {
                int trimWidth = width - forcedWidth;
                int trimHeight = height - forcedHeight;

                overlapWidth_0 =((forcedWidth / 3) * (1-overlapColumn))+trimWidth/2;
                overlapWidth_1 =((forcedWidth / 3) * (2-overlapColumn))+trimWidth/2;

                overlapHeight_0 = ((forcedHeight / 3) * (1-overlapRow))+trimHeight/2;
                overlapHeight_1 = ((forcedHeight / 3) * (2-overlapRow))+trimHeight/2;
                
		if(DEBUG_ALL) 
		{
		  print("altitude: " + altitude);
		  print("width: " + width);
		  print("height: " + height);
		  print("forcedWidth: " + forcedWidth);
		  print("forcedHeight: " + forcedHeight);
		  print("trimWidth: " + trimWidth);
		  print("trimHeight: " + trimHeight);
		  print("overlapColumn: " + overlapColumn);
		  print("overlapRow: " + overlapRow); 
		  print("overlapWidth_0: " + overlapWidth_0);
		  print("overlapWidth_1: " + overlapWidth_1);
		  print("overlapHeight_0: " + overlapHeight_0);
		  print("overlapHeight_1: " + overlapHeight_1);
		} 

            }


        }

/*
        if(overlapTest || takeOnlyOnePOV)
        {
            if(forcedWidth == 0 && forcedHeight ==0)
            {
                overlapWidth_0 =((width / 3) * (1-overlapColumn));
                overlapWidth_1 =((width / 3) * (2-overlapColumn));

                overlapHeight_0 = ((height / 3) * (1-overlapRow));
                overlapHeight_1 = ((height / 3) * (2-overlapRow));
            }
            else
            {
                int trimWidth = width - forcedWidth;
                int trimHeight = height - forcedHeight;

                overlapWidth_0 =((forcedWidth / 3) * (1-overlapColumn))+trimWidth/2;
                overlapWidth_1 =((forcedWidth / 3) * (2-overlapColumn))+trimWidth/2;

                overlapHeight_0 = ((forcedHeight / 3) * (1-overlapRow))+forcedHeight/2;
                overlapHeight_1 = ((forcedHeight / 3) * (2-overlapRow))+forcedHeight/2;
            }
        }

*/


	if(DEBUG_ALL) 
	{
	  print("loop finished, setting material and respawn delays");
	}

        accumulatedDelay = 0;

        if (TakeScreenshots)
        {
            //Invoke("saveSingleMasks", 1f);
            //Invoke("SaveRGB", 0.5f);
            accumulatedDelay += delayToSave;
            Invoke("SaveRGB", accumulatedDelay);
        }

        if (Include_NIR)
        {
            accumulatedDelay += delayToSwitch;
            Invoke("SwitchToNIR", accumulatedDelay);
            if (TakeScreenshots)
            {
                //Invoke("SaveNIR", 3.0f);
                //Invoke("SaveTAG", 5.0f);

                accumulatedDelay += delayToSave;
                Invoke("SaveNIR", accumulatedDelay);
            }
        }

        if(Include_TAG)
        {
            accumulatedDelay += delayToSwitch;
            Invoke("SwitchToTAG", accumulatedDelay);
            if (TakeScreenshots)
            {
                //Invoke("saveTAG", 1.5f);
                accumulatedDelay += delayToSave;
                Invoke("SaveTAG", accumulatedDelay);
            }
        }


	if(DEBUG_ALL) 
	{
	  print("materials invoked");
	}

        //if (varyField || (varyFieldInterval > 0 && counter % varyFieldInterval == 0))
        if ((varyField && (varyFieldInterval > 0 && counter > 0 && (counter) % varyFieldInterval == 0)) 
        && (!altitudeTest || altitudeId == altitudesSize) && (!overlapTest || (overlapTest && overlapRow > 1)))
        {
            accumulatedDelay += delayToSwitch;
            Invoke("clearScene", accumulatedDelay);
            accumulatedDelay += delayToRespawn;
            Invoke("dataLoop", accumulatedDelay);
        }
        else
        {
            accumulatedDelay += delayToRespawn;
            Invoke("dataLoop", accumulatedDelay);
        }
	
	if(DEBUG_ALL) 
	{
	  print("respawns invoked");
	} 
	firstSpawn = false;

    }

    private void CounterUpdate()
    {
        if (firstSpawn || ((!altitudeTest || altitudeId == altitudesSize) 
        && (!overlapTest || (overlapTest && overlapRow > 1))))
        { counter++; }
    
        if(counter > maxImageIndex)// && (!altitudeTest || altitudeId == altitudesSize))
        {
	  #if UNITY_EDITOR  
	  UnityEditor.EditorApplication.isPlaying = false;
	  #endif
        }
    }

    private void clearScene()
    {
	
	if(DEBUG_ALL) 
	{
	  print("clearing scene");
	}
        Destroy(newTerrain);

        foreach (GameObject p in newPlant)
        {
            Destroy(p);
        }

        
        if (weedPlantSpawner != null)
        {
            foreach (GameObject w in newWeed)
            {
                Destroy(w);
            }
        }

        if(textWeedSpawner != null)
        {
            foreach (GameObject w in newTextWeeds)
            {
                Destroy(w);
            }
            List<GameObject> cells = spawned_textWeedSpawner.GetComponent<readerSpawner>().getCellObjects();
            foreach (GameObject w in cells)
            {
                Destroy(w);
            }
        }

	#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate();
	#endif
        GC.Collect();

    }



    private void Spawn()
    {
        int cnt = 0;
        float x_offset = 1.25f;
        float z_offset = 0.53f;

        Vector3 pos = RandomPosition();
        Vector3 plant_start_pos = goodPlant_Offset;
        Vector3 weed_start_pos = weedPlants_Offset[0];
	
	if(DEBUG_ALL) 
	{
	print("spawning all");
	}
        if(useForcedAmounts_goodPlant)
        {
	  //print("forced plants iteration, overlap column " + overlapColumn + " overlapRow " + overlapRow + " overlapNum " + overlapNum);
	  // Considering both overlapNums for first sim spawn and loops
	   

	  if(!overlapTest || (overlapNum == 0) || (overlapNum == 9 ))
	  {
            if(iterateUp_goodPlant)
            { 
                if(current_ForcedAmountLow_goodPlant == -1 && current_sub_ForcedAmountLow_goodPlant ==-1)
                {
                    current_ForcedAmountLow_goodPlant = 0;
                    current_sub_ForcedAmountLow_goodPlant = 0;
                }
		else
		{
		  if(!(current_ForcedAmountLow_goodPlant == -1 && current_sub_ForcedAmountLow_goodPlant ==-1))
		  {
		      // This restarts the counting of the forced spawns. 
		      //if(current_ForcedAmountLow_goodPlant == ForcedAmountLow_goodPlant && current_sub_ForcedAmountLow_goodPlant == sub_ForcedAmountLow_goodPlant)
		      if(current_ForcedAmountLow_goodPlant == ForcedAmountLow_goodPlant && current_sub_ForcedAmountLow_goodPlant == sub_ForcedAmountLow_goodPlant)
		      {
			  current_ForcedAmountLow_goodPlant = 0;
			  current_sub_ForcedAmountLow_goodPlant = 0;
		      }
		      else
		      {
			  // This drops the counter back for simulation 
			  counter --;

			  if(current_sub_ForcedAmountLow_goodPlant == current_ForcedAmountLow_goodPlant)
			  {
			      current_ForcedAmountLow_goodPlant ++;
			      current_sub_ForcedAmountLow_goodPlant = 0;
			  }
			  else
			  {
			      current_sub_ForcedAmountLow_goodPlant ++;
			  }
		      }
		  }
		}
		if(DEBUG_ALL) 
		{
		  print ("iterating. Current low " +  current_ForcedAmountLow_goodPlant + " current sub low " + current_sub_ForcedAmountLow_goodPlant);
		  print ("iterating. forced low " +  ForcedAmountLow_goodPlant + " forced sub low " + sub_ForcedAmountLow_goodPlant);
		}     
            }
            else
            {
                current_ForcedAmountLow_goodPlant = ForcedAmountLow_goodPlant;
                current_sub_ForcedAmountLow_goodPlant = sub_ForcedAmountLow_goodPlant;
            }
	  }
        }

	if(DEBUG_ALL) 
	{
	  print ("Spawning plant. Current low " +  current_ForcedAmountLow_goodPlant + " current sub low " + current_sub_ForcedAmountLow_goodPlant);
	}

        if(goodPlantSpawner!=null)
        { 
            if(spawned_goodPlantSpawner == null)
            {
                spawned_goodPlantSpawner = Instantiate(goodPlantSpawner, plant_start_pos, Quaternion.Euler(0, 0, 0));
	    }
	    if(spawned_goodPlantSpawner != null)
	    { 
                if(useForcedAmounts_goodPlant)
                {spawned_goodPlantSpawner.GetComponent<PrefabInstatiation>().setForcedAmounts(current_ForcedAmountLow_goodPlant, ForcedAmountHigh_goodPlant, current_sub_ForcedAmountLow_goodPlant, sub_ForcedAmountHigh_goodPlant);}
                newPlant = spawned_goodPlantSpawner.GetComponent<PrefabInstatiation>().procedural_Instantiate(goodPlant);
                if(GetComponent<BoundingBox_Plants>() != null && newPlant != null)
                {
                    //allPlants.Clear();
                    allPlants = newPlant;
                }
            }
        }
	if(DEBUG_ALL) 
	{
	  print("good plant spawned");
	}
        if (weedPlantSpawner != null)
        {
            if (spawned_weedPlantSpawner == null)
            {
                spawned_weedPlantSpawner = Instantiate(weedPlantSpawner, weed_start_pos, Quaternion.Euler(0, 0, 0));
            }
            if (spawned_weedPlantSpawner != null)
            {
                newWeed = spawned_weedPlantSpawner.GetComponent<PrefabInstatiation>().procedural_Instantiate(weedPlants[0]);
                if(GetComponent<BoundingBox_Plants>() != null)
                {
                    allPlants.AddRange(newWeed);
                }
            }
        }

        if(textWeedSpawner != null)
        {
            if (spawned_textWeedSpawner == null)
            {
                spawned_textWeedSpawner = Instantiate(textWeedSpawner, textWeed_Offset, Quaternion.Euler(0, 0, 0));
            }
            if(spawned_textWeedSpawner != null)
            {
                string fieldFile = "field_" + counter.ToString();
                spawned_textWeedSpawner.GetComponent<readerSpawner>().readFile(fieldFile);
                newTextWeeds = spawned_textWeedSpawner.GetComponent<readerSpawner>().getSpawnedObjects();
                if(GetComponent<BoundingBox_Plants>() != null)
                {
                    allPlants.AddRange(newTextWeeds);
                }
            }
        }

        if(GetComponent<BoundingBox_Plants>() != null && allPlants.Count>0 && (spawned_goodPlantSpawner != null || spawned_textWeedSpawner != null))
        {
            GetComponent<BoundingBox_Plants>().setPlantSpawner(allPlants);
        }
	  
	if(overlapTest && useForcedAmounts_goodPlant)
	{
	  getOverlapId_basedOnOverallRotation();
	}

    }

    private void SwitchToRGB()
    {
	if(DEBUG_ALL) 
	{
	  print("Switching to RGB");
	}
        foreach (GameObject g in newPlant)
        {
            g.GetComponent<SpawnerAndSwitch>().SwitchToRGB();
        }
        if (weedPlantSpawner != null)
        {
            foreach (GameObject g in newWeed)
            {
                g.GetComponent<SpawnerAndSwitch>().SwitchToRGB();
            }
        }
        if (textWeedSpawner != null)
        {
            foreach (GameObject g in newTextWeeds)
            {
                g.GetComponent<SpawnerAndSwitch>().SwitchToRGB();
            }
        }
        newTerrain.GetComponent<SpawnerAndSwitch>().SwitchToRGB();
    }

    private void SwitchToNIR()
    {
	if(DEBUG_ALL) 
	{
	  print("Switching to NIR");
	}
        foreach (GameObject g in newPlant)
        {
            g.GetComponent<SpawnerAndSwitch>().SwitchToNIR();
        }
        if (weedPlantSpawner != null)
        {
            foreach (GameObject g in newWeed)
            {
                g.GetComponent<SpawnerAndSwitch>().SwitchToNIR();
            }
        }
        if (textWeedSpawner != null)
        {
            foreach (GameObject g in newTextWeeds)
            {
                g.GetComponent<SpawnerAndSwitch>().SwitchToNIR();
            }
        }
        newTerrain.GetComponent<SpawnerAndSwitch>().SwitchToNIR();
    }

    private void SwitchToTAG()
    {
	if(DEBUG_ALL) 
	{
	  print("Switching to TAG");
	}
        foreach (GameObject g in newPlant)
        {
            g.GetComponent<SpawnerAndSwitch>().SwitchToTAG();
        }
        if (weedPlantSpawner != null)
        {
            foreach (GameObject g in newWeed)
            {
                g.GetComponent<SpawnerAndSwitch>().SwitchToTAG();
            }
        }
        if (textWeedSpawner != null)
        {
            foreach (GameObject g in newTextWeeds)
            {
                g.GetComponent<SpawnerAndSwitch>().SwitchToTAG();
            }
        }
        newTerrain.GetComponent<SpawnerAndSwitch>().SwitchToTAG();
    }



    private void SaveRGB() //mode can be type.Image or type.Mask
    {
	if(DEBUG_ALL) 
	{
	  print("Saving rgb with counter " + counter + " CFALG " + current_sub_ForcedAmountLow_goodPlant + " CFASLG " 
	      + current_sub_ForcedAmountLow_goodPlant + " overlapNum " + overlapNum);
	}
        
	RenderTexture rt;
        Texture2D screenShot;
        if (overlapTest || takeOnlyOnePOV)
	{
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            if(forcedWidth == 0 && forcedHeight == 0)
            {
                screenShot = new Texture2D(width/3, height/3, TextureFormat.RGB24, false);
            }
            else
            {
                screenShot = new Texture2D(forcedWidth/3, forcedHeight/3, TextureFormat.RGB24, false);
            }
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(overlapWidth_0, overlapHeight_0, overlapWidth_1, overlapHeight_1), 0, 0, false);
        }
        else
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);     
        }


        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        //string filename = ScreenshotName(mode, field);
        string filename = "rgb variable not assigned";
        if (altitudeTest && !overlapTest)
        {
            //filename = string.Format("{0}/Dataset/{1}/rgb/{2}_{3}{4}.png"
            //, Application.persistentDataPath, currentTestAltitude, counter, overlapRow, overlapColumn);
            filename = string.Format("{0}/Dataset/rgb/{1}_{2}_{3}.png"
            , Application.persistentDataPath, currentTestAltitude, counter, overlapNum);
        }
        if (!altitudeTest && overlapTest)
	{
            filename = string.Format("{0}/Dataset/rgb/{1}_{2}.png"
            , Application.persistentDataPath, counter, overlapNum);

            if(useForcedAmounts_goodPlant)
            {
            filename = string.Format("{0}/Dataset/rgb/{1}_{2}_{3}_{4}_{5}.png"
            , Application.persistentDataPath, counter, overlapNum, current_ForcedAmountLow_goodPlant, current_sub_ForcedAmountLow_goodPlant, distancesInPOVsIds[overlapNum - 1]);
            }

	}
        if(!altitudeTest && !overlapTest)
	{
            //filename = string.Format("{0}/Dataset/rgb/{1}_{2}{3}.png"
            //, Application.persistentDataPath, counter, overlapRow, overlapColumn);
            filename = string.Format("{0}/Dataset/rgb/{1}_{2}.png"
            , Application.persistentDataPath, counter, overlapNum);

            if(useForcedAmounts_goodPlant)
            {
            filename = string.Format("{0}/Dataset/rgb/{1}_{2}_{3}.png"
            , Application.persistentDataPath, counter, current_ForcedAmountLow_goodPlant, current_sub_ForcedAmountLow_goodPlant);
            }
        }

        System.IO.File.WriteAllBytes(filename, bytes);

        

        if(SaveBoundingBoxes)
        {
            string boxFileName = "box variable not assigned";
            if (altitudeTest && !overlapTest)
            {
                //boxFileName = string.Format("{0}/Dataset/{1}/boxes/{2}_{3}{4}.txt"
                //, Application.persistentDataPath, currentTestAltitude, counter, overlapRow, overlapColumn);
                boxFileName = string.Format("{0}/Dataset/boxes/{1}_{2}_{3}.txt"
                , Application.persistentDataPath, currentTestAltitude, counter, overlapNum);
            }
	    if (!altitudeTest && overlapTest)
	    {
                boxFileName = string.Format("{0}/Dataset/boxes/{1}_{2}.txt"
                , Application.persistentDataPath, counter, overlapNum);
                
                if(useForcedAmounts_goodPlant)
                {
                boxFileName = string.Format("{0}/Dataset/boxes/{1}_{2}_{3}_{4}_{5}.txt"
		, Application.persistentDataPath, counter, overlapNum, current_ForcedAmountLow_goodPlant, current_sub_ForcedAmountLow_goodPlant, distancesInPOVsIds[overlapNum - 1]);
                }

	    }
	    if(!altitudeTest && !overlapTest)
            {
                boxFileName = string.Format("{0}/Dataset/boxes/{1}_{2}.txt"
                , Application.persistentDataPath, counter, overlapNum);
                
                if(useForcedAmounts_goodPlant)
                {
                boxFileName = string.Format("{0}/Dataset/boxes/{1}_{2}_{3}.txt"
                , Application.persistentDataPath, counter, current_ForcedAmountLow_goodPlant, current_sub_ForcedAmountLow_goodPlant);
                }

            }
            if (GetComponent<BoundingBox_Plants>())
            { GetComponent<BoundingBox_Plants>().saveBoxes(newPlant, filename, boxFileName, 
            overlapWidth_0, overlapHeight_0, overlapWidth_1, overlapHeight_1); }
        }


        if (takeOverallScreenshot && overlapTest && !takeOnlyOnePOV && overlapRow==0 && overlapColumn == 1)
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            bytes = screenShot.EncodeToPNG();

            if (altitudeTest)
            {
                filename = string.Format("{0}/Dataset/rgb/{1}_{2}.png"
                , Application.persistentDataPath, currentTestAltitude, counter);
            }
            else
            {
                filename = string.Format("{0}/Dataset/rgb/{1}.png"
                , Application.persistentDataPath, counter);
            }
            System.IO.File.WriteAllBytes(filename, bytes);

            

            if(SaveBoundingBoxes)
            {
                string boxFileName;
                if (altitudeTest)
                {
                    boxFileName = string.Format("{0}/Dataset/boxes/{1}_{2}.txt"
                    , Application.persistentDataPath, currentTestAltitude, counter);
                }
                else
                {
                    boxFileName = string.Format("{0}/Dataset/boxes/{1}.txt"
                    , Application.persistentDataPath, counter);
                }
                if (GetComponent<BoundingBox_Plants>())
                { GetComponent<BoundingBox_Plants>().saveBoxes(newPlant, filename, boxFileName, 
                -1, -1, -1, -1); } //TODO remove class, it is being done somewhere else
            }


        }




    }

    private void SaveNIR() //mode can be type.Image or type.Mask
    {

        RenderTexture rt;
        Texture2D screenShot;
        if (overlapTest)
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            //screenShot = new Texture2D(width / 3, height / 3, TextureFormat.RGB24, false);
            if(forcedWidth == 0 && forcedHeight == 0)
            {
                screenShot = new Texture2D(width/3, height/3, TextureFormat.RGB24, false);
            }
            else
            {
                screenShot = new Texture2D(forcedWidth/3, forcedHeight/3, TextureFormat.RGB24, false);
            }
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(overlapWidth_0, overlapHeight_0, overlapWidth_1, overlapHeight_1), 0, 0, false);
        }
        else
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        }

        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        //string filename = ScreenshotName(mode, field);
	//string filename = string.Format("{0}/Dataset/nir/{1}.png", Application.persistentDataPath, counter);
        string filename;
        if (altitudeTest)
        {
            //filename = string.Format("{0}/Dataset/{1}/tag/{2}_{3}{4}.png"
            //, Application.persistentDataPath, currentTestAltitude, counter, overlapRow, overlapColumn);
            filename = string.Format("{0}/Dataset/nir/{1}_{2}_{3}.png"
            , Application.persistentDataPath, currentTestAltitude, counter, overlapNum);
        }
        else
        {
            //filename = string.Format("{0}/Dataset/tag/{1}_{2}{3}.png"
            //, Application.persistentDataPath, counter, overlapRow, overlapColumn);
            filename = string.Format("{0}/Dataset/nir/{1}_{2}.png"
            , Application.persistentDataPath, counter, overlapNum);
        }
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    private void SaveTAG() //mode can be type.Image or type.Mask
    {

        RenderTexture rt;
        Texture2D screenShot;
        if (overlapTest || takeOnlyOnePOV)
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            //screenShot = new Texture2D(width / 3, height / 3, TextureFormat.RGB24, false);
            if(forcedWidth == 0 && forcedHeight == 0)
            {
                screenShot = new Texture2D(width/3, height/3, TextureFormat.RGB24, false);
            }
            else
            {
                screenShot = new Texture2D(forcedWidth/3, forcedHeight/3, TextureFormat.RGB24, false);
            }
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(overlapWidth_0, overlapHeight_0, overlapWidth_1, overlapHeight_1), 0, 0, false);
        }
        else
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        }

        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        //string filename = string.Format("{0}/Dataset/tag/{1}.png", Application.persistentDataPath, counter);
        string filename;
        if (altitudeTest && !overlapTest)
        {
            //filename = string.Format("{0}/Dataset/{1}/tag/{2}_{3}{4}.png"
            //, Application.persistentDataPath, currentTestAltitude, counter, overlapRow, overlapColumn);
            filename = string.Format("{0}/Dataset/tag/{1}_{2}_{3}.png"
            , Application.persistentDataPath, currentTestAltitude, counter, overlapNum);
        }
        else
        {
            //filename = string.Format("{0}/Dataset/tag/{1}_{2}{3}.png"
            //, Application.persistentDataPath, counter, overlapRow, overlapColumn);
            filename = string.Format("{0}/Dataset/tag/{1}_{2}.png"
            , Application.persistentDataPath, counter, overlapNum);
        }

        if (!altitudeTest && overlapTest)
	{
            filename = string.Format("{0}/Dataset/tag/{1}_{2}.png"
            , Application.persistentDataPath, counter, overlapNum);

            if(useForcedAmounts_goodPlant)
            {
            filename = string.Format("{0}/Dataset/tag/{1}_{2}_{3}_{4}_{5}.png"
            , Application.persistentDataPath, counter, overlapNum, current_ForcedAmountLow_goodPlant, current_sub_ForcedAmountLow_goodPlant, distancesInPOVsIds[overlapNum - 1]);
            }

	}


        System.IO.File.WriteAllBytes(filename, bytes);



        if (takeOverallScreenshot && overlapTest && !takeOnlyOnePOV && overlapRow==0 && overlapColumn == 1)
        {
            rt = new RenderTexture(width, height, 24);
            GetComponent<Camera>().targetTexture = rt;
            screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            bytes = screenShot.EncodeToPNG();
            if (altitudeTest)
            {
                filename = string.Format("{0}/Dataset/tag/{1}_{2}.png"
                , Application.persistentDataPath, currentTestAltitude, counter);
            }
            else
            {
                filename = string.Format("{0}/Dataset/tag/{1}.png"
                , Application.persistentDataPath, counter);
            }
            System.IO.File.WriteAllBytes(filename, bytes);


        }



    }



    private void RandomLightAndPosition()
    {
        //change light
        //if (myLight == null)
        GameObject myLight = GameObject.Find("Directional Light"); 
        if (myLight != null)
        {
            //print("change light");
            if (varyIllumination_intensity)
            { myLight.GetComponent<RandomLight>().changeLight_intensity(); }
            if(varyIllumination_orientation)
            { myLight.GetComponent<RandomLight>().changeLight_orientation(); }
        }
    }

    private void DefinePositions()
    {
        positions.Clear();
        positions.Add(transform.position);
        for (int i = 0; i < 9; i++)
        {
            positions.Add(transform.position + new Vector3(UnityEngine.Random.Range(-0.15f, +0.15f), 0f, UnityEngine.Random.Range(-0.15f, +0.15f)));
        }

        rotations.Clear();
        rotations.Add(Quaternion.Euler(cameraRot));
        for (int i = 0; i < 9; i++)
        {
            rotations.Add(Quaternion.Euler(cameraRot + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f))));
        }
    }

    private void SpawnTerrain()
    {
        spawnPoint = defaultPos;
        rotation = zeroRot;
        newTerrain = Instantiate(terrain, terrainOffset, zeroRot);
        //newTerrain.transform.localScale = defaultScale;
    }

    private Vector3 RandomPosition()
    {
        Vector3 ret = new Vector3(UnityEngine.Random.Range(-4.1f, -3.9f), 0f, UnityEngine.Random.Range(-4.1f, -3.9f));
        return ret;
    }

    private Vector3 RandomScale()
    {
        Vector3 ret = new Vector3(UnityEngine.Random.Range(minScaleValue, maxScaleValue), UnityEngine.Random.Range(minScaleValue, maxScaleValue), UnityEngine.Random.Range(minScaleValue, maxScaleValue));
        return ret;
    }

    private Quaternion RandomRotation()
    {
        Quaternion ret = Quaternion.Euler(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(-30f, 30f));
        return ret;
    }

    /////////////////////////////////////////////////////////////
    //////////////// Annotations generators /////////////////////
    /////////////////////////////////////////////////////////////

    private void changeColor(GameObject go, float min, float max)
    {
        //Color newColor = new Color(0f, UnityEngine.Random.Range(min, max), 0f, 1f);

        Renderer[] children;
        children = go.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in children)
        {
            Color newColor = new Color(UnityEngine.Random.Range(0.4f, 0.8f), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(0f, 0.4f), 1f);
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = rend.materials[j];
                mats[j].SetColor("_Color", newColor);
            }
            rend.materials = mats;
        }
    }

    private void changeMaterial(Material newMat, GameObject go)
    {
        Renderer[] children;
        children = go.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }

    private void saveMasks(type type, field field)
    {
        if (TakeScreenshots)
        { 
            TakeShot(type, field);
        }
    }

    private string ScreenshotName(type name, field field)
    {
        string ret;
        if (name == type.Image)
        {
            ret = string.Format("{0}/Dataset/Field{2}/rgb/{1}.png", Application.persistentDataPath, counter, field);
        }
        else if (name == type.Mask)
        {
            ret = string.Format("{0}/Dataset/Field{2}/nir/{1}.png", Application.persistentDataPath, counter, field);
        }
        else
        {
            ret = string.Format("{0}/Dataset/Field{2}/lbl/{1}.txt", Application.persistentDataPath, counter, field);
        }
        return ret;
    }

    private void TakeShot(type mode, field field) //mode can be type.Image or type.Mask
    {

        RenderTexture rt = new RenderTexture(width, height, 24);
        GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenshotName(mode, field);
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    private string GUIRectWithObject(GameObject go, cls cls) //compute bounding box from camera view
    {
	#if UNITY_EDITOR	
        Renderer[] rr = go.GetComponentsInChildren<Renderer>();
        Bounds b = rr[0].bounds;
        foreach (Renderer r in rr) { b.Encapsulate(r.bounds); }
        Vector3 cen = b.center;
        Vector3 ext = b.extents;

        //Vector3 cen = go.GetComponent<Renderer>().bounds.center;
        //Vector3 ext = go.GetComponent<Renderer>().bounds.extents;

	Vector2[] extentPoints = new Vector2[8]
        {
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        // transform min and manx in strings read to be save
        float x, y, w, h;
        x = Mathf.Clamp(min.x, 0f, width - 1f);
        y = Mathf.Clamp(min.y, 0f, height - 1f);
        w = Mathf.Clamp(max.x, 0f, width - 1f);
        h = Mathf.Clamp(max.y, 0f, height - 1f);

        string species = null;
        if (cls == cls.Crop)
        {
            species = "1";
        }
        else
        {
            species = "0";
        }
        return species + " " + x.ToString() + " " + y.ToString() + " " + w.ToString() + " " + h.ToString();
	#else
	return "none";
	#endif
    }

    private void SaveBoundingBox(string[] content)
    {
        string filename = ScreenshotName(type.Box, field.A);
        File.WriteAllLines(filename, content);
    }

    private void GenerateBoundingBoxes()
    {
        if (SaveBoxes)
        {
            GameObject[] objects = new GameObject[plantNumber + WeedNumber];
            newPlant.CopyTo(objects, 0);
            newWeed.CopyTo(objects, newPlant.Count);

            for (int i = 0; i < objects.Length; i++)
            {
                if (i < plantNumber)
                {
                    if (objects[i] != null)
                    {
                        boxes[i] = GUIRectWithObject(objects[i], cls.Crop);
                    }
                }
                else
                {
                    boxes[i] = GUIRectWithObject(objects[i], cls.Weed);
                }
            }

            SaveBoundingBox(boxes);
        }
    }

    /////////////////////////////////////////////////////////////
    //////////////// Plants generators //////////////////////////
    /////////////////////////////////////////////////////////////

    
    


    public GameObject SpawnGoodPlant(Vector3 position)
    {

        GameObject createdPrefabPlant = Instantiate(goodPlant, position, Quaternion.Euler(0, 0, 0));
        //createdPrefabPlant.AddComponent<MeshRenderer>();
        //Debug.Log(position.ToString());

        return createdPrefabPlant;
    }


    public GameObject SpawnWeedPlant(Vector3 position)
    {
        int plantChosen=Random.Range(0, weedPlants.Length);
        float rot_x = Random.Range(weedPlants_Rotation_Min[plantChosen].x, weedPlants_Rotation_Max[plantChosen].x);
        float rot_y = Random.Range(weedPlants_Rotation_Min[plantChosen].y, weedPlants_Rotation_Max[plantChosen].y);
        float rot_z = Random.Range(weedPlants_Rotation_Min[plantChosen].z, weedPlants_Rotation_Max[plantChosen].z);
        Vector3 weedRotation= new Vector3(rot_x, rot_y, rot_z);
        //GameObject createdPrefabPlant = Instantiate(weedPlants[plantChosen], position+weedPlants_Offset[plantChosen], Quaternion.Euler(weedRotation));
        GameObject createdPrefabPlant = Instantiate(weedPlants[plantChosen], position + weedPlants_Offset[plantChosen], Quaternion.Euler(weedRotation) * weedPlants[plantChosen].transform.rotation);

        createdPrefabPlant.transform.localScale = weedPlants_Scale[plantChosen];
        //Debug.Log(position.ToString());

        return createdPrefabPlant;
    }



    ///////////////////////////////////////////////////////////////////////////////
    /// OVERLAP IDS HANDLING ///////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    private void getOverlapId_basedOnOverallRotation()
    {
	distancesInPOVsIds.Clear();
        for (int i = 0; i < 9; i++)
	{distancesInPOVsIds.Add(-1);}
        
	int closestMappedId = -1;
        
        float current_overallRotationRandomness = 500;
        if(spawned_goodPlantSpawner != null)
        {
            //Vector3 current_overallRotationRandomness = spawned_goodPlantSpawner.GetComponent<PrefabInstatiation>().getCurrentOverallRotation();
            current_overallRotationRandomness = spawned_goodPlantSpawner.GetComponent<PrefabInstatiation>().getCurrentOverallRotation().y;
            //current_overallRotationRandomness.y;
        }
        else
        {
            return;
        }



        // POVs are normally organized for camera positioning like this
        /* 
        789
        456
        123
        */
        // Now remaping to clock wise for easier coding to locate POVs based on rotation, starting from center right
        /*
        567
        4X0
        321
        */
        List<int> remappedNums = new List<int>();
        remappedNums.Add(6);
        remappedNums.Add(3);
        remappedNums.Add(2);
        remappedNums.Add(1);
        remappedNums.Add(4);
        remappedNums.Add(7);
        remappedNums.Add(8);
        remappedNums.Add(9);
 
        float rotationStep = 45.0f;
        float currentLoopAngle = -22.5f;

        if (DEBUG_ALL) {print("going over mapped ids for current_overallRotationRandomness " + current_overallRotationRandomness);}

        for (int i = 0; i < remappedNums.Count; i++) 
        {
            if (DEBUG_ALL)print("iterating over angle " + currentLoopAngle + " in it " + i);
            if(current_overallRotationRandomness >= currentLoopAngle 
                && current_overallRotationRandomness < (currentLoopAngle+rotationStep)
            )
            {
                closestMappedId = i;
                if (DEBUG_ALL) {print("closest Mapped id found " + i + " at angle " + currentLoopAngle);}
            }
            if((currentLoopAngle + rotationStep) > 180)
            {
                currentLoopAngle = -202.5f;
                if(current_overallRotationRandomness >= currentLoopAngle 
                && current_overallRotationRandomness < (currentLoopAngle+rotationStep)
                )
                {
                    closestMappedId = i;
                    if (DEBUG_ALL) {print("closest Mapped id found " + i + " at angle " + currentLoopAngle);}
                }
            }


            currentLoopAngle += rotationStep;
        }
	distancesInPOVsIds[remappedNums[closestMappedId]-1] = 0;
        if (DEBUG_ALL) {print("remapped ids angle loop finished");}

	for(int i = 0; i < 3; i++)
	{
	  int nextId_cw = closestMappedId + (i + 1);
	  int nextId_cc = closestMappedId - (i + 1);
	  
	  if(nextId_cw > remappedNums.Count-1)
	  {
	      nextId_cw -= remappedNums.Count;
	  }
	  if(nextId_cc < 0)
	  {
	      nextId_cc += remappedNums.Count;
	  }

	  distancesInPOVsIds[remappedNums[nextId_cw]-1]=i+1;
	  distancesInPOVsIds[remappedNums[nextId_cc]-1]=i+1;
	}

	
    } 

    private void setOverlapId_basedOnOverallRotation()
    {
        int closestMappedId = -1;
        
        float current_overallRotationRandomness = 500;
        if(spawned_goodPlantSpawner != null)
        {
            //Vector3 current_overallRotationRandomness = spawned_goodPlantSpawner.GetComponent<PrefabInstatiation>().getCurrentOverallRotation();
            current_overallRotationRandomness = spawned_goodPlantSpawner.GetComponent<PrefabInstatiation>().getCurrentOverallRotation().y;
            //current_overallRotationRandomness.y;
        }
        else
        {
            return;
        }

        // POVs are normally organized for camera positioning like this
        /* 
        789
        456
        123
        */
        // Now remaping to clock wise for easier coding to locate POVs based on rotation, starting from center right
        /*
        567
        4X0
        321
        */
        List<int> remappedNums = new List<int>();
        remappedNums.Add(6);
        remappedNums.Add(3);
        remappedNums.Add(2);
        remappedNums.Add(1);
        remappedNums.Add(4);
        remappedNums.Add(7);
        remappedNums.Add(8);
        remappedNums.Add(9);
        
        float rotationStep = 45.0f;
        float currentLoopAngle = -22.5f;

        if (DEBUG_ALL) {print("going over mapped ids for current_overallRotationRandomness " + current_overallRotationRandomness);}

        for (int i = 0; i < remappedNums.Count; i++) 
        {
            if (DEBUG_ALL)print("iterating over angle " + currentLoopAngle + " in it " + i);
            if(current_overallRotationRandomness >= currentLoopAngle 
                && current_overallRotationRandomness < (currentLoopAngle+rotationStep)
            )
            {
                closestMappedId = i;
                //closestId = remappedNums(i+1);
                if (DEBUG_ALL) {print("closest Mapped id found " + i + " at angle " + currentLoopAngle);}
            }
            if((currentLoopAngle + rotationStep) > 180)
            {
                currentLoopAngle = -202.5f;
                if(current_overallRotationRandomness >= currentLoopAngle 
                && current_overallRotationRandomness < (currentLoopAngle+rotationStep)
                )
                {
                    closestMappedId = i;
                    if (DEBUG_ALL) {print("closest Mapped id found " + i + " at angle " + currentLoopAngle);}
                }
            }


            currentLoopAngle += rotationStep;
        }
        if (DEBUG_ALL) {print("remapped ids angle loop finished");}


        if(overlapId_Distance == 0)
        {
            overlapNum_selectedBasedOnDistance = remappedNums[closestMappedId];
        }
        
        
        int Direction = 1 + (-2 *Random.Range(0,2));
 

        if(overlapId_Distance >= 1)
        {
            
            int selectedId = closestMappedId + overlapId_Distance * Direction;
            
            if(selectedId > remappedNums.Count-1)
            {
                selectedId -= remappedNums.Count;
            }
            if(selectedId < 0)

            {
                selectedId += remappedNums.Count;
            }

            overlapNum_selectedBasedOnDistance = remappedNums[selectedId];
            
        }

        if (DEBUG_ALL) {print("selected overlap id " + overlapNum_selectedBasedOnDistance );}
        
    }


    ///////////////////////////////////////////////////////////////////////////////
    /// SINGLE MASK GENERATION TO EXTRACT BOXES ///////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    private void SingleMaskScreenshot(int idx, species cls, field field)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = SingleMaskScreenshotName(idx, cls, field);
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    private string SingleMaskScreenshotName(int idx, species cls, field field)
    {
        return string.Format("{0}/Dataset/Field{4}/Boxes/{1}_{2}_{3}.png", Application.persistentDataPath, counter, idx, cls, field);
    }

    private void saveSingleMasks(field field)
    {
        if (SaveBoxes)
        {

        }
    }
}
