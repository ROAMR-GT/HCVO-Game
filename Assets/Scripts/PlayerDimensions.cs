using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sets player gameobjects scaling to 0.4 and hides the transform component from inspector.
public class PlayerDimensions : MonoBehaviour
{
    private float dim = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(dim, dim, dim);
        gameObject.GetComponent<Transform>().hideFlags = HideFlags.HideInInspector;
    }
}
