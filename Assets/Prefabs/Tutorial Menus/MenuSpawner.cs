using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawner : MonoBehaviour
{
    private static int windowCount;
    // Start is called before the first frame update
    private Transform spawnOne;
    private Transform spawnTwo;

    [SerializeField]
    private GameObject MenuToSpawn;

    [SerializeField]
    private Transform cameraReference;

    [SerializeField]
    private bool isActiveOnStart = false;


    void Start() {
        if (isActiveOnStart) {
            setCamReference();
            transform.position = spawnOne.position;
        }
    
    }

    public void spawnMenu() {
        if (spawnOne == null || spawnTwo == null)
        {
            setCamReference();
        }

        if (windowCount == 0 && !MenuToSpawn.activeSelf) {
            MenuToSpawn.SetActive(true);
            MenuToSpawn.transform.position = new Vector3(spawnOne.position.x,cameraReference.position.y-0.25f,spawnOne.position.z);
            MenuToSpawn.transform.LookAt(new Vector3(2*transform.position.x-cameraReference.position.x,transform.position.y, 2 * transform.position.z -cameraReference.position.z),Vector3.up);
            windowCount++;
        }
        else if (windowCount == 1 && !MenuToSpawn.activeSelf)
        {
            MenuToSpawn.SetActive(true);
            MenuToSpawn.transform.position = new Vector3(spawnTwo.position.x, cameraReference.position.y-0.25f, spawnTwo.position.z);
            MenuToSpawn.transform.LookAt(new Vector3(2 * transform.position.x-cameraReference.position.x, transform.position.y, 2 * transform.position.z -cameraReference.position.z), Vector3.up);
            windowCount++;
        }
        
    }

    private void setCamReference() {
        if (spawnOne == null || spawnTwo == null)
        {
            spawnOne = cameraReference.Find("FrontSpawn");
            spawnTwo = cameraReference.Find("SideSpawn");
        }
    }

    public void closeMenu() {
        if (MenuToSpawn.activeSelf) {
            MenuToSpawn.SetActive(false);
            windowCount--;
        }
    }
    private void OnDestroy()
    {
        windowCount = 0;
    }



}
