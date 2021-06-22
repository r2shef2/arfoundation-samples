using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime;

public class Wall : MonoBehaviour
{
    public Save bricks;

    public void Save()
    {
        //string json = JsonUtility.ToJson();
        //Debug.Log(json);
    }

    public void LoadWall()
    {
        
    }

    public void Clear()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}