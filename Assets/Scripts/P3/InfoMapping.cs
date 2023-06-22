using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMapping : MonoBehaviour
{
    public string[] acceptedSpeechCommands;
    public string[] availableGoogleNames;
    public string[] acceptedGoogleNames;

    public string[] getAcceptedNames() { return acceptedGoogleNames; }
}
