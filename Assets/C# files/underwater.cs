using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underwater : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] GameObject sphereFx;
    [SerializeField] GameObject blue_panel;
    [SerializeField] Light Light;



    private Color defaultFogColor;
    private FogMode defaultFogMode;
    private float defaultFogDensity;
    private void Start()
    {
        RenderSettings.fogColor = Color.black; // Set your desired underwater fog color
        RenderSettings.fogMode = FogMode.Exponential; // Set your desired fog mode
        RenderSettings.fogDensity = 0.0503f;
       // Light.intensity = 0f;
        // Store the default fog settings
     
    }
    private void Update()
    {
        sphereFx.transform.position=camera.transform.position;
       // Debug.Log("Entered Trigger");

        if (camera.transform.position.y < 13.2f)
        {
            sphereFx.SetActive(true);
            blue_panel.SetActive(true);
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.black; // Set your desired underwater fog color
            RenderSettings.fogMode = FogMode.Exponential; // Set your desired fog mode
            RenderSettings.fogDensity = 0.0503f;
            Light.intensity = 0f;
        }
        else if (camera.transform.position.y > 13.2f)
        {


            Light.intensity = 0.2f;

            sphereFx.SetActive(false);
            blue_panel.SetActive(false);


        }
     
    }
   
 



}
