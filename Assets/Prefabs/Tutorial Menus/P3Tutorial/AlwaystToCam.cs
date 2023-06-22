using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaystToCam : MonoBehaviour
{
    Camera main;
    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(main.transform.position, Vector3.up);
    }
}
