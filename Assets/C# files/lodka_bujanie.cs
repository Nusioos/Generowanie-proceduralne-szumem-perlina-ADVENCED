using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class lodka_bujanie : MonoBehaviour
{
    public GameObject lodka;


    private void Update()
    {

        float sin = 0.4f*Mathf.Sin( Time.time *3f);
        
        lodka.transform.position = new Vector3( 1000f,13.6f+sin ,1000f );
    }
}
