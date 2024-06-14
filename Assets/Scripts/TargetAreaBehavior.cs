using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAreaBehavior : MonoBehaviour
{
    // instance data
    private float radius;
    private float height; //something small
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
    public float inner_rad;
    private float obstacleRadius;
    private float targetRadius;
    private float offset = 0.05f;
    private GameObject staticObs;
    private bool firstChange = false;
    private bool rotate = true;

    private float rotateTimeLeft;
    private float rotateTime = 2.5f;
    private float offset2 = 0.1f;


    private bool isStaticObs;

    // Start is called before the first frame update
    void Start()
    {
        isStaticObs = GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle;
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        height = GameObject.Find("MainMenu").GetComponent<MainMenu>().target_height;
        timeLeft = randomTime;
        //gameObject.transform.position = new Vector3(0.0f, height * 2.0f, 0.0f);
        gameObject.transform.position = new Vector3(0f, 1.125f*height, 0f);
        //gameObject.transform.localScale = new Vector3(radius, height, radius);
        gameObject.layer = 11;
        obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius/2;
        targetRadius = GetComponent<Transform>().localScale.x / 2;

        for (int i = 0; i < 15; i++)
        {
            Physics.IgnoreLayerCollision(i, 11);
        }

        noiseangle = Random.Range(0, 360);
        transform.Rotate(0, noiseangle, 0);
        noiseDir = transform.forward;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isStaticObs) //Case ensures target never overlaps with static obs in center.
        {
            Vector3 velocity = transform.GetComponent<Rigidbody>().velocity;
            //print("Target Velocity: " + velocity);
            staticObs = GameObject.Find("StaticObstacle");
            Vector3 staticPos = new Vector3(staticObs.transform.position.x, 0, staticObs.transform.position.z);

            Vector3 targetPos = new Vector3(transform.position.x, 0, transform.position.z);

            Vector3 relPos = staticPos - targetPos;

            float c_space = obstacleRadius + targetRadius; //rudimentary VO algorithm to determine if there is a collision path between target and obs.
            float cone = Mathf.Asin(c_space / relPos.magnitude);

            float theta = Mathf.Acos(Vector3.Dot(relPos, velocity) / (relPos.magnitude * velocity.magnitude));

            float obsDist = Vector3.Distance(staticPos, targetPos);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            Vector3 curPos = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            float dist = Vector3.Distance(center, curPos); //gets radius
            float Force = gameObject.GetComponent<ForceField>().ForceFunction(dist); //uses radius to determine force Mag 
            timeLeft -= Time.deltaTime; //random timer indicating when to randomize direction
            if (obsDist > targetRadius + obstacleRadius + offset2) //only rotate when target is sufficiently away from obs. Prevents oscillations/dead band.
            {
                rotate = true;
            }
            if (timeLeft <= 0 && obsDist > targetRadius + obstacleRadius + offset2) //randomly rotate and change direction
            {
                transform.Rotate(0, Random.Range(-60f, 60f), 0);//, Space.Self);
                noiseDir = transform.forward;


                timeLeft += randomTime;
            }
            /*rotateTimeLeft -= Time.deltaTime;
            if (rotateTimeLeft <=0)
            {
                if (!rotate)
                {
                    rotate = true;
                }
                rotateTimeLeft += rotateTime;
            }*/

            // Pull variables from Mainmenu
            targetAreaVelocity = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_velocity;
            radius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area / 2.0f;
            maxDistFromCenter = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_travel_radius;

            //gameObject.transform.localScale = new Vector3(radius, height, radius);

            gameObject.GetComponent<Rigidbody>().AddForce(noiseDir * Force); //adds force

            if (dist >= maxDistFromCenter) //reverse direction when reaching max radius.
            {
                firstChange = true;
                float randomAngle = Random.Range(0f, 2 * Mathf.PI);
                Vector3 innerPoint = new Vector3(inner_rad * Mathf.Cos(randomAngle), 1.125f * height, inner_rad * Mathf.Sin(randomAngle));

                //transform.LookAt(center);
                transform.LookAt(innerPoint);

                /*float waitLoop = 1f;

                while (waitLoop >= 0)
                {
                    waitLoop -= Time.deltaTime;
                }*/
            }

            if (firstChange && obsDist <= targetRadius + obstacleRadius + offset && rotate)// && (theta <= cone))//obsDist >= targetRadius + obstacleRadius - offset && rotate)
            {
                transform.Rotate(0, 140f, 0); //switch directions when colliding with static obs. 140 degrees ensures it doesn't oscillate back and forth.
                //transform.Rotate(0, Random.Range(135,165), 0);
                rotate = false;
                //noiseDir = -transform.forward;
            }
            noiseDir = transform.forward;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, targetAreaVelocity);
        } else //same code as above but without static obs 
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            Vector3 curPos = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            float dist = Vector3.Distance(center, curPos);
            float Force = gameObject.GetComponent<ForceField>().ForceFunction(dist);
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                transform.Rotate(0, Random.Range(-45f, 45f), 0);//, Space.Self);
                noiseDir = transform.forward;


                timeLeft += randomTime;
            }

            // Pull variables from Mainmenu
            targetAreaVelocity = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_velocity;
            radius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area / 2.0f;
            maxDistFromCenter = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_travel_radius;

            //gameObject.transform.localScale = new Vector3(radius, height, radius);

            gameObject.GetComponent<Rigidbody>().AddForce(noiseDir * Force);

            if (dist >= maxDistFromCenter)
            {
                float randomAngle = Random.Range(0f, 2 * Mathf.PI);
                Vector3 innerPoint = new Vector3(inner_rad * Mathf.Cos(randomAngle), 1.125f * height, inner_rad * Mathf.Sin(randomAngle));

                //transform.LookAt(center);
                transform.LookAt(innerPoint);

                /*float waitLoop = 1f;
                while (waitLoop >= 0)
                {
                    waitLoop -= Time.deltaTime;
                }*/
            }
            noiseDir = transform.forward;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, targetAreaVelocity);
        }
       

    }
}

