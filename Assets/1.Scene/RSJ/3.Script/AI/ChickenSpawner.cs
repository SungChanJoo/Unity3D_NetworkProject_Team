using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    [SerializeField] private GameObject chickenPrefab;

    public int chicken_num;

    [Header("���� ����")]
    public float xValue;
    public float zValue;

    //���� ��ġ
    private float xPos;
    private float zPos;

    private void Start()
    {
        StartCoroutine(GenerateChicken());
    }

    private IEnumerator GenerateChicken()
    {
        for(int i = 0; i < chicken_num; i++)
        {
            xPos = Random.Range(-xValue, xValue);
            zPos = Random.Range(-zValue, zValue);
            Vector3 spawnPos = new Vector3(xPos, 1f, zPos);
            GameObject chicken = Instantiate(chickenPrefab, spawnPos, Quaternion.Euler(0f, Random.Range(0, 360f), 0f));
            yield return new WaitForSeconds(0.1f);
        }
    }

}
