using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    #region CubePool
    Stack<GameObject> footPrints = new Stack<GameObject>();
    GameObject tempFootPrint;
    int generatedFootPrint;
    [SerializeField] private int firstSpawnCount;
    [SerializeField] private GameObject footPrintPrefab;

    [SerializeField] private Transform readyToUseContainer;
    [SerializeField] private Transform usingContainer;

    #endregion

    //Singleton
    public static ObjectPooling instance;
    
    private void Awake()
    {
        AwakeMethods();
    }
    
    #region Awake Methods

    void AwakeMethods()
    {
        SetInstance();
    }

    void SetInstance()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    
    
    #endregion

    void Start()
    {
        StartMethods();
    }

    #region StartMethods

    void StartMethods()
    {
        GenerateFootPrints(firstSpawnCount);
    }

    #endregion
    void GenerateFootPrints(int footPrintCount)
    {
        for (int i = 0; i < footPrintCount; i++)
        {
            tempFootPrint = Instantiate(footPrintPrefab, Vector3.zero, Quaternion.identity, readyToUseContainer);
            tempFootPrint.name = "CubeParticle  " + generatedFootPrint.ToString();
            generatedFootPrint++;
            footPrints.Push(tempFootPrint);
            tempFootPrint.SetActive(false);
        }
    }

    public GameObject SpawnFromPool(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (footPrints.Count < 1)
        {
            GenerateFootPrints(10);
        }
        tempFootPrint = footPrints.Pop();
        tempFootPrint.transform.SetParent(usingContainer);
        tempFootPrint.transform.position = spawnPosition;
        tempFootPrint.transform.rotation = spawnRotation;
        tempFootPrint.SetActive(true);
        return tempFootPrint;
    }

    public void ClearAllFootPrints()
    {
        Debug.Log("FootPrints Cleared!");
        while(usingContainer.childCount > 0)
        {
            tempFootPrint = usingContainer.GetChild(0).gameObject;
            tempFootPrint.transform.SetParent(readyToUseContainer);
            footPrints.Push(tempFootPrint);
            tempFootPrint.SetActive(false);
        }
    }

    public void BackToThePool(GameObject particleCubeObject)
    {
        tempFootPrint = particleCubeObject;
        tempFootPrint.SetActive(false);
        tempFootPrint.transform.SetParent(readyToUseContainer);
        footPrints.Push(tempFootPrint);
    }
}
