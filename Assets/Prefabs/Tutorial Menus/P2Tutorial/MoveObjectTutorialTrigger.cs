using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveObjectTutorialTrigger : MonoBehaviour
{
    public UnityEvent onObjectMoved;

    private bool alreadyCollided = false;
    private bool activated = false;

    private void Start()
    {
        if (onObjectMoved == null) {
            onObjectMoved = new UnityEvent();
        }
    }

    public void Activate() {
        activated = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
        {
            if (!alreadyCollided)
            {
                onObjectMoved.Invoke();
                alreadyCollided = true;
            }
        }
    }


}
