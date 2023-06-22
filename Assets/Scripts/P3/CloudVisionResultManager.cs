using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


//Class for handling the found results by the cloud vision algorithm
public class CloudVisionResultManager : MonoBehaviour
{

    private Dictionary<string, Vector3> foundobjects;
    private Dictionary<string, Vector3> validatedObjects;
    private Dictionary<string, Vector3> invalidatedObjects;
    private Dictionary<string, GameObject> maybeFoundmarker;
    private Dictionary<string, GameObject> foundMarker;

    [SerializeField]
    private GameObject maybeFound;

    [SerializeField]
    private GameObject found;

    private static CloudVisionResultManager thisObject;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxDistance;

    [SerializeField]
    TargetList tList;


    // Start is called before the first frame update
    // Init all dictionaries
    void Start()
    {
        if (thisObject == null) {
            thisObject = this;

            foundobjects = new Dictionary<string, Vector3>();
            validatedObjects = new Dictionary<string, Vector3>();
            invalidatedObjects = new Dictionary<string, Vector3>();
            maybeFoundmarker = new Dictionary<string, GameObject>();
            foundMarker = new Dictionary<string, GameObject>();
            tList.clear();
        }
    }

    //clears all found objects
    public void clearAll() {
        foundobjects = new Dictionary<string, Vector3>();
        validatedObjects = new Dictionary<string, Vector3>();
        invalidatedObjects = new Dictionary<string, Vector3>();

        foreach (GameObject go in maybeFoundmarker.Values) {
            Destroy(go);
        }
        foreach (GameObject go in foundMarker.Values)
        {
            Destroy(go);
        }

        maybeFoundmarker = new Dictionary<string, GameObject>();
        foundMarker = new Dictionary<string, GameObject>();

    }

    //add an object to the dictionary
    public static void addObject(string objectType, Vector3 position) {
        addObject(objectType, position, Vector3.zero);
    }
    //add an object to the dictionary
    public static void addObject(string objectType, Vector3 position, Vector3 cameraPos, bool hideText = false) {
        //if its validated everything is fine
        if (isValidated(objectType)) { return; }


        //if not then process it
        if (!thisObject.foundobjects.ContainsKey(objectType))
        {
            Debug.Log("Object added to maybe found objects: " + objectType);
            thisObject.spawnMaybeFoundMarker(objectType, position,cameraPos,hideText);
            thisObject.foundobjects.Add(objectType, position);

        }
        //the object is already in the ListOfObjects
        else {
            //check if the new position is near the old one
            if (thisObject.isNearFoundObject(objectType, position))
            {
                //If yes then add it to the validated Objects
                // and remove it from other lists;
                Debug.Log("Object added to validated objects: "+objectType);
                thisObject.addToValid(objectType, position);          
                
            }
            else {
                //the object is not near the found object
                if (!thisObject.invalidatedObjects.ContainsKey(objectType))
                {
                    //if it is not in invalidaated Objects add it there
                    thisObject.invalidatedObjects.Add(objectType, position);

                }
                else {
                    //the object is both in found objects and invalid objects but the distance to found objects is too high
                    //check if it is near the invalid object so it is therefore validated
                    if (thisObject.isNearInvalidObject(objectType, position))
                    {
                        // so the invalid object was near enough
                        // therefore the new one is valid now
                        Debug.Log("Object added to validated objects(from invalid)" + objectType);
                        thisObject.addToValid(objectType, position);
                        
                    }
                    else {
                        // the newly found object is too far from both, just discard it 
                        Debug.Log("Object "+objectType +"too far from found objects: Discarded");
                    }

                
                }
                
            }
        }
        
    }

    //Spawns yellow marker after the first scan
    private void spawnMaybeFoundMarker(string objectName, Vector3 position, Vector3 cameraPos, bool hideText = false) {
        var marker = Instantiate(maybeFound);
        marker.transform.parent = null;
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one / 10;
        marker.transform.LookAt(new Vector3(cameraPos.x, marker.transform.position.y, cameraPos.z), Vector3.up);
        marker.GetComponent<FoundObjectMarker>().setObjectName(objectName);
        if (hideText)
        {
            marker.GetComponent<FoundObjectMarker>().hideText();
        }
        maybeFoundmarker.Add(objectName,marker);

    }
    //Spawns greedn marker after the validation scan
    private void spawnValidMarker(string objectName, bool hideText = false)
    {
        //this should always return true
        if (maybeFoundmarker.TryGetValue(objectName, out GameObject value))
        {
            var marker = Instantiate(found);
            marker.transform.parent = null;
            marker.transform.localScale = Vector3.one / 10;
            marker.transform.SetPositionAndRotation(value.transform.position, value.transform.rotation);
            marker.GetComponent<FoundObjectMarker>().setObjectName(objectName);
            foundMarker.Add(objectName,marker);
            maybeFoundmarker.Remove(objectName);
            if (hideText) {
                marker.GetComponent<FoundObjectMarker>().hideText();
            }
            Destroy(value);
        }

    }
    //This function should only be called if an object (+marker) was already in the foundObject list.
    private void addToValid(string objectType, Vector3 position, bool hideText = false) {
        //Event call that an object was added to validated
        AddToList(objectType);
        //call the function to apply voice control
        // voice. add to voice commands....

        //spawn marker, add to valid and remove from other lists;
        spawnValidMarker(objectType,hideText);
        validatedObjects.Add(objectType, position);
        invalidatedObjects.Remove(objectType);
        foundobjects.Remove(objectType);
    }

    public static bool isValidated(string objectType) {
        if (thisObject.validatedObjects.ContainsKey(objectType)) { return true; }
        return false;
    }


    private bool isNearFoundObject(string objectType, Vector3 position) {
        if (foundobjects.TryGetValue(objectType, out Vector3 value)) {
            if (Vector3.Distance(position, value) < maxDistance) {
                return true;
            }
        } 
        return false;
    }
    private bool isNearInvalidObject(string objectType, Vector3 position)
    {
        if (invalidatedObjects.TryGetValue(objectType, out Vector3 value))
        {
            if (Vector3.Distance(position, value) < maxDistance)
            {
                return true;
            }
        }
        return false;
    }

    public static bool getPosition(string objectName, out Vector3 pos) {
        var succ = thisObject.validatedObjects.TryGetValue(objectName, out Vector3 value);
        pos = value;
        return succ;
    }

    private void AddToList(string s) {
        tList.addToList(s);

    }

    //DEBUG
    public static void ForceAdd(string objectType,Vector3 position)
    {
        if (thisObject.validatedObjects == null) {
            thisObject.validatedObjects = new Dictionary<string, Vector3>();
        }
        thisObject.validatedObjects.Remove(objectType);
        thisObject.validatedObjects.Add(objectType, position);
        
    }
    public void ForceAddLocal(string objectType, Vector3 position)
    {
        if (thisObject.validatedObjects == null)
        {
            thisObject.validatedObjects = new Dictionary<string, Vector3>();
        }
        thisObject.validatedObjects.Remove(objectType);
        thisObject.validatedObjects.Add(objectType, position);
        AddToList(objectType);
    }


}
