using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Class for settin up the ip-adress in the callibration scene
public class InputField : MonoBehaviour
{

    public TouchScreenKeyboard keyboard;
    public TextMeshProUGUI inputText;
    public static string ipAdress = "";
    bool activated;

    public void OpenSystemKeyboard()
    {
            //keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
        activated = true;
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    { if (activated)
        {
            if (ipAdress != keyboard.text)
            {

                ipAdress = "tcp://"+keyboard.text+":5555";
                inputText.text = keyboard.text;
            }
        }
    }


}
