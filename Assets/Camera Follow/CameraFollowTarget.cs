using UnityEngine;
using System.Collections.Generic;

public class CameraFollowTarget : MonoBehaviour
{
    public float objectBorder = 0;

    public IEnumerable<Vector3> GetBorderPoints()
    {
        yield return transform.position;
        yield return transform.position + objectBorder*Vector3.left;
        yield return transform.position + objectBorder*Vector3.right;
        yield return transform.position + objectBorder*Vector3.up;
        yield return transform.position + objectBorder*Vector3.down;
    }
    
}
