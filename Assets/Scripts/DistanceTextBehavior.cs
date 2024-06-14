using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DistanceTextBehavior : MonoBehaviour
{
    public string excelText;
    public string goodText = "On Target";
    public string okText = "Close to Target";
    public string poorText = "Far from Target";

    public Color excellentColor;
    public Color goodColor;
    public Color okColor;
    public Color poorColor;

    private float goodCutoff;
    private float excellentCutoff;
    private float okCutoff;

    private float arrowCutoff;
    private float closeCutoff;

    public float distance;
    private GameObject target;
    private GameObject player;
    private GameObject arrow;
    public Text text;
    public int status = 2;
    public int oldStatus = 1;
    Vector3 arrowscale;
    float obj1_vel;
    public bool on_target;
    bool assistance_active;
    private float targetRadius;
    float prev_a;
    int LP_buffer_size;
    float inst;
    public float scaling;
    public float arrow_width;
    public float arrow_height;


    Queue<float> buff = new Queue<float>();

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("TargetArea");
        player = GameObject.Find("[CameraRig]/Player");
        arrow =  GameObject.Find("Assistance Arrow");
        excellentCutoff= GameObject.Find("MainMenu").GetComponent<MainMenu>().excellentCutoff;
        goodCutoff = GameObject.Find("MainMenu").GetComponent<MainMenu>().good_cutoff;
        okCutoff= GameObject.Find("MainMenu").GetComponent<MainMenu>().ok_cutoff;
        // text = gameObject.GetComponent<Text>();
        targetRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area / 2.0f;

        arrowCutoff = GameObject.Find("MainMenu").GetComponent<MainMenu>().arrow_cutoff;
        //closeCutoff = 1.2f*targetRadius;//GameObject.Find("MainMenu").GetComponent<MainMenu>().close_cutoff;
        // text.text = poorText;
        // text.color = poorColor;

        LP_buffer_size = GameObject.Find("MainMenu").GetComponent<MainMenu>().AI_LowPass_buffer_size;
        for (int i = 0; i < LP_buffer_size; i++) { buff.Enqueue(0); }
        prev_a = 0;
        arrow_width = 0.3f;
        arrow_height = 0.2f;
    }

    // Update is called once per frame

    //Two states: in ring or out of ring
    void Update()
    {
        target = GameObject.Find("TargetArea");
        assistance_active = GameObject.Find("Player").GetComponent<Logging>().assistance_active;
        Vector3 p = player.transform.position;
        Vector3 t = target.transform.position;
        p.y = 0;
        t.y = 0;
        distance = (p - t).magnitude;
        //print("MarkDist: " + distance);
        //print("Close: " + (onCutoff * targetRadius));

        if (distance < excellentCutoff)
        {
            oldStatus = status;
            status = 0;
        }
        else if (distance < goodCutoff)
        {
            oldStatus = status;
            status = 1;
        }
        else if (distance < okCutoff)
        {
            oldStatus = status;
            status = 2;
        }
        else
        {
            oldStatus = status;
            status = 3;
        }

        updateText();
        if (assistance_active  == true ) {
            if (GameObject.Find("Obstacle 1") == null && distance < arrowCutoff * targetRadius) { //decouples arrow cutoff from distance text.
                // GameObject.Find("Assistance Arrow").transform.localScale = new Vector3(0,0,0);
                on_target = true;
            }
            else {
                Vector3 best_vel = GameObject.Find("Perceptual Cues").GetComponent<VO>().best_vel;
                float tuneMag = GameObject.Find("MainMenu").GetComponent<MainMenu>().playerMaxVel;
                scaling =   best_vel.magnitude/tuneMag* arrow_height;

                bool use_lp_filter;
                inst = scaling;

                use_lp_filter = true;
                if (use_lp_filter == true) {
                    inst = prev_a - Mathf.DeltaAngle(inst, prev_a);
                    if (inst == inst)
                    {
                        buff.Dequeue();
                        buff.Enqueue(inst);
                    }

                    scaling = buff.Average(); // dont need the conversions!!!!!!!!!!!
                    prev_a = scaling;
                }
                else {
                    scaling = inst;
                }

                GameObject.Find("Assistance Arrow").transform.localScale = new Vector3(arrow_width, arrow_height   , 1); // scaling used to be arrow height
                on_target = false;
            }
        }
    }


    void updateText()
    {
        if (oldStatus != status)
        {
            if (status == 0)
            {
                text.text = excelText;
                text.color = excellentColor;
            } else if (status == 1)
            {
                text.text = goodText;
                text.color = goodColor;
            } else if (status == 2)
            {
                text.text = okText;
                text.color = okColor;
            } else
            {
                text.text = poorText;
                text.color = poorColor;
            }
        }
    }
}
