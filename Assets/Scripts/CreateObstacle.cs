using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateObstacle : MonoBehaviour
{
    public float obsHeight = 0.2f;
    public int maxObstacles = 3;
    public Material obsMat;
    public float obstacleVelocity;
    public float obstacleRadius;
    public int num_of_threats;

    //[HideInInspector]
    private float t_delaythrow;
    public int numObstacles = 0;
    private float obsStartRadius;
    private float delay_lower_limit;
    private float delay_upper_limit;
    private bool active_bool;
    public float t;
    public float t_countdown;
    private float deltat;
    public bool Automatic_Trials;
    bool activate_threats;
    private float overlapVal;
    private bool repeat;



    // Start is called before the first frame update
    private void Start()
    {
        obsStartRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().arenaR - (float)(0.5 * obstacleRadius);
        delay_upper_limit = GameObject.Find("MainMenu").GetComponent<MainMenu>().delay_upper_limit;
        delay_lower_limit = GameObject.Find("MainMenu").GetComponent<MainMenu>().delay_lower_limit;
        t_delaythrow = Random.Range(delay_upper_limit, delay_lower_limit);
        overlapVal = GameObject.Find("MainMenu").GetComponent<MainMenu>().overlapAngle; //menu item in degrees
        overlapVal = Deg2Rad(overlapVal);
        active_bool = false;
        t = 0;
        t_countdown = 0;


    }

    // Update is called once per frame
    void Update()
    {
        deltat = Time.deltaTime;
        num_of_threats = GameObject.Find("MainMenu").GetComponent<MainMenu>().NumberOfThreats;
        obstacleVelocity = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_velocity;
        obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        Automatic_Trials = GameObject.Find("MainMenu").GetComponent<MainMenu>().Automatic_Trials;
        obsHeight = 0.5f;
        if (numObstacles == 3) { return; }

        if (Automatic_Trials == false)
        {
            if (Input.GetKeyDown("space") || active_bool == true)
            {
                //print("hello");
                active_bool = true;
                t = t + deltat;
                t_countdown = t_delaythrow - t;
                if (t > t_delaythrow)
                {
                    //makeObstacle(num_of_threats);
                    active_bool = false;
                    t = 0;
                    t_countdown = 0;
                    t_delaythrow = Random.Range(delay_upper_limit, delay_lower_limit);
                }
            }
        }

        activate_threats = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().activate_threats;
        //if (activate_threats == true) { makeObstacle(num_of_threats); }

        // if (Input.GetKeyDown("1")) {makeObstacle();}
        // if (Input.GetKeyDown("2")) {makeObstacle(); makeObstacle();}
        // if (Input.GetKeyDown("3")) {makeObstacle(); makeObstacle(); makeObstacle();}
    }

    public void makeObstacle(int threats, int counter)
    {
        //int c = counter;
        float[] thetaArr = new float[threats];
        //GameObject[] obstacleArr = new GameObject[threats];
        string[] obstacleArr = new string[threats];
        GameObject[] obsArrgb = new GameObject[threats];
        do
        {
            //counter = 1;
            for (int i = 0; i < threats; i++)
            {
                counter = 1;
                //print(threats);
                // create obstacle
                numObstacles += 1;
                // GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                obsArrgb[i] = obs;
                //obstacleArr[i] = obs;
                Rigidbody obsRB = obs.AddComponent<Rigidbody>(); // Add the rigidbody.
                obsRB.useGravity = false;
                obsRB.freezeRotation = true;
                // obsRB.constraints = RigidbodyConstraints.FreezePositionY;

                // set transform and rotation
                float theta = UnityEngine.Random.Range(0.0f, Mathf.PI * 2.0f);
                thetaArr[i] = theta;
                float x = obsStartRadius * Mathf.Cos(theta);
                float z = obsStartRadius * Mathf.Sin(theta);
                Vector3 pos = new Vector3(x, obsHeight / 2.0f, z);
                obs.transform.position = pos;
                obs.transform.Rotate(0, Rad2Deg(-1.0f * theta), 0);

                // scale obstacle
                obs.transform.localScale = new Vector3(obstacleRadius, obsHeight, obstacleRadius);

                obs.GetComponent<Renderer>().material = obsMat;
                if (obstacleArr[i]!=null)
                {
                    // print("x: " + obstacleArr[i]);
                    obs.name = obstacleArr[i];
                    //print("obsname: " + obs.name);
                }
                else
                {
                    // print("Here");
                    while (GameObject.Find("Obstacle " + counter) != null)
                    {
                        // print("Here2");
                        // print("C: Pos: Counter: " + counter + " " + GameObject.Find("Obstacle " + counter).name);
                        //print("C: " + counter);
                        //obs.name = "Obstacle " + counter.ToString();
                        // print("obsname: " + obs.name);
                        //obstacleArr[i] = obs.name;
                        counter++;

                    }
                    obs.name = "Obstacle " + counter.ToString();
                    obstacleArr[i] = obs.name;
                }

                //print("C: " + counter);
                //obs.name = "Obstacle " + counter.ToString();//string.Format("{0:0}", numObstacles);
                //obstacleArr[i] = obs.name;
                obs.tag = "Obstacle";
                obs.transform.SetParent(GameObject.Find("/ObstacleFactory").transform, true);
                obs.AddComponent<ObstacleBehavior>();
                //counter++;

                // for minimap tracking
                // GameObject obs_m = GameObject.CreatePrimitive(PrimitiveType.Sphere); ////
                // obs_m.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                // obs_m.transform.position = pos;
                // obs_m.transform.Rotate(0, Rad2Deg(-1.0f*theta), 0);
                // obs_m.name = "Obstacle_M " + string.Format("{0:0}", numObstacles);
                // obs_m.tag = "Obstacle_M";
                // obs_m.transform.SetParent(GameObject.Find("/ObstacleFactory/"+ obs.name).transform, true);
                // obs_m.layer = 12;
                // obs_m.GetComponent<Renderer>().material = obsMat;
                obs.layer = 10;
                //if (numObstacles == 1) { obs.layer = 8; }
                //else if (numObstacles == 2) { obs.layer = 9; }
                //else { obs.layer = 10; }
            } // end for loop
            Array.Sort(thetaArr);
            float diff;
            bool overlap = false;
            for (int ndx = 0; ndx < threats - 1; ndx++)
            {
                diff = Math.Abs(thetaArr[ndx + 1] - thetaArr[ndx]);
                if (diff <= overlapVal)
                {
                    overlap = true;
                    numObstacles = 0;
                    foreach (GameObject cyl in obsArrgb)
                    {
                        Destroy(cyl);
                    }
                    //counter = 1;
                    break;
                }

            }
            repeat = overlap;

        } while (repeat);
        obstacleArr = new string[threats];

    }// end make obsticle

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }
    float Deg2Rad(float deg)
    {
        return deg * (Mathf.PI / 180);
    }
}

/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class CreateObstacle : MonoBehaviour
{
    public float obsHeight = 0.2f;
    public int maxObstacles = 3;
    public Material obsMat;
    public float obstacleVelocity;
    public bool targetSubject = true;
    public float obstacleRadius;
    public int num_of_threats;

    //[HideInInspector]
    private float t_delaythrow;
    public int numObstacles = 0;
    private float obsStartRadius;
    private float delay_lower_limit;
    private float delay_upper_limit;
    private bool active_bool;
    public float t;
    public float t_countdown;
    private float deltat;
    public bool Automatic_Trials;
    bool activate_threats;
    private float overlapVal;
    private bool repeat;
    private GameObject[] createObsarr = new GameObject[3];



    // Start is called before the first frame update
    private void Start()
    {
        obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        obsHeight = 0.5f;
        obsStartRadius = GameObject.Find("Walls").GetComponent<WallCreator>().arenaRadius - (float)(0.5*obstacleRadius);
        delay_upper_limit = GameObject.Find("MainMenu").GetComponent<MainMenu>().delay_upper_limit;
        delay_lower_limit = GameObject.Find("MainMenu").GetComponent<MainMenu>().delay_lower_limit;
        t_delaythrow = Random.Range(delay_upper_limit, delay_lower_limit);
        overlapVal = GameObject.Find("MainMenu").GetComponent<MainMenu>().overlapAngle; //menu item in degrees
        overlapVal = Deg2Rad(overlapVal);
        active_bool = false;
        t = 0;
        t_countdown = 0;
        float spawnTheta = 0f;
        for (int i = 0; i < maxObstacles; i++)
        {
            //print("Creating dynamic obstacle");
            GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Rigidbody obsRB = obs.AddComponent<Rigidbody>(); // Add the rigidbody.
            obsRB.useGravity = false;
            obsRB.freezeRotation = true;
            obs.transform.localScale = new Vector3(obstacleRadius, obsHeight, obstacleRadius);
            print("Localscale: " + obs.transform.localScale);
            obs.GetComponent<Renderer>().material = obsMat;

            obs.name = "Obstacle " + string.Format("{0:0}", (i+1));
            obs.tag = "Obstacle";
            obs.transform.SetParent(GameObject.Find("/ObstacleFactory").transform, true);
            obs.AddComponent<ObstacleBehavior>();
            obs.GetComponent<ObstacleBehavior>().setActivateVel(false);
            //obs.GetComponent<ObstacleBehavior>().enabled = false;
            obsRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

            //obsRB.constraints = RigidbodyConstraints.FreezePositionY;//RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            obs.transform.position = new Vector3(obsStartRadius * Mathf.Cos(spawnTheta), obsHeight / 2, obsStartRadius * Mathf.Sin(spawnTheta));
            createObsarr[i] = obs;
            obs.SetActive(false);
            spawnTheta += Mathf.PI / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {

        deltat = Time.deltaTime;
        num_of_threats = GameObject.Find("MainMenu").GetComponent<MainMenu>().NumberOfThreats;
        obstacleVelocity = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_velocity;
        //obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        Automatic_Trials = GameObject.Find("MainMenu").GetComponent<MainMenu>().Automatic_Trials;
        //obsHeight = 0.5f;
        //if (numObstacles == 3) {return;}

        if (Automatic_Trials == false) {
            if (Input.GetKeyDown("space") || active_bool == true)
            {
                active_bool = true;
                t = t + deltat;
                t_countdown = t_delaythrow - t;
                if (t > t_delaythrow)
                {
                    makeObstacle(num_of_threats);
                    active_bool = false;
                    t = 0;
                    t_countdown = 0;
                    t_delaythrow = Random.Range(delay_upper_limit, delay_lower_limit);
                }
            }
        }

        activate_threats = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().activate_threats;
        if (activate_threats == true) {makeObstacle(num_of_threats);}

        // if (Input.GetKeyDown("1")) {makeObstacle();}
        // if (Input.GetKeyDown("2")) {makeObstacle(); makeObstacle();}
        // if (Input.GetKeyDown("3")) {makeObstacle(); makeObstacle(); makeObstacle();}
    }

    private void makeObstacle(int threats)
    {
        float[] thetaArr = new float[threats];
        GameObject[] obstacleArr = new GameObject[threats];
        do{
            for (int i = 0; i < threats; i++) {
                String obsName = "Obstacle " + (i + 1);
                GameObject obs = GameObject.Find(obsName);
                //print(threats);
                // create obstacle
                numObstacles += 1;
                // GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                //obstacleArr[i] = obs;
                obstacleArr[i] = obs;
                //Rigidbody obsRB = obs.AddComponent<Rigidbody>(); // Add the rigidbody.
                //obsRB.useGravity = false;
                //obsRB.freezeRotation = true;
                // obsRB.constraints = RigidbodyConstraints.FreezePositionY;

                // set transform and rotation
                float theta = UnityEngine.Random.Range(0.0f, Mathf.PI * 2.0f);
                thetaArr[i] = theta;
                float x = obsStartRadius * Mathf.Cos(theta);
                float z = obsStartRadius * Mathf.Sin(theta);
                Vector3 pos = new Vector3(x, obsHeight / 2.0f, z);
                obs.transform.position = pos;
                obs.transform.Rotate(0, Rad2Deg(-1.0f * theta), 0);

                // scale obstacle
                //obs.transform.localScale = new Vector3(obstacleRadius, obsHeight, obstacleRadius);

                //obs.GetComponent<Renderer>().material = obsMat;

                //obs.name = "Obstacle " + string.Format("{0:0}", numObstacles);
                //obs.tag = "Obstacle";
                //obs.transform.SetParent(GameObject.Find("/ObstacleFactory").transform, true);
                //obs.AddComponent<ObstacleBehavior>();

                // for minimap tracking
                // GameObject obs_m = GameObject.CreatePrimitive(PrimitiveType.Sphere); ////
                // obs_m.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                // obs_m.transform.position = pos;
                // obs_m.transform.Rotate(0, Rad2Deg(-1.0f*theta), 0);
                // obs_m.name = "Obstacle_M " + string.Format("{0:0}", numObstacles);
                // obs_m.tag = "Obstacle_M";
                // obs_m.transform.SetParent(GameObject.Find("/ObstacleFactory/"+ obs.name).transform, true);
                // obs_m.layer = 12;
                // obs_m.GetComponent<Renderer>().material = obsMat;

                if (numObstacles == 1) { obs.layer = 8; }
                else if (numObstacles == 2) { obs.layer = 9; }
                else { obs.layer = 10; }
            } // end for loop
            Array.Sort(thetaArr);
            float diff;
            bool overlap = false;
            for (int ndx = 0; ndx < threats-1; ndx++)
            {
                diff = Math.Abs(thetaArr[ndx + 1] - thetaArr[ndx]);
                if (diff <= overlapVal)
                {
                    overlap = true;
                    numObstacles = 0;
                    //foreach (GameObject cyl in obstacleArr)
                    //{
                    //    Destroy(cyl);
                    //}
                    break;
                }

            }
            repeat = overlap;

        } while (repeat) ;
        for (int i = 0; i < obstacleArr.Length; i++)
        {
            createObsarr[i].SetActive(true);
            obstacleArr[i].GetComponent<Rigidbody>().constraints &= (~RigidbodyConstraints.FreezePositionX | ~RigidbodyConstraints.FreezePositionZ | ~RigidbodyConstraints.FreezeRotationX | ~RigidbodyConstraints.FreezePositionZ);
            obstacleArr[i].GetComponent<ObstacleBehavior>().setActivateVel(true);

        }
    }// end make obsticle

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }
    float Deg2Rad(float deg)
    {
        return deg * (Mathf.PI / 180);
    }

}*/
