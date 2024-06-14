using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private float maxRadius;
    private Vector3 center;
    private float radius;
    public float minForce;
    public float maxForce;
    public GameObject targetArea;
    private float force;
    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3(0.0f, 0.0f, 0.0f);
        maxRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_travel_radius;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = targetArea.GetComponent<Transform>().position;
        //radius = Vector3.Distance(center, targetPos);
        //force = ForceFunction(radius);


    }
    public float ForceFunction(float rad) //determines equation for force mag based on radius.
    {
        /*float forceVal = (minForce + maxForce) / 2;
        return forceVal;*/
        float slope = (maxForce - minForce) / maxRadius;
        float forceVal = slope * rad + minForce;
        return forceVal;
    }
}
