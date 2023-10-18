#if UNITY_ANDROID
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class AdaptiveCameraSize : MonoBehaviour
{
    private Vector2 DefaultResolution = new Vector2 (1080, 1920);

    [SerializeField, Range(0f, 1f)]
    private float WidthOrHeight = 0f;

    private Camera _camera;
    private float _initialSize;
    private float _targetAspect;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _initialSize = _camera.orthographicSize;
        _targetAspect = DefaultResolution.x / DefaultResolution.y;
        float constantWidthSize = _initialSize * (_targetAspect / _camera.aspect);
        _camera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, WidthOrHeight);
    }
}
#endif
