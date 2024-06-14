using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Same code as RingWaypointBehavior.
public class WaypointBehavior : MonoBehaviour
{
    private int[] angleArr = new int[8];
    private float maxDistFromCenter;
    private float uprad;
    private float lowrad;
    public GameObject player;
    private float height;
    public Material auraMat;
    private float timer;
    private float resetTimer;
    private bool preCenter;
    private bool pass;
    private Vector3 center;
    private float radius;
    private float minSpawnRadius;
    private float maxSpawnRadius;
    private float minOnTime;
    private float maxOnTime;
    private Text timerUI;
    private Text waypointUI;
    private Text distanceUI;
    private int angle;
    private float targetSpawnDist;
    private bool changedPos;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;//RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        height = GameObject.Find("MainMenu").GetComponent<MainMenu>().target_height;
        center = new Vector3(0.0f, 1.125f*height, 0.0f);
        distanceUI = GameObject.Find("DistanceText").GetComponent<Text>();
        timerUI = GameObject.Find("TargetTimer").GetComponent<Text>();
        waypointUI = GameObject.Find("WaypointMoved").GetComponent<Text>();
        radius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area;
        maxDistFromCenter = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_travel_radius;
        minSpawnRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Min_Target_Spawn_Radius;
        maxSpawnRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Max_Target_Spawn_Radius;
        minOnTime = GameObject.Find("MainMenu").GetComponent<MainMenu>().Min_On_Target_Time;
        maxOnTime = GameObject.Find("MainMenu").GetComponent<MainMenu>().Max_On_Target_Time;
        //gameObject.transform.localScale = new Vector3(radius, height, radius);
        pass = false;
        preCenter = false;
        resetTimer = Random.Range((int)minOnTime, (int)maxOnTime);
        timer = resetTimer;
        gameObject.transform.position = new Vector3(0.0f, 1.125f*height, 0.0f);
        //gameObject.transform.localScale = new Vector3(radius, height, radius);
        gameObject.layer = 11;


        for (int i = 0; i < 15; i++)
        {
            Physics.IgnoreLayerCollision(i, 11);
        }

        int ndx = 0;
        for(int i = 0; i<360; i += 45)
        {
            angleArr[ndx] = i;
            ndx++;
        }
        uprad = 0;
        lowrad = 0;
    }

    // Update is called once per frame
    void Update()
    {
        changedPos = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        Vector3 playerPos = player.transform.position;
        Vector3 targetPos = transform.position;
        float margin = 0.2f * radius;

        if (distanceUI.color.g == Color.green.g)//if (playerPos.x <= targetPos.x+margin && playerPos.x >= targetPos.x - margin && playerPos.z <= targetPos.z + margin && playerPos.z >= targetPos.z - margin)
        {
            timer -= Time.deltaTime;
            timerUI.text = "Timer: " + timer.ToString("F2");
            //waypointUI.text = "On Waypoint";
            waypointUI.color = Color.green;
            if (timer <= 0)
            {
                pass = true;
                resetTimer = Random.Range((int)minOnTime, (int)maxOnTime);
                timer += resetTimer;
            }
        }
        else
        {
            pass = false;
            timer = resetTimer;
            timerUI.text = "Timer: " + timer.ToString("F2");
            //waypointUI.text = "Find Waypoint";
            waypointUI.color = Color.red;
        }
        if (pass && preCenter)
        {
            pass = false;
            transform.position = center;
            preCenter = false;
        }
        else if (pass)
        {
            pass = false;
            System.Random rnd = new System.Random();
            int angleNdx = rnd.Next(angleArr.Length);
            angle = angleArr[angleNdx];
            uprad = maxSpawnRadius;//Random.Range(0.8f * maxDistFromCenter, maxDistFromCenter);
            //float spawnMargin = Random.Range(0.1f, 0.2f);
            lowrad = minSpawnRadius;//uprad - spawnMargin;
            targetSpawnDist = Random.Range(lowrad, uprad);
            float angleRad = deg2rad(angle);
            Vector3 updateTargetPos = new Vector3(targetSpawnDist * Mathf.Cos(angleRad), 1.125f * height, targetSpawnDist * Mathf.Sin(angleRad));
            transform.position = updateTargetPos;
            preCenter = true;
            changedPos = true;
        }
    }
    public bool isChangedPos()
    {
        return changedPos;
    }

    private float deg2rad(int degrees)
    {
        return (Mathf.PI / 180) * degrees;
    }
    public int getAngle()
    {
        return angle;
    }
    public float getRad()
    {
        return targetSpawnDist;
    }
    public float getTimer()
    {
        return timer;
    }
    public float getSpawnDist()
    {
        return targetSpawnDist;
    }
}
