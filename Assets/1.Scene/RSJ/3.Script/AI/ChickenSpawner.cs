using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChickenSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject chickenPrefab;

    public int chicken_num;

    [Header("积己 裹困")]
    public float xValue;
    public float zValue;

    //积己 困摹
    private float xPos;
    private float zPos;

    public override void OnStartServer()
    {
        if (!isServer) return;

        base.OnStartServer();
        StartCoroutine(GenerateChicken());
    }

    //private void Start()
    //{
    //    StartCoroutine(GenerateChicken());
    //}

    private IEnumerator GenerateChicken()
    {
        for(int i = 0; i < chicken_num; i++)
        {
            xPos = Random.Range(-xValue, xValue);
            zPos = Random.Range(-zValue, zValue);
            Vector3 spawnPos = new Vector3(xPos, 1f, zPos);
            GameObject chicken = Instantiate(chickenPrefab, spawnPos, Quaternion.Euler(0f, Random.Range(0, 360f), 0f));
            yield return new WaitForSeconds(0.1f);

            // Network Spawn
            NetworkServer.Spawn(chicken);

        }
    }

}
