using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class BoundsHelper : MonoBehaviour
{
    [SerializeField]
    Transform _top;

    [SerializeField]
    Transform _bottom;

    [SerializeField]
    Transform _left;

    [SerializeField]
    Transform _right;

    RectTransform _rectTransform = null;
    Vector3[] _corners = null;
    
    void SetUpLocalVariables()
    {
        if (_corners == null || _corners.Length != 4)
            _corners = new Vector3[4];

        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
    }

	// Update is called once per frame
	void Update ()
    {
        SetUpLocalVariables();

        _rectTransform.GetLocalCorners(_corners);

        if (_left != null)
        {
            _left.localPosition = (_corners[0] + _corners[1]) / 2;
            _left.localScale = VerticalStretchScale(_corners[0], _corners[1]);
        }
        if (_top != null)
        {
            _top.localPosition = (_corners[1] + _corners[2]) / 2;
            _top.localScale = HorizonalStretchScale(_corners[1], _corners[2]);
        }
        if (_right != null)
        {
            _right.localPosition = (_corners[2] + _corners[3]) / 2;
            _right.localScale = VerticalStretchScale(_corners[2], _corners[3]);
        }
        if (_bottom != null)
        {
            _bottom.localPosition = (_corners[3] + _corners[0]) / 2;
            _bottom.localScale = HorizonalStretchScale(_corners[3], _corners[0]);
        }
    }

    static Vector3 VerticalStretchScale(Vector3 a, Vector3 b)
    {
        return new Vector3(1, Mathf.Abs(a.y - b.y), 1);
    }
    static Vector3 HorizonalStretchScale(Vector3 a, Vector3 b)
    {
        return new Vector3(Mathf.Abs(a.x - b.x), 1, 1);
    }

}


