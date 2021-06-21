using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceBrick : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    
    
    public GameObject brickPrefab;
    public GameObject wall;

    public int maxNumColumns;
    private int numColumnsTopRow;
    private int numRows;

    public Color[] colors;
    private int colorIndex;

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

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

    public void SwitchBrickColor()
    {
        colorIndex++;
        colorIndex = colorIndex > colors.Length - 1 ? 0 : colorIndex;
    }

    public Color GetNextBrickColor()
    {
        return colors[colorIndex];
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        Debug.Log("width: " + touchPosition.x / Screen.width);
        Debug.Log("how high:" + touchPosition.y / Screen.height);

        if (Input.touchCount >= 1)
        {
            //if (Input.touches[0].phase == TouchPhase.Began)
            //{
            //}

            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                    // start wall
                    if(wall.transform.childCount == 0)
                    {
                        wall.transform.position = hitPose.position;
                        wall.transform.rotation = hitPose.rotation.normalized;
                        spawnedObject = Instantiate(brickPrefab, wall.transform);
                    }
                }

                
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit raycastHit;
                // check to see if it should add a brick to the existing wall
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    Collider brickCollider = raycastHit.collider;
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

                        // no duplicate locations
                        foreach (Transform placedbrick in wall.transform)
                        {
                            if (placedbrick.position == spawnPose)
                                return;
                        }
                        spawnedObject = Instantiate(brickPrefab, spawnPose, Quaternion.identity);
                    }
                }
                //// if it never collids
                //else
                //{
                //    return;
                //}
                spawnedObject.GetComponentInChildren<MeshRenderer>().material.color = GetNextBrickColor();
                spawnedObject.transform.parent = wall.transform;
            }
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
