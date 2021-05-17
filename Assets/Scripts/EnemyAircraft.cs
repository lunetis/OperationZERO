using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAircraft : TargetObject
{
    [SerializeField]
    float destroyDelay = 1;

    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    Transform smokeTransformParent;
    
    protected override void DestroyObject() 
    {
        CommonDestroyFunction();
        Invoke("DelayedDestroy", destroyDelay);
    }

    void DelayedDestroy()
    {
        GameObject obj = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        obj.transform.localScale *= 3;
        Destroy(gameObject);
    }

    public override void OnDamage(float damage, int layer)
    {
        base.OnDamage(damage, layer);

        for(int i = 0; i < smokeTransformParent.childCount; i++)
        {
            GameManager.Instance.CreateDamageSmokeEffect(smokeTransformParent.GetChild(i));
        }
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        transform.Translate(new Vector3(0, 0, moveSpeed * Time.deltaTime));
    }
}
