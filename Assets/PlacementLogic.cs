using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

/// <summary>
// script houses main application logic and functions to be called by buttons
// Podium prefab not appearing
/// </summary>

/*******************************/
// Current Bugs
// Prefabs not placed with correct rotation. Use Quaterion rotations to adjust depending on object
/******************************/


[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class PlacementLogic : MonoBehaviour
{
    // Adding prefabs in placement script to reduce search times.
    // This is only freasable for a small finite number of objects
    [SerializeField]
    GameObject trifoldPrefab;
    [SerializeField]
    GameObject ampPrefab;
    [SerializeField]
    GameObject treePrefab;
    [SerializeField]
    GameObject podiumPrefab;
    [SerializeField]
    GameObject indicator;
    [SerializeField]
    Text debugInfo;  // Infomation for the user on the status of placement, hosting, or resolving

    ARAnchorManager m_AnchorManager;
    ARRaycastManager m_RaycastManager; // This is so mother fucking important. THIS RUNS ALL OF THE RAYCASTING ACROSS ALL SCRIPTS. OBJECTS CANNOT HAVE POSITIONS IN THE WORLD WITHOUT THIS
    // Dictionary to enable object selection via dropdown menu
    Dictionary<string, GameObject> hintObjects = new Dictionary<string, GameObject>();
    Dictionary<string, bool> hintPlaced = new Dictionary<string, bool>(); // Control to prevent double placement
    Dictionary<string, (Vector3, Quaternion)> adjustments = new Dictionary<string, (Vector3, Quaternion)>(); // Positional and Rotational adjustments for each object
    

    public GameObject spawnedObject {get; private set;} // hold the most recently spawned object
    
    Text objectSelector;
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        // init dictionairies 
        hintObjects.Add("Clubs", trifoldPrefab);
        hintPlaced.Add("Clubs", false);
        adjustments.Add("Clubs", (new Vector3(0,0,0), new Quaternion(-90, 0, 90, 1)));

        hintObjects.Add("Hearts", ampPrefab);
        hintPlaced.Add("Hearts", false);
        adjustments.Add("Hearts", (new Vector3(0,0,0), new Quaternion(0, 90, 0, 1)));

        hintObjects.Add("Spades", treePrefab);
        hintPlaced.Add("Spades", false);
        adjustments.Add("Spades", (new Vector3(0,0,0), new Quaternion(0, 90, 90, 1)));

        hintObjects.Add("Diamonds", podiumPrefab);
        hintPlaced.Add("Diamonds", false);
        adjustments.Add("Diamonds", (new Vector3(0,0,1), new Quaternion(0, 90, 0, 1)));
        


    }

    // Update is called once per frame
    void Update()
    {
            debugInfo.text = "IndX: " + indicator.transform.position.x + " IndY: " + indicator.transform.position.y + "\nObjX: " + spawnedObject.transform.position.x + " ObjY: " + spawnedObject.transform.position.y + " ObjZ: " + spawnedObject.transform.position.z;
    }

    public void PlaceObject(string item)
    {
        
        if(!hintPlaced[item]){
            spawnedObject = Instantiate(hintObjects[item], (indicator.transform.position + adjustments[item].Item1), new Quaternion (indicator.transform.rotation.x + adjustments[item].Item2.x, indicator.transform.rotation.y + adjustments[item].Item2.y, indicator.transform.rotation.z + adjustments[item].Item2.z, 1));
            if(spawnedObject.GetComponent<ARAnchor>() == null)
            {
                spawnedObject.AddComponent<ARAnchor>();
            }
            hintPlaced[item] = true;
        }
        else
        {
            debugInfo.text = "Object moved";
            spawnedObject.GetComponent<ARAnchor>().transform.position = indicator.transform.position;
            spawnedObject.GetComponent<ARAnchor>().transform.rotation = new Quaternion (indicator.transform.rotation.x + adjustments[item].Item2.x, indicator.transform.rotation.y + adjustments[item].Item2.y, indicator.transform.rotation.z + adjustments[item].Item2.z, 1);
        }
    }
}
