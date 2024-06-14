using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class Tactile : MonoBehaviour
{
    public Vector3 target_pos;
    public Vector3 cam_angles;
    static SerialPort ser;
    public float t0;
    public float t1;
    public float dt;
    public float angle;
    public float output_angle_VO;
    public float output_angle_Goal;
    public string t_mode;

    public float mag;
    public bool test_mode;
    public int dir;
    private float t_x;
    private float t_z;
    private float a_g;
    private int[] int_options = new int[] {1, 2, 3, 4, 5, 6 }; // Distance Thresholds
    private float[] dir_options = new float[] {22.50f, 67.50f,112.50f,157.50f,202.50f,247.50f,292.50f,337.50f,382.50f}; // Angle Thresholds
    bool bool_tactile;
    bool print_err = true;
    string port_name;
    float abs;
    float inc;
    bool on_target;

    void Start()
    {
        port_name = "COM4";//GameObject.Find("MainMenu").GetComponent<MainMenu>().TactilePort; //TODO
        if (SerialPort.GetPortNames().ToList().Contains(port_name))
        {
            bool_tactile = true;
            ser = new SerialPort();
            ser.PortName = port_name;
            ser.ReadTimeout = 50;
            ser.BaudRate = 9600;
            ser.Open();
        }
        else
        {
            bool_tactile = false;
            if (print_err == true)
            {
                print("                     ERR: Arduino Not Connected");
                print_err = false;
            }
        }
        inc = 0;
    }

    // Update is called once per frame
    void Update()
    {
        test_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().Tactile_Test;

        if (test_mode && bool_tactile){
            if (Input.GetKeyDown(KeyCode.Keypad0)) {ser.Write("0");}
            if (Input.GetKeyDown(KeyCode.Keypad6)) {WhatDirInt(  0.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad9)) {WhatDirInt( 45.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad8)) {WhatDirInt( 90.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad7)) {WhatDirInt(135.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad4)) {WhatDirInt(180.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad1)) {WhatDirInt(225.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad2)) {WhatDirInt(270.0f, mag, 0);}
            if (Input.GetKeyDown(KeyCode.Keypad3)) {WhatDirInt(315.0f, mag, 0);}
        }// end test_mode

        else {
            // Camera orientaiton
            GameObject camera_gameobject = GameObject.Find("Player");
            Transform camera_transform = camera_gameobject.GetComponent<Transform>();
            // Relative position
            GameObject target_gameobject = GameObject.Find("TargetArea");
            Vector3 rel_target_pos = camera_gameobject.transform.InverseTransformPoint(target_gameobject.transform.position);
            t_x = rel_target_pos[0];
            t_z = rel_target_pos[2];
            // Angle Calculation
            angle = Mathf.Atan(t_z/t_x)*Mathf.Rad2Deg;
            // print(angle);
            // output_angle_Goal = AngleWrapCorrection(angle, t_x, t_z) ; // world ref frame
            cam_angles = transform.eulerAngles;
            // print(cam_angles[1]);
            output_angle_Goal = AngleWrapCorrection(angle, t_x, t_z) - cam_angles[1]; // world ref frame
            // print(AngleWrapCorrection(angle, t_x, t_z));
            // print(Mathf.DeltaAngle(cam_angles[1], -90)+180);

            output_angle_VO = GameObject.Find("Perceptual Cues").GetComponent<VO>().output_angle_VO; //+
            inc = Absolute2Incremental(output_angle_VO,inc);
            output_angle_VO = (Mathf.DeltaAngle(Mathf.DeltaAngle(cam_angles[1], -90)+180, inc-90))+180;


            // Tactile Serial Dispatch
            t_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().t_mode;
            on_target = GameObject.Find("DistanceText").GetComponent<DistanceTextBehavior>().on_target;
            if (t_mode == "T_VO" && bool_tactile && on_target == false)   {
                WhatDirInt(output_angle_VO, t_x, t_z);
            }
            if (t_mode == "T_Goal" && bool_tactile && on_target == false) {
                WhatDirInt(output_angle_Goal + cam_angles[1], t_x, t_z);
            }
            if (t_mode == "T_none" && bool_tactile)   {ser.Write("0");}
            if (on_target == true && SerialPort.GetPortNames().ToList().Contains(port_name)) {ser.Write("0");}

        } // end ~test_mode


    } // end update


    // ------------------------------ FUNCTIONS --------------------------------
    float AngleWrapCorrection(float a, float x, float y)
    {
        if (x >= 0 && y >= 0) {a_g = a;}
        else if (x < 0  && y >= 0) {a_g = a+180;}
        else if (x >= 0  && y < 0) {a_g = a+360;}
        else if (x < 0 && y < 0) {a_g = a+180;}
        else {print("AngleWrapCorrection error!");}
        return a_g;
    }


    float Absolute2Incremental(float abs, float inc)
    {
        if (abs>360) {inc = abs - 360;}
        else if (abs<0) {inc = abs + 360;}
        else{inc = abs;}
        return inc;
    }

    int WhatDirInt(float angle, float x, float y)
    {
        if (     angle > dir_options[0] && angle <=  dir_options[1]){dir = 9; ser.Write("9"); }
        else if (angle > dir_options[1] && angle <=  dir_options[2]){dir = 8; ser.Write("8"); }
        else if (angle > dir_options[2] && angle <=  dir_options[3]){dir = 7; ser.Write("7"); }
        else if (angle > dir_options[3] && angle <=  dir_options[4]){dir = 4; ser.Write("4"); }
        else if (angle > dir_options[4] && angle <=  dir_options[5]){dir = 1; ser.Write("1"); }
        else if (angle > dir_options[5] && angle <=  dir_options[6]){dir = 2; ser.Write("2"); }
        else if (angle > dir_options[6] && angle <=  dir_options[7]){dir = 3; ser.Write("3"); }
        else if (angle > dir_options[7] || angle <=  dir_options[0]){dir = 6; ser.Write("6"); }
        else {print("WhatDir error!");}
        mag = Mathf.Sqrt(x*x+y*y);
        ser.Write("r");
        // if (     mag <= int_options[0]) {ser.Write("r");}
        // else if (mag >  int_options[0] && mag <= int_options[1]) {ser.Write("r");}
        // else if (mag >  int_options[1] && mag <= int_options[2]) {ser.Write("r");}
        // else if (mag >  int_options[2] && mag <= int_options[3]) {ser.Write("r");}
        // else if (mag >  int_options[3] && mag <= int_options[4]) {ser.Write("r");}
        // else if (mag >  int_options[4] && mag <= int_options[5]) {ser.Write("r");}
        // else if (mag >  int_options[5]) {ser.Write("r");}
        // else {print("WhatInt error!");}
        return dir ;
        // }

    }

    void OnApplicationQuit()
    {
        if (SerialPort.GetPortNames().ToList().Contains(port_name)) {
            ser.Write("0"); // kill the motors when system turns off
        }
    }

    void OnDisable()
    {
        if (SerialPort.GetPortNames().ToList().Contains(port_name)) {
            ser.Write("0"); // kill the motors when system turns off
        }
    }

}
