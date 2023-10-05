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
    public float returnSpeed = 50f;


    public float _moveSense = 0.05f;
    public float _wheelSense = 10f;
    public float _zoomSense = 0.05f;
    [SerializeField] float _x, _y;
    [SerializeField] float _zoom_startDistance, _zoom_currentDistance;
    [SerializeField] float _distance;
    [SerializeField] float _startSize;
    Camera _cam;
    [SerializeField] Vector2 _testStartPos, _testCurrentPos;

    public float _targetDistance = 150f;
    StageManager _StageManager;
    public StageManager _stageManager
    {
        get
        {
            if (_StageManager == null) _StageManager = Managers.Game._stageManager;
            return _StageManager;
        }
        set
        {
            _StageManager = value;
        }
    }

    public bool isFix = false;
    public float _lookdistance = 50f;
    public Vector3 _lookOffset = new Vector3(0f, 10f, 0f);
    public bool isClick = false;
    public Material _beltMat;


    // ===========
    private void Start()
    {
        _cam = GetComponent<Camera>();

        _stageManager = Managers.Game._stageManager;

        _beltMat.DOOffset(Vector2.zero, 0f);
        _beltMat.DOOffset(new Vector2(0f, -1f), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }


    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q))
        {
            Managers.Game.CalcMoney(100000000);
            Managers.Game.CalcMoney(1000, 1);
            Managers.Game.CalcGem(10);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            //_stageManager.AddStaff(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //_stageManager.AddIncome(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _stageManager.AddParts(false);
        }

        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.orthographicSize += 0.1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Camera.main.orthographicSize -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }







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


        if (Input.GetMouseButtonDown(0) && !_stageManager.isCinema)
        {
            _startXY = new Vector2(Input.mousePosition.x /*0f*/, Input.mousePosition.y);
            _endXY = _startXY;
            isClick = true;
            _startPos = transform.position;
            if (!EventSystem.current.IsPointerOverGameObject())// ui가 아닌곳을 눌렀을때 ui 끄기
            {
                _stageManager.OffPopup();
            }


            Vector3 touchPos;
            Ray ray;
            RaycastHit hit;

            Vector3 touchPosToVector3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100);
            touchPos = Camera.main.ScreenToWorldPoint(touchPosToVector3);
            ray = Camera.main.ScreenPointToRay(touchPosToVector3);

            if (!EventSystem.current.IsPointerOverGameObject())// 
            {
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);



                    if (hit.collider.tag == "Upgrade_Obj")
                    {

                        _stageManager.SelecteTarget(hit.transform);
                        isClick = false;
                        if (TutorialManager._instance._tutorialLevel == 4)
                        {

                            TutorialManager._instance.Tutorial_Comple();
                            TutorialManager._instance.Tutorial(false);
                            if (Managers.Game.Money < 5)
                            {
                                Managers.Game.CalcMoney(3);
                            }
                        }
                    }
                    else if (hit.collider.tag == "WorkerBox")
                    {
                        Managers.Sound.Play("Effect_9");
                        int _landNum = hit.transform.GetComponent<WorkerBox>()._landNum;
                        _StageManager._landManagers[_landNum].AddStaff(hit.transform.gameObject);
                        //_stageManager.AddStaff(false, hit.transform.gameObject);

                    }
                    else if (hit.collider.tag == "WorkerBox_Init")
                    {
                        Managers.Sound.Play("Effect_9");
                        int _landNum = hit.transform.GetComponent<WorkerBox>()._landNum;
                        _StageManager._landManagers[_landNum].AddStaff(hit.transform.gameObject);
                        TutorialManager._instance.Tutorial_Comple();

                    }
                    else if (hit.collider.tag == "LabotoryManager")
                    {
                        _StageManager.OffPopup();
                        Managers.GameUI.Laboratory_Panel.SetActive(true);

                        LookTarget(hit.transform);
                        isClick = false;
                        //if (TutorialManager._instance._tutorialLevel == 8)
                        //{
                        //    TutorialManager._instance.Tutorial_Comple();
                        //}
                    }

                }
            }
        }

        else if (Input.GetMouseButton(0) && !isFix && !_stageManager.isCinema)
        {
            if (EventSystem.current.IsPointerOverGameObject()) // ui 터치시 화면 이동 안하도÷
            {

                isClick = false;
                //_stageManager.MachinePressFalse();
                return;
            }
            if (isClick)
            {
                _endXY = new Vector2(Input.mousePosition.x/*0f */, Input.mousePosition.y);

                _x = (Vector3.right * (_startXY.x - _endXY.x) * _moveSense).x;
                _y = (Vector3.up * (_startXY.y - _endXY.y) * _moveSense).y;

                Vector3 _horizon = Vector3.right * (_startXY.x - _endXY.x) * _moveSense;
                Vector3 _vertical = Vector3.up * (_startXY.y - _endXY.y) * _moveSense;

                Vector3 _delta = _startPos + _horizon + _vertical;

                transform.position = _delta;

            }
        }

        else if (Input.GetMouseButtonUp(0) && !_stageManager.isCinema)
        {
            isClick = false;
            _startXY = Vector2.zero;
            _endXY = _startXY;

        }

        if (isClick == false && !_stageManager.isCinema)
        {
            if (transform.localPosition.x < _limitPos.x)
            {
                transform.localPosition += Vector3.right * Time.deltaTime * returnSpeed;
            }
            if (transform.localPosition.x > _limitPos.y)
            {
                transform.localPosition -= Vector3.right * Time.deltaTime * returnSpeed;
            }
            if (transform.localPosition.y > _limitPos.w)
            {
                transform.localPosition -= Vector3.up * Time.deltaTime * returnSpeed;
            }
            if (transform.localPosition.y < _limitPos.z)
            {
                transform.localPosition += Vector3.up * Time.deltaTime * returnSpeed;
            }

        }


#elif !UNITY_EDITOR


        if (Input.touchCount > 0 && Input.touchCount < 2 && !_stageManager.isCinema)
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

                 if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);

                    //Debug.Log(hit.collider.tag);
                     if (hit.collider.tag == "Upgrade_Obj")
                    {

                        _stageManager.SelecteTarget(hit.transform);
                        isClick = false;
                        if (TutorialManager._instance._tutorialLevel == 4)
                        {
      
                            TutorialManager._instance.Tutorial_Comple();
                            TutorialManager._instance.Tutorial(false);
                        }
                    }
                    else if (hit.collider.tag == "WorkerBox")
                    {
                        Managers.Sound.Play("Effect_9");
                        int _landNum = hit.transform.GetComponent<WorkerBox>()._landNum;
                        _StageManager._landManagers[_landNum].AddStaff(hit.transform.gameObject);
                        //_stageManager.AddStaff(false, hit.transform.gameObject);

                    }
                    else if (hit.collider.tag == "WorkerBox_Init")
                    {
                        Managers.Sound.Play("Effect_9");
                        int _landNum = hit.transform.GetComponent<WorkerBox>()._landNum;
                        _StageManager._landManagers[_landNum].AddStaff(hit.transform.gameObject);
                        TutorialManager._instance.Tutorial_Comple();

                    }
                    else if (hit.collider.tag == "LabotoryManager")
                    {
                        _StageManager.OffPopup();
                        Managers.GameUI.Laboratory_Panel.SetActive(true);

                        LookTarget(hit.transform);
                        isClick = false;
                        if (TutorialManager._instance._tutorialLevel == 8)
                        {
                            TutorialManager._instance.Tutorial_Comple();
                        }
                    }
                }}
            }
            else if ((_touch.phase == TouchPhase.Moved || _touch.phase == TouchPhase.Stationary) && !isFix && !_stageManager.isCinema)
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

                    //if (Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).x >= _limitPos.x && Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).x <= _limitPos.y && Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).y >= _limitPos.z && Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f).y <= _limitPos.w)
                    //{
                        transform.position = Vector3.Lerp(transform.position, _delta, Time.deltaTime * 5f);
                    //}
                }
            }

            else if (_touch.phase == TouchPhase.Ended && !_stageManager.isCinema)
            {
                isClick = false;
                _startXY = Vector2.zero;
                _endXY = _startXY;
            }

          

        }

        if (isClick == false && !_stageManager.isCinema)
        {
            if (transform.localPosition.x < _limitPos.x)
            {
                transform.localPosition += Vector3.right * Time.deltaTime * returnSpeed;
            }
            if (transform.localPosition.x > _limitPos.y)
            {
                transform.localPosition -= Vector3.right * Time.deltaTime * returnSpeed;
            }
            if (transform.localPosition.y > _limitPos.w)
            {
                transform.localPosition -= Vector3.up * Time.deltaTime * returnSpeed;
            }
            if (transform.localPosition.y < _limitPos.z)
            {
                transform.localPosition += Vector3.up * Time.deltaTime * returnSpeed;
            }
        }


        if (Input.touchCount == 2 && !_stageManager.isCinema) //손가락 2개가 눌렸을 때
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
        //Vector3 _temppos = new Vector3(_target.position.x, _target.position.y, -_lookdistance)

        _lookdistance = (-_target.position.z - _targetDistance) * 1.156f;


        DOTween.Sequence()
            .Append(transform.DOMove(_target.position + Vector3.up * 5f + transform.forward * _lookdistance, 0.5f).SetEase(Ease.Linear))
            //.Append(transform.DOMove(_target.position + _lookOffset - transform.forward * _lookdistance, 0.5f).SetEase(Ease.Linear))
            //.Append(transform.DOMove(_target.transform.position + new Vector3(30f, 50f, 50f), 0.5f).SetEase(Ease.Linear))
            .AppendCallback(() => { isFix = true; })
            .OnComplete(() => isFix = false);
    }




}
