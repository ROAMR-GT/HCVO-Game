using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public Text DispText1;
    public float max_health = 400; //400 for good pizel size
    public float target_health = 400;
    public float curr_health = 400;
    public float starting_health = 0;//-200;
    public int max_failed_escapes;
    public int failed_escapes;
    public float health_reduction;
    public bool dead = false;
    public float score;
    public float score_increase;
    public GameObject heart;
    public GameObject heart_big;
    RectTransform health_bar;


    // Start is called before the first frame update
    void Start()
    {
        max_failed_escapes = GameObject.Find("MainMenu").GetComponent<MainMenu>().max_failed_escapes;
        health_reduction = max_health/max_failed_escapes;
        health_bar = GameObject.Find("Front Fill").GetComponent<RectTransform>();

        heart = GameObject.Find("HealthHeart");
        heart_big = GameObject.Find("HealthHeart_big");

    }

    // Update is called once per frame
    void Update()
    {

        failed_escapes = GameObject.Find("HitDetectedText").GetComponent<HitDetectedTextBehavior>().num_failed_escapes;
        score = 4 * GameObject.Find("Score").GetComponent<Scoring>().score;

        if (target_health < 0) {
            dead = true;
            target_health = 0;
        }
        else if (target_health >= max_health) {
            target_health = max_health;
            score_increase = Mathf.Round(score_increase); // ensure the score value doesnt cause over max health
        }
        else {
            dead = false;
            score_increase += score;
        }

        target_health  = max_health  +  starting_health  +  score_increase  -  failed_escapes*health_reduction ;
        if (target_health > max_health) {target_health = max_health;}

        // print("Current: " + target_health +  "    max: " + max_health +  "    score_increase: " + score_increase  +  "    penalty: " + failed_escapes * health_reduction);
        DispText1.text = " " + (target_health/4).ToString("0.0");

        // Have values slide
        var color = heart.GetComponent<Image>().color;
        if (target_health < curr_health) {
            curr_health -= 1.0f;
            color.a = 1.0f; //0.2f;
            heart.GetComponent<Image>().color = color;
        }
        else if (target_health > curr_health) {
            curr_health = target_health;
            color.a = 1.0f;
            heart.GetComponent<Image>().color = color;
        }
        else {
            color.a = 1.0f; //0.55f;
            heart.GetComponent<Image>().color = color;
        }
        health_bar.SetRight(max_health - curr_health); // will need to define a curr_health that is changing and sliding - inverse of cur_hurt
    }


}


public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
