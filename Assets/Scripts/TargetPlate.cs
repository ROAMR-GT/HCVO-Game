using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this plate is layed on top of the target ring and has solid color. This makes the target visible in drone view.
public class TargetPlate : MonoBehaviour
{
    private GameObject goal;
    private float targetRad;
    // Start is called before the first frame update
    void Start()
    {
        targetRad = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area;
        transform.localScale = new Vector3(targetRad, 0.01f, targetRad);
        gameObject.layer = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("TargetArea") != null)
        {
            goal = GameObject.Find("TargetArea");
        }
      
        Vector3 targetPos = goal.transform.position;
        targetPos.y = targetPos.y - 1f;
        transform.position = targetPos;

    }
}
