using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
//using PlayerSettings;

public class MainMenu : MonoBehaviour
{
    [Header("                  ROAMR VR Simulaton V3 ")]
    public string User_ID;
    [Header("      ---------- Conditions ----------")]
    //public bool useSceneView;
    //4 game modes for experiment 4. Ignore Tactile modes.
    public int nObstaclesThrown;
    public bool VO_View;
    //public bool VO_Intensity_View;
    [HideInInspector]   public bool VO_Drone;
    public bool Drone_View;
    [HideInInspector]   public bool Tactile_AI;
    [HideInInspector]   public bool Tactile_Goal;
    [HideInInspector]   public bool Tactile_Test;

    public bool Automatic_Trials;
    public bool Testing;

    [Header("      ---------- Monitors ----------")]
    public float trial_time;
    [HideInInspector]   public int TrialNum;
    [HideInInspector]   public float countdown;
    public float failure_rate;
    [HideInInspector]   public float score; // CHANGE TO HEALTH IF YOU WANT TO KEEP
    public float failed_escapes;
    public float t_countdown;
    [HideInInspector]   public string v_mode;
    [HideInInspector]   public string t_mode;

    [Header("      ---------- Target Mode ----------")]
    public bool Randomized_Target_Behavior;
    public bool Waypoint_Behavior;
    public bool Add_Static_Obstacle;

    public float excellentCutoff = 0.05f;
    public float good_cutoff = 0.2f;
    public float ok_cutoff = 0.4f;
    //public float poor_cutoff;

    [Header("      ---------- Player Control ----------")]
    public bool Joystick_Control;
    public bool VIVE_HMD_Control;
    public bool VIVE_Tracker_Control;
    public bool VO_Control;
    [HideInInspector]   public bool Biomechaical_Bias;
    [HideInInspector]   public bool PlayerVel_Bias; // ??
    [HideInInspector]   public float Chest_Distance;





    [Header("      ---------- Assistance Control ----------")]
    public float obj_max_diff = 120;
    public float playerMaxVel = 3;
    public bool continuous_state_update = false;
    public bool arrow_off = false;

    public int min_safety_arc_upper = 66;
    public int min_safety_arc_lower = 18;
    public int min_safety_arc_current;
    public float arrow_cutoff; // ?

    [HideInInspector]   public int AI_LowPass_buffer_size;
    public int Player_LowPass_buffer_size = 10;
    public float PlayerVel_Preference; // look through all scripts

    [Header("      ---------- Threat Control ----------")]

    public float Obstacle_velocity = 7;

    [HideInInspector]   public int NumberOfThreats = 1;
    public float obstacle_rate = 0.25f;
    public float obstacle_randomness = 0.2f; //value between 0 and 0.5
    public float Obstacle_radius = 0.2f; //If the Obstacle_radius is below 0.2f, the VO algorithm will not work well


    [HideInInspector]   public int num_of_groups;
    [HideInInspector]   public float delay_lower_limit; //? look at testing boll as well
    [HideInInspector]   public float delay_upper_limit;

    //Variables determine random spawn for static obs in waypoint
    [HideInInspector]   public float Static_obs_marg1;
    [HideInInspector]   public float Static_obs_marg2;

    [Header("      ------ Other Control -------")]
    public float overlapAngle; // ? look up
    [HideInInspector]   public float Experiment_start_delay; // ? look up
    [HideInInspector]   public float AutoTrial_base_time; // ? look up
    [HideInInspector]   public string TactilePort;
    public float on_cutoff = 0.05f; // ?look up
    public float trialStartCountdown = 10;
    public float trialTimeLimit = 60;
    public float arenaR = 10;
    public int max_failed_escapes = 15;
    //public float close_cutoff;

    [Header("      ---------- Target Control ----------")]
    public float Target_Area_velocity = 0.3f;
    public float Target_Area_area = 0.7f; //Target_Area_area cannot be set to 0.6 or larger or else the target and obstacle will overlap
    public float Target_Area_travel_radius = 0.65f;
    public float target_height;
    public float Min_Target_Spawn_Radius = 1.1f; //min spawn radius for waypoint
    public float Max_Target_Spawn_Radius = 1.4f; //max spawn radius for waypoint
    public float Min_On_Target_Time = 3.0f; //time on target for waypoint
    public float Max_On_Target_Time = 6.0f;



    [Header("      ------ Linked GameObjects -------")]
    public GameObject Visual_Cue_Display;
    public GameObject Visual_Cue_Arrow;
    public GameObject Tactile_Cue;


    private Camera miniGameCam; //drone camera
    private GameObject obsFact;
    private int totalCount;
    void Start()
    {
        /*if (useSceneView)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }*/
        miniGameCam = GameObject.Find("Minigame Camera").GetComponent<Camera>();
        TactilePort = "COM4"; // Currently disconnected
        Chest_Distance = 0.2f;
        //GameObject.Find("Minimap Mask").active = false;
        //PlayerSettings.enableFrameTimingStats = true;

        obsFact = GameObject.Find("ObstacleFactory");
    }

    // Update is called once per frame
    void Update()
    {
        if (continuous_state_update == true) {print("STATE UPDATE ON");}

        if  (nObstaclesThrown == 1) {
            min_safety_arc_upper = 162;
        }
        else if (nObstaclesThrown == 2) {
            min_safety_arc_upper = 78;
        }
        else if (nObstaclesThrown == 3) {
            min_safety_arc_upper = 42;
        }
        else if (nObstaclesThrown == 4) {
            min_safety_arc_upper = 30;
        }

        /*if (VO_View)
        {
            playerMaxVel = 1.25f;
        }*/
        //GameObject.Find("Minimap Mask").active = false;
        //Target Behavior setting
        totalCount = obsFact.GetComponent<AutoTrial>().getTotalCount();
        if (Randomized_Target_Behavior)
        {
            GameObject.Find("TargetArea").GetComponent<TargetAreaBehavior>().enabled = true;
            GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().enabled = false;
        } else if (Waypoint_Behavior)
        {
            GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().enabled = true;
            GameObject.Find("TargetArea").GetComponent<TargetAreaBehavior>().enabled = false;
        }
        if (Add_Static_Obstacle)
        {
            GameObject.Find("ObstacleFactory").GetComponent<CreateStaticObstacle>().enabled = true;
        } else
        {
            //print("No static");
            GameObject.Find("ObstacleFactory").GetComponent<CreateStaticObstacle>().enabled = false;
        }

        // Num of Threats
        if (Automatic_Trials == true) {
            NumberOfThreats = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().auto_numofthreats;
        }
        // Variables to display
        score = GameObject.Find("Score").GetComponent<Scoring>().score;
        t_countdown = GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().t_countdown;
        trial_time = Time.time;
        failed_escapes = GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().num_failed_escapes;
        // trial_number  = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().trial_number; //TODO check if this is going through
        TrialNum = GameObject.Find("Player").GetComponent<Logging>().TrialNum;
        failure_rate = failed_escapes/totalCount; //TrialNum;
        countdown = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().t_next_trial-GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().t;
        min_safety_arc_current = GameObject.Find("Perceptual Cues").GetComponent<VO>().min_safety_arc;

        if (Testing == true) {delay_lower_limit = 5.0f; delay_upper_limit = 7.0f;} // Add false if needed
        if (Testing == true) {delay_lower_limit = 0.0f; delay_upper_limit = 0.1f;}
        if (NumberOfThreats > 5) {NumberOfThreats  = 5;}
        if (NumberOfThreats <= 0) {NumberOfThreats  = 0;}

        if (Tactile_Goal == false && Tactile_AI == true) { Tactile_Cue.SetActive(true); t_mode = "T_VO"; }
        else if (Tactile_Goal == true && Tactile_AI == false) { Tactile_Cue.SetActive(true); t_mode = "T_Goal"; }
        else if (Tactile_Goal == false && Tactile_AI == false) { Tactile_Cue.SetActive(true); t_mode = "T_none"; } // t needs to be active because goal is in there
        else { };//print("Err in MainMenu Tactile");}

        //Below are game modes. Used a bitmask to determine which objects to add to camera culling layer.
        //Enabling different scripts and cameras depending on game mode.
        if (VO_Drone == false && VO_View == true && /*VO_Intensity_View == false &&*/ Drone_View == false)
        {
            Visual_Cue_Display.SetActive(true); Visual_Cue_Arrow.SetActive(true); v_mode = "V_VO";
            GameObject.Find("Minigame Camera").GetComponent<DroneView>().enabled = false;
            GameObject.Find("Minigame Camera").GetComponent<DroneView_Back>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_I_visualization>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_visualization>().enabled = true;
            miniGameCam.cullingMask |= 1 << LayerMask.NameToLayer("MiniMap");
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Drone"));
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Drone2"));
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Objects"));
        }
        else if (VO_Drone == true && VO_View == false && /*VO_Intensity_View == false &&*/ Drone_View == false)
        {
            Visual_Cue_Display.SetActive(true); Visual_Cue_Arrow.SetActive(true);   v_mode = "V_VO";
            GameObject.Find("Minigame Camera").GetComponent<DroneView>().enabled = true;
            GameObject.Find("Minigame Camera").GetComponent<DroneView_Back>().enabled = false;
            //if (GameObject.Find("Walls_SR") != null)
            //{
             //   GameObject.Find("Walls_SR").SetActive(false);
            //}
            //GameObject.Find("Perceptual Cues").GetComponent<VO>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_visualization>().enabled = true;
            GameObject.Find("Perceptual Cues").GetComponent<VO_I_visualization>().enabled = false;
            miniGameCam.cullingMask |= 1 << LayerMask.NameToLayer("MiniMap");
        } /*else if (VO_Drone == false && VO_View == false && VO_Intensity_View == true && Drone_View == false)
        {
            Visual_Cue_Display.SetActive(true); Visual_Cue_Arrow.SetActive(true); v_mode = "V_VO";
            GameObject.Find("Minigame Camera").GetComponent<DroneView>().enabled = false;
            GameObject.Find("Minigame Camera").GetComponent<DroneView_Back>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_visualization>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_I_visualization>().enabled = true;
            miniGameCam.cullingMask |= 1 << LayerMask.NameToLayer("MiniMap");
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Drone"));
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Drone2"));
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));
            miniGameCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Objects"));
        }*/ else if (VO_Drone == false && VO_View == false && /*VO_Intensity_View == false &&*/ Drone_View == true)
        {
            Visual_Cue_Arrow.SetActive(false);
            GameObject.Find("Minigame Camera").GetComponent<DroneView>().enabled = false;
            GameObject.Find("Minigame Camera").GetComponent<DroneView_Back>().enabled = true;
            if (GameObject.Find("Walls_SR") != null)
            {
                GameObject.Find("Walls_SR").SetActive(false);
            }
            if (GameObject.Find("Walls_SR_Outer") != null)
            {
                GameObject.Find("Walls_SR_Outer").SetActive(false);
            }
            GameObject.Find("Perceptual Cues").GetComponent<VO_visualization>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_I_visualization>().enabled = false;
            GameObject.Find("Minimap Mask").GetComponent<Image>().enabled = true;
            //GameObject.Find("DistanceText").GetComponent<Text>().enabled = false;
        }
        else if (VO_Drone == false && VO_View == false && /*VO_Intensity_View == false &&*/ Drone_View == false)
        {
            Visual_Cue_Display.SetActive(false); Visual_Cue_Arrow.SetActive(false); v_mode = "V_none";
            GameObject.Find("Minigame Camera").GetComponent<DroneView>().enabled = false;
            GameObject.Find("Minigame Camera").GetComponent<DroneView_Back>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_I_visualization>().enabled = false;
            GameObject.Find("Perceptual Cues").GetComponent<VO_visualization>().enabled = false;
            //GameObject.Find("DistanceText").GetComponent<Text>().enabled = false;
        }
        else { print("Err in MainMenu Visual"); }

        // Send Logging


    }
}
