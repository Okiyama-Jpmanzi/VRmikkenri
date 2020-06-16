using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player_teleport : MonoBehaviour
{
    [SerializeField]
    private GameObject _camera = null;
    private GameObject _1st_object;
    private GameObject _line;
    private bool _check = false;
    private bool _onTeleport = false;
    private float _fall_amount = 2.0f;
    private float _distance = 4.0f;
    [SerializeField]
    private int _poscount = 10;

    private LineRenderer _lineren;
    private RaycastHit _hit;
    private Vector3 _hitpoint;

    private void Start()
    {
        _1st_object = this.gameObject;
        BezierLine();
    }


    private void Update()
    {


        if (_check)
        {
            Draw();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))  //トリガーを引いた時
        {
            _check = true;
            _line.SetActive(true);
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))    //トリガーを離したとき
        {
            _check = false;
            _line.SetActive(false);
            if (_hit.collider.tag == "OnTeleport")  //ここで地面で解除したかの判定を行う 
            {    
                _camera.transform.position = new Vector3(_hitpoint.x, _camera.transform.position.y, _hitpoint.z);
                _onTeleport = false;
            }
        }


    }

    private void Draw()
    {
        Vector3 _first_point = _1st_object.transform.position;
        Vector3 _middle_point = _1st_object.transform.position + _1st_object.transform.forward * _distance / 2;
        Vector3 _end_point = _1st_object.transform.position + _1st_object.transform.forward * _distance;
        _end_point.y = _first_point.y - _fall_amount;

        //_end_pointer.transform.position = _end_point;

        Vector3 _012p = _first_point;

        for (int i = 0; i < _poscount; i++)
        {
            float _bezier_count = ((float)i / (float)(_poscount - 1));
            Vector3 _01p = Vector3.Lerp(_first_point, _middle_point, _bezier_count);
            Vector3 _12p = Vector3.Lerp(_middle_point, _end_point, _bezier_count);  //3点の計算

            Vector3 _bezier_curve = Vector3.Lerp(_01p, _12p, _bezier_count);

            if (Physics.Linecast(_012p, _bezier_curve, out _hit))
            {
                _hitpoint = _hit.point;
                
                for (int n = i; n < _poscount; n++)
                {
                    _lineren.SetPosition(n, _hitpoint);
                }
            }
            else
            {
                _lineren.SetPosition(i, _bezier_curve);
                _012p = _bezier_curve;
            }

        }
    }

    private void BezierLine()
    {
        _line = new GameObject("Line");
        _line.transform.parent = _1st_object.transform;
        _line.SetActive(false);

        _lineren = _line.AddComponent<LineRenderer>();
        _lineren.positionCount = _poscount;
        _lineren.startWidth = 0.01f;
        _lineren.endWidth = 0.01f;
    }
}
