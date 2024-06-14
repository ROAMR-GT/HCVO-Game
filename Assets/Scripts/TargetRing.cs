using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//Script is adapted from wall creator script. Makes a ring out of smaller cubes.
public class TargetRing : MonoBehaviour
{
    private int numEdges = 20;
    private float targetRadius;
    private float wallHeight;

    private GameObject[] blocks;


    void Start()
    {
        targetRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().Target_Area_area;
        targetRadius /= 2f;
        wallHeight = GameObject.Find("MainMenu").GetComponent<MainMenu>().target_height;
        // create array of blocks
        blocks = new GameObject[numEdges];
        float arcTheta = (2.0f * Mathf.PI) / (float)numEdges;

        for (int i = 0; i < numEdges; i++)
        {
            // make one block
            float theta = (float)i * (2.0f * Mathf.PI / (float)numEdges);
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // set scale, position, and rotation
            float x = targetRadius * Mathf.Cos(theta);
            float z = targetRadius * Mathf.Sin(theta);
            block.transform.localScale = new Vector3(0.01f, wallHeight, 2.0f * targetRadius * Mathf.Tan(arcTheta / 2.0f));
            block.transform.position = new Vector3(x, wallHeight/2.0f, z);
            block.transform.Rotate(0, -1f * Rad2Deg(theta), 0);

            // Set parent of block, name, and tag
            //block.transform.SetParent(GameObject.Find("/Walls").transform, true);
            //block.name = "Wall " + string.Format("{0:0}", i + 1);
            //block.tag = "Wall";
            block.GetComponent<BoxCollider>().enabled = false;

            block.transform.SetParent(GameObject.Find("/TargetArea").transform, true);

            Color color = new Color(16f / 255, 158f / 255, 35f / 255, 0.5f); //sets color to green

            //makes ring transparent
            block.GetComponent<Renderer>().material.SetColor("_Color", color);
            block.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
            block.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            block.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            block.GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
            block.GetComponent<Renderer>().material.renderQueue = 3000;

            UnityEngine.Object.Destroy(block.GetComponent<Rigidbody>());

            //Renderer renderer = block.GetComponent<Renderer>();
            //renderer.material.color = Color.clear;
            //Color color = block.GetComponent<Renderer>().material.color;
            //color.a = 0f;
            //block.GetComponent<Renderer>().material.color = color;
            //Material mat = block.GetComponent<SkinnedMeshRenderer>().materials[0];
            //mat.SetFloat("_Mode", 2.0f);

            blocks[i] = block;
        }
    }

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }
}
