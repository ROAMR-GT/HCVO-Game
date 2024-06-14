using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VO_visualization : MonoBehaviour
{

    private float[] safe_angles_dynamic;
    private float[] safe_angles_static;
    private float[] safe_angles_not_moving;
    private float[] toggle_angle = new float[60];
    private bool isNotMoving;
    private float staticArc;
    private Vector3 relPos;
    GameObject PerceptualCues;
    //private float[] safe_angles_dynamic;


    // Start is called before the first frame update
    void Start()
    {
        while (PerceptualCues == null)
        {
            PerceptualCues = GameObject.Find("Perceptual Cues");
        }
        //safe_angles_dynamic = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_dynamic;
        //safe_angles_static = GameObject.Find("Perceptual Cues").GetComponent<VO>().safe_angles_static;

    }

    // Update is called once per frame
    void Update()
    {
        isNotMoving = /*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().getIsNotMoving();
        safe_angles_dynamic = /*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().safe_angles_dynamic;
        safe_angles_static = /*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().safe_angles_static;
        staticArc = /*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().getStaticArc(); //Getting NaN values
        Vector3 relPos = /*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().getRelPos();
        //print("Relative pos: " + relPos + " arc: " + staticArc);
        safe_angles_not_moving = /*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().calculateStaticArc(relPos, staticArc);
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
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
            else
            {
                //print("Done: " + wall_name);
                GameObject.Find(wall_name).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
            Color invisible = new Color(1, 1, 1, 0f);
            if (GameObject.Find("StaticObstacle") == null || !GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle)
            {
                invisible = new Color(1, 1, 1, 0f);
                GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = invisible;
            }
            //MODIFIES OUTER RING
            //Make angle invisible if player is moving away from static threat. Make angle white if player is standing still and there
            //exists a static threat in that direction. Make angle clear if player is moving towards static threat
            if (safe_angles_static[angle] == 0 && GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle) //modify outer ring based on admissibility of static threat
            {
                invisible = new Color(1, 1, 1, 0f);
                /*for (int i = 0; i < safe_angles_dynamic.Length; i++)
                {
                    string outer = "Wall_SR_Outer " + string.Format("{0:0}", (i + 1));
                    if (GameObject.Find(outer).GetComponent<Renderer>().material.color == Color.white)
                    {
                        //GameObject.Find(outer).GetComponent<Renderer>().material.color = invisible;
                        GameObject.Find(outer).GetComponent<Renderer>().material.color = Color.clear;

                    }

                }*/
                if (GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color == Color.white && GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle)
                {
                    GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = invisible;
                    //GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = Color.clear;
                }
                //else
                //{
                    GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.clear);
                //}
            }
            else if (/*GameObject.Find("Perceptual Cues")*/PerceptualCues.GetComponent<VO>().getIsMovingAway() && GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle)
            {
                invisible = new Color(1, 1, 1, 0f);
                GameObject.Find(wall_name_outer).GetComponent<SpriteRenderer>().material.color = invisible;
            }
            else if (safe_angles_not_moving[angle] == 1 && isNotMoving && GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle)
            {
                invisible = new Color(1, 1, 1, 0f);
                /*for (int i = 0; i < safe_angles_dynamic.Length; i++)
                {
                    string outer = "Wall_SR_Outer " + string.Format("{0:0}", (i + 1));
                    if (GameObject.Find(outer).GetComponent<Renderer>().material.color == Color.clear)
                    {
                        //GameObject.Find(outer).GetComponent<Renderer>().material.color = invisible;
                        GameObject.Find(outer).GetComponent<Renderer>().material.color = Color.white;
                    }

                }*/
                if (GameObject.Find(wall_name_outer).GetComponent<Renderer>().material.color == Color.clear && GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle)
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
                string wall_name_outer = "Wall_SR_Outer " + string.Format("{0:0}", (i+1));
                GameObject.Find(wall_name_outer).GetComponent<Renderer>().material.color = invisible;
            }
        }

    }
}
