using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
    public Text DispText1;
    public float score;
    public float goal_dist;
    public float dist_sum;
    public float num_failed_escapes;
    public int numHits;
    private int closeness_status;
    private float multiplier;
    public bool track_score;
    // Summate score of how close you are, time, subtraction of when you get hit
        // zero points for being too far
        //

    // Start is called before the first frame update
    void Start()
    {
        score = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        num_failed_escapes = GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().num_failed_escapes;
        goal_dist = GameObject.Find("DistanceText").GetComponent<DistanceTextBehavior>().distance;
        closeness_status = GameObject.Find("DistanceText").GetComponent<DistanceTextBehavior>().status; // 0 is closest, 3 is worst
        if (closeness_status == 0) {multiplier = 2.0f;}
        if (closeness_status == 1) {multiplier = 1.0f;}
        if (closeness_status == 2) {multiplier = 0.5f;}
        if (closeness_status == 3) {multiplier = 0.0f;}

        track_score  = GameObject.Find("ObstacleFactory").GetComponent<AutoTrial>().track_score;
        if (track_score == true) {dist_sum =  multiplier*Time.deltaTime;}
        else {dist_sum =  0;}

        score = dist_sum + 0*num_failed_escapes;
        // DispText1.text = " " + score.ToString("0.00");
    }

    float scoreFunction()
    {
        return 0.0f;
    }
}
