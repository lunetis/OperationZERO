using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectInfo", menuName = "Scriptable Object Asset/ObjectInfo")]
public class ObjectInfo : ScriptableObject
{
    [SerializeField]
    string objectName;
    [SerializeField]
    string objectNickname;
    [SerializeField]
    int score;
    [SerializeField]
    int hp;
    [SerializeField]
    bool mainTarget;

    [SerializeField]
    float warningDistance;

    public string ObjectName
    {
        get { return objectName; }
    }
    public string ObjectNickname
    {
        get { return objectNickname; }
    }
    public int Score
    {
        get { return score; }
    }
    public int HP
    {
        get { return hp; }
    }
    public bool MainTarget
    {
        get { return mainTarget; }
    }

    public float WarningDistance
    {
        get { return warningDistance; }
    }
}
