using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
class BuildSettings
{
    static BuildSettings()
    {
        //WebGL
        PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.WebGL.memorySize = 512;
    }
}

public class WebGLMemorySettings : MonoBehaviour
{
    
}
