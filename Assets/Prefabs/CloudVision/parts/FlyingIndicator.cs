using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlyingIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject flyinfHighlight;

    [Range(1.0f,10.0f)]
    [SerializeField]
    private float highlightTime= 1.0f;

    public UnityEvent clickEvent;


    // Start is called before the first frame update

    private bool timerStarted = false;
    void Start()
    {
        flyinfHighlight.SetActive(false);
        if (clickEvent == null) {
            clickEvent = new UnityEvent();
        }
    }



    // it seems that this noes not work in debugmode
    public void TriggerClickEvent() {
        if (Debugger.inDebugMode()) { 
        
        }
        
        if (!timerStarted)
        {
            Debug.Log("EventTriggered");
            clickEvent.Invoke();
            timerStarted = true;
            flyinfHighlight.SetActive(true);
            StartCoroutine(endFly());
        }
    }

    IEnumerator endFly() {
        yield return new WaitForSeconds(highlightTime);
        timerStarted = false;
        flyinfHighlight.SetActive(false);
    }


}
