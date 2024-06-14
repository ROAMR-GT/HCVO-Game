using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class AutoTrial : MonoBehaviour
{
    // This script is for the creation of trials and their time stamps such that they are random but the same number of threats happen in a given time period
    // Threats are activated to move in CreateObstacle.cs

    //In logging save trial occurs

    public int group_number;
    public int num_of_groups;
    public float base_time;
    public float pr_multiplier;
    public float delay_lower_limit;
    public float delay_upper_limit;
    public float running_sum;
    public List<float> t_trialtimes_array = new List<float>();
    public List<float> t_savetimes_array = new List<float>();
    public List<int> numofthreats_array = new List<int>();
    public int auto_numofthreats;
    public List<int> temp_pool_numofthreats1 = new List<int>();
    public List<int> temp_pool_numofthreats2 = new List<int>();
    public List<int> temp_pool_numofthreats_all = new List<int>();
    public int numofthreats_auto;
    public int next_trial_num; // THIS IS A LOCAL VARIABLE - changes earlier than it should
    public bool activate_threats;
    public bool activate_savetrial;
    bool Automatic_Trials;
    public float t_next_trial;
    bool active_bool;
    float deltat;
    public float t;
    public float t_save;
    public float save_time;
    float start_delay;
    public float obsPerSec;
    public float randObsGen;
    private float lowerDeltaT;
    private float upperDeltaT;

    private float trialDuration = 0;
    public bool isLaunch;
    Random rnd = new Random();


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
    //private float delay_lower_limit;
    //private float delay_upper_limit;
    //private bool active_bool;
    //public float t;
    public float t_countdown;
    //private float deltat;
    //public bool Automatic_Trials;
    //bool activate_threats;
    private float overlapVal;
    private bool repeat;
    public float obsLaunchTime = 0;

    private float trialStartCountdown;
    private float margin;

    private int nObstacles;
    private float secPerObs;
    int numThrows = 0;

    public bool isFirstthrow = false;
    private float curTime = 0;
    private int curNum = 0;
    private float spawnDeltaT;
    public float tbin;
    private float trialTimeLimit;
    private int totalcount = 0;
    public int TrialNum;

    public bool track_score = false;


    void Start()
    {
        obsPerSec = GameObject.Find("MainMenu").GetComponent<MainMenu>().obstacle_rate;
        randObsGen =  GameObject.Find("MainMenu").GetComponent<MainMenu>().obstacle_randomness;
        trialStartCountdown = GameObject.Find("MainMenu").GetComponent<MainMenu>().trialStartCountdown;
        nObstacles = GameObject.Find("MainMenu").GetComponent<MainMenu>().nObstaclesThrown;
        trialTimeLimit = GameObject.Find("MainMenu").GetComponent<MainMenu>().trialTimeLimit;
        num_of_threats = GameObject.Find("MainMenu").GetComponent<MainMenu>().NumberOfThreats;
        isFirstthrow = true;
        isLaunch = false;
        secPerObs = 1 / obsPerSec;
        margin = randObsGen * secPerObs;
        lowerDeltaT = secPerObs - margin;
        upperDeltaT = secPerObs + margin;
        track_score = false;
        //lowerDeltaT = secPerObs - margin / 2;
        //upperDeltaT = secPerObs + margin / 2;
        //lowerDeltaT = secPerObs*(1 - randObsGen * secPerObs);
        //upperDeltaT = secPerObs*(1 + randObsGen * secPerObs);



        delay_upper_limit = GameObject.Find("MainMenu").GetComponent<MainMenu>().delay_upper_limit;
        delay_lower_limit = GameObject.Find("MainMenu").GetComponent<MainMenu>().delay_lower_limit;
        base_time = GameObject.Find("MainMenu").GetComponent<MainMenu>().AutoTrial_base_time;
        start_delay = GameObject.Find("MainMenu").GetComponent<MainMenu>().Experiment_start_delay;
        pr_multiplier = 5;
        group_number = 6;
        num_of_groups = GameObject.Find("MainMenu").GetComponent<MainMenu>().num_of_groups;
        running_sum = 0;
        active_bool = false;
        t = 0;
        activate_threats = false;
        activate_savetrial = false;
        save_time = 6;

        // Create Random Delays
        for (int j = 0; j<num_of_groups; j++)
        {
            float [] t_random = new float[group_number];
            float [] t_pseudorandom = new float[group_number];
            for (int i = 0; i<group_number; i++) {t_random[i] = Random.Range(delay_upper_limit, delay_lower_limit);}
            // print("Random Set-------" + t_random.Sum());
            // foreach (float val in t_random) {print(val);}
            for (int i = 0; i<group_number; i++) {t_pseudorandom[i] = t_random[i]/t_random.Sum()*group_number;}
            // print("Pseudo Set-------" + t_pseudorandom.Sum());
            foreach (float val in t_pseudorandom) {
                // print(val);
                running_sum = running_sum + (val*pr_multiplier+base_time);
                t_trialtimes_array.Add(running_sum+start_delay);
                t_savetimes_array.Add(running_sum+save_time+start_delay);
            }
        }
        t_trialtimes_array.Add(running_sum + (pr_multiplier+base_time)+start_delay);
        t_savetimes_array.Add(running_sum + (pr_multiplier+base_time)+save_time+start_delay);


        // // DEBUG: Printing out Trial List order
        // print("CUED LIST ---------------------" + t_trialtimes_array[t_trialtimes_array.Count-1] + "     # of throws: " + t_trialtimes_array.Count);
        float last_val = 0;  foreach (float val in t_trialtimes_array) { };//print(val + "   diff:"+  (val-last_val)); last_val = val;}
        //print("CUED LIST ---------------------" + t_trialtimes_array[t_trialtimes_array.Count-1] + " (sec)     # of throws: " + t_trialtimes_array.Count);

        // Create Random Threat Numbers
        for (int j = 0; j<num_of_groups; j++)
        {
            while (temp_pool_numofthreats1.Count < 3 ) {
                int num1 = (int) Mathf.Round(Random.Range(1, 4));
                if (temp_pool_numofthreats1.Contains(num1) == false) {temp_pool_numofthreats1.Add(num1);}
            }
            while (temp_pool_numofthreats2.Count < 3 ) {
                int num2 = (int) Mathf.Round(Random.Range(1, 4));
                if (temp_pool_numofthreats2.Contains(num2) == false) {temp_pool_numofthreats2.Add(num2);}
            }
            temp_pool_numofthreats_all.AddRange(temp_pool_numofthreats1);
            temp_pool_numofthreats_all.AddRange(temp_pool_numofthreats2);
            // Shuffle set
            for (int i = 0; i < temp_pool_numofthreats_all.Count; i++) {
                int temp = temp_pool_numofthreats_all[i];
                int randomIndex = Random.Range(i, temp_pool_numofthreats_all.Count);
                temp_pool_numofthreats_all[i] = temp_pool_numofthreats_all[randomIndex];
                temp_pool_numofthreats_all[randomIndex] = temp;
            }

            numofthreats_array.AddRange(temp_pool_numofthreats_all);
            temp_pool_numofthreats_all.Clear();
            temp_pool_numofthreats1.Clear();
            temp_pool_numofthreats2.Clear();
        }

        // foreach (int val in numofthreats_array) {print(val);}
        numofthreats_array.Add(1);

        next_trial_num = 0;
        t_next_trial = t_trialtimes_array[0];
        t_save =  t_savetimes_array[0];
        auto_numofthreats = numofthreats_array[0];
        spawnDeltaT = Random.Range(0, margin);
        obsLaunchTime += spawnDeltaT;
        tbin = 0;
        track_score = false;
        TrialNum = 0;
    }





    // Update is called once per frame
    void Update()
    {
        // if (isLaunch == false)
        // {
        //     curTime = trialDuration;
        // }


        // Initially time countdown is paused
        Automatic_Trials = GameObject.Find("MainMenu").GetComponent<MainMenu>().Automatic_Trials;

        // Cycling Trials
        deltat = Time.deltaTime;
        //trialDuration = 0;
        //if (Input.GetKeyDown("space")) { active_bool = true;}
        int counter = 1;
        trialStartCountdown -= Time.deltaTime;
        //trialDuration += deltat;


        if (Automatic_Trials == true && trialStartCountdown<=0 && nObstacles > 0 && trialDuration<=trialTimeLimit && TrialNum<=trialTimeLimit/secPerObs)/*&& active_bool == true*/
        {
            track_score = true;
            trialDuration += deltat;

            if (trialDuration >= obsLaunchTime && trialDuration >= tbin)
            {
                TrialNum++;
                if (TrialNum > 15) {
                    print("Ending Game");
                    // UnityEditor.EditorApplication.isPlaying = false;
                    Debug.Break();
                }
                GameObject.Find("ObstacleFactory").GetComponent<CreateObstacle>().makeObstacle(nObstacles, counter);
                totalcount++;


                spawnDeltaT = Random.Range(lowerDeltaT, upperDeltaT);
                if (totalcount == (trialTimeLimit/secPerObs))
                {
                    // print("exceeding max time");
                    spawnDeltaT = Random.Range(lowerDeltaT, secPerObs);
                }

                obsLaunchTime = tbin + spawnDeltaT;

                // print("total count: " + totalcount+  "   tbin: " + tbin +  "   obsLaunchTime: " + obsLaunchTime + "   spawnDeltaT: " + spawnDeltaT);

                tbin += secPerObs;

            }
        }
        else {
            track_score = false;
        }


    }
    public int getNObstacles()
    {
        return nObstacles;
    }
    public int getTotalCount()
    {
        return totalcount;
    }

}
