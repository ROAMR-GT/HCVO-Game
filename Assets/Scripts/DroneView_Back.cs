using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//different drone view game mode. Only difference from DroneView.cs is you can see the background play area.
public class DroneView_Back : MonoBehaviour
{
    private Camera drone;
    private GameObject player;
    private GameObject playerPlate;
    private GameObject staticObs;
    private GameObject obs1;
    private GameObject obs2;
    private GameObject obs3;
    private GameObject obs4;
    private GameObject obs5;
    private GameObject obs6;
    private GameObject obs7;
    private GameObject obs8;
    private GameObject obs9;
    private GameObject obs10;
    private GameObject goal;
    // Start is called before the first frame update
    void Start()
    {
        drone = GetComponent<Camera>();
        //player = GameObject.Find("Player");
        //player.layer = 14;
        playerPlate = GameObject.Find("Cylinder");
        playerPlate.layer = 15;
        drone.cullingMask |= 1 << LayerMask.NameToLayer("Drone");
        drone.cullingMask |= 1 << LayerMask.NameToLayer("Drone2");
        drone.cullingMask |= 1 << LayerMask.NameToLayer("Default");
        drone.cullingMask &= ~(1 << LayerMask.NameToLayer("MiniMap"));
        drone.cullingMask &= ~(1 << LayerMask.NameToLayer("Objects"));
        //Camera.main.orthographic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("StaticObstacle") != null)
        {
            staticObs = GameObject.Find("StaticObstacle");
            staticObs.layer = 14;
        }
        if (GameObject.Find("Obstacle 1") != null)
        {
            obs1 = GameObject.Find("Obstacle 1");
            obs1.layer = 14;
        }
        if (GameObject.Find("Obstacle 2") != null)
        {
            obs2 = GameObject.Find("Obstacle 2");
            obs2.layer = 14;
        }
        if (GameObject.Find("Obstacle 3") != null)
        {
            obs3 = GameObject.Find("Obstacle 3");
            obs3.layer = 14;
        }
        if (GameObject.Find("Obstacle 4") != null)
        {
            obs4 = GameObject.Find("Obstacle 4");
            obs4.layer = 14;
        }
        if (GameObject.Find("Obstacle 5") != null)
        {
            obs5 = GameObject.Find("Obstacle 5");
            obs5.layer = 14;
        }
        if (GameObject.Find("Obstacle 6") != null)
        {
            obs6 = GameObject.Find("Obstacle 6");
            obs6.layer = 14;
        }
        if (GameObject.Find("Obstacle 7") != null)
        {
            obs7 = GameObject.Find("Obstacle 7");
            obs7.layer = 14;
        }
        if (GameObject.Find("Obstacle 8") != null)
        {
            obs8 = GameObject.Find("Obstacle 8");
            obs8.layer = 14;
        }
        if (GameObject.Find("Obstacle 9") != null)
        {
            obs9 = GameObject.Find("Obstacle 9");
            obs9.layer = 14;
        }
        if (GameObject.Find("Obstacle 10") != null)
        {
            obs10 = GameObject.Find("Obstacle 10");
            obs10.layer = 14;
        }
        /*if (GameObject.Find("TargetArea") != null)
        {
            goal = GameObject.Find("TargetArea");
            goal.layer = 14;
            foreach (Transform child in goal.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Drone");
            }
        }*/
    }
}
