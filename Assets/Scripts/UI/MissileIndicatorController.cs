using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileIndicatorController : MonoBehaviour
{
    [SerializeField]
    ObjectPool mslIndicatorObjectPool;

    public void AddMissileIndicator(Missile missile)
    {
        GameObject obj = mslIndicatorObjectPool.GetPooledObject();
        obj.GetComponent<MissileIndicator>().Missile = missile;
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }
}
