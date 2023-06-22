using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ComposedTutorialCommands : MonoBehaviour
{
#if UNITY_EDITOR 
    [ReadOnly] public string[] Commands = new string[] {"Listen: Command","Compound: Next"}; 
 #endif
    public UnityEvent listenEvent;
    public UnityEvent succSpeech;
    public UnityEvent failSpeech;

    [SerializeField]
    [Range(0.1f,10.0f)]
    private float speechTimeout = 4.0f;



    // Start is called before the first frame update
    void Start()
    {
        if (listenEvent == null)
            listenEvent = new UnityEvent();
        if (succSpeech == null)
            succSpeech = new UnityEvent();
        if (failSpeech == null)
            failSpeech = new UnityEvent();
    }


    private float timer;
    private bool isLitening;
    private string seecondCommand = "";
    // Update is called once per frame
    void Update()
    {
        if (isLitening == true)
        {
            if (timer < speechTimeout)
            {

                if (seecondCommand != "")
                {

                    isLitening = false;
                    seecondCommand = "";
                    timer = 0;
                    Debug.Log("Speech Succeded");
                    //Invoke Event 
                    succSpeech.Invoke();
                }

                timer += Time.deltaTime;
            }
            else
            {
                isLitening = false;
                Debug.Log("Speech failed, Try again");
                //Invoke Event 
                failSpeech.Invoke();
            }

            

        }
    }

     
   
    //Start listen command
    public void StartListening() {
        timer = 0;
        isLitening = true;

        //Invoke Event 
        listenEvent.Invoke();


    }

    //So if we are listening, allow the second command.
    public void ListenThisCommand(string s)
    {
        if (isLitening)
        {
            seecondCommand = s;
        }
    }

  


 
}
