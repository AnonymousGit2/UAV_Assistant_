using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoaderRadProxi: MonoBehaviour
{

    [SerializeField]
    GameObject observedObject;

    [SerializeField]
    private Image radialImage;

    [SerializeField]
    float timeToFill;

    [SerializeField]
    private int waitforTask;

    [SerializeField]
    private FlowingTutorial currentTutorial;

    [SerializeField]
    private float detectionProximity;

    private int currentTask;

    public UnityEvent timerDone;

    private bool currentlyOverObserved;

    private Camera camReference;
    // Start is called before the first frame update
    void Start()
    {
        if (timerDone == null)
        {
            timerDone = new UnityEvent();
        }
        currentTutorial.onTaskChanged.AddListener(OnTaskChanged);
        camReference = Camera.main;

    }


    private void Update()
    {
        if (currentTask == waitforTask)
        {
            if (IsNearTarget())
            {
                timer += Time.deltaTime;

                radialImage.fillAmount = (Mathf.Lerp(0f, 1f, timer / timeToFill));

                if (timer >= timeToFill)
                {
                    timerDone.Invoke();
                    timer = 0;
                    radialImage.fillAmount = 0;
                    enabled = false;
                }

            }
        }


    }


    private float timer;

   private bool currentlyNear = false;
    private bool IsNearTarget() {
        if (Vector2.Distance(
                new Vector2(camReference.transform.position.x, camReference.transform.position.z),
                new Vector2(observedObject.transform.position.x, observedObject.transform.position.z))
            < detectionProximity)
        {
            if (!currentlyNear) {
                //we were not near before but now
                timer = 0;
                currentlyNear = true;
                
            }
            return true;
        }
        else {
            if (currentlyNear) {
                // we were near befor but not any more
                currentlyNear = false;
                radialImage.fillAmount = 0;

            }
            return false;
        }  
    }

    void OnTaskChanged(int i) {
        currentTask = i;
    }

}
