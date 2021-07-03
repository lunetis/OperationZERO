using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixyMPBMController : MonoBehaviour
{
    [SerializeField]
    float cooldown;
    [SerializeField]
    Transform mpbmLaunchTransform;

    public ObjectPool MPBMObjectPool;

    void LaunchMissile()
    {
        if(GameManager.Instance.IsGameOver == true)
        {
            CancelInvoke();
            return;
        }

        GameObject mpbm = MPBMObjectPool.GetPooledObject();
        mpbm.transform.position = mpbmLaunchTransform.position;
        mpbm.transform.rotation = mpbmLaunchTransform.rotation;
        mpbm.SetActive(true);

        MPBM mpbmScript = mpbm.GetComponent<MPBM>();
        mpbmScript.Launch(GameManager.PlayerAircraft.transform.position);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("LaunchMissile", cooldown, cooldown);
    }
}
