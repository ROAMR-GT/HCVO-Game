using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDBehavior : MonoBehaviour
{
    // instance data
    public float maxNotificationDuration = 3.0f;
    public bool hitDetected = false;

    private GameObject canvasGO;
    private Text hitDetectedText;
    private Text targetAreaText;
    private Vector3 targetPos;
    private Vector3 playerPos;
    private float hitCountdown = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        canvasGO = GameObject.Find("HUD");
        // Debug.Log(canvasGO.name);

        // set up hit detected text
        GameObject hitDetectedTextGO = new GameObject();
        hitDetectedTextGO.transform.parent = canvasGO.transform;
        hitDetectedTextGO.name = "Hit Detected Text fake";

        hitDetectedTextGO.AddComponent<Text>();
        hitDetectedText = hitDetectedTextGO.GetComponent<Text>();
        hitDetectedText.text = "";
        hitDetectedText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        hitDetectedText.color = Color.black;
        hitDetectedText.fontSize = 30;
        // hitDetectedText.minWidth = 200;
        // hitDetectedText.preferredWidth = 400;

        RectTransform rectTransform;
        rectTransform = hitDetectedText.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 200, 0);
        rectTransform.localScale = new Vector3(1, 1, 1);

        ContentSizeFitter csf = hitDetectedTextGO.AddComponent(typeof(ContentSizeFitter)) as ContentSizeFitter;
        // csf


        /*
        // set up text that tells distance to target area
        GameObject targetAreaTextGO = new GameObject();
        targetAreaTextGO.transform.parent = canvasGO.transform;
        targetAreaTextGO.name = "Target Area Text";

        targetAreaTextGO.AddComponent<Text>();
        targetAreaText = targetAreaTextGO.GetComponent<Text>();
        targetAreaText.text = "dida dee da dee da do do";
        targetAreaText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        targetAreaText.color = Color.black;

        RectTransform rectTransform2;
        rectTransform2 = hitDetectedText.GetComponent<RectTransform>();
        //rectTransform2.localPosition = new Vector3(0, 0, 0);
        rectTransform2.position = new Vector3(0, 0, 0);
        // Debug.Log(targetAreaText.text);
        */
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = GameObject.Find("TargetArea").transform.position;
        playerPos = GameObject.Find("Player").transform.position;

        if (hitDetected)
        {
            hitDetected = false;
            hitDetectedText.text = "Hit Detected";
            hitCountdown = maxNotificationDuration;
        }

        if (hitCountdown > 0)
        {
            hitCountdown -= Time.deltaTime;
        } else
        {
            hitDetectedText.text = "";
        }

        /*
        playerPos = GameObject.Find("Player").transform.position;
        targetPos = GameObject.Find("TargetArea").transform.position;
        float dist = Vector3.Distance(playerPos, targetPos);
        targetAreaText.text = "Distance from Target Area = " + dist;
        */
    }

}
