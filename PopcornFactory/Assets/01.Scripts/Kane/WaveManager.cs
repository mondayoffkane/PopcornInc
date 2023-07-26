using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public GameObject[] EnemyPrefs;

    public List<Enemy> _enemyList;


    public float _spawnInterval = 0.2f;


    public int _waveLevel;
    public float _corInterval = 1f;

    // ===============================================
    public enum WaveState
    {
        Wait,
        Wave,
        Clear,
        Fail


    }
    public WaveState _waveState;
    Transform[] _spawnPos;

    // ================================================

    private void Start()
    {
        int _count = transform.childCount;
        _spawnPos = new Transform[_count];

        for (int i = 0; i < _count; i++)
        {
            _spawnPos[i] = transform.GetChild(i);
        }

        StartCoroutine(Cor_Update());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _waveState = WaveState.Wait;                
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _waveState = WaveState.Wave;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _waveState = WaveState.Clear;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _waveState = WaveState.Fail;
        }
    }



    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return new WaitForSeconds(_corInterval);

            switch (_waveState)
            {
                case WaveState.Wait:

                    break;

                case WaveState.Wave:

                    Transform _enemyObj = Instantiate(EnemyPrefs[_waveLevel]).transform;
                    _enemyObj.position = _spawnPos[Random.Range(0, _spawnPos.Length)].position;


                    break;

                case WaveState.Clear:

                    break;


                case WaveState.Fail:

                    break;


                default:

                    break;
            }
        }


    }
}
