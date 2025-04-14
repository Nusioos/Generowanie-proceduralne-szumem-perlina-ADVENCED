using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class stawrzacz_kelps_and_stuff : MonoBehaviour
{
    public List<GameObject> cubePosObjects = new List<GameObject>();
  
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float checkInterval = 2f; // Time in seconds between distance checks
    [SerializeField]
    private float distanceThreshold = 50f; // Distance threshold for visibility

    private string saveDirectoryPath;

    private void Start()
    {
        saveDirectoryPath = Application.persistentDataPath + "/faunaData/";

        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }

        // Load saved data
   //     LoadFaunaData();

        // Start checking distances periodically
        StartCoroutine(CheckDistanceRoutine());
        StartCoroutine(Checkplants());
    }

    // Update list only when new objects are generated
    public IEnumerator Checkplants()
    {
        while (true)
        {

            
            GameObject[] allFauna = GameObject.FindGameObjectsWithTag("fauna");

            foreach (GameObject kelp in allFauna)
            {
                if (!cubePosObjects.Contains(kelp))
                {
                    cubePosObjects.Add(kelp);
                }




            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator CheckDistanceRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                CheckAndSetObjectVisibility();
            }
            yield return new WaitForSeconds(checkInterval);
            if(cubePosObjects.Count>3000)
            {
             //   cubePosObjects.Clear();
            }
        }
    }

    private void CheckAndSetObjectVisibility()
    {
        for (int i = cubePosObjects.Count - 1; i >= 0; i--)
        {
            GameObject cubePos = cubePosObjects[i];

            if (cubePos != null)
            {
                float distance = Vector3.Distance(player.position, cubePos.transform.position);

                if (distance > distanceThreshold)
                {
                    cubePos.SetActive(false);

                }
                else
                {
                    cubePos.SetActive(true);
                }
            }
        }
    }

   

 

   
}
