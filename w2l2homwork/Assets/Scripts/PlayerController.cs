using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerController : MonoBehaviour
{
    private float _horizontal, _vertical;
    [SerializeField] private float _speed = 0.5f;
    private Vector3 _direction;
    private float _currentVelocity, _turnTime = 0.2f;

    [SerializeField] private FloatingJoystick _joystick;

    [SerializeField] private Transform _holdTransform, _detectTransform;
    private float _detectionRange = 1;
    [SerializeField] private LayerMask _collectedLayer;
    private int _itemCount = 0;
    private Collider[] _colliders;
    [SerializeField] private float _distance = 1;

    [SerializeField] private Transform _leaveTransform;
    private int _itemsCount = 0;
    [SerializeField] private LayerMask _leavedLayer;


    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    //void Update()
    //{
    //    _colliders = Physics.OverlapSphere(_detectTransform.position, _detectionRange, _layer);

    //    foreach (var hit in _colliders)
    //    {
    //        hit.tag = "Collected";
    //        hit.transform.parent = _holdTransform;
    //        var seq = DOTween.Sequence();

    //        seq.Append(hit.transform.DOLocalJump(new Vector3(0, _itemCount * _distance, 0), 2, 1, 0.2f)).Join(hit.transform.DOScale(1.5f, 0.2f)).Insert(0.1f, hit.transform.DOScale(0.5f, 0.1f));
    //        seq.AppendCallback(() =>
    //        {
    //            hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
    //        });
    //        _itemCount++;
    //    }
    //}
    void Update()
    {
        _colliders = Physics.OverlapSphere(_detectTransform.position, _detectionRange, _collectedLayer);
        foreach (var hit in _colliders)
        {
            if (hit.CompareTag("Collectable") && _itemCount <= 15)
            {
                hit.tag = "Collected";
                hit.gameObject.layer = 7;
                hit.transform.SetParent(_holdTransform);
                var seq = DOTween.Sequence();
                seq.Append(hit.transform.DOLocalJump(new Vector3(0, (_itemCount * _distance), 0), 2, 1, 0.3f))
                .Insert(0, hit.transform.DOScale(1.25f, 0.1f))
                .Insert(0.1f, hit.transform.DOScale(0.5f, 0.2f));
                seq.AppendCallback(() =>
                {
                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                });
                _itemCount++;
            }
        }
    }
    private void FixedUpdate()
    {
        _horizontal = _joystick.Horizontal;
        _vertical = _joystick.Vertical;
        _direction = new Vector3(_horizontal, 0, _vertical);

        if (_direction.magnitude > 0.1f)
        {
            float _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _currentVelocity, _turnTime);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            _rb.MovePosition(transform.position + (_direction * _speed * Time.deltaTime));
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    _colliders = Physics.OverlapSphere(_holdTransform.position, _detectionRange, _layer);

    //    if (other.CompareTag("Warehouse"))
    //    {
    //        foreach (var item in _colliders)
    //        {
    //            item.tag = "Leaved";
    //            item.transform.parent = _leaveTransform;

    //            var seq = DOTween.Sequence();
    //            seq.Append(item.transform.DOLocalJump(new Vector3(0, _itemsCount * _distance, 0), 2, 1, 0.2f)).Join(item.transform.DOScale(1.5f, 0.2f)).Insert(0.1f, item.transform.DOScale(0.5f, 0.2f));

    //            seq.AppendCallback(() =>
    //            {
    //                item.transform.rotation = Quaternion.Euler(0, 0, 0);
    //            });

    //            _itemsCount++;
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Warehouse"))
        {
            _colliders = Physics.OverlapSphere(_holdTransform.position, 5, _leavedLayer);
            foreach (var itemss in _colliders)
            {
                itemss.tag = "Leaved";
                itemss.gameObject.layer = 8;
                itemss.transform.SetParent(_leaveTransform);
                var seq1 = DOTween.Sequence();
                seq1.Append(itemss.transform.DOLocalJump(new Vector3(0, (_itemsCount * _distance), 0), 2, 1, 0.3f))
                .Insert(0, itemss.transform.DOScale(1.25f, 0.1f))
                .Insert(0.1f, itemss.transform.DOScale(1f, 0.2f));
                seq1.AppendCallback(() =>
                {
                    itemss.transform.localRotation = Quaternion.Euler(0, 0, 0);
                });

                _itemsCount++;
                _itemCount--;
            }
        }
    }
}
