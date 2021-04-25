using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public GameObject poolObject;
    
    List<GameObject> pooledObjects;
    public int initPoolAmount;

    public GameObject GetPooledObject()
    {
        // Exist : return disabled object
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if(pooledObjects[i].activeInHierarchy == false)
            {
                return pooledObjects[i];
            }
        }

        // Not exist : Create, expand pool and return the object
        GameObject temp = CreateObject();
        return temp;
    }

    GameObject CreateObject()
    {
        GameObject temp = Instantiate(poolObject, Vector3.zero, Quaternion.identity, transform);
        temp.SetActive(false);
        pooledObjects.Add(temp);

        return temp;
    }

    void Awake()
    {
        SharedInstance = this;
        pooledObjects = new List<GameObject>();
    }

    void Start()
    {
        for(int i = 0; i < initPoolAmount; i++)
        {
            CreateObject();
        }
    }
}
