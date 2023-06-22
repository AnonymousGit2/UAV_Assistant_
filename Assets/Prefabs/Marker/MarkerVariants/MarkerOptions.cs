using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerOptions : MonoBehaviour
{
    [SerializeField]
    private GameObject head;

    [SerializeField]
    private GameObject head2;

    [SerializeField]
    private GameObject sphere;

    private bool locked = false;
    public void SetStartMarker() 
    {
        head.transform.localEulerAngles = Vector3.zero;
        head.transform.localPosition = Vector3.zero;

        head2.SetActive(false);

        sphere.GetComponent<Renderer>().enabled = false;
        GetComponent<ObjectManipulator>().enabled = false ;
        GetComponent<NearInteractionGrabbable>().enabled = false;
        locked = true;
    }

    public void SetPathMarker()
    {
        if (!locked)
        {
            head.transform.localPosition = new Vector3(-0.5f, 0, 0);
            head.transform.localEulerAngles = new Vector3(0, 0, 90);

            head2.SetActive(true);

            sphere.GetComponent<Renderer>().enabled = true;
        }
    }

    public void SetTargetMarker() 
    {
        if (!locked)
        {
            head.transform.localPosition = Vector3.zero;
            head.transform.localEulerAngles = new Vector3(0, 0, 180);

            head2.SetActive(false);

            sphere.GetComponent<Renderer>().enabled = false;
        }
    }


}
