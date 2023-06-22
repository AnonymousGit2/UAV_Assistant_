using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MiniTable : MonoBehaviour
{
    [SerializeField]
    private CoordinateTransformer origin;

    [SerializeField]
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        var right = origin.GetRight();
        transform.position = origin.getDroneOrigin() + new Vector3(right.x * offset.x,offset.y, right.z*offset.z); 
    }

}
