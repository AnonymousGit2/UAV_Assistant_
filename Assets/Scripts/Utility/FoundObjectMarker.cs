using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Test manager for the green found object marker
public class FoundObjectMarker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Canvas textArea;
    public void setObjectName(string s) {
        text.text = (s+"\"");
    }

    public void hideText() {
        textArea.enabled = false;
    }



}
