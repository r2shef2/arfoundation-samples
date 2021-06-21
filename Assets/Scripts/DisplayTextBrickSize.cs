using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayTextBrickSize : MonoBehaviour
{
    public PlaceBrick ARSession;
    void Update()
    {
        GetComponent<Text>().text = ARSession.GetNextBrickSize().ToString();
    }
}
