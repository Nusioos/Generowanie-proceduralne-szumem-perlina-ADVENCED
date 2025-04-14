using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class stworz : MonoBehaviour
{
    [SerializeField]
    private Transform room;
    [SerializeField, Range(1, 100)]
    private int resolution;

  
    Vector3 position;
  

    private void Start()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("cubes");
            foreach (GameObject th_cube in gos)
              
            {
                Destroy(th_cube);
            }
            funkcja();

        }
       
    }
  
    public void funkcja()
    {
       
       

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                for (int y = 0; y < resolution; y++)
                {


                    // Calculate Perlin noise value for the current position
                    float probabilty = Random.Range(0.0f, 1.0f);




                 
                    if (probabilty >= 0.5f)
                    {
                    Transform point = Instantiate(room, position, Quaternion.identity);

                        position.x = x;
                        position.z = z;
                        position.y = y;

                        point.SetParent(transform, false);

                    }






                }
            }
        }
    }

    }

 

      

