using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class KeepCameraOnTargets : MonoBehaviour
{
    public enum SmoothingType { Lerp, SmoothDamp, Snap }

    [Header("Editor Mode")]
    [SerializeField]
    bool _executeInEditMode = false;

    [Header("Bounds and Limit Options")]
    [SerializeField]
    RectTransform _viewportBounds;

    [SerializeField]
    float _minFrustrumHeight = 0;

    [SerializeField]
    float _frustrumBorder = 0.1f;
    
    [Header("Perspective Mode Options")]
    bool _fixedFieldOfView = false;

    [SerializeField]
    bool _useRaycastForBoundCalculations = false;

    [Header("Target Options")]
    [SerializeField]
    List<CameraFollowTarget> _targets;

    [SerializeField]
    bool _ignoreOutOfBoundsObjects = false;

    [SerializeField]
    bool _ignoreInactiveObjects = true;

    [Header("Movement Options")]
    [SerializeField]
    SmoothingType _smoothingType;

    [SerializeField]
    float _lerpFactor = 3;

    [SerializeField]
    float _smoothSpeed = 0.4f;


    [Header("Debug Options")]
    [SerializeField]
    bool _drawTargetBounds = false;
    [SerializeField]
    bool _drawUnconstrainedBounds = false;
    [SerializeField]
    bool _drawUnconstrainedExtendedBounds = false;
    [SerializeField]
    bool _drawNaiveCameraBounds = false;
    [SerializeField]
    bool _drawBorderConstrainedBounds = false;
    [SerializeField]
    bool _drawFinalCameraBounds = false;
    [SerializeField]
    bool _drawProjectionRaycasts = false;

    Camera _camera = null;

    bool _boundsInitialized;
    Bounds _smoothedBounds;

    Vector3 _boundCenterSpeed;
    Vector3 _boundSizeSpeed;

    void EnsureCameraExists()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();
    }

    void Start()
    {
        EnsureCameraExists();

        Bounds targetBounds = CalculateTargetBounds();
        
        _smoothedBounds = targetBounds;
    }

    void LateUpdate()
    {
        // checking if we are running in edit mode
        if (!Application.isPlaying)
        {
            if (_executeInEditMode)
                EnsureCameraExists();
            else
                return;
        }

        // rest is as normal

        Rect viewportRect = _viewportBounds.GetWorldRect();

        if (viewportRect.Contains(_camera.transform.position))
            CameraUpdate(viewportRect);
    }

    void CameraUpdate(Rect viewportRect)
    {
        Bounds targetBounds = CalculateTargetBounds();

        if (_drawTargetBounds) MiscUtil.DrawRect(targetBounds, Color.white);

        // Calculate the target bounds
        Bounds cameraBounds;
        cameraBounds = CalculateCameraBounds(targetBounds);
        if (_drawUnconstrainedBounds) MiscUtil.DrawRect(cameraBounds, (Color.white + Color.red) / 2);

        // Adding a constant border
        cameraBounds = AddBorder(cameraBounds);
        if (_drawUnconstrainedExtendedBounds) MiscUtil.DrawRect(cameraBounds, Color.red);

        // Expand the bounds to force the correct aspect ratio
        cameraBounds = cameraBounds.ForceToAspect(_camera.aspect, false);
        if (_drawNaiveCameraBounds) MiscUtil.DrawRect(cameraBounds, (Color.red + Color.yellow) / 2);

        // smooth the size adjust *before* constraining to the viewport (otherwise it can sneak onto the screen when moving)
        if (Application.isPlaying)
            _smoothedBounds.size = SmoothBoundsTo(_smoothedBounds, cameraBounds, ref _boundCenterSpeed, ref _boundSizeSpeed).size;
        else
            _smoothedBounds.size = cameraBounds.size;
        _smoothedBounds.center = cameraBounds.center;
        cameraBounds = _smoothedBounds;

        // Constrain the camera bounds to the viewport so that we don't see outside
        cameraBounds = ConstrainToViewport(cameraBounds, viewportRect);
        if (_drawBorderConstrainedBounds) MiscUtil.DrawRect(cameraBounds, Color.yellow);

        // We further constrain the camera to be the same aspect ratio but still inside the constrained bounds
        cameraBounds = cameraBounds.ForceToAspect(_camera.aspect, true);
        if (_drawFinalCameraBounds) MiscUtil.DrawRect(cameraBounds, Color.green);

        SetCameraToRect(cameraBounds);

    }

    Bounds SmoothBoundsTo(Bounds from, Bounds to, ref Vector3 boundCenterSpeed, ref Vector3 boundSizeSpeed)
    {
        switch (_smoothingType)
        {
            case SmoothingType.Lerp:
                from.center = Vector3.Lerp(from.center, to.center, _lerpFactor * Time.deltaTime);
                from.size = Vector3.Lerp(from.size, to.size, _lerpFactor * Time.deltaTime);
                break;
            case SmoothingType.SmoothDamp:
                from.center = Vector3.SmoothDamp(from.center, to.center, ref boundCenterSpeed, _smoothSpeed);
                from.size = Vector3.SmoothDamp(from.size, to.size, ref boundSizeSpeed, _smoothSpeed);
                break;
            default:
                from.center = to.center;
                from.size = to.size;
                break;
        }
        return from;
    }

    Bounds CalculateCameraBounds(Bounds targetBounds)
    {
        Vector3 cameraTargetPosition = transform.position;
        cameraTargetPosition.z = _viewportBounds.position.z;
        Bounds cameraBounds = new Bounds(cameraTargetPosition, Vector3.zero);

        Vector3 furthestDistanceToTarget = Vector3.zero;
        
        furthestDistanceToTarget = Vector3.Max(cameraTargetPosition - targetBounds.min, targetBounds.max - cameraTargetPosition);
        furthestDistanceToTarget.z = 0;
        cameraBounds.min -= furthestDistanceToTarget;
        cameraBounds.max += furthestDistanceToTarget;

        Vector3 minBounds = new Vector3(_minFrustrumHeight, _minFrustrumHeight * _camera.aspect, 0);
        cameraBounds.extents = Vector3.Max(cameraBounds.extents, minBounds);
        
        return cameraBounds;
    }


    Bounds AddBorder(Bounds cameraBounds)
    {
        Vector3 border = new Vector3(_frustrumBorder, _frustrumBorder, 0);

        cameraBounds.min -= border;
        cameraBounds.max += border;

        return cameraBounds;
    }


    Bounds ConstrainToViewport(Bounds cameraBounds, Rect viewportRect)
    {
        Vector2 distToEdge = Vector2.Min((Vector2)cameraBounds.center - viewportRect.min, viewportRect.max - (Vector2)cameraBounds.center);
        cameraBounds.extents = Vector3.Min(cameraBounds.extents, distToEdge);

        return cameraBounds;
    }


    void SetCameraToRect(Bounds bounds)
    {
        float frustumHeight = bounds.extents.y;

        if (_camera.orthographic)
        {
            _camera.orthographicSize = frustumHeight;
        }
        else // camera is perspective
        {
            if (_fixedFieldOfView)
            {
                float distance = frustumHeight / Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

                float cameraZPosition = _viewportBounds.position.z - distance;

                Vector3 newCameraPosition = _camera.transform.position;
                newCameraPosition.z = cameraZPosition;
                _camera.transform.position = newCameraPosition;
            }
            else // variable field of view
            {
                float distance = bounds.center.z - _camera.transform.position.z;
                float fieldOfView = 2.0f * Mathf.Atan(frustumHeight / distance) * Mathf.Rad2Deg;
                
                _camera.fieldOfView = fieldOfView;
            }
            
        }

    }


    Bounds CalculateTargetBounds()
    {
        Bounds targetBounds = new Bounds();
        bool rectUninitialised = false;
        Rect viewportRect = _viewportBounds.GetWorldRect();
        
        foreach (var t in GetValidTargets())
        {
            Vector3 targetPositionOnBounds = t.transform.position;

            // Check out of bound objects
            if (_ignoreOutOfBoundsObjects)
            {
                Vector3 planeCenterPosition = _useRaycastForBoundCalculations ? ProjectedPosition(targetPositionOnBounds) : targetPositionOnBounds;
                if (!viewportRect.Contains(targetPositionOnBounds))
                    continue;
            }

            if (!rectUninitialised)
            {
                targetBounds = new Bounds(t.transform.position, Vector3.zero);
                rectUninitialised = true;
            }

            foreach (var v in t.GetBorderPoints())
            {
                Vector3 planePosition = _useRaycastForBoundCalculations ? ProjectedPosition(v) : v;
                targetBounds.Encapsulate(planePosition);
            }
            
            
        }

        return targetBounds;
    }

    Vector3 ProjectedPosition(Vector3 point)
    {
        Plane viewportPlane = new Plane(Vector3.forward, _viewportBounds.position);

        Ray ray = _camera.ViewportPointToRay(_camera.WorldToViewportPoint(point));
        float distance;
        viewportPlane.Raycast(ray, out distance);

        Vector3 hitPoint = ray.GetPoint(distance);

        if (_drawProjectionRaycasts)
            Debug.DrawLine(ray.origin, hitPoint);

        return hitPoint;
    }


    IEnumerable<CameraFollowTarget> GetValidTargets()
    {
        foreach (var t in _targets)
        {
            if (t == null)
                continue;

            if (_ignoreInactiveObjects && !t.gameObject.activeInHierarchy)
                continue;

            yield return t;
        }
    }
    
}
