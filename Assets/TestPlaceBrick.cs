using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestPlaceBrick : MonoBehaviour
{


    public GameObject brickPrefab;
    public GameObject wall;

    public int maxNumColumns;
    private int numColumnsTopRow;
    private int numRows;

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }


    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (Input.touchCount >= 1)
        {

            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit raycastHit;
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    Collider brickCollider = raycastHit.collider;

                    if (brickCollider.name == "Plane")
                    {
                        var hitPose = raycastHit.point;

                        // start wall
                        if (wall.transform.childCount == 0)
                        {
                            wall.transform.position = hitPose;
                            spawnedObject = Instantiate(brickPrefab, wall.transform.position, Quaternion.identity);
                        }
                    }



                    if (brickCollider.tag == "Brick")
                    {
                        var hitPose = raycastHit.point;
                        Vector3 spawnPose = brickCollider.transform.position;
                        // placement math
                        spawnPose.z = brickCollider.transform.position.z;

                        float xDif = Mathf.Abs(hitPose.x) - Mathf.Abs(brickCollider.transform.position.x);
                        float yDif = Mathf.Abs(hitPose.y) - Mathf.Abs(brickCollider.transform.position.y);
                        if (xDif > yDif)
                        {
                            Debug.Log("side");
                            spawnPose.x += hitPose.x > brickCollider.transform.position.x ? brickCollider.transform.localScale.x : -brickCollider.transform.localScale.x;
                        }
                        else
                        {
                            Debug.Log("high or low");
                            spawnPose.y += hitPose.y > brickCollider.transform.position.y ? brickCollider.transform.localScale.y : -brickCollider.transform.localScale.y;
                        }

                        // no duplicates
                        foreach(Transform placedbrick in wall.transform)
                        {
                            if (placedbrick.position == spawnPose)
                                return;
                        }
                        spawnedObject = Instantiate(brickPrefab, spawnPose, Quaternion.identity);
                    }
                    spawnedObject.transform.parent = wall.transform;


                }
            }
        }
    }
}

