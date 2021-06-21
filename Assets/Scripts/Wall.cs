using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    struct Brick
    {
        Color color;
        Vector2 position;
    }

    public void Save()
    {
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