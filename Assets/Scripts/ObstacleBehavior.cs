using UnityEngine;
using System;

public class ObstacleBehavior : MonoBehaviour
{
    public float obstacleVelocity;
    public bool hit = false;

    private Vector3 position;
    private Vector3 target;
    public float velocity;
    public Vector3 direction;
    private bool targetSubject;
    private float t = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        position = gameObject.transform.position;
        targetSubject = true;
        velocity = GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().obstacleVelocity;

        if (targetSubject)
        {
            // target subject
            target = GameObject.Find("Player").transform.position;
        }
        else
        {
            target = GameObject.Find("TargetArea").transform.position;
        }
        // direction = goal - start
        direction = target - position;
        direction.y = 0.0f; // no need for y component, only x and z
        direction = direction / direction.magnitude; // make into unit vector

        //ignore collisions between the obstacles & minimapz
        Physics.IgnoreLayerCollision(8, 9);
        Physics.IgnoreLayerCollision(10, 9);
        Physics.IgnoreLayerCollision(8, 10);
        Physics.IgnoreLayerCollision(14, 14);
        Physics.IgnoreLayerCollision(12, 12);
        Physics.IgnoreLayerCollision(10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(direction);
        gameObject.transform.position += direction * velocity * Time.deltaTime;
        Vector3 pos = gameObject.transform.position;
        gameObject.transform.position = new Vector3(pos[0], 1, pos[2]);
        t += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);
        if (t > 0.01f) // this stops the obstacle from instantly being destroyed, since it spawns on top of wall essentially
        {
            if (collision.gameObject.tag == "Player")
            {
                GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hitDetected = true;
                hit = true;
                // sent data to logging
                // Debug.Log("Collision with player");
                Destroy(gameObject);
                GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles = Math.Max(GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles - 1, 0);
                GameObject.Find("Score").GetComponent<Scoring>().numHits = GameObject.Find("Score").GetComponent<Scoring>().numHits + 1;
            }
            else if (collision.gameObject.tag == "Wall")
            {
                // Debug.Log("collision with wall");
                Destroy(gameObject);
                GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles = Math.Max(GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles - 1, 0);
            }
        }
    }
}



/*using UnityEngine;
using System;

public class ObstacleBehavior : MonoBehaviour
{
    public float obstacleVelocity;
    public bool hit = false;

    private Vector3 position;
    private Vector3 target;
    public float velocity;
    public Vector3 direction;
    private bool targetSubject;
    private float t = 0.0f;
    private float obsStartRadius;
    private float obsHeight = 0.5f;
    private float obstacleRadius;
    private bool isCollision = false;
    private Vector3 start1;
    private Vector3 start2;
    private Vector3 start3;
    private Vector3 prevPos;
    private Vector3 curPos;
    private Vector3 curVel;
    private bool activateVel;

    // Start is called before the first frame update
    void Start()
    {
        activateVel = false;
        curPos = transform.position;
        prevPos = curPos;
        start1 = new Vector3(obsStartRadius * Mathf.Cos(0), obsHeight / 2, obsStartRadius * Mathf.Sin(0));
        start2 = new Vector3(obsStartRadius * Mathf.Cos(Mathf.PI / 2), obsHeight / 2, obsStartRadius * Mathf.Sin(Mathf.PI / 2));
        start3 = new Vector3(obsStartRadius * Mathf.Cos(Mathf.PI), obsHeight / 2, obsStartRadius * Mathf.Sin(Mathf.PI));
        obstacleRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        obsStartRadius = GameObject.Find("Walls").GetComponent<WallCreator>().arenaRadius - (float)(0.5 * obstacleRadius);
        position = gameObject.transform.position;
        targetSubject = GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().targetSubject;
        velocity = GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().obstacleVelocity;

        if (targetSubject)
        {
            // target subject
            target = GameObject.Find("Player").transform.position;
        } else
        {
            target = GameObject.Find("TargetArea").transform.position;
        }
        // direction = goal - start
        direction = target - position;
        direction.y = 0.0f; // no need for y component, only x and z
        direction = direction / direction.magnitude; // make into unit vector

        //ignore collisions between the obstacles & minimapz
        Physics.IgnoreLayerCollision(8, 9);
        Physics.IgnoreLayerCollision(10, 9);
        Physics.IgnoreLayerCollision(8, 10);
        Physics.IgnoreLayerCollision(12, 12);
    }

    // Update is called once per frame
    void Update()
    {
        if (activateVel)
        {
            curPos = transform.position;
            curVel = (curPos - prevPos / (Time.deltaTime));
            prevPos = curPos;
            print("Velocity: " + curVel);
            //Debug.Log(direction);
            gameObject.transform.position += direction * velocity * Time.deltaTime;
            Vector3 pos = gameObject.transform.position;
            gameObject.transform.position = new Vector3(pos[0], 1, pos[2]);
        }
        t += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);
        if (t > 0.01f) // this stops the obstacle from instantly being destroyed, since it spawns on top of wall essentially
        {
            if (collision.gameObject.tag == "Player")
            {
                isCollision = true;
                GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hitDetected = true;
                hit = true;
                // sent data to logging
                // Debug.Log("Collision with player");
                if (transform.name == "Obstacle 1")
                {
                    transform.position = start1;
                } else if (transform.name == "Obstacle 2")
                {
                    transform.position = start2;
                } else if (transform.name == "Obstacle 3")
                {
                    transform.position = start3;
                }
                //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                //GetComponent<ObstacleBehavior>().enabled = false;

                //Destroy(gameObject);
                GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles = Math.Max(GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles - 1, 0);
                GameObject.Find("Score").GetComponent<Scoring>().numHits = GameObject.Find("Score").GetComponent<Scoring>().numHits + 1;
            }
            else if (collision.gameObject.tag == "Wall" && curVel.magnitude >= 0.001f)
            {
                isCollision = true;
                if (transform.name == "Obstacle 1")
                {
                    transform.position = new Vector3(obsStartRadius * Mathf.Cos(0), obsHeight / 2, obsStartRadius * Mathf.Sin(0));
                }
                else if (transform.name == "Obstacle 2")
                {
                    transform.position = new Vector3(obsStartRadius * Mathf.Cos(Mathf.PI / 2), obsHeight / 2, obsStartRadius * Mathf.Sin(Mathf.PI / 2));
                }
                else if (transform.name == "Obstacle 3")
                {
                    transform.position = new Vector3(obsStartRadius * Mathf.Cos(Mathf.PI), obsHeight / 2, obsStartRadius * Mathf.Sin(Mathf.PI));
                }
                //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                //GetComponent<ObstacleBehavior>().enabled = false;
                // Debug.Log("collision with wall");
                //Destroy(gameObject);
                GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles = Math.Max(GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles - 1, 0);
            }
            isCollision = false;
        }
        activateVel = false;

    }
    public bool getCollision()
    {
        return isCollision;
    }

    public Vector3 getCurVel()
    {
        return curVel;
    }
    public void setActivateVel(bool value)
    {
        activateVel = value;
    }
}*/
