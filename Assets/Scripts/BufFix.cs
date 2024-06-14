using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

//This script will find set the Target gameobject as active at runtime. The target gameobject is set as inactive initially.
//This fixes the bug where the target will teleport randomly in randomized movement.
public class BufFix : MonoBehaviour
{
    private int counter;
    // Start is called before the first frame update
    void Start()
    {
        var obj = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g =>g.CompareTag("Target"));
        //print("Found:" + obj.name);
        obj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

    /*public static GameObject Find(string name)
    {
        var scene = SceneManager.GetActiveScene();
        var sceneRoots = scene.GetRootGameObjects();

        GameObject result = null;
        foreach (var root in sceneRoots)
        {
            if (root.name.Equals(name)) return root;

            result = FindRecursive(root, name);

            if (result) break;
        }

        return result;
    }
    private static GameObject FindRecursive(GameObject obj, string search)
    {
        GameObject result = null;
        foreach (Transform child in obj.transform)
        {
            if (child.name.Equals(search)) return child.gameObject;

            result = FindRecursive(child.gameObject, search);

            if (result) break;
        }

        return result;
    }
}*/
