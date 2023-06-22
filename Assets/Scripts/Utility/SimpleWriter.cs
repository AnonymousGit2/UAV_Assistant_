using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Debug Console to show in AR
public class SimpleWriter : MonoBehaviour
{
    public TextMeshProUGUI textField;

    private static TextMeshProUGUI textf;

    private void Start()
    {
        if (textf == null) {
            textf = textField;
        } else {
            Debug.LogError("Only one Instance fo this Class is allowed");
        }
    }


    public static void WriteConsole(string s) {
        Debug.Log(s);
    }
    public static void WriteToTextfield(string s) {
        if (textf != null) { textf.text = textf.text + "\n" + s; }
    }

    public static void Write(string s) {

        WriteConsole(s);
        WriteToTextfield(s);
    }
}
