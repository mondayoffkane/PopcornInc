using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;



public class CameraMove : MonoBehaviour
{

    [SerializeField] Vector3 _startPos;
    public Vector4 _limitPos = new Vector4(-20f, 20f, 0f, 60f);
    public Vector2 _startXY, _endXY;

    public Vector2 _viewLimit = new Vector2(30f, 60f);
    //public float _minY = 0f, _maxY = 30f;
    //public float _startY, _endY;

    public float _moveSense = 0.05f;
    public float _wheelSense = 10f;
    public float _zoomSense = 0.05f;
    [SerializeField] float _x, _y;
    [SerializeField] float _zoom_startDistance, _zoom_currentDistance;
    [SerializeField] float _distance;
    [SerializeField] float _startSize;
    Camera _cam;
    [SerializeField] Vector2 _testStartPos, _testCurrentPos;

    StageManager _stageManager;
    //public Text TestText;
    bool isFix = false;
    public float _lookdistance = 50f;
    public Vector3 _lookOffset = new Vector3(0f, 10f, 0f);
    public bool isClick = false;
    // ===========
    private void Start()
    {
        _cam = GetComponent<Camera>();
        //TestText = Managers.GameUI.TestText;
        _stageManager = Managers.Game._stageManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Game.CalcMoney(100);
        }
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Managers.Sound.Play("Effect_1");
        //}
        if (Input.GetKeyDown(KeyCode.A))
        {
            _stageManager.AddStaff(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _stageManager.AddIncome(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _stageManager.AddParts(false);
        }
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    _stageManager._staff_upgrade_level = 0;
        //    _stageManager._income_ugrade_level = 0;
        //    _stageManager._parts_upgrade_level = 0;
        //    Managers.Game.Money = 0;
        //    _stageManager.SaveData();
        //}
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.orthographicSize += 0.1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Camera.main.orthographicSize -= 0.1f;
        }




#if UNITY_EDITOR


        float _val = Input.GetAxis("Mouse ScrollWheel") * _wheelSense;

        _cam.orthographicSize -= _val;
        if (_cam.orthographicSize < _viewLimit.x)
        {
            _cam.orthographicSize = _viewLimit.x;
        }
        else if (_cam.orthographicSize > _viewLimit.y)
        {
            _cam.orthographicSize = _viewLimit.y;
        }


        if (Input.GetMouseButtonDown(0))
        {
            _startXY = new Vector2(Input.mousePosition.x /*0f*/, Input.mousePosition.y);
            _endXY = _startXY;
            isClick = true;
            _startPos = transform.position;
            if (!EventSystem.current.IsPointerOverGameObject())// ui가 아닌곳을 눌렀을때 ui 끄기
            {
                _stageManager.OffPopup();
            }
            //else
            //{
            //    return;

            //}

            Vector3 touchPos;
            Ray ray;
            RaycastHit hit;

            Vector3 touchPosToVector3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100);
            touchPos = Camera.main.ScreenToWorldPoint(touchPosToVector3);
            ray = Camera.main.ScreenPointToRay(touchPosToVector3);


            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);

                //Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Upgrade_Obj")
                {
                    //Debug.Log("Hit Obj");
                    _stageManager.SelecteTarget(hit.transform);
                    isClick = false;
                }
                else if (hit.collider.tag == "WorkerBox")
                {
                    _stageManager.AddStaff(false, hit.transform.gameObject);
                }
            }
        }

        else if (Input.GetMouseButton(0) && !isFix)
        {
            if (EventSystem.current.IsPointerOverGameObject()) // ui 터치시 화면 이동 안하도÷
            {

                isClick = false;
                return;
            }
            if (isClick)
            {
                _endXY = new Vector2(Input.mousePosition.x/*0f */, Input.mousePosition.y);

                _x = (transform.right * (_startXY.x - _endXY.x) * _moveSense).x;
                _y = (transform.up * (_startXY.y - _endXY.y) * _moveSense).y;

                Vector3 _horizon = transform.right * (_startXY.x - _endXY.x) * _moveSense;
                Vector3 _vertical = transform.up * (_startXY.y - _endXY.y) * _moveSense;

                Vector3 _delta = _startPos + _horizon + _vertical;

                if (_delta.x >= _limitPos.x && _delta.x <= _limitPos.y && _delta.y >= _limitPos.z && _delta.y <= _limitPos.w)
                {
                    transform.position = _delta;
                }
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            isClick = false;
            _startXY = Vector2.zero;
            _endXY = _startXY;

        }

#endif

#if !UNITY_EDITOR


        if (Input.touchCount > 0 && Input.touchCount < 2)
        {
            _startSize = _cam.orthographicSize;
            Touch _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                isClick = true;
                _startXY = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                _endXY = _startXY;

                _startPos = transform.position;
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    _stageManager.OffPopup();
                }


                Vector3 touchPos;
                Ray ray;
                RaycastHit hit;

                Vector3 touchPosToVector3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100);
                touchPos = Camera.main.ScreenToWorldPoint(touchPosToVector3);
                ray = Camera.main.ScreenPointToRay(touchPosToVector3);


                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);

                    //Debug.Log(hit.collider.tag);
                    if (hit.collider.tag == "Upgrade_Obj")
                    {
                        //Debug.Log("Hit Obj");
                        _stageManager.SelecteTarget(hit.transform);
                        isClick = false;
                    }
                    else if (hit.collider.tag == "WorkerBox")
                    {
                        _stageManager.AddStaff(false, hit.transform.gameObject);
                    }
                }
            }
            else if ((_touch.phase == TouchPhase.Moved || _touch.phase == TouchPhase.Stationary) && !isFix)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    isClick = false;
                    return;
                }
                if (isClick)
                {
                    _endXY = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                    _x = (transform.right * (_startXY.x - _endXY.x) * _moveSense).x;
                    _y = (transform.up * (_startXY.y - _endXY.y) * _moveSense).y;

                    Vector3 _horizon = transform.right * (_startXY.x - _endXY.x) * _moveSense;
                    Vector3 _vertical = transform.up * (_startXY.y - _endXY.y) * _moveSense;

                    Vector3 _delta = _startPos + _horizon + _vertical;

                    if (Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).x >= _limitPos.x && Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).x <= _limitPos.y && Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).y >= _limitPos.z && Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).y <= _limitPos.w)
                    {
                        transform.position = Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f);
                    }
                }
            }

            else if (_touch.phase == TouchPhase.Ended)
            {
                isClick = false;
                _startXY = Vector2.zero;
                _endXY = _startXY;
            }

        }


        if (Input.touchCount == 2) //손가락 2개가 눌렸을 때
        {
            Touch touchZero = Input.GetTouch(0); //첫번째 손가락 터치를 저장
            Touch touchOne = Input.GetTouch(1); //두번째 손가락 터치를 저장

            //터치에 대한 이전 위치값을 각각 저장함
            //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition는 이동방향 추적할 때 사용
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 각 프레임에서 터치 사이의 벡터 거리 구함
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 만약 카메라가 OrthoGraphic모드 라면

            if ((_cam.orthographicSize + deltaMagnitudeDiff * _zoomSense >= _viewLimit.x) && (_cam.orthographicSize + deltaMagnitudeDiff * _zoomSense <= _viewLimit.y))
            {
                _cam.orthographicSize += deltaMagnitudeDiff * _zoomSense;
                _cam.orthographicSize = Mathf.Max(_cam.orthographicSize, 0.1f);
            }

        }
#endif


    }


    public void LookTarget(Transform _target)
    {
        DOTween.Sequence()
            .Append(transform.DOMove(_target.position + _lookOffset - transform.forward * _lookdistance, 0.5f).SetEase(Ease.Linear))
            //.Append(transform.DOMove(_target.transform.position + new Vector3(30f, 50f, 50f), 0.5f).SetEase(Ease.Linear))
            .AppendCallback(() => { isFix = true; })
            .OnComplete(() => isFix = false);
    }



}
