using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAreaBehavior2 : MonoBehaviour
{
    // instance data
    public float radius;
    public float height = 0.4f; //something small
    public Material auraMat;
    private float targetAreaVelocity;
    public float maxDistFromCenter;
    //public float velocity;

    private Vector3 direction;
    private Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
    private GameObject camera;
    private Vector3 cameraPos;
    public float randomTime;
    private float timeLeft;
    private Vector3 noiseDir;
    private float noiseangle;
    private bool switched = true;
    public float angularSpeed;
    public float maxAccel;
    private float randomAccel;


    // Start is called before the first frame update
    void Start()
    {
        timeLeft = randomTime;
        //camera = GameObject.Find("Main Camera");
        gameObject.transform.position = new Vector3(0.0f, height * 2.0f, 0.0f);
        gameObject.transform.localScale = new Vector3(radius, height, radius);
        gameObject.layer = 11;


        for (int i = 0; i < 15; i++)
        {
            Physics.IgnoreLayerCollision(i, 11);
        }

        //direction = new Vector3(Random.Range(0f, 1.0f), 0, Random.Range(0f, 1.0f));
        //direction /= direction.magnitude;
        noiseangle = Random.Range(0, 2*Mathf.PI);
        randomAccel = Random.Range(0, maxAccel);
        direction = new Vector3(Mathf.Cos(noiseangle), 0, Mathf.Sin(noiseangle));

    }

    // Update is called once per frame
    void FixedUpdate()
    {


        targetAreaVelocity = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_velocity;
        radius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area;
        maxDistFromCenter = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_travel_radius;

        gameObject.transform.localScale = new Vector3(radius, height, radius);
        float dist = Vector3.Distance(center, gameObject.transform.position);
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0 && dist<maxDistFromCenter)
        {
            noiseangle = Random.Range(0, 2 * Mathf.PI);
            direction = new Vector3(Mathf.Cos(noiseangle), 0, Mathf.Sin(noiseangle));
            randomAccel = Random.Range(0, maxAccel);
            timeLeft += randomTime;
        }
        Vector3 deltaV = randomAccel * direction;
        Vector3 newposition = gameObject.GetComponent<Rigidbody>().velocity+deltaV * Time.deltaTime;
        print(newposition);
        transform.position = transform.position + newposition;
        if (dist >= maxDistFromCenter)
        {
            direction = -transform.forward;
        }
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, targetAreaVelocity);
    }
}
