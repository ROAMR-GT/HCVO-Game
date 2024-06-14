using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script creates the VO ring using sprites that appear on HUD. Script adapted from Wall Creator.
public class CreateVORing : MonoBehaviour
{
    //public Texture2D tex;
    public int numEdges;
    private int numEdgesOuter;
    public float arenaRadius;
    private float radiusOuter;
    public float wallHeight;

    private GameObject[] blocks;
    private GameObject[] blocksOuter;

    public GameObject GO_parent;
    public GameObject Body;
    Vector3 BodyPos;

    public Sprite mySprite;
    GameObject go;
    //private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        numEdgesOuter = 60;
        radiusOuter = arenaRadius + 16f;
        blocks = new GameObject[numEdges];
        blocksOuter = new GameObject[numEdges];
        float arcTheta = (2.0f * Mathf.PI) / (float)numEdges;

        for (int i = 0; i < numEdges; i++)
        {
            // make one block
            float theta = (float)i * (2.0f * Mathf.PI / (float)numEdges);
            GameObject block = new GameObject();

            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = mySprite;

            // set scale, position, and rotation
            float x = arenaRadius * Mathf.Cos(theta);
            float z = arenaRadius * Mathf.Sin(theta);
            block.transform.SetParent(GameObject.Find("/HMD/Minimap Mask/Walls_SR").transform, true);
            block.transform.localScale = new Vector3(100 * 0.1f, 100 * 0.1f, 0);//3f * arenaRadius * Mathf.Tan(arcTheta / 2.0f), 0);//wallHeight, 2.0f * arenaRadius * Mathf.Tan(arcTheta / 2.0f));
            block.transform.localPosition = new Vector3(x, z, 0);//-wallHeight / 2 + 0.01f);
            //float dist = Vector3.Distance(GameObject.Find("Minimap Image").transform.position, block.transform.position);
            //block.transform.localPosition = new Vector3(x, z+dist, 0);//-wallHeight / 2 + 0.01f);
            block.transform.Rotate(0, 0, 1f * Rad2Deg(theta));

            // Set parent of block, name, and tag
            //block.transform.SetParent(GameObject.Find("/HMD/Minimap Mask/Walls_SR").transform, true);
            Vector3 srPos = block.transform.localPosition;
            srPos.z = 0;
            block.transform.localPosition = srPos;
            block.name = "Wall_SR " + string.Format("{0:0}", i + 1);
            // block.tag = "Wall_SR";

            blocks[i] = block;
            block.layer = LayerMask.NameToLayer("UI");
        }
        //if (GameObject.Find("MainMenu").GetComponent<MainMenu>().Add_Static_Obstacle) //Second part creates the static outer ring for VO_View
        //{
            for (int i = 0; i < numEdgesOuter; i++)
            {
                // make one block
                float theta = (float)i * (2.0f * Mathf.PI / (float)numEdgesOuter);
                GameObject blockOuter = new GameObject();

                SpriteRenderer renderer = blockOuter.AddComponent<SpriteRenderer>();
                renderer.sprite = mySprite;

                // set scale, position, and rotation
                float x = radiusOuter * Mathf.Cos(theta);
                float z = radiusOuter * Mathf.Sin(theta);
                blockOuter.transform.SetParent(GameObject.Find("/HMD/Minimap Mask/Walls_SR_Outer").transform, true);
                blockOuter.transform.localScale = new Vector3((100 * 0.1f) / 1.1f, (100 * 0.1f) / 1.1f, 0);//3f * arenaRadius * Mathf.Tan(arcTheta / 2.0f), 0);//wallHeight, 2.0f * arenaRadius * Mathf.Tan(arcTheta / 2.0f));
                blockOuter.transform.localPosition = new Vector3(x, z, 0);//-wallHeight / 2 + 0.01f);
                                                                          //float dist = Vector3.Distance(GameObject.Find("Minimap Image").transform.position, block.transform.position);
                                                                          //block.transform.localPosition = new Vector3(x, z+dist, 0);//-wallHeight / 2 + 0.01f);
                blockOuter.transform.Rotate(0, 0, 1f * Rad2Deg(theta));

                // Set parent of block, name, and tag
                //block.transform.SetParent(GameObject.Find("/HMD/Minimap Mask/Walls_SR").transform, true);
                Vector3 srPos = blockOuter.transform.localPosition;
                srPos.z = 0;
                blockOuter.transform.localPosition = srPos;
                blockOuter.name = "Wall_SR_Outer " + string.Format("{0:0}", i + 1);
                // block.tag = "Wall_SR";

                blocksOuter[i] = blockOuter;
                blockOuter.layer = LayerMask.NameToLayer("UI");
            }
        //}

        GO_parent.transform.localPosition = new Vector3(0, 0, 0);






        /*go = new GameObject("New Sprite");
        //Vector3 srPos = go.transform.localPosition;
        //srPos.z = 0;
        //go.transform.localPosition = srPos;
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = mySprite;
        go.transform.SetParent(GameObject.Find("/HMD").transform, true);
        Vector3 srPos = go.transform.localPosition;
        srPos.z = 0;
        go.transform.localPosition = srPos;
        */


        //sr.sprite = mySprite;
    }
    // Update is called once per frame
    void Update()
    {
        //BodyPos = Body.transform.position;
        //GO_parent.transform.localPosition = new Vector3(0,0,0);
    }

    float Rad2Deg(float rad)
    {
        return rad * (180.0f / Mathf.PI);
    }
}
