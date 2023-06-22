using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScanningText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI textField;

    private bool isScanning;
    private bool isShowingMessage;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame

    float timer = 0;
    void Update()
    {
        if (isScanning) {
            timer += Time.deltaTime;
            if (timer > 0.5f) {
                textField.text += ".";
                timer = 0;
            }
        } else
        if (isShowingMessage)
        {
            timer += Time.deltaTime;
            if (timer > 4f)
            {
                gameObject.SetActive(false);
                isScanning = false;
                isShowingMessage = false;
            }
        }


    }


    public void ScanMode() {
        if (!isScanning && !isShowingMessage)
        {
            isScanning = true;
            gameObject.SetActive(true);
            textField.text = "Scanning";
        }
    }

    public void ScanSucceded() {
        timer = 0;
        isScanning = false;
        textField.text = "Success: Found Object";
        isShowingMessage = true;
    }

    public void ScanFailed() {
        timer = 0;
        isScanning = false;
        textField.text = "No object found, try again";
        isShowingMessage = true;
    }


}
