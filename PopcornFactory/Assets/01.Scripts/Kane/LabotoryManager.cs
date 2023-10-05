using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class LabotoryManager : MonoBehaviour
{
    [SerializeField] float _power = 10f;
    public GameObject _lab_Product;
    public int[] _productType_Count;

    public Material[] _lab_Mat;


    [SerializeField] bool isSpawn = false;
    [Required]
    public Transform _spawnPos;
    public float _spawnInterval = 0.1f;

    public int[] _resourceList;

    //public GameObject[] _lab_Contents; //  Source List in Labotory Panel
    public GameObject[] _laboratory_list; // UI Contents

    [SerializeField] int _matNum = 0;
    public Material[] _popcornMats;

    UI_GameScene _gameUi;

    public GameObject[] _recipe = new GameObject[4];
    public bool[] _recipeNum = new bool[4];

    private void OnEnable()
    {
        if (_lab_Product == null) _lab_Product = Resources.Load<GameObject>("Lab_Product").gameObject;

        _productType_Count = new int[System.Enum.GetValues(typeof(Product.ProductType)).Length];

        if (_gameUi == null) _gameUi = Managers.GameUI;

        
        _resourceList = new int[_laboratory_list.Length];

        _spawnPos.GetComponent<Renderer>().enabled = false;
        // add Load Data
        // Recipe
        int _index = 0;
        for (int i = 0; i < _gameUi.Recipe_Content.transform.childCount; i++)
        {
            for (int j = 0; j < _gameUi.Recipe_Content.transform.GetChild(i).childCount; j++)
            {
                _recipe[_index] = _gameUi.Recipe_Content.transform.GetChild(i).GetChild(j).gameObject;
                _recipe[_index].SetActive(false);
                _index++;
            }
        }

        LoadRecipe();
        _gameUi.Recipe_Button.gameObject.SetActive(false);
        if (_recipeNum[0]) _gameUi.Recipe_Button.gameObject.SetActive(true);

        for (int i = 0; i < _recipeNum.Length; i++)
        {

            if (_recipeNum[i]) _recipe[i].SetActive(true);
        }


    }

    public void Init()
    {
        foreach (GameObject _obj in _laboratory_list)
        {
            _obj.SetActive(false);
        }

        StartCoroutine(Cor_Spawn());

    }



    public float _jumpPower = 5f;


    public void PushProduct(Transform _trans, float _moveInterval = 0.5f, Product.ProductType _type = Product.ProductType.Popcorn)
    {



        _trans.SetParent(transform);
        _trans.DOLocalJump(Vector3.zero, _jumpPower, 1, _moveInterval).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _productType_Count[(int)_type]++;


                Managers.Pool.Push(_trans.GetComponent<Poolable>());
            });

        //Managers.Game.CalcMoney(_trans.GetComponent<Product>()._price);
        //Managers.Game.PopText(_trans.GetComponent<Product>()._price, transform);


    }

    IEnumerator Cor_Spawn()
    {
        WaitForSeconds _term = new WaitForSeconds(_spawnInterval);
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval); //_term;
            if (isSpawn)
            {
                //Debug.Log("Cor spawn");
                if (_productType_Count[0] > 0 && _productType_Count[1] >= _resourceList[0] && _productType_Count[2] >= _resourceList[1])
                {
                    _productType_Count[0] -= 1;
                    _productType_Count[1] -= _resourceList[0];
                    _productType_Count[2] -= _resourceList[1];


                    if (_resourceList[0] == 0 && _resourceList[1] == 0) // basic
                    {
                        _matNum = 0;

                        if (_recipeNum[0] == false)
                        {
                            SaveRecipe(0);
                            _gameUi.Recipe_Button.gameObject.SetActive(true);
                            //if (TutorialManager._instance._tutorialLevel == 7)
                            //{
                            //    TutorialManager._instance.Tutorial();
                            //}

                        }
                    }
                    else if (_resourceList[0] == 1 && _resourceList[1] == 0) // choco
                    {


                        _matNum = Random.Range(0, 2) == 0 ? 0 : 1;
                        if (_matNum == 1) _productType_Count[1] -= 1;
                        SaveRecipe(1);
                    }
                    else if (_resourceList[0] == 2 && _resourceList[1] == 0) // choco
                    {

                        _matNum = 1; // Random.Range(0, 2) == 0 ? 0 : 1;
                        _productType_Count[1] -= 1;
                        SaveRecipe(1);
                    }
                    else if (_resourceList[0] == 0 && _resourceList[1] == 1) // strawberry
                    {
                        //_matNum = 2;
                        _matNum = Random.Range(0, 2) == 0 ? 0 : 2;
                        if (_matNum == 2) _productType_Count[2] -= 1;
                        SaveRecipe(2);
                    }
                    else if (_resourceList[0] == 0 && _resourceList[1] == 2) // strawberry
                    {
                        _matNum = 2;
                        //_matNum = Random.Range(0, 2) == 0 ? 0 : 2;
                        _productType_Count[_matNum] -= 1;
                        SaveRecipe(2);
                    }
                    else if (_resourceList[0] == 1 && _resourceList[1] == 1) // half
                    {
                        //_matNum = 3;
                        _matNum = Random.Range(0, 3);
                        if (_matNum != 0) _productType_Count[_matNum] -= 1;
                        SaveRecipe(3);
                    }

                    Transform _trans = Managers.Pool.Pop(_lab_Product, transform).transform;
                    _trans.position = _spawnPos.position;
                    _trans.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _trans.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.right * Random.Range(-1f, 1f)) * _power);
                    _trans.GetComponent<Renderer>().material = _popcornMats[_matNum];



                }

            }
        }


    }

    public void SaveRecipe(int _num)
    {
        //for (int i = 0; i < _recipeNum.Length; i++)
        //{
        if (_recipeNum[_num] == false)
        {
            _recipeNum[_num] = true;
            ES3.Save("Recipe_" + _num, _recipeNum[_num]);
            _recipe[_num].SetActive(true);
            _gameUi.Recipe_Button.transform.GetChild(0).gameObject.SetActive(true);
            //}
        }
    }
    public void LoadRecipe()
    {
        for (int i = 0; i < _recipeNum.Length; i++)
        {
            _recipeNum[i] = ES3.Load("Recipe_" + i, false);

        }
    }


    public void ResourceValue(int _num, float _value)
    {


        if ((int)_value != 2)
        {
            int _count = 0;
            for (int i = 0; i < _resourceList.Length; i++)
            {
                if (i != _num)
                    _count += _resourceList[i];
            }
            if ((_count + _value) > 2)
            {
                _count += (int)_value;
                for (int j = _count - 2; j >= 0; j--)
                {
                    if (j != _num && _count > 2)
                    {
                        _resourceList[j]--;
                        _laboratory_list[j].transform.Find("Slider").GetComponent<Slider>().value = _resourceList[j];
                        _laboratory_list[j].transform.Find("Ratio_Text").GetComponent<Text>().text = $"{50 * _resourceList[j]} %";
                    }
                }
            }


            _resourceList[_num] = (int)_value;
            _laboratory_list[_num].transform.Find("Slider").GetComponent<Slider>().value = _resourceList[_num];
            _laboratory_list[_num].transform.Find("Ratio_Text").GetComponent<Text>().text = $"{50 * _resourceList[_num]} %";
        }

        else if ((int)_value == 2)
        {
            for (int i = 0; i < _resourceList.Length; i++)
            {
                if (i != _num)
                {
                    _resourceList[i] = 0;
                    _laboratory_list[i].transform.Find("Slider").GetComponent<Slider>().value = _resourceList[i];
                    _laboratory_list[i].transform.Find("Ratio_Text").GetComponent<Text>().text = $"{50 * _resourceList[i]} %";
                }
                else
                {
                    _resourceList[i] = 2;
                    _laboratory_list[i].transform.Find("Slider").GetComponent<Slider>().value = _resourceList[i];
                    _laboratory_list[i].transform.Find("Ratio_Text").GetComponent<Text>().text = $"{50 * _resourceList[i]} %";
                }
            }
        }





    }

}
