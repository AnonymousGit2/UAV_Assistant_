using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetList : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;


    private string input;


    public void addToList(string item) {
        input += "- " + item + " \n";
        text.text = input; 
    }

    public void clear() {
        text.text = "\n";
        input = "\n";
    }

    public void OnEnable()
    {
        text.text = input;
    }



}
