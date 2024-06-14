using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float turnSpeed = 4.0f;
    public float moveSpeed = 0.0001f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;
    private float rotY;
    private Camera playerCamera;
    private float lockPos = 0;
    private Vector3 vo_vel;
    private float vo_mag;
    private float vo_ang;
    private GameObject agent;
    private Rigidbody agent_rb;

    public bool Joystick_mode;
    public bool VIVE_mode;
    public bool VO_mode;
    public bool Tracker_mode;
    private bool bool_mouse;

    private Vector3 userPos;
    private Quaternion userRot;
    private Vector3 angles;
    private float newx;
    private float newz;
    private float dist;

    // Start is called before the first frame update
    void Start()
    {
        rb.useGravity = true;
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        bool_mouse = true;
    }

    // Update is called once per frame
    void Update()
    {
        // pull variables
        Joystick_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().Joystick_Control;
        VIVE_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().VIVE_HMD_Control;
        Tracker_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().VIVE_Tracker_Control;
        VO_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().VO_Control;
        dist = GameObject.Find("MainMenu").GetComponent<MainMenu>().Chest_Distance;//float.Parse

        transform.rotation = Quaternion.Euler(lockPos, playerCamera.transform.rotation.eulerAngles.y, lockPos);
        if (Joystick_mode) {Joystick_Update();}
        else if (VO_mode) {VO_Update();}
        else if (VIVE_mode) {HMD_Update();}
        else if (Tracker_mode){Tracker_Update();}
        else {transform.position = new Vector3(0,0,0);}

        // if (bool_mouse == true) {playerCamera.GetComponent<MouseCamera>().enabled = true;}
        // else {playerCamera.GetComponent<MouseCamera>().enabled  = false;}
    }


    void Tracker_Update()
    {
        bool_mouse = false;
        userPos = GameObject.Find("Tracker").transform.position;
        userRot = GameObject.Find("Tracker").transform.rotation;
        angles = userRot.eulerAngles;
        newx = -dist*Mathf.Sin(angles[1]*Mathf.PI/180);
        newz = -dist*Mathf.Cos(angles[1]*Mathf.PI/180);
        transform.position = userPos + new Vector3(newx,0-1,newz);
    }


    void HMD_Update()
    {
        bool_mouse = false;
        Vector3 userPos = playerCamera.transform.position;
        transform.position = userPos + new Vector3(0, -0.55f, 0);
    }


    void Joystick_Update()
    {
        bool_mouse = true;
        rotY = Input.GetAxis("Mouse X");
        rotX = Input.GetAxis("Mouse Y");
        // rotate player1
        this.transform.Rotate(0, rotY * turnSpeed, 0);
        //Rotate Camera
        playerCamera.transform.position = transform.position + new Vector3(0,1,0);

        Vector3 dir = new Vector3(0, 0, 0);

        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        transform.Translate(0.0001f* dir * moveSpeed * Time.deltaTime);
    }


    void VO_Update()
    {
        // get velocity from VO and apply it to the person
        bool_mouse = false;
        vo_ang = GameObject.Find("Perceptual Cues").GetComponent<VO>().output_angle_VO;
        float max_vel = GameObject.Find("MainMenu").GetComponent<MainMenu>().playerMaxVel;
        vo_mag = GameObject.Find("DistanceText").GetComponent<DistanceTextBehavior>().scaling/0.2f*max_vel;
        Quaternion rotation = Quaternion.AngleAxis(vo_ang,Vector3.forward);
        vo_vel = rotation * new Vector3(vo_mag,0.0f,0.0f);
        // Vector3 best_vel = GameObject.Find("Perceptual Cues").GetComponent<VO>().best_vel;
        // vo_vel = best_vel;
        float x_l = vo_vel[0];
        float z_l = vo_vel[1];
        float y_l = vo_vel[2];
        Vector3 vo_vel_corr = new Vector3(x_l,y_l ,z_l); //best_vel;//new Vector3(x_l,y_l ,z_l);
        agent = GameObject.Find("Player");
        agent_rb = agent.GetComponent<Rigidbody>();
        agent_rb.velocity = vo_vel_corr;
    }


    /*
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Collision with obstacles");
            Destroy(collision.gameObject);
            GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().numObstacles -= 1;
        }
    }
    */
}
