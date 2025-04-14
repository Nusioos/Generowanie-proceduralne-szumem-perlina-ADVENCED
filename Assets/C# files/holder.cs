using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holder : MonoBehaviour
{
public Transform holderGameObject;
    public GameObject camera;
    public Vector3 cahnger= new Vector3(0, 2f, 0);
  public  float i;

    private void Update()
    {
        camera.transform.position= holderGameObject.transform.position+cahnger;

       // Vector3 sigma= new Vector3(0,0,0);

        if (Input.GetKeyDown(KeyCode.W))
        {
            i=0.024f;
          
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            i = 0;
        }

        Vector3 sigma = new Vector3(0, 0, i);

        holderGameObject.position = holderGameObject.position + sigma;
    }
}
