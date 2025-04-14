using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunking : MonoBehaviour
{
    public Vector3 pozycja;
    public Transform player;
    public GameObject[] chunks;
    public GameObject chunk;
    public Transform wall_x;
    //public float distance;
    public generation1 odwo³ywacz;
    public void Update()
    {
        pozycja=player.position;

   //     distance=Distance_chcker(distance,player,wall_x);

    }
    private void Start()
    {
        Vector3 changed= new Vector3(pozycja.x-27.2f,-19,pozycja.z-20.2f);
        Instantiate(chunk,changed,Quaternion.identity);
        
    }
    private float Distance_chcker(float distance,Transform gracz,Transform sciana)
    {
       // float dx = Mathf.Pow(player.position.x - wall_x.position.x,2);
        
     //   distance=Mathf.Sqrt(Mathf.Pow(gracz.position.x - sciana.position.x, 2)+ Mathf.Pow(gracz.position.y - sciana.position.y, 2)+ Mathf.Pow(gracz.position.z - sciana.position.z, 2));
        return distance;
    }
}
