using System;
using UnityEngine;
public class WallCreator : MonoBehaviour
{
    public int numEdges = 8;
    private float arenaRadius;
    public float wallHeight;

    private GameObject[] blocks;


    void Start()
    {
        arenaRadius = GameObject.Find("MainMenu").GetComponent<MainMenu>().arenaR;
        // create array of blocks
        blocks = new GameObject[numEdges];
        float arcTheta = (2.0f * Mathf.PI) / (float)numEdges;

        for (int i = 0; i < numEdges; i++)
        {
            // make one block
            float theta = (float)i * (2.0f * Mathf.PI / (float)numEdges);
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // set scale, position, and rotation
            float x = arenaRadius * Mathf.Cos(theta);
            float z = arenaRadius * Mathf.Sin(theta);
            block.transform.localScale = new Vector3(0.1f, wallHeight, 2.0f * arenaRadius * Mathf.Tan(arcTheta / 2.0f));
            block.transform.position = new Vector3(x, wallHeight / 2.0f, z);
            block.transform.Rotate(0, -1f*Rad2Deg(theta), 0);

            // Set parent of block, name, and tag
            block.transform.SetParent(GameObject.Find("/Walls").transform, true);
            block.name = "Wall " + string.Format("{0:0}", i+1);
            block.tag = "Wall";

            blocks[i] = block;
        }
    }

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }
}
