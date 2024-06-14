using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VO_I_visualization : MonoBehaviour
{

    private float[] safe_angles_dynamic;
    private float[] safe_angles_static;
    private float[] safe_angles_not_moving;
    private float[] toggle_angle = new float[60];
    private bool isNotMoving;
    //private float[] safe_angles_dynamic;


    // Start is called before the first frame update
    void Start()
    {
        safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
        safe_angles_static = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_static;
        //safe_angles_not_moving = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_not_moving;
        //safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
    }

    // Update is called once per frame
    void Update()
    {
        isNotMoving = GameObject.Find("Perceptual Cues").GetComponent<VO>().getIsNotMoving();
        safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
        safe_angles_static = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_static;
        float staticArc = GameObject.Find("Perceptual Cues").GetComponent<VO>().getStaticArc(); //Getting NaN values
        Vector3 relPos = GameObject.Find("Perceptual Cues").GetComponent<VO>().getRelPos();
        //print("Relative pos: " + relPos + " arc: " + staticArc);
        safe_angles_not_moving = GameObject.Find("Perceptual Cues").GetComponent<VO>().calculateStaticArc(relPos, staticArc);
        //safe_angles_not_moving = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_not_moving;
        //safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
        int angle_count = 0;
        // Color walls if direction works fine
        for (int angle = 0; angle < safe_angles_dynamic.Length; angle++)
        {
            angle_count = angle_count + 1;
            string wall_name = "Wall_SR " + string.Format("{0:0}", angle_count);
            string wall_name_outer = "Wall_SR_Outer " + string.Format("{0:0}", angle_count);
            if (safe_angles_dynamic[angle] == 0) //modify inner ring based on admissibility of dynamic threats
            {
                //print(wall_name);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            else
            {
                //print("Done: " + wall_name);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
            Color invisible = new Color(1, 1, 1, 0f);
            if (GameObject.Find("StaticObstacle") == null)
            {
                invisible = new Color(1, 1, 1, 0f);
                GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = invisible;
            }
            //MODIFIES OUTER RING
            //Make angle invisible if player is moving away from static threat. Make angle white if player is standing still and there
            //exists a static threat in that direction. Make angle red if player is moving towards static threat
            if (safe_angles_static[angle] == 0) //modify outer ring based on admissibility of static threat
            {
                invisible = new Color(1, 1, 1, 0f);
                /*for (int i = 0; i < safe_angles_dynamic.Length; i++)
                {
                    string outer = "Wall_SR_Outer " + string.Format("{0:0}", (i + 1));
                    if (GameObject.Find(outer).GetComponent<Renderer>().material.color == Color.white)
                    {
                        //GameObject.Find(outer).GetComponent<Renderer>().material.color = invisible;
                        GameObject.Find(outer).GetComponent<Renderer>().material.color = Color.red;

                    }

                }*/
                if (GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color == Color.white)
                {
                    GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = invisible;
                    //GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = Color.red;
                }
                //else
                //{
                GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
                //}
            }
            else if (GameObject.Find("Perceptual Cues").GetComponent<VO>().getIsMovingAway())
            {
                invisible = new Color(1, 1, 1, 0f);
                GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = invisible;
            }
            else if (safe_angles_not_moving[angle] == 1 && isNotMoving)
            {
                invisible = new Color(1, 1, 1, 0f);
                /*for (int i = 0; i < safe_angles_dynamic.Length; i++)
                {
                    string outer = "Wall_SR_Outer " + string.Format("{0:0}", (i + 1));
                    if (GameObject.Find(outer).GetComponent<Renderer>().material.color == Color.red)
                    {
                        //GameObject.Find(outer).GetComponent<Renderer>().material.color = invisible;
                        GameObject.Find(outer).GetComponent<Renderer>().material.color = Color.white;
                    }

                }*/
                if (GameObject.Find(wall_name_outer).GetComponent<Renderer>().material.color == Color.red)
                {
                    GameObject.Find(wall_name_outer).GetComponent<Renderer>().material.color = invisible;
                    //GameObject.Find(wall_name_outer).GetComponent<Renderer>().material.color = Color.white;
                }
                GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.white);
            }
        }

        if (GameObject.Find("TargetArea").GetComponent<WaypointBehavior>().isChangedPos())
        {
            Color invisible = new Color(1, 1, 1, 0f);
            for (int i = 0; i < safe_angles_dynamic.Length; i++)
            {
                string wall_name_outer = "Wall_SR_Outer " + string.Format("{0:0}", (i + 1));
                GameObject.Find(wall_name_outer).GetComponent<Renderer>().material.color = invisible;
            }
        }

    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VO_I_visualization : MonoBehaviour
{

    private float[] safe_angles;
    private float[] low_threat;
    private float[] med_threat;
    private float[] high_threat;
    //private float[] safe_angles_dynamic;


    // Start is called before the first frame update
    void Start()
    {
        safe_angles = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles;
        low_threat = GameObject.Find("Perceptual Cues").GetComponent<VO>().getLowThreat();
        med_threat = GameObject.Find("Perceptual Cues").GetComponent<VO>().getMedThreat();
        high_threat = GameObject.Find("Perceptual Cues").GetComponent<VO>().getHighThreat();
        //safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
    }

    // Update is called once per frame
    void Update()
    {
        safe_angles = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles;
        low_threat = GameObject.Find("Perceptual Cues").GetComponent<VO>().getLowThreat();
        med_threat = GameObject.Find("Perceptual Cues").GetComponent<VO>().getMedThreat();
        high_threat = GameObject.Find("Perceptual Cues").GetComponent<VO>().getHighThreat();
        //safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
        int angle_count = 0;
        // Color walls if direction works fine
        for (int angle = 0; angle < safe_angles.Length; angle++)
        {
            angle_count = angle_count + 1;
            string wall_name = "Wall_SR " + string.Format("{0:0}", angle_count);
            if (high_threat[angle] == 0 && med_threat[angle] == 0 && low_threat[angle] == 0) //set angle to red if no viable escape velocity
            {
                //print(wall_name);
                //GameObject.Find(wall_name).GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            else if (low_threat[angle] == 0 && high_threat[angle] == 1 && med_threat[angle] == 0) //set angle to yellow if min escape velocity is high
            {
                //medium red
                Color medRed = new Color(1, 0, 0, 0.3f);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                //GameObject.Find(wall_name).GetComponent<Renderer>().material.color = medRed;
            }
            else if (low_threat[angle] == 0 && med_threat[angle] == 1 && high_threat[angle] == 1) //set angle to green if min escape velocity is med
            {
                //low red
                Color lowRed = new Color(1, 0, 0, 0.1f);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                //GameObject.Find(wall_name).GetComponent<Renderer>().material.color = lowRed;
            }
            else //set angle to white if min escape velocity is low.
            {
                //print("Done: " + wall_name);
                //GameObject.Find(wall_name).GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.white);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
        }

    }
}*/
