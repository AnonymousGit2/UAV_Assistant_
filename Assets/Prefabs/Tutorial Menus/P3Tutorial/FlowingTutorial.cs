using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class MyIntEvent : UnityEvent<int>
{
}


public class FlowingTutorial : MonoBehaviour
{

    [SerializeField]
    private string[] tasks = { "Say 'Tutorial'" };

    [SerializeField]
    private SpeechInputHandler speechComms;


    [SerializeField]
    private GameObject apple;

    public MyIntEvent onTaskChanged;

    private TextMeshProUGUI text;
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        Debugger.debugMode = true;
        speechComms.enabled = false;
        if (onTaskChanged == null)
        {
            onTaskChanged = new MyIntEvent();
        }
        text = GetComponentInChildren<TextMeshProUGUI>();
        counter = 0;
    }


    public void ChangeTask(int taskNumber) {
        if (counter == taskNumber - 1) { 
            counter++;
            onTaskChanged.Invoke(counter);
            text.text = tasks[counter];
        }
    }

    public void DelayNextTask(float seconds) {
        Invoke("NextTask", seconds);
    }

    public void NextTask() {
        counter++;
        onTaskChanged.Invoke(counter);
        text.text = tasks[counter];
  
    }

    private void Reset()
    {
        counter = 0;
        onTaskChanged.Invoke(counter);
        text.text = tasks[counter]; 
    }
    public int GetCurrentTask() {
        return counter;
    }

  

    public void StartRealTask() {
        if (counter == tasks.Length - 1)
        {
            gameObject.SetActive(false);
            Debugger.debugMode = false;
            speechComms.enabled = true;
        }
    }


    public async void ScanApple() {
        if (counter == 4) {
            ChangeTask(5);
            await Task.Delay(2000);
            CloudVisionResultManager.addObject("Apple", apple.transform.position,Camera.main.transform.position,true);
            ChangeTask(6);
            DelayNextTask(4);
        }
        if (counter == 7) {
            ChangeTask(8);
            await Task.Delay(2000);
            CloudVisionResultManager.addObject("Apple", apple.transform.position, Camera.main.transform.position);
            ChangeTask(9);
            DelayNextTask(5);


        }
    }

    public void FlyToApple() {
        if (counter == 10) {
            ChangeTask(11);
            DelayNextTask(7);
            DelayNextTask(14);
        }
    
    }

    


}
