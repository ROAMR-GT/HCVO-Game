using System;
using UnityEngine;
public class WallCreator_SR : MonoBehaviour
{
    public int numEdges;
    public float arenaRadius;
    public float wallHeight;

    private GameObject[] blocks;

    public GameObject GO_parent;
    public GameObject Body;
    Vector3 BodyPos;

    void Start()
    {
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
            block.transform.localScale = new Vector3(0.3f, wallHeight, 2.0f * arenaRadius * Mathf.Tan(arcTheta / 2.0f));
            block.transform.position = new Vector3(x, -wallHeight/2+0.01f, z);
            block.transform.Rotate(0, -1f*Rad2Deg(theta), 0);

            // Set parent of block, name, and tag
            block.transform.SetParent(GameObject.Find("/[CameraRig]/Walls_SR").transform, true);
            block.name = "Wall_SR " + string.Format("{0:0}", i+1);
            // block.tag = "Wall_SR";

            blocks[i] = block;
            block.layer = LayerMask.NameToLayer("MiniMap");
        }
    }

    void Update()
    {
        BodyPos = Body.transform.position;
        GO_parent.transform.position = new Vector3(BodyPos[0], -wallHeight/2+0.01f, BodyPos[2]);
    }

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }
}
