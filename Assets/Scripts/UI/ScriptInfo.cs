using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ScriptInfo
{
    public string subtitleKey = "";
    public string name = "";
    public string side = "A";   // "A"lly, "E"nemy, "N"eutral
    public float preDelay = 0.5f;
    
    public bool isImportant = false;     // AWACS only
    public string invokeMethodName = "";
    public float invokeMethodDelay = 0;

    public bool isRemovable = true;     // Removable with ScriptManager.ClearScriptQueue()
}