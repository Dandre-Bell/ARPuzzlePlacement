using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
// script houses main application logic and functions to be called by buttons
// Podium prefab not appearing
/// </summary>

/*******************************/
// Current Bugs

/******************************/


[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class PlacementLogic : MonoBehaviour
{
    // Adding prefabs in placement script to reduce search times.
    // This is only freasable for a small finite number of objects
    [SerializeField]
    private GameObject trifoldPrefab;
    [SerializeField]
    private GameObject ampPrefab;
    [SerializeField]
    private GameObject treePrefab;
    [SerializeField]
    private GameObject podiumPrefab;
    [SerializeField]
    GameObject indicator;
    [SerializeField]
    private Text debugInfo;  // Infomation for the user on the status of placement, hosting, or resolving
    [SerializeField]
    private TMP_InputField idInput;
    [SerializeField]
    private TMP_InputField suitSelect;

    ARAnchorManager m_AnchorManager;
    ARRaycastManager m_RaycastManager; // This is important. THIS RUNS ALL OF THE RAYCASTING ACROSS ALL SCRIPTS. OBJECTS CANNOT HAVE POSITIONS IN THE WORLD WITHOUT THIS
    // Dictionary to enable object selection via dropdown menu
    Dictionary<string, GameObject> hintObjects = new Dictionary<string, GameObject>(); // Contains the objects to be placed
    Dictionary<string, bool> hintPlaced = new Dictionary<string, bool>(); // Control to prevent double placement
    Dictionary<string, (Vector3, Quaternion)> adjustments = new Dictionary<string, (Vector3, Quaternion)>(); // Positional and Rotational adjustments for each object
    

    public GameObject spawnedObject {get; private set;} // hold the most recently spawned object
    public GameObject resolvedObject { get; private set; } // hold the resolved object
    private ARCloudAnchor _localAnchor;
    private bool hostLock = true;
    private bool resolveLock = true;
    
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
        if(_localAnchor && (!hostLock))
        {
            // Check cloud anchor state
            CloudAnchorState cloudAnchorState = _localAnchor.cloudAnchorState;
            if(cloudAnchorState == CloudAnchorState.Success)
            {
                debugInfo.text = "Success";
                idInput.text = _localAnchor.cloudAnchorId;
                hostLock = true;
                _localAnchor = null;
            }
            else if(cloudAnchorState == CloudAnchorState.TaskInProgress)
            {
                // Wait, not done yet
                debugInfo.text = "Hosting in progress";
            }
            else
            {
                debugInfo.text = "Hosting failed";
            }

        }
        if(_localAnchor && (!resolveLock))
        {
            // Check cloud anchor state
            CloudAnchorState cloudAnchorState = _localAnchor.cloudAnchorState;
            if(cloudAnchorState == CloudAnchorState.Success)
            {
                debugInfo.text = "Success";
                resolvedObject = Instantiate(hintObjects[suitSelect.text], _localAnchor.transform.position, _localAnchor.transform.rotation);
                resolvedObject.transform.SetParent(_localAnchor.transform, false); // 2nd param false makes object keep local orientation rather than global
                resolveLock = true;
                _localAnchor = null;
            }
            else if(cloudAnchorState == CloudAnchorState.TaskInProgress)
            {
                // Wait, not done yet
                debugInfo.text = "Resolving in progress";
            }
            else
            {
                debugInfo.text = "Resolve failed";
            }
        }
        
    }

    public void Host()
    {
        debugInfo.text = "Attemping host";
        _localAnchor = m_AnchorManager.HostCloudAnchor(spawnedObject.GetComponent<ARAnchor>(), 1);
        hostLock = false;
        
    }

        public void Resolve()
    {
        debugInfo.text = "Attempting resolve";
        _localAnchor = m_AnchorManager.ResolveCloudAnchorId(idInput.text);
        resolveLock = false;
        
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
