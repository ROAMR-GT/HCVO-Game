using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitDetectedTextBehavior : MonoBehaviour
{
    public string displayText = "Hit Detected!";
    public bool hitDetected;
    public float maxNotificationDuration = 0.5f;
    public bool just_hit = true;
    public int num_failed_escapes;
    public int hit_notification;
    public GameObject hitflash;
    public float alpha;
    public bool beinghit;

    private Text text;
    private float hitCountdown = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
        text.text = "";
        hitDetected = false;
        hit_notification = 0;
        hitflash = GameObject.Find("HitFlash");
        beinghit = false;
    }

    // Update is called once per frame
    void Update()
    {
        // print(beinghit);
        var color = hitflash.GetComponent<Image>().color;

        if (hitDetected)
        {
            hitDetected = false;
            text.text = displayText;
            hit_notification = 1;
            hitCountdown = maxNotificationDuration;

            color.a = 0.25f;

        }

        if(color.a > 0) {
            color.a -= 0.002f;
        }
        hitflash.GetComponent<Image>().color = color;

        if (hitCountdown > 0)
        {
            beinghit = true;
            hitCountdown -= Time.deltaTime;
            if (just_hit == true)
            {
                num_failed_escapes += 1;
                just_hit = false;
            }
        }
        else
        {
            beinghit = false;
            text.text = "";
            hit_notification = 0;
            just_hit = true;
        }
    }
}
