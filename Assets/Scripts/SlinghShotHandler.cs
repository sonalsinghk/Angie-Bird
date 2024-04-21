using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transforms")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;

    [Header("Sling Shot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;

    [Header("Bird")]
    [SerializeField] private GameObject _angieBirdPreFab;
    [SerializeField] private float _angieBirdPositionOffset = 2f;

    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;
    

    private bool _clickedWithinArea;

    private GameObject _spawnedAngieBird;

    private void Awake()
    {
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngieBird();

    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame && _slingShotArea.IsWithinSlingShotArea())
        {
            _clickedWithinArea = true;
        }
        if (Mouse.current.leftButton.isPressed && _clickedWithinArea)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _clickedWithinArea = false;
        }
    }

    #region SlingShot Methods

    private void DrawSlingShot() {

        
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);
        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {

        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }

    #endregion

    #region AngieBird Methods

    private void SpawnAngieBird()
    {
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angieBirdPositionOffset;

        _spawnedAngieBird = Instantiate(_angieBirdPreFab, spawnPosition, Quaternion.identity);
        _spawnedAngieBird.transform.right = dir;
    }

    private void PositionAndRotateAngieBird()
    {
        _spawnedAngieBird.transform.position = _slingShotLinesPosition + _directionNormalized * _angieBirdPositionOffset;
        _spawnedAngieBird.transform.right = _directionNormalized;
    }
    #endregion
}
