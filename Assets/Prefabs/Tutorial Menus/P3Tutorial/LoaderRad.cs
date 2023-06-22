using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoaderRad: MonoBehaviour
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


    private int currentTask;

    public UnityEvent timerDone;

    private bool currentlyOverObserved;

    
    // Start is called before the first frame update
    void Start()
    {
        if (timerDone == null)
        {
            timerDone = new UnityEvent();
        }
        currentTutorial.onTaskChanged.AddListener(OnTaskChanged);

    }


    private void Update()
    {
        if (currentTask == waitforTask)
        {
            if (IsCorrectGazeTarget())
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
    bool IsCorrectGazeTarget()
    {
        if (CoreServices.InputSystem.GazeProvider.GazeTarget == observedObject)
        {
            if (!currentlyOverObserved)
            {
                timer = 0;
                currentlyOverObserved = true;
                return false;
            }
            return true;

        }
        else
        {
            if (currentlyOverObserved) {
                currentlyOverObserved = false;
                radialImage.fillAmount = 0;
            }
            return false;

        }
        
    }

    void OnTaskChanged(int i) {
        currentTask = i;
    }

}
