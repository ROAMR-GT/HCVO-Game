using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System;
using System.Linq;

public class Logging : MonoBehaviour
{
    // Start is called before the first frame update
    /*
        Variables to be Tracked:
            time, trial number, userPos[0,1,2] (variable from player location), objPos[0,1,2]_1,
            objPos[0,1,2]_2, objPos[0,1,2]_3 (if they exist), obj_1_hit, obj_2_hit, obj_3_hit, score,
            VO angle output, Maybe velocities?,
            CameraPos[0,1,2], CameraQua[0,1,2,3], trackerPos[0,1,2], trackerQua[0,1,2,3]
            static obstacle[0,1,2]
    */
    public string User_ID;
    public string datafolder = "CSV";
    public float time;
    public int trial_number;
    public string condition; // TODO need to make a function that pulls in booleans to output String
    public Vector3 userPos; // for the player body
    public Quaternion userRot;
    public Vector3 cameraPos; // For camera
    public Quaternion cameraRot;
    public Vector3 trackerPos; // for tracker
    public Quaternion trackerRot;
    public Vector3 objPos1; // object positions
    public Vector3 objPos2;
    public Vector3 objPos3;
    private Vector3 sobjPos;
    public Vector3 goalPos;
    public bool objHit1;
    public bool objHit2;
    public bool objHit3;
    public float score; // overall score
    public float health; // overall score
    public float assist_angle; // overall score
    public bool VO_View;
    //public bool VO_I;
    public bool VO_Drone;
    public bool Drone_View;
    public bool Tactile_AI;
    public bool Tactile_Goal;
    public bool assistance_active;
    public bool Automatic_Trials;

    public string basePath;
    public string folderPath;
    public string filePathall;
    public string filePath;
    public string rowData;
    public string rowData_vars;
    public List <string> loglist = new List<string> ();
    public List <string> loglistall = new List<string> ();

    public int TrialNum = 1; // starts at 1, for first trial

    public int numHits;
    public int num_failed_escapes;
    public int hit_notification;
    public float goal_dist;
    public string v_mode;
    public string t_mode;
    public string num_obj;

    private int assistanceMode;
    private int threatType;
    private int goalType;

    private float[] safe_angles;

    private float waypointBehaviorTimer;
    private float waypointSpawnRadius;
    private float safetyArc;
    private float targetArea;
    private float timeThreats;

    private float obsRad;
    private float targetHeight;

    private float travelRad;
    private int maxObsNum = 20;

    private Vector3 objPos;


    void Start()
    {
        // get trial/subject id
        // new csv file for trial, open with write permissions
        // subnote, need folder for all trial data
        // https://stackoverflow.com/questions/60477919/closing-a-file-in-c-sharp-unity

        // get user id
        User_ID = GameObject.Find("MainMenu").GetComponent<MainMenu>().User_ID;

        // make subject folder
        string testingString = "";
        if (GameObject.Find("MainMenu").GetComponent<MainMenu>().Testing){
            testingString = "Test/";}

        basePath = Application.dataPath + "/" + datafolder;
        folderPath = basePath + "/" + testingString;
        var folder = Directory.CreateDirectory(folderPath); // returns a DirectoryInfo object

        safe_angles = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles;

        filePathall = folderPath + "/" + User_ID + ".csv";
        if (System.IO.File.Exists(filePathall)){
            print("           ---------- ALERT: File exists ---------- CHECK FILE NAME");
            Debug.Break();
            // DeleteFile(filePathall);
        } // delete if file exists


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetHeight = GameObject.Find("MainMenu").GetComponent<MainMenu>().target_height;
        obsRad = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        timeThreats = GameObject.Find("MainMenu").GetComponent<MainMenu>().countdown;
        targetArea = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area;
        safetyArc = GameObject.Find("Perceptual Cues").GetComponent<VO>().min_safety_arc;
        safe_angles = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles;
        // Other Key Variables
        time += Time.deltaTime;
        // trial_number  = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().trial_number; //TODO check if this is going through
        condition = "test"; // TODO use this for nameing convention of the file or logged in file

        // User State
        userPos = GameObject.Find("Player").transform.position;
        userRot = GameObject.Find("Player").transform.rotation;
        cameraPos = GameObject.Find("Camera").transform.position;
        cameraRot = GameObject.Find("Camera").transform.rotation;
        trackerPos = GameObject.Find("Tracker").transform.position;
        trackerRot = GameObject.Find("Tracker").transform.rotation;

        // Assistance
        VO_View = GameObject.Find("MainMenu").GetComponent<MainMenu>().VO_View;
        //VO_I = GameObject.Find("MainMenu").GetComponent<MainMenu>().VO_Intensity_View;
        Tactile_AI = GameObject.Find("MainMenu").GetComponent<MainMenu>().Tactile_AI;
        VO_Drone = GameObject.Find("MainMenu").GetComponent<MainMenu>().VO_Drone;
        Drone_View = GameObject.Find("MainMenu").GetComponent<MainMenu>().Drone_View;
        Tactile_Goal = GameObject.Find("MainMenu").GetComponent<MainMenu>().Tactile_Goal;
        if (VO_View == true) {
            assistance_active = true;
            assist_angle = GameObject.Find("Assistance Arrow").transform.eulerAngles.y;}
        else {assistance_active = false; assist_angle = 1000;} // angle over 360

        //Goal type
        if (GameObject.Find("TargetArea").GetComponent<TargetAreaBehavior>().enabled == true && GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().enabled == false){
            goalType = 1;
            waypointBehaviorTimer = 1000f;
            waypointSpawnRadius = 1000f;
            travelRad = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_travel_radius;
        } else if (GameObject.Find("TargetArea").GetComponent<TargetAreaBehavior>().enabled == false && GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().enabled == true){
            goalType = 2;
            waypointBehaviorTimer = GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().getTimer();
            waypointSpawnRadius = GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().getSpawnDist();
            travelRad = 1000f;
        }

        //Threat Type
        if (GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle == false)
        {
            threatType = 1;
        } else
        {
            threatType = 2;
        }

        //Assistance Mode
        if (VO_View == false && VO_Drone == false && /*VO_I == false &&*/ Drone_View == false)
        {
            assistanceMode = 1;
        } else if (VO_View == false && VO_Drone == false && /*VO_I == false &&*/ Drone_View == true)
        {
            assistanceMode = 2;
        } else if (VO_View == true && VO_Drone == false && /*VO_I == false &&*/ Drone_View == false)
        {
            assistanceMode = 3;
        } /*else if (VO_View == false && VO_Drone == false && VO_I == true && Drone_View == false)
        {
            assistanceMode = 4;
        }*/ else if (VO_View == false && VO_Drone == true && /*VO_I == false &&*/ Drone_View == false)
        {
            assistanceMode = 4;
        }

        // Metrics
        goal_dist = GameObject.Find("DistanceText").GetComponent<DistanceTextBehavior>().distance;
        num_failed_escapes = GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().num_failed_escapes;
        hit_notification = GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().hit_notification;
        numHits = GameObject.Find("Score").GetComponent<Scoring>().numHits;
        score = GameObject.Find("Score").GetComponent<Scoring>().score;
        health = GameObject.Find("Health Bar").GetComponent<Health>().target_health/4.0f;
        TrialNum = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().TrialNum;

        // Object Positions
        if (GameObject.Find("Obstacle 1") != null){
            objPos1 = GameObject.Find("Obstacle 1").transform.position;}
        else {objPos1 = new Vector3(1000,-1,1000);} // change to negative, more easy to find in processing, or large x and z, negative y
        if (GameObject.Find("Obstacle 2") != null) {
            objPos2 = GameObject.Find("Obstacle 2").transform.position;}
        else {objPos2 = new Vector3(1000, -1, 1000);}
        if (GameObject.Find("Obstacle 3") != null) {
            objPos3 = GameObject.Find("Obstacle 3").transform.position;}
        else {objPos3 = new Vector3(1000, -1, 1000);}
        if (GameObject.Find("StaticObstacle") != null)
        {
            sobjPos = GameObject.Find("StaticObstacle").transform.position;
        }
        else
        {
            sobjPos = new Vector3(1000, -1, 1000);
        }
        goalPos = GameObject.Find("TargetArea").transform.position;

        // Condition
        num_obj = GameObject.Find("MainMenu").GetComponent<MainMenu>().NumberOfThreats.ToString();
        t_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().t_mode;
        v_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().v_mode;

        rowData_vars = "Time,Trial,user0,user1,user2,cam0,cam1,cam2,camr0,camr1,camr2,camr3," +
            "tra0,tra1,tra2,trar0,trar1,trar2,trar3,goalPos0,goalPos1,goalPos2," +
            "assistance-" + t_mode + "-" + v_mode + "-" + num_obj + ",hitHMD,hits,failed_escapes,"+
            "goaldist,score,health,assistance,threattype,goaltype,waypointTimer,minSafetyArc,targetArea,obsRad,targetHeight";
        rowData = time.ToString() + ',' + TrialNum.ToString() + ',' +
            userPos[0].ToString() + ',' + userPos[1].ToString() + ',' + userPos[2].ToString() + ',' +
            cameraPos[0].ToString() + ',' + cameraPos[1].ToString() + ',' + cameraPos[2].ToString() + ',' +
            cameraRot[0].ToString() + ',' + cameraRot[1].ToString() + ',' +
            cameraRot[2].ToString() + ',' + cameraRot[3].ToString() + ',' +
            trackerPos[0].ToString() + ',' + trackerPos[1].ToString() + ',' + trackerPos[2].ToString() + ',' +
            trackerRot[0].ToString() + ',' + trackerRot[1].ToString() + ',' +
            trackerRot[2].ToString() + ',' + trackerRot[3].ToString() + ',' + goalPos[0].ToString()+ ',' +
            goalPos[1].ToString() + ',' + goalPos[2].ToString() + ',' +
            assist_angle.ToString() + ',' + hit_notification.ToString() + ',' +
            numHits.ToString() + ',' + num_failed_escapes.ToString() + ',' +
            goal_dist.ToString() + ',' + score.ToString() + ',' + health.ToString() + ',' + assistanceMode.ToString() + ',' +
            threatType.ToString() + ',' + goalType.ToString() + ',' + waypointBehaviorTimer.ToString("F2") + ',' +
            safetyArc.ToString() + ',' + targetArea.ToString() + ',' + obsRad.ToString() + ',' + targetHeight.ToString();

        for (int i = 0; i < maxObsNum; i++)
        {
            rowData_vars = rowData_vars + ",obj" + (i + 1).ToString() + "Pos0" + ",obj" + (i + 1).ToString() + "Pos1" + ",obj" + (i + 1).ToString() + "Pos2";
            if (GameObject.Find("Obstacle " + (i + 1)) != null)
            {
                objPos = GameObject.Find("Obstacle " + (i + 1)).transform.position;
                rowData = rowData + ',' + objPos[0].ToString() + ',' + objPos[1].ToString() + ',' + objPos[2].ToString();

            } else
            {
                rowData = rowData + ',' + 1000 + ',' + -1 + ',' + 1000;
            }
        }


        for (int i = 0; i < safe_angles.Length; i++)
        {
            rowData_vars = rowData_vars + ",angle" + i.ToString();
            rowData = rowData + ',' + safe_angles[i].ToString();
        }

        loglist.Add(rowData); // data saved after each trial so if game quits unexpectedly, we still have previous data
        loglistall.Add(rowData); //TODO need a way to check if system was quit unexpectedly so we know how many trials to do

        // TODO: find a better way to determine end of trial
        // TODO: if we ned up going with this, need to eliminate unity recognizing 2 clicks of button

        Automatic_Trials = GameObject.Find("MainMenu").GetComponent<MainMenu>().Automatic_Trials;
        // if (Input.GetKeyDown("n") && Automatic_Trials == false) {OnTrialEnd();}
        // if (GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().activate_savetrial == true)
        // {
        //     //OnTrialEnd();
        //     TrialNum++;
        //     GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().activate_savetrial = false;
        //     // print("Trial Save - Done");
        //
        // }

        // auto_trial_num = ;
        // auto_t_save = ;
        // auto_t = ;
        // if (Automatic_Trials == true) {
        //
        //     print("Trial Saved");
        //     OnTrialEnd();
        // }


    }


    void OnApplicationQuit()
    {

        filePathall = folderPath + "/" + User_ID + ".csv";
        if (System.IO.File.Exists(filePathall)){
            print("File Exists already, Delete it to run game. Look for: " + filePathall);
            Debug.Break();
        } // delete if file exists
        else {
            DeleteFile(filePathall);
            StreamWriter sw = new StreamWriter(filePathall, true);
            sw.WriteLine(rowData_vars);
            sw.Flush();
            for(int i = 0; i<loglistall.Count; i++) // cycle through and write log list variables
            {
                sw.WriteLine(loglistall[i]);
                sw.Flush();
            }
            loglistall.Clear();

            // copy to dump folder
            if (!GameObject.Find("MainMenu").GetComponent<MainMenu>().Testing)
            {
                FileUtil.CopyFileOrDirectory(folderPath + "/" + User_ID + "_" + condition + "_all.csv", basePath + "/Dump/" + User_ID + "_" + condition + "_all.csv");
            }
        }

    }

    // void OnTrialEnd()
    // {
    //     filePath = folderPath + "/" + TrialNum.ToString() + ".csv";
    //     if (System.IO.File.Exists(filePath)) { DeleteFile(filePath); } // delete if file exists
    //     StreamWriter sw = new StreamWriter(filePath, true);
    //     sw.WriteLine(rowData_vars);
    //     sw.Flush();
    //     for (int i = 0; i < loglist.Count; i++) // cycle through and write log list variables
    //     {
    //         sw.WriteLine(loglist[i]);
    //         sw.Flush();
    //     }
    //     loglist.Clear();
    //
    //     // copy to dump folder
    //     // if (!GameObject.Find("MainMenu").GetComponent<MainMenu>().Testing)
    //     // {
    //     //     FileUtil.CopyFileOrDirectory(folderPath + "/" + User_ID + "_" + condition + "_" + TrialNum.ToString() + ".csv", basePath + "/Dump/" + User_ID + "_" + condition + "_" + TrialNum.ToString() + "_" + GetTimestamp() + ".csv");
    //     // }
    //
    //     TrialNum++;
    // }


    void DeleteFile(string fileName)
    {
        if ( File.Exists( fileName) )  {
            File.Delete( fileName );
            UnityEditor.AssetDatabase.Refresh();
        } // end of if
    } // end of DeleteFile


    public static String GetTimestamp() // if you need a unique identifier to save files
    {
        return DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss_ffff");
    }

}
