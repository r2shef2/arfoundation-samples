using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public float[] sizes;
    private int sizeIndex;

    public GameObject sizeButton;
    public GameObject clearButton;

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

    public void SwitchBrickSize()
    {
        sizeIndex++;
        sizeIndex = sizeIndex > sizes.Length - 1 ? 0 : sizeIndex;
    }
    public float GetNextBrickSize()
    {
        return sizes[sizeIndex];
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
                GameObject newBrick = null;
                // start wall
                if (wall.transform.childCount == 0)
                {
                    if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
                    {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                        wall.transform.position = hitPose.position;
                        newBrick = Instantiate(brickPrefab, wall.transform.position, Quaternion.identity);
                        sizeButton.SetActive(false);
                        clearButton.SetActive(true);
                    }
                }
                // check to see if it should add a brick to the existing wall
                else if (Physics.Raycast(raycast, out raycastHit))
                {
                    Collider brickCollider = raycastHit.collider;
                    if (brickCollider.tag == "Brick")
                    {
                        var hitPose = raycastHit.normal;
                        Vector3 spawnPose = brickCollider.transform.position;
                        // placement math
                        spawnPose.z = brickCollider.transform.position.z;


                        float xDif = Mathf.Abs(hitPose.x) - Mathf.Abs(brickCollider.transform.position.x);
                        float yDif = Mathf.Abs(hitPose.y) - Mathf.Abs(brickCollider.transform.position.y);
                        if (xDif > yDif)
                        {
                            float xScale = GetNextBrickSize() * brickCollider.transform.localScale.x;
                            spawnPose.x += hitPose.x > brickCollider.transform.position.x ? xScale : -xScale;
                        }
                        else
                        {
                            float yScale = GetNextBrickSize() * brickCollider.transform.localScale.y;
                            spawnPose.y += hitPose.y > brickCollider.transform.position.y ? yScale : -yScale;
                        }

                        // no duplicate locations
                        foreach (Transform placedbrick in wall.transform)
                        {
                            if (placedbrick.position == spawnPose)
                                return;
                        }
                        newBrick = Instantiate(brickPrefab, spawnPose, Quaternion.identity);
                    }
                    // if nothing is placed
                    else
                    {
                        return;
                    }
                }

                // set size, parent and color
                newBrick.GetComponentInChildren<MeshRenderer>().material.color = GetNextBrickColor();
                newBrick.transform.parent = wall.transform;
                newBrick.transform.localScale *= GetNextBrickSize();
            }
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
