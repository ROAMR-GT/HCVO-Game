using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class VO_pilots : MonoBehaviour
{
    public Vector3 output_vel;
    public float obj1_vel;
    public Vector3 obj1_dir;
    public Vector3 obj1_vect;
    public float obj2_vel;
    public Vector3 obj2_dir;
    public Vector3 obj2_vect;
    public float obj3_vel;
    public Vector3 obj3_dir;
    public Vector3 obj3_vect;
    public float obj4_vel;
    public Vector3 obj4_dir;
    public Vector3 obj4_vect;
    private Vector3[] obs_vels;
    private Vector3[] obs_poss;
    private Vector3 next_pos;
    private float best_score;
    private Vector3 obj1_pos;
    private Vector3 obj2_pos;
    private Vector3 obj3_pos;
    private Vector3 obj4_pos;
    private float dist;
    public Vector3 best_vel;
    public float inst = 0.0f;
    public float objective_angle;
    public float obj_goal;
    public float obj_avel;
    public int min_safety_arc;

    //avel, obs_vels[i], agent_pos, obs_poss[i], C_spaces[i]
    public Vector3 debug_avel;
    public Vector3 debug_ovel;
    public Vector3 debug_apos;
    public Vector3 debug_opos;
    public float debug_Cspace;
    public int debug_i;
    public float mag;

    public Vector3[] obj_vels;
    public Vector3[] obj_poss;

    public float output_angle_VO;
    public float output_angle_VO_prev;
    public bool safe_above_min_arc;
    public float output_angle_Goal;
    public string v_mode;
    float a_g;
    Queue<float> buff = new Queue<float>();
    float prev_a;
    int LP_buffer_size;

    int player_LP_buffer_size;
    Queue<float> player_velx_buff = new Queue<float>();
    Queue<float> player_vely_buff = new Queue<float>();
    float player_velx_ang_0;
    float player_vely_ang_0;
    float player_velx_diff;
    float player_vely_diff;
    float player_vel_ang;
    int sum_check;
    private float obj_max_diff;
    bool continuous_state_update;


    float a;
    float b;
    float biomech_bias;
    public bool biomechanical_bias;

    public float[] safe_angles_static; //array for determining if angle is safe from static obs
    public float[] safe_angles_dynamic; //array for determining if angle is safe from dynamic obs
    public float[] safe_angles; //array for determining whether angle is safe from both static and dynamic

    //public float[] safe_angles_not_moving;
    public bool all_clear;
    private float playerRadius;
    private float obsRadius;
    private float threat_space;

    private float oldth;
    private bool isAway;

    private Vector3 prevPos;
    private Vector3 newPos;
    private Vector3 velocity;

    private float[] low_threat_dynamic; //determines angles with low escape velocity fron dynamic obs
    private float[] med_threat_dynamic; //determines angles with med escape velocity from dynamic obs
    private float[] high_threat_dynamic; //determines angles with high escape velocity from dynamic obs

    private float[] low_threat_static = new float[60]; //didn't really use these
    private float[] med_threat_static = new float[60];
    private float[] high_threat_static = new float[60];

    private float[] low_threat; //didn't really use these except to copy low_threat_dynamic cases
    private float[] med_threat;
    private float[] high_threat;

    private bool thadmissible;
    bool maskAllRed; //determines whether all angles are unsafe.

    private bool isMovingAway; //determines whether player is moving away from static obst
    private bool isNotMoving; //determines whether player is not moving.

    private float staticArc; //arc that indicates static obs direction
    private Vector3 relativePos;


    //public GameObject agent;
    //public GameObject goal;
    // public Dictionary<int, List<float>> regions_dict = new Dictionary<int, List<float>>(); //(region#, list of indexes)
    // public List<float> region_list_idx = new List<float>();

    private bool isVO;
    //private bool isVO_I;
    private float tuneMag;

    //GameObject playerCam;
    //Vector3 prevCamAngle;
    //Vector3 newCamAngle;

    GameObject playerObj;
    Vector3 prevPlayerAngle;
    Vector3 newPlayerAngle;


    GameObject wallsOuter;// = GameObject.Find("Walls_SR_Outer");
    GameObject wallsInner;

    private int maxObsNum = 20;
    // Start is called before the first frame update

    public float obj_vel;
    public Vector3 obj_dir;
    public Vector3 obj_vect;
    public Vector3 obj_pos;

    void Start()
    {
        wallsInner = GameObject.Find("Walls_SR");
        wallsOuter = GameObject.Find("Walls_SR_Outer");
        if (wallsOuter == null)
        {
            print("no walls Outer");
        }
        //playerCam = GameObject.Find("Camera");
        isVO = GameObject.Find("MainMenu").GetComponent<MainMenu>().VO_View;
        //isVO_I = GameObject.Find("MainMenu").GetComponent<MainMenu>().VO_Intensity_View;
        oldth = 0.0f;
        dist = 0f;
        isAway = false;
        LP_buffer_size = GameObject.Find("MainMenu").GetComponent<MainMenu>().AI_LowPass_buffer_size;
        player_LP_buffer_size = GameObject.Find("MainMenu").GetComponent<MainMenu>().Player_LowPass_buffer_size;
        obj_max_diff = GameObject.Find("MainMenu").GetComponent<MainMenu>().obj_max_diff;
        for (int i = 0; i < LP_buffer_size; i++) { buff.Enqueue(0); }
        for (int i = 0; i < player_LP_buffer_size; i++) { player_velx_buff.Enqueue(0); }
        for (int i = 0; i < player_LP_buffer_size; i++) { player_vely_buff.Enqueue(0); }
        prev_a = 0;
        all_clear = true;
        playerRadius = GameObject.Find("Player").GetComponent<Transform>().localScale.x;
        //print(playerRadius);
        obsRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Obstacle_radius;
        //print(obsRadius);
        threat_space = playerRadius/2 + obsRadius/2;

        playerObj = GameObject.Find("Player");
        prevPlayerAngle = playerObj.transform.forward;
        prevPlayerAngle.y = 0;
        newPlayerAngle = playerObj.transform.forward;
        newPlayerAngle.y = 0;

        prevPos = GameObject.Find("Player").GetComponent<Transform>().position;
        newPos = GameObject.Find("Player").GetComponent<Transform>().position;
        tuneMag = GameObject.Find("MainMenu").GetComponent<MainMenu>().playerMaxVel;
        output_angle_VO_prev = 0;

        //prevCamAngle = new Vector3(playerObj.transform.forward.x, 0f, playerObj.transform.forward.z);
        //newCamAngle = new Vector3(playerObj.transform.forward.x, 0f, playerObj.transform.forward.z);

    }

    // Update is called once per frame
    void Update()
    {
        isNotMoving = false;
        isMovingAway = false;
        maskAllRed = false;
        GameObject agent = GameObject.Find("Player");
        Transform agent_transform = agent.GetComponent<Transform>();
        Vector3 agent_pos = agent_transform.position;
        agent_pos.y = 0; //comment when testing;

        newPos = agent_pos;
        velocity = (newPos - prevPos) / Time.fixedDeltaTime;
        prevPos = newPos;

        prevPlayerAngle = newPlayerAngle;
        //newPlayerAngle = new Vector3(0f, 0f, playerObj.transform.forward.z);
        //prevCamAngle = newCamAngle;
        //newCamAngle = playerCam.transform.forward;

        //print("newAngle: " + newPlayerAngle);

        // Velocity Vector Direction
        GameObject arrow_vel = GameObject.Find("Velocity Arrow");
        // bool show_vel = true;
        // arrow_vel.SetActive(show_vel);
        //print("velarrow: " + arrow_vel);
        //print("angles: " + Mathf.Atan(velocity[2] / velocity[0]));
        //print("velvec: " + velocity);


        player_velx_buff.Dequeue();
        player_velx_buff.Enqueue(velocity[0]);
        player_vely_buff.Dequeue();
        player_vely_buff.Enqueue(velocity[2]);
        float player_velx_avg = player_velx_buff.Average();
        float player_vely_avg = player_vely_buff.Average();


        Transform arrow_vel_transform = arrow_vel.GetComponent<Transform>();
        arrow_vel_transform.eulerAngles = new Vector3(90, -player_vel_ang+ 90, 0);


        GameObject goal = GameObject.Find("TargetArea");
        Transform goal_transform = goal.GetComponent<Transform>();
        Vector3 goal_pos = goal_transform.position;
        goal_pos.y = 0; //comment when testing

        float[] C_spaces = new float[20];
        Vector3[] obs_vels = new Vector3[20];
        Vector3[] obs_poss = new Vector3[20];
        for (int i = 0; i < maxObsNum; i++)
        {
            if (GameObject.Find("Obstacle " + (i+1))!= null)
            {
                GameObject obj = GameObject.Find("Obstacle " + (i+1));
                Transform obj_transform = obj.GetComponent<Transform>();
                obj_pos = obj_transform.position;
                obj_pos.y = 0; //comment when testing
                obj_vel = GameObject.Find("Obstacle " + (i+1)).GetComponent<ObstacleBehavior>().velocity;
                obj_dir = GameObject.Find("Obstacle " + (i+1)).GetComponent<ObstacleBehavior>().direction;
                obj_vect = obj_vel * obj_dir; // unit vector direction * vel
            } else
            {
                obj_vel = 0.0f;
                obj_vect = new Vector3(0, 0, 0);
                obj_pos = new Vector3(0, 0, 0);
            }
            if (obj_vel >= 0.0001)
            {
                C_spaces[i] = threat_space*1.0f; //obstacle size
            }
            else
            {
                C_spaces[i] = 0;
            }
            obs_vels[i] = obj_vect;
            obs_poss[i] = obj_pos;
        }

        //print("Velocities: " + obs_vels[0] + " " + obs_vels[1] + " " + obs_vels[2] + " " + obs_vels[3]);
        //print("Goal Vec: " + (goal_pos - agent_pos));


        // VO SEARCH THROUGH ANGLES AND MAGNITUDES -----------------------------
        safe_angles_static = new float[60];
        safe_angles_dynamic = new float[60];

        //safe_angles_not_moving = new float[60];

        low_threat_dynamic = new float[60];
        med_threat_dynamic = new float[60];
        high_threat_dynamic = new float[60];

        low_threat = new float[60];
        med_threat = new float[60];
        high_threat = new float[60];

        int theta_count = 0;
        best_score = 36000;
        Dictionary<float, float> admissibility_idx_dict = new Dictionary<float, float>(); //(region#, list of indexes)
        //int count = 0;
        for (float th = 0; th < 2 * Mathf.PI - 0.05; th = th + 2 * Mathf.PI / 60) // need 0.05 because of rounding error
        {
            //TODO: float max_mag = maximum_velocity(th);

            theta_count = theta_count + 1;
            float max_mag;
            max_mag = tuneMag;
            /*if (isVO)
            {
                max_mag = 1.25f;
            } else
            {
                max_mag = tuneMag;
            }*/
            //print("Max mag: " + max_mag);
            a = 1.5f; //horizontal frontal plane
            b = 0.5f; //vertical sagittal plane
            biomechanical_bias = GameObject.Find("MainMenu").GetComponent<MainMenu>().Biomechaical_Bias;
            //float mag = 0.6f;
            bool staticadmissible = true;
            int count = 0;
            // for (float mag = 0.1f; mag <= max_mag+ 0.1f; mag = mag + max_mag / 1) //3
            // {
            mag = max_mag;
            // biomech_bias = mag * (a * b) / Mathf.Sqrt(a * a * Mathf.Sin(th) * Mathf.Sin(th) + b * b * Mathf.Cos(th) * Mathf.Cos(th));
            // if (biomechanical_bias == true) { mag = biomech_bias * mag; }

            Vector3 avel = new Vector3(mag * Mathf.Cos(th), 0, mag * Mathf.Sin(th));
            next_pos = agent_pos + avel * Time.deltaTime;
            bool admissible = true;
            //print(C_spaces[3]);
            for (int i = 0; i < obs_vels.Length; i++)
            {
                if (C_spaces[i] == 0 && obs_vels[i].magnitude == 0)
                {
                    continue;
                }
                admissible = admissible && !VO_fn(avel, obs_vels[i], agent_pos, obs_poss[i], C_spaces[i], i); //determines dynamic admissibility at each velocity
                //admissible = admissible && !VO_fn(avel, obs_vels[i], agent_pos, obs_poss[i], C_spaces[i], i);
                if (i == obs_vels.Length - 1)
                {
                    staticadmissible = staticadmissible && !VO_fn(avel, obs_vels[i], agent_pos, obs_poss[i], C_spaces[i], i); //determines static admissibility
                }

            }
            if (staticadmissible && admissible && ((next_pos - goal_pos).magnitude < best_score))
            {
                best_score = (next_pos - goal_pos).magnitude;
                best_vel = avel;
            }

            //This logic will make the arrow point to a "safe" direction although this may not guarantee it is completely safe.
            //There is no logic that reverts an angle back to unsafe. For example, a region may be unsafe at low velocity but safe at high velocity.
            // VO will still point in this direction but if the player moves too slow, then they will get hit.
            if (admissible){safe_angles_dynamic[theta_count - 1] = 1;}
            if (count == 3 && admissible) //high mags -> high threat
            {high_threat_dynamic[theta_count - 1] = 1;}
            else if ((count == 2) && admissible) //med mags -> med threat
            {med_threat_dynamic[theta_count - 1] = 1;}
            else if (count == 1 && admissible) //low mags -> low threat
            {low_threat_dynamic[theta_count - 1] = 1;}
            count++;

            // print(best_vel.magnitude);
            // }
            if (staticadmissible) {safe_angles_static[theta_count - 1] = 1;                //count++;
            }
        }
        safe_angles = new float[60];
        for (int i = 0; i < safe_angles.Length; i++) //unionize dynamic and static arrays into one array
        {
            if (safe_angles_static[i] == 1 && safe_angles_dynamic[i] == 1)
            {safe_angles[i] = 1;}
        }

        for (int i = 0; i < safe_angles.Length; i++)
        {
            if (high_threat_dynamic[i] == 1)
            {high_threat[i] = 1;}
        }

        for (int i = 0; i < safe_angles.Length; i++)
        {
            if (med_threat_dynamic[i] == 1)
            {med_threat[i] = 1;}
        }
        for (int i = 0; i < safe_angles.Length; i++)
        {
            if (low_threat_dynamic[i] == 1)
            {low_threat[i] = 1;}
        }




        continuous_state_update = GameObject.Find("MainMenu").GetComponent<MainMenu>().continuous_state_update;

        if (continuous_state_update == true) {
            if (player_velx_avg == 0) {
                player_vel_ang = 0;
            }
            else {
                player_vel_ang = AngleWrapCorrection(Mathf.Atan(player_vely_avg /player_velx_avg)*Mathf.Rad2Deg,player_velx_avg, player_vely_avg );
            }
            // print(" -------            Level Potential");

            // ----------------------------------- DETERMINE GOAL DIRECTION  -----------------------------------
            output_angle_Goal = GameObject.Find("Perceptual Cues").GetComponent<Tactile>().output_angle_Goal;
            if (output_angle_Goal < 0 && output_angle_Goal >= -360) {output_angle_Goal = output_angle_Goal +360;}
            else if (output_angle_Goal >= 0 && output_angle_Goal <= 360) {output_angle_Goal = output_angle_Goal;}
            else{print("                            ERROR: ISSUE WITH GOAL CALC");}
        }


        // ----------------------------------- DEFINE DIFFERENT SAFETY REGIONS -----------------------------------
        float min = safe_angles.Min();
        float max = safe_angles.Max();
        //float maxStatic = safe_angles_static.Max();
        //float maxDynamic = safe_angles_dynamic.Max();
        // Dummy values for different variables
        float best_value = 10000;
        float selected_angle =  360000;
        int selected_region =  50;
        int selected_region_goal = 51;
        int selected_region_avel = 52;
        // float local_best_value = 10000;
        int min_safety_arc_upper = GameObject.Find("MainMenu").GetComponent<MainMenu>().min_safety_arc_upper;
        int min_safety_arc_lower= GameObject.Find("MainMenu").GetComponent<MainMenu>().min_safety_arc_lower;


        Dictionary<int, List<float>> regions_idxs_dict = new Dictionary<int, List<float>>(); //(region#, list of indexes)
        Dictionary<int, List<float>> regions_vals_dict = new Dictionary<int, List<float>>(); //(region#, list of safe/unsafe)
        List<float> region_list_idx = new List<float>();//temporary
        List<float> region_list_val = new List<float>();


        if (min == max)
        {
            // if the area is full of safe angles then you point to goal. if full of danger and essentially guarenteed to get hit, point to goal
            all_clear = true;
            inst = output_angle_Goal;
            min_safety_arc = min_safety_arc_upper;

            if (continuous_state_update == false) {
                if (player_velx_avg == 0) {
                    player_vel_ang = 0;
                }
                else {
                    player_vel_ang = AngleWrapCorrection(Mathf.Atan(player_vely_avg /player_velx_avg)*Mathf.Rad2Deg,player_velx_avg, player_vely_avg );
                }
                print(" -------            Level Potential");

                // ----------------------------------- DETERMINE GOAL DIRECTION  -----------------------------------
                output_angle_Goal = GameObject.Find("Perceptual Cues").GetComponent<Tactile>().output_angle_Goal;
                if (output_angle_Goal < 0 && output_angle_Goal >= -360) {output_angle_Goal = output_angle_Goal +360;}
                else if (output_angle_Goal >= 0 && output_angle_Goal <= 360) {output_angle_Goal = output_angle_Goal;}
                else{print("                            ERROR: ISSUE WITH GOAL CALC");}
            }

        }
        else
        {
            print(" -------            Potential Gradient Detected ");
            // print("Goal Vector: " + output_angle_Goal);
            all_clear = false;
            // Loop through and compare current value to previous and build out the region locations (idx) and values in two seperate dictionaries
            // inst = player_vel_ang;
            float prev_state = safe_angles[0];
            int region_num = 0;
            for (int idx = 1; idx < safe_angles.Length + 1; idx++)
            { // goes from 0 to 59 (1 is on left and goes CCW to 60)
                float curr_state = safe_angles[idx - 1]; // current state
                if (curr_state == prev_state)
                {// check if previous element = current, if so add to list
                    region_list_idx.Add(idx); //add id to the region
                    region_list_val.Add(curr_state);
                    if (idx == 60)
                    {
                        regions_idxs_dict.Add(region_num, new List<float>(region_list_idx));
                        regions_vals_dict.Add(region_num, new List<float>(region_list_val));
                    }
                }
                else
                {
                    regions_idxs_dict.Add(region_num, new List<float>(region_list_idx));
                    regions_vals_dict.Add(region_num, new List<float>(region_list_val));
                    region_list_idx.Clear(); // get rid of idxs values in current state.
                    region_list_val.Clear(); // get rid of vals values in current state.
                    region_list_idx.Add(idx); //add idxs to the region
                    region_list_val.Add(curr_state); //add vals to the region
                    region_num = region_num + 1;

                }
                prev_state = curr_state;
            }

            // Above now we have regions created for 0-59 elements (corresponded to 1-60 Walls_SR).
            //In many cases we need to combine the first and last regions becasue they have the same value.
                // below we do this concatination
            // Join last region to first if they match!
            List<float> region_list_idx_first = new List<float>();
            List<float> region_list_idx_last = new List<float>();

            if (safe_angles[0] == safe_angles[59])
            {
                // print("concatination of first and last"); // DEBUG: incorrect combining of regions at one point
                region_list_idx_first = regions_idxs_dict[0]; // define
                region_list_idx_last = regions_idxs_dict[regions_idxs_dict.Count - 1]; // define
                region_list_idx_last.AddRange(region_list_idx_first); // Add both together
                regions_idxs_dict[0] = new List<float>(region_list_idx_last); // Reassign merged list to first
                regions_idxs_dict.Remove(region_num);
                region_num = region_num - 1;  // number of regions falls by 1

            }

            // Assigning the regions values: 1 means safe, 0 is unsafe.
            Dictionary<int, float> regions_val_dict = new Dictionary<int, float>(); //(region#, list of indexes)
            for (int i = 0; i < regions_idxs_dict.Count; i++)
            {
                // foreach(int val in regions_idxs_dict[i]){print("IDX: " + val);}
                if (regions_vals_dict[i].Sum() > 0) { regions_val_dict.Add(i, 1); }
                else { regions_val_dict.Add(i, 0); }
            }

            // // // DEBUG UNCOMMENT BELOW to print out the regions and their IDs as well as their values (1 means safe, 0 is unsafe.)
            print("<<<<<<<<<<<<<   NEW LOOP   >>>>>>>>>>>>");
            // Print Regions List
            print("The number of identified regions is " + regions_idxs_dict.Count);
            // for (int i = 0; i<regions_idxs_dict.Count; i++) {
            //     print("-------------Region: " + i);
            //     foreach(int val in regions_idxs_dict[i]){print("IDX: " + val);}
            // }
            //
            // // Print Regions Value Assignment
            // print("   -------- ");
            for (int i = 0; i<regions_val_dict.Count; i++) {print("Region: " + i + "  Val: " + regions_val_dict[i]);}




            // ----------------------------------- DEFINE BEST DIRECTION  -----------------------------------

            // With the VO output complete. We now add intelligence in selecting the safest direction torward the goal.
            // First we need to check what directions/regions satisfy the min arc Distance
            Dictionary<int, float> regions_safe_starts = new Dictionary<int, float>();
            Dictionary<int, float> regions_safe_ends= new Dictionary<int, float>();
            List<int> relevant_region_list = new List<int>(); // Track the relevant regions to loop through them
            int max_safe_size = 0;

            for (int i = 0; i<regions_idxs_dict.Count; i++) {

                // DEBUG: see what regions are safe and unsafe and their sizes.
                // print("-------------Region: " + i);
                // // foreach(int val in regions_idxs_dict[i]){print("IDX: " + val);}
                // print("Region Size: " + regions_idxs_dict[i].Count);
                // print("Region Size (deg): " + regions_idxs_dict[i].Count * 6);
                // print("Safe (1) Unsafe (0): " + regions_val_dict[i]);

                if (regions_val_dict[i] == 1) {
                    if (regions_idxs_dict[i].Count * 6 >= min_safety_arc) {
                        // print("safe enough");
                        // print("number of elements : " + regions_idxs_dict[i].Count);
                        int extra_elements = ((min_safety_arc-6)/2)/6;
                        int element_counter = 0;
                        foreach(int val in regions_idxs_dict[i]){
                            // print("IDX: " + val);
                            if (element_counter == extra_elements ) {
                                int start_element = val;
                                // print("start IDX: " + start_element);
                                regions_safe_starts.Add(i, start_element);
                                relevant_region_list.Add(i);
                            }
                            if (element_counter == (regions_idxs_dict[i].Count - (extra_elements+1))) {
                                int end_element = val;
                                // print("end IDX: " + end_element);
                                regions_safe_ends.Add(i, end_element);
                            }
                            element_counter = element_counter + 1;
                        }
                    }
                    // else {
                    //     print("not safe enough");
                    // }

                    if  (regions_idxs_dict[i].Count > max_safe_size) {
                        max_safe_size = regions_idxs_dict[i].Count;
                        // print("Max size safe arc Updated: " + max_safe_size);
                    }
                }
            }// end of  defining starts and ends


            if (max_safe_size >= min_safety_arc/6) {
                safe_above_min_arc = true;
            }
            else {
                safe_above_min_arc = false;
                // print("No safe regions");
            }


            // // DEBUG: see what regions are safe and unsafe and their sizes.
            List<int> point_to_goal_list = new List<int>(); // Track the relevant regions to loop through them
            List<int> point_to_avel_list = new List<int>(); // Track the relevant regions to loop through them


            for (int o = 0; o < 2; o++) {
                if (o == 0) {
                    objective_angle =  output_angle_Goal;
                    // print("checking goal angle: "  + objective_angle);
                }
                else {
                    objective_angle =  player_vel_ang;
                    // print("checking player angle: " + objective_angle);
                }

                for (int i = 0; i < relevant_region_list.Count; i++) {
                    float region_start_angle = regions_safe_starts[relevant_region_list[i]]*6;
                    float region_end_angle = regions_safe_ends[relevant_region_list[i]]*6;
                    // print("Region: "+ relevant_region_list[i] +"  Start Angle: " + region_start_angle  + "    End Angle: " + region_end_angle + "   goal: " + output_angle_Goal);
                    if (region_end_angle < region_start_angle) {region_end_angle = region_end_angle + 360;}  // DEBUG this could be where the 500 comes up!
                    if (region_start_angle <= objective_angle  && objective_angle  <= region_end_angle) {
                        if (o == 0) {
                            // print("       goal angle within this safe region      ^^");
                            point_to_goal_list.Add(1);
                            selected_region_goal = relevant_region_list[i];
                        }
                        else {
                            // print("       avel angle within this safe region      ^^");
                            point_to_avel_list.Add(1);
                            selected_region_avel = relevant_region_list[i];
                        }
                    }
                    else {

                        if (o == 0) {
                            // print("       goal angle is outside this safe region      ^^");
                            point_to_goal_list.Add(0);
                        }
                        else {
                            // print("       avel angle is outside this safe region      ^^");
                            point_to_avel_list.Add(0);
                        }
                    }
                }

                if (o == 0) {sum_check = point_to_goal_list.Sum(); selected_region = selected_region_goal; }
                if (o == 1) {sum_check = point_to_avel_list.Sum(); selected_region = selected_region_avel; }


                // print("total sum: " + sum_check);
                // if (sum_check == 1 ||  sum_check == 0) {  // DEBUG Not sure if this sum_check is suppose to be here
                    // print("iter: " + o + "    sum_check: " + sum_check);
                // }



                // In the case where the objevtive is in a dangerous region, we use the endpoints and find the closest one and just point to that
                if (sum_check == 1){ // Inside a safe region so display best angle
                    selected_angle = objective_angle;
                    selected_region = selected_region;
                }
                else if (sum_check == 0){
                    // print("No direct options, look at closest safe boundary");
                    // print(relevant_region_list.Count);
                    float min_diff = 3600; // initialize the min difference
                    if (relevant_region_list.Count == 0) {selected_angle = objective_angle ;}
                    for (int i = 0; i < relevant_region_list.Count; i++) {
                        float region_start_angle = regions_safe_starts[relevant_region_list[i]]*6;
                        float region_end_angle = regions_safe_ends[relevant_region_list[i]]*6;
                        if (region_end_angle < region_start_angle) {region_end_angle = region_end_angle + 360;} // wraps
                        // print("Region: "+ relevant_region_list[i] +"  Start Angle: " + region_start_angle  + "    End Angle: " + region_end_angle + "   goal: " + output_angle_Goal);
                        // print("Diff between start and end : "+Mathf.Abs(Mathf.DeltaAngle(region_start_angle,region_end_angle)));

                        float start_diff = Mathf.Abs(Mathf.DeltaAngle(region_start_angle, objective_angle));
                        if (min_diff > start_diff) {
                            min_diff = start_diff;
                            selected_angle = region_start_angle-6;
                            selected_region = relevant_region_list[i];
                        }
                        float end_diff = Mathf.Abs(Mathf.DeltaAngle(region_end_angle, objective_angle));
                        if (min_diff > end_diff) {
                            min_diff = end_diff;
                            selected_angle = region_end_angle-6;
                            selected_region = relevant_region_list[i];
                        }

                        // print(">>>>>> selected region: " + relevant_region_list[i]);
                        // print(">>>>>> selected region size: " + regions_idxs_dict[relevant_region_list[i]].Count);
                        if (regions_idxs_dict[relevant_region_list[i]].Count * 6 <= min_safety_arc )  {// size of chosen region
                            if (min_safety_arc > min_safety_arc_lower) {
                                min_safety_arc = min_safety_arc-12;
                                // print("Min Arc Reduced to : " + min_safety_arc);
                            }
                        }
                        // if (Mathf.Abs(Mathf.DeltaAngle(region_start_angle,region_end_angle)) < 6) {
                        //     // selected_angle = objective_angle ;
                        //     print("lets go to goal");
                        // }
                    }

                }
                // else {print("                                                     ERROR: Sum check multiple regions best?");}


                if (o == 0) {
                    obj_goal = selected_angle;
                    // print( "Goal Angle: " + selected_angle);
                }
                if (o == 1) {
                    obj_avel = selected_angle;
                    // print( "Avel Angle: " + selected_angle);
                }


            }

            // check difference between the suggestions, If they are within a windown, then suggest goal, if too different stick with velocity
            float obj_diff = Mathf.Abs(Mathf.DeltaAngle(obj_goal, player_vel_ang));

            // print("Goal Suggested Direction: " + obj_goal);
            // print("AVel Direction: " +  player_vel_ang);
            // print("Difference: " + obj_diff);
            // print("Difference Max: " +  obj_max_diff);


            if (obj_diff <= obj_max_diff) { // DEBUG IS THIS EVEN TRue
                inst = obj_goal;
                // print("goal preference");
            }
            else if (safe_above_min_arc == true) {
                inst = obj_avel;
                // print("avel preference (above min arc)");

                if (min_safety_arc == min_safety_arc_lower) {
                    if (Mathf.Abs(Mathf.DeltaAngle(inst, output_angle_VO_prev)) >= 2*min_safety_arc_lower) {
                        // print("Suggested Direction too difficult to switch directions so just point to goal");
                        // print("Old: " +  output_angle_VO_prev + "     New: " +  inst);
                        inst = obj_goal;
                    }
                }
            }
            else {
                inst = obj_goal;
                // print("goal preference (below min arc)");
            }




            // inst = selected_angle;
            // inst = AngleWrapCorrection(Mathf.Atan(best_vel[2]/best_vel[0])*Mathf.Rad2Deg,best_vel[0],best_vel[2]);

        }


        output_angle_VO = inst;
        // print("Curr Output: " + output_angle_VO);
        // print("Prev Output: " + output_angle_VO_prev)

        output_angle_VO_prev = output_angle_VO; // ensure this line is below the others



        // Grabbing mode and activating correct direction
        v_mode = GameObject.Find("MainMenu").GetComponent<MainMenu>().v_mode;
        GameObject arrow = GameObject.Find("Assistance Arrow");




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (v_mode == "V_VO")
        {
            SpriteRenderer arrow_obj = GameObject.Find("arrow").GetComponent<SpriteRenderer>();
            SpriteRenderer box_obj = GameObject.Find("box").GetComponent<SpriteRenderer>();

            arrow.SetActive(true);
            Transform arrow_transform = arrow.GetComponent<Transform>();

            if (all_clear == false) {
                box_obj.color = Color.red;
                arrow_obj.color = Color.red;
            }
            else  {
                box_obj.color = Color.blue;
                arrow_obj.color = Color.blue;
            }
            //arrow_transform.eulerAngles = new Vector3(90, -output_angle_VO + 90, 0);
            /*if (!maskAllRed) //point to goal if every angle is unsafe/red. For some reason, buffer doesn't work when pointing to goal when angles are all red.
            {
                //print("Pointed at goal; all angles red" + maskAllRed);
                arrow_transform.eulerAngles = new Vector3(90, -output_angle_Goal + 90, 0);
            } else
            {
                arrow_transform.eulerAngles = new Vector3(90, -output_angle_VO + 90, 0);
            }*/
            arrow_transform.eulerAngles = new Vector3(90, -output_angle_VO + 90, 0);

        }
        if (v_mode == "V_Goal")
        {
            arrow.SetActive(true);
            Transform arrow_transform = arrow.GetComponent<Transform>();
            arrow_transform.eulerAngles = new Vector3(90, -output_angle_Goal + 90, 0);
        }
        if (v_mode == "V_none" && arrow != null)
        {
            arrow.SetActive(false);
        }
        newPlayerAngle = new Vector3(playerObj.transform.forward.x, 0f, playerObj.transform.forward.z);

    }

































    private bool VO_fn(Vector3 avel, Vector3 ovel, Vector3 apos, Vector3 opos, float crad, int i)
    {
        //string wall_name = "Wall_SR " + string.Format("{0:0}", theta_count-1);
        //#parameters:
        //#avel: agent velocity <Vector3>
        //#ovel: obstacle velocity <Vector3>
        //#apos: agent position <Vector3>
        //#opos: obstacle position <Vector3>
        //#crad: C-space radius <float>

        //returns true if VO is NOT admissible (velocity lies within the VO)

        Vector3 rel_vel = avel - ovel;
        Vector3 rel_pos = opos - apos;
        //print("rel_pos: " + rel_pos + " rel_vel: " + rel_vel);
        //print("RelPosVec: " + rel_pos);
        relativePos = rel_pos;
        GameObject agent = GameObject.Find("Player");
        Vector3 actual_rel_vel = new Vector3(1000, 0, 1000);
        if (i == 3)
        {
            actual_rel_vel = agent.GetComponent<Rigidbody>().velocity - ovel;
        }


        float th_cone = Mathf.Asin(crad / rel_pos.magnitude);
        float th = Mathf.Acos(Vector3.Dot(rel_pos, rel_vel) / (rel_pos.magnitude * rel_vel.magnitude));
        //print("obs: " + i + " th_cone: " + th_cone + " th: " + th + " relvel: " + rel_vel);

        //print("VelMag: " + velocity.magnitude);
        //print("vel: " + velocity.magnitude);
        if (i == 3 && (velocity.magnitude <= 0.2f)) //if player is standing still, it is always admissible from static obs
        {
            staticArc = th_cone;
            isNotMoving = true;
            return false;
        }
        else if (i == 3)
        {
            if (Vector3.Dot(rel_pos, velocity) <= 0.1f) //if player is moving away from static obs, it is always admissibel for static array.
            {
                isMovingAway = true;
                return false;
            }

        }
        if ((float.IsNaN(th_cone) || float.IsNaN(th)) && i != 3)
        {
            return true;
        }
        //print("Th<cone: " + (th < th_cone));
        return th < th_cone; //rest of VO calc
    }

    float AngleWrapCorrection(float a, float x, float y)
    {
        if (x >= 0 && y >= 0) { a_g = a; }
        else if (x < 0 && y >= 0) { a_g = a + 180; }
        else if (x >= 0 && y < 0) { a_g = a + 360; }
        else if (x < 0 && y < 0) { a_g = a + 180; }
        else { print("AngleWrapCorrection error!"); }
        return a_g;
    }

    float Rad2Deg(float radians)
    {
        return (180 / Mathf.PI) * radians;
    }
    public float[] getLowThreat()
    {
        return low_threat;
    }
    public float[] getMedThreat()
    {
        return med_threat;
    }
    public float[] getHighThreat()
    {
        return high_threat;
    }
    public bool getIsMovingAway()
    {
        return isMovingAway;
    }
    public bool getIsNotMoving()
    {
        return isNotMoving;
    }
    public float getStaticArc()
    {
        return staticArc;
    }
    public Vector3 getRelPos()
    {
        return new Vector3(relativePos.x, 0, relativePos.z);
    }
    public float getThreatSpace()
    {
        return threat_space;
    }
    public float[] calculateStaticArc(Vector3 relPos, float sArc) //This function reveals location of static threat even when person is standing still.
    {
        Vector3 xVec = new Vector3(1, 0, 0);
        //Vector3 zVec = new Vector3(0, 0, 1);
        float[] angles_not_moving = new float[60];
        float relPosMag = relPos.magnitude;
        //GameObject playerCam = GameObject.Find("Camera");
        //float camRotation = playerCam.transform.eulerAngles.y;
        //print("Camerarotation: " + camRotation);

        //Vector3 cameraForward = playerCam.transform.forward;
        //Vector3 oldDir = cameraForward;

        float deltaCamangle = Vector3.SignedAngle(newPlayerAngle, prevPlayerAngle, Vector3.up);
        /*if (deltaCamangle < 0.1)
        {
            deltaCamangle = 0;
        }*/
        //print("deltaCam: " + deltaCamangle);
        //GameObject wallsOuter = GameObject.Find("Walls_SR_Outer");
        //float wallsOuterRounded = wallsOuter.transform.eulerAngles.z;//Mathf.Round(wallsOuter.transform.eulerAngles.z);
        //float newZRotate = wallsOuterRounded - deltaCamangle;
        //print("wallsOuter: " + wallsOuterRounded);
        //wallsOuter.transform.localEulerAngles = new Vector3(0f, 0f, newZRotate);
        wallsOuter.transform.Rotate(0, 0, -deltaCamangle, Space.Self);// = new Vector3(0f, 0f, newZRotate);
        wallsInner.transform.Rotate(0, 0, -deltaCamangle, Space.Self);
        //int shiftBlocks = (int)(Mathf.Round(deltaCamangle / 6));
        //print("deltaCam: " + deltaCamangle);
        //print("Cameraforward: " + cameraForward);
        // find theta start position
        //print("RelativePos x: " + relPos.x);
        float totalAngle = Vector3.SignedAngle(xVec, relPos, -Vector3.up); //calculates relPos angle from positive x direction
        //print("Total angle: " + totalAngle);
        if (totalAngle < 0)
        {
            totalAngle = 180 + (180 - Mathf.Abs(totalAngle)); //converts negative angles to value between 180 and 360.
        }
        //print("totalAngle: " + totalAngle);
        float startThetaDeg = totalAngle - Rad2Deg(sArc); //gets start of angle that indiciates static threat
        float checkSign = startThetaDeg; //accounts for overflow condition
        if (startThetaDeg < 0)
        {
            //startThetaDeg = -startThetaDeg;
            startThetaDeg = 360 + startThetaDeg;
        }

        int blockDeg = 360 / angles_not_moving.Length;
        //print("start: " + startThetaDeg);
        int numBlocks = (int)(startThetaDeg / blockDeg); //where 6 represents the # of degrees of each block of the array. Start for loop here (+1) for index.

        float sArcDeg = Rad2Deg(sArc);
        int numArcBlocks = (int)((2 * sArcDeg) / blockDeg);
        //numBlocks -= (numArcBlocks/2);
        int[] angleWrapArr = new int[60]; //this array contains rewrapped indices as values
        for (int i = 0; i < angleWrapArr.Length; i++) //Performs angle wrapping so that the first index of the array is the start of static obs region.
        {
            if (i +  numBlocks+1 < 60)
            {
                angleWrapArr[i] = i  + numBlocks + 1; //start angle
            } else
            {
                angleWrapArr[i] = (i + numBlocks + 1) - angles_not_moving.Length;
            }
        }
        //print("anglewrap: startwrap");
        /*foreach (int element in angleWrapArr)
        {
            print("anglewrap: " + element);
        }*/
        /*for (int i = 0; i < angleWrapArr.Length; i++)
        {
            print("Wrap Index: " + angleWrapArr[i] + "StartTheta: " + startThetaDeg + "ArcDeg: " + Rad2Deg(sArc));
        }*/

        //int numBlocks = (int)(startThetaDeg / blockDeg); //where 6 represents the # of degrees of each block of the array. Start for loop here (-1) for index

        //float sArcDeg = Rad2Deg(sArc);
        //int numArcBlocks = (int)((2 * sArcDeg) / blockDeg);
        //print("starttheta: " + startThetaDeg);
        //if (checkSign > 0)
        //{
            for (int i = 0; i < numArcBlocks; i++)
            {
                int getAngleIndex = angleWrapArr[i];
                angles_not_moving[getAngleIndex] = 1;

            }
        /*} else if (totalAngle >= 180)//addresses case where static obs arc wraps around array.
        {
            int startIndex = angleWrapArr[0];
            angles_not_moving[startIndex] = 1;
            /*for (int i = 0; i < numArcBlocks/2; i++)
            {
                int getAngleIndex = angleWrapArr[i];
                angles_not_moving[getAngleIndex] = 1;

            }
            for (int i = angles_not_moving.Length - 1; i >= angles_not_moving.Length - 1 - numArcBlocks; i--)
            {
                int getAngleIndex = angleWrapArr[i];
                angles_not_moving[getAngleIndex] = 1;
            }
        } else if (startThetaDeg < 180)
        {
            for (int i = 0; i < numArcBlocks; i++)
            {
                int getAngleIndex = angleWrapArr[i];
                angles_not_moving[getAngleIndex] = 1;
            }
        }*/

        //print("End Wrap");


            /*else //if ((numBlocks + numArcBlocks) <= angles_not_moving.Length)
        {
            startThetaDeg = -startThetaDeg;
            int numBlocks = (int)(startThetaDeg / blockDeg); //where 6 represents the # of degrees of each block of the array. Start for loop here (-1) for index

            float sArcDeg = Rad2Deg(sArc);
            int numArcBlocks = (int)((2 * sArcDeg) / blockDeg);
            int halfArcBlocks = numArcBlocks / 2;
            //print("StartBlocks: " + numBlocks);
            //print("EndBlock: " + (numBlocks + halfArcBlocks));
            for (int index = numBlocks; index < (numBlocks + halfArcBlocks + 3); index++)
            {
                //try
                //{
                    angles_not_moving[index] = 1;
                /*} catch{
                    print("Index out of bounds first loop: " + index);
                }
            }
            for (int index = angles_not_moving.Length - 1; index >= angles_not_moving.Length-halfArcBlocks - 3; index--)
            {
                //try
                //{
                    //angles_not_moving[index] = 1;
                /*} catch{
                    print("Index out of bounds second loop: " + index);
                }
                angles_not_moving[index] = 1;
            }
        }*/


        return angles_not_moving;




    }
}
