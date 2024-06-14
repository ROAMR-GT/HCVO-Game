using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateStaticObstacle : MonoBehaviour
{
    public float obsHeight;
    private float obstacleRadius;
    public GameObject target;
    public GameObject player;
    private float maxMargin = 0f;
    private bool waypointChanged;
    private Vector3 prevPos;
    private bool firstTrial;
    private bool changeTrial = false;
    public Material obsMat;
    private GameObject obs;
    private float offset1;
    private float offset2;
    private bool isTargetRandom;
    private Vector3 playerPos;
    private float targetRadius;
    private float pObsDistance;
    private float tObsDistance;
    private float minpDistance;
    private float mintDistance;
    private bool isFirstFrame = true; //if true, static obs will not spawn in first frame
    //private bool hitDetect;


    // Start is called before the first frame update
    void Start()
    {
        isTargetRandom = GameObject.Find("MainMenu").GetComponent<MainMenu>().Randomized_Target_Behavior;
        //if (!isTargetRandom)
        //{
        target = GameObject.Find("TargetArea");
        firstTrial = true;
        obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        offset1 = GameObject.Find("MainMenu").GetComponent<MainMenu>().Static_obs_marg1;
        offset2 = GameObject.Find("MainMenu").GetComponent<MainMenu>().Static_obs_marg2;
        prevPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        waypointChanged = false;
        obs = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        UnityEngine.Object.Destroy(obs.GetComponent<CapsuleCollider>());
        obs.transform.localScale = new Vector3(obstacleRadius, obsHeight, obstacleRadius);
        //obs.transform.position = new Vector3(prevPos.x + 2, 100, prevPos.z + 2);
        obs.transform.position = new Vector3(100, 100, 100);
        obs.GetComponent<Renderer>().material = obsMat;
        obs.SetActive(false);
        //print("Target radius: " + target.GetComponent<CapsuleCollider>().radius);
        //print("Obstacle radius: " + obstacleRadius);
        //}
        /*else
        {
            obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
            obs = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obs.transform.localScale = new Vector3(obstacleRadius, obsHeight, obstacleRadius);
            obs.transform.position = new Vector3(prevPos.x + 2, 100, prevPos.z + 2);
            obs.GetComponent<Renderer>().material = obsMat;
            print("Creating static obstacle in random");
            obs.transform.position = new Vector3(0, 0, 0);
            obs.SetActive(true);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.Find("TargetArea");
        //Calculate vector between player and target
        //Randomize margin (Random.Range(0, maxMargin)
        //Find orthogonal vector between player and target
        //Nomralize this vector
        //multiply unit vector by radnomized margin
        //Select random point on vector between player and target
        //Add random point vector and orthogonal vector to get obstacle spawn position
        //print(prevPos);
        //print(target.transform.position);
        if (!isTargetRandom)
        {
            playerPos = player.transform.position;
            //print("Previous: " + prevPos);
            //print("Current: " + new Vector3(target.transform.position.x, 0, target.transform.position.z));
            //print("Distance5: " + Vector3.Distance(prevPos, new Vector3(target.transform.position.x, 0, target.transform.position.z)));
            if (Vector3.Distance(prevPos, new Vector3(target.transform.position.x, 0, target.transform.position.z)) <= 0.01f)
            {
                //print("Here5");
                waypointChanged = false;
                //check if player collides with static obs using positions
                if (Vector3.Distance(new Vector3(obs.transform.position.x, 0, obs.transform.position.z), new Vector3(playerPos.x, 0, playerPos.z)) 
                    <= obs.transform.localScale.x/2 + player.transform.localScale.x/2 + 0.01f)
                {
                    GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hitDetected = true;
                }
                else
                {
                    GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hitDetected = false;
                }
            }
            else
            {
                //print("Here6");
                //print("Dist: " + Vector3.Distance(prevPos, target.transform.position));
                //print("Prev: " + prevPos);
                //print("Curr: " + target.transform.position);
                waypointChanged = true;
                prevPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            }
            if (!isFirstFrame && waypointChanged)
            {
                //print("Create obstacle");


                Vector3 targetPos = target.transform.position;
                targetPos.y = 0f;
                Vector3 playerPos = player.transform.position;
                playerPos.y = 0f;
                Vector3 diffVec = targetPos - playerPos;
                int angle = GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().getAngle();
                float rad = GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().getRad();
                float distToTarget = diffVec.magnitude;
                float radAngle = deg2rad(angle);
                int counter = 0; //prevents infinite loop in case there are no possible solutions (i.e target is too big).
                do //will spawn obs approrpiate distance between player and target. Will keep looping until it finds valid position.
                {
                    float midPoint = distToTarget / 2;
                    float minDist = distToTarget - player.transform.localScale.x / 2 - target.transform.localScale.x / 2;
                    float offset = minDist / 2 - 0.02f; //0.02 is buffer
                    float randRadius = midPoint + Random.Range(-offset, offset);
                    obs.transform.position = new Vector3(randRadius * Mathf.Cos(radAngle), 1.125f * obsHeight, randRadius * Mathf.Sin(radAngle));
                    //float marg1 = Random.Range(rad - offset1 * rad, rad + offset1 * rad);
                    //print("Marg1: " + marg1);
                    //float marg1 = Random.Range(distToTarget - offset1 * distToTarget, distToTarget + offset1 * distToTarget);
                    //marg1 /= 2.0f;
                    //float marg2 = Random.Range(marg1 - offset2 * marg1, marg1 + offset2 * marg1);
                    //float marg1 = rad - offset1 * rad;
                    //float marg2 = rad * offset2;
                    //obs.transform.position = new Vector3(marg2 * Mathf.Cos(radAngle), 0f, marg2 * Mathf.Sin(radAngle));
                    //pObsDistance = Vector3.Distance(player.transform.position, obs.transform.position);
                    //tObsDistance = Vector3.Distance(target.transform.position, obs.transform.position);
                    pObsDistance = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), 
                        new Vector3(obs.transform.position.x, 0, obs.transform.position.z));
                    tObsDistance = Vector3.Distance(new Vector3(target.transform.position.x, 0, target.transform.position.z),
                        new Vector3(obs.transform.position.x, 0, obs.transform.position.z));
                    minpDistance = 0.4f/2 + obstacleRadius/2;
                    mintDistance = obstacleRadius/2 + target.GetComponent<Transform>().localScale.x/2;
                    counter++;
                } while (counter <= 20000 && (tObsDistance <= mintDistance) || (pObsDistance <= minpDistance+0.02f));
                print("Counter: " + counter);
                //print("Min Distance: " + mintDistance + "Actual Distance: " + tObsDistance);
                counter = 0;
                //obs.transform.position = new Vector3(Random.Range(marg2, marg1) * Mathf.Cos(radAngle), 0f, Random.Range(marg2, marg1) * Mathf.Sin(radAngle));
                obs.SetActive(true);
                obs.name = "StaticObstacle";
                //print(obstacleRadius + 0.4f);
                //print(Vector3.Distance(obs.transform.position, playerPos));

                /*float margin = Random.Range(-maxMargin, maxMargin);
                Vector3 perpVec = Vector3.Cross(diffVec, Vector3.up).normalized;
                perpVec = margin * perpVec;
                perpVec.y = 0;
                float slope = (targetPos.z - playerPos.z) / (targetPos.x - playerPos.x);
                float b = targetPos.z - slope * targetPos.x;
                float randomX = 0;
                do
                {
                    if (targetPos.x <= playerPos.x)
                    {
                        randomX = Random.Range(targetPos.x, playerPos.x);
                    }
                    else if (targetPos.x > playerPos.x)
                    {
                        randomX = Random.Range(playerPos.x, targetPos.x);
                    }
                    float randomZ = slope * randomX + b;
                    Vector3 rndPoint = new Vector3(randomX, 0, randomZ);
                    Vector3 spawnPos = rndPoint + perpVec;
                    obs.transform.position = spawnPos;
                    obs.SetActive(false);
                } while (Vector3.Distance(obs.transform.position, targetPos) < 0.4f && Vector3.Distance(obs.transform.position, playerPos) < 0.4f);
                obs.SetActive(true);*/

            }
        } else //spawns static obstacle at center of play area for randomized target behavior.
        {
            obs.transform.position = new Vector3(0, 1.125f * obsHeight, 0);
            obs.SetActive(true);
            playerPos = player.transform.position;
            obs.name = "StaticObstacle";
            if (Vector3.Distance(new Vector3(obs.transform.position.x, 0, obs.transform.position.z), new Vector3(playerPos.x, 0, playerPos.z)) <= obs.transform.localScale.x + 0.01f)
            {
                GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hitDetected = true;
            }
            else
            {
                GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hitDetected = false;
            }
        }
        isFirstFrame = false;
    }
    private float deg2rad(int degrees)
    {
        return (Mathf.PI / 180) * degrees;
    }
}
