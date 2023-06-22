using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Transforms the positions from the HoloLens coordinate system to the drone ones.
//IMPORTANT: Therfore  the drones 0,0 Coordinate and the forward axis is needed.
//Place this script on an object. The objects position represents the 0,0 position of the drones coordinate system on calibration command
// The forward position needs to be a second object. placed at the right position.
//Example. Place the drone at its origin and move this virtual object to the drones position and place the forward position object 1m in front of the drone.
public class CoordinateTransformer : MonoBehaviour
{
    [SerializeField]
    private Transform forwardPosition;

    //private static Transform droneCoordinates;

    //save betweeen the scenes
    private static Quaternion rot;
    private static Vector3 pos;
    private static Vector3 forwardpos;

    private void Start()
    {
        if (pos != Vector3.zero) {
            transform.SetPositionAndRotation(pos, rot);
            forwardPosition.position = forwardpos;
        }
        
    }

    public void printPosandOrientation() {
        Debug.Log("Position: "+ transform.position);
        Debug.Log("Orientation:" +transform.rotation.eulerAngles);
    }

    public void InitPosition() {
        var positionTo = new Vector3(forwardPosition.position.x, transform.position.y, forwardPosition.position.z);
        transform.LookAt(positionTo, Vector3.up);
        Debug.Log("Callibration Result:" + GetDroneCoordinate(forwardPosition.position));
        //droneCoordinates = transform;
        rot = transform.rotation;
        pos = transform.position;
        forwardpos = forwardPosition.position;
    }

    public Vector3 GetDroneCoordinate(Vector3 input)
    {
        var localPoint = transform.InverseTransformPoint(input);
        var localPointScaled = new Vector3(localPoint.x * transform.localScale.x *(-1.0f) , localPoint.y * transform.localScale.y, localPoint.z * transform.localScale.z);
        return localPointScaled;
    }

    public Vector3 getDroneOrigin() {
        return pos;
    }

    public Vector3 GetRight() {

        return transform.right;
    }


    public Vector3 GetLocalUp()
        // nort local yet
    {
        return Vector3.up;
    }

    public Vector3 GetLocalRight() {
        //not local yet
        return Vector3.right;
    }

}
