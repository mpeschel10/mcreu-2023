using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraSizeToScreen : MonoBehaviour, Fixable
{
    [SerializeField] Pillars pillars;
    Camera orthoCamera;
    float normalDistance;
    void Awake()
    {
        orthoCamera = GetComponent<Camera>();
        Vector3 myOffset = transform.position - pillars.transform.position;
        normalDistance = Vector3.Project(myOffset, pillars.transform.forward * -1).magnitude;
    }
    
    public void Fix()
    {
        PillarCover[] covers = pillars.pillarCovers;
        if (covers.Length <= 4) return;

        // Transform orientation = covers[2].transform;
        // Vector3 leftMost = covers[2].transform.position + pillars.pillarWidth * orientation.right * -0.5f;
        // Vector3 rightMost = covers[covers.Length - 3].transform.position + pillars.pillarWidth * orientation.right * 0.5f;
        // float span = (rightMost - leftMost).magnitude;
        // Debug.Log("Expected span: " + span + " observed " + pillars.pillarSpan);
        orthoCamera.orthographicSize = pillars.pillarSpan / 2f / orthoCamera.aspect;
        // Debug.Log("Span is " + span + " leftmost " + leftMost + " rightmost " + rightMost);
        // Debug.Log("Screen height " + Screen.height + "screen width " + Screen.width);
        // Debug.Log("ortho size " + orthoCamera.orthographicSize + " aspect " + orthoCamera.aspect);
        float expectedSize = orthoCamera.orthographicSize * 2 * orthoCamera.aspect;
        // Debug.Log("Expected camera width: " + expectedSize);
        transform.position = pillars.centerPosition + normalDistance * pillars.transform.forward * -1;
    }
}
