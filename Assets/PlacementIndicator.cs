using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager rayManager;
    [SerializeField]
    private GameObject indicator;


    // Start is called before the first frame update
    void Start()
    {
        //get the components
        rayManager = FindObjectOfType<ARRaycastManager>();
        indicator = transform.GetChild(0).gameObject;
        
        
        // hide the placement indicator visual
        indicator.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Shoot a raycast from the center of the screen
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width/2, Screen.height/2);
        rayManager.Raycast(screenCenter, hits, TrackableType.Planes);

        // if we hit an AR plane surface, update the position and rotation
        if(hits.Count > 0)
        {
            // Move visual along with center screen ray
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
            // rotate to lie flat
            indicator.transform.Rotate(90, 0, 0);

            // enable visual if it's disabeld
            if(!indicator.activeInHierarchy)
            {
                indicator.SetActive(false); // swapped to false
            }
        }
    }
}
