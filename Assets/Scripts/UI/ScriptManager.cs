using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Xml;
using System;
using TMPro;

[Serializable]
public class ScriptData
{
    public List<ScriptInfo> scripts;
}

public class ScriptManager : MonoBehaviour
{
    ScriptData scriptData;

    TextAsset scriptJSONAsset;
    TextAsset subtitleXMLAsset;
    XmlDocument subtitleXMLDocument;
    
    string subtitleFormat = "<size=24><mspace=15><color={0}><b><<</mspace=15></color=#ff4444></b><size=30> {1} <size=24><mspace=15><color={0}><b>>>";

    [Header("UI")]
    [SerializeField]
    GameObject scriptUI;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI subtitleText;
    [SerializeField]
    WarningSignController warningSignController;

    [Space(10)]
    [SerializeField]
    GameObject portraitUI;
    [SerializeField]
    RawImage portraitImage;

    [SerializeField]
    Color allyColor = new Color(0.25f, 0.25f, 1);
    [SerializeField]
    Color enemyColor = new Color(1, 0.25f, 0.25f);
    [SerializeField]
    Color neutralColor = new Color(1, 1, 0.25f);

    [Header("Audio")]
    [SerializeField]
    AudioSource scriptAudioSource;
    [SerializeField]
    AudioSource transmissionAudioSource;

    // Queue
    LinkedList<ScriptInfo> scriptQueue;

    bool isPrintingScript;
    ScriptInfo currentScript;

    // Addressable
    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<AudioClip> audioClipHandle;
    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Texture> portraitHandle;
    

    public bool AddressableResourceExists(object key, Type type = null)
    {
        foreach (var l in Addressables.ResourceLocators)
        {
            IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locs;
            if (l.Locate(key, type, out locs)) return true;
        }
        return false;
    }

    // Scripts
    ScriptInfo SearchScriptInfoByKey(string scriptKey)
    {
        foreach(ScriptInfo script in scriptData.scripts)
        {
            if(script.subtitleKey == scriptKey) return script;
        }
        return null;
    }

    public void AddScript(string scriptKey)
    {
        scriptQueue.AddLast(SearchScriptInfoByKey(scriptKey));
    }

    public void AddScript(List<string> scriptKeyList)
    {
        foreach(string scriptKey in scriptKeyList)
        {
            scriptQueue.AddLast(SearchScriptInfoByKey(scriptKey));
        }
    }

    public void AddScriptAtFront(string scriptKey)
    {
        scriptQueue.AddFirst(SearchScriptInfoByKey(scriptKey));
    }

    public void ClearScriptQueue(bool clearNotRemovableScripts = false)
    {
        if(clearNotRemovableScripts == true)
        {
            scriptQueue.Clear();
        }
        else
        {
            var node = scriptQueue.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.isRemovable == true)
                {
                    scriptQueue.Remove(node);
                }
                node = next;
            }
        }
    }


    Color GetColorBySide(string sideString)
    {
        switch(sideString)
        {
            case "A": return allyColor;
            case "E": return enemyColor;
            case "N": return neutralColor;
            default:  return allyColor;
        }
    }

    public string GetSubtitleText(string subtitleKey)
    {
        XmlNode subtitleNode = subtitleXMLDocument.SelectSingleNode("subtitle/" + subtitleKey);

        if(subtitleNode == null) return ""; // Exception

        return subtitleNode.InnerText;
    }


    void SetScript()
    {
        // Dequeue
        currentScript = scriptQueue.First.Value;
        scriptQueue.RemoveFirst();

        string subtitleKey = currentScript.subtitleKey;

        // Name
        Color textColor = GetColorBySide(currentScript.side);
        nameText.text = currentScript.name;
        nameText.color = textColor;

        // Subtitle
        string colorHexCode = "#" + ColorUtility.ToHtmlStringRGB(textColor);
        string subtitle = GetSubtitleText(currentScript.subtitleKey);
        subtitleText.text = string.Format(subtitleFormat, colorHexCode, subtitle);

        // Portrait
        string portraitKey = currentScript.name;
        if(AddressableResourceExists(portraitKey) == true)
        {
            portraitHandle = Addressables.LoadAssetAsync<Texture>(portraitKey);
            portraitHandle.Completed += (operationHandle) =>
            {
                portraitUI.SetActive(true);
                portraitImage.texture = operationHandle.Result;
            };
        }
        else
        {
            portraitUI.SetActive(false);
        }
        

        // Get AudioClip
        audioClipHandle = Addressables.LoadAssetAsync<AudioClip>(subtitleKey);
        audioClipHandle.Completed += (operationHandle) =>
        {
            AudioClip audioClip = operationHandle.Result;
            scriptAudioSource.clip = audioClip;
        };
        
        if(currentScript.isImportant == true)
        {
            Invoke("PlayTransmissionAudio", currentScript.preDelay);
        }
        else
        {
            Invoke("ShowScript", currentScript.preDelay);
        }
        
    }

    void PlayTransmissionAudio()
    {
        transmissionAudioSource.Play();
        Invoke("ShowScript", transmissionAudioSource.clip.length);
    }

    void ShowScript()
    {
        scriptUI.SetActive(true);
        if(currentScript.isImportant == true)
        {
            warningSignController.enabled = true;
        }
        scriptAudioSource.Play();
        
        if(currentScript.invokeMethodName != string.Empty)
        {
            GameManager.MissionManager.InvokeMethod(currentScript.invokeMethodName, currentScript.invokeMethodDelay);
        }
        Invoke("HideScript", scriptAudioSource.clip.length);
    }

    void HideScript()
    {
        scriptUI.SetActive(false);
        isPrintingScript = false;
        warningSignController.enabled = false;

        Addressables.Release(audioClipHandle);

        if(portraitUI.activeInHierarchy == true)
        {
            Addressables.Release(portraitHandle);
            portraitUI.SetActive(false);
        }

        currentScript = null;
    }

    public void PlayCutsceneAudio(string subtitleKey)
    {
        // Get AudioClip
        audioClipHandle = Addressables.LoadAssetAsync<AudioClip>(subtitleKey);
        audioClipHandle.Completed += (operationHandle) =>
        {
            AudioClip audioClip = operationHandle.Result;
            scriptAudioSource.clip = audioClip;
            scriptAudioSource.Play();
            if(currentScript.isImportant == true)
            {
                Invoke("ReleaseCutsceneAudio", audioClip.length);
            }
        };
    }

    void ReleaseCutsceneAudio()
    {
        Addressables.Release(audioClipHandle);
    }

    void Awake()
    {
        scriptQueue = new LinkedList<ScriptInfo>();
        scriptUI.SetActive(false);

    }

    void Start()
    {
        scriptJSONAsset = GameManager.MissionManager.MissionInfo.ScriptJSON;
        subtitleXMLAsset = GameManager.MissionManager.MissionInfo.GetScriptXML();
        
        // Load Subtitle XML
        subtitleXMLDocument = new XmlDocument();
        subtitleXMLDocument.LoadXml(subtitleXMLAsset.text);

        // Load Script JSON
        scriptData = JsonUtility.FromJson<ScriptData>(scriptJSONAsset.text);
    }

    void Update()
    {
        if(isPrintingScript == false && scriptQueue.Count > 0)
        {
            SetScript();
            isPrintingScript = true;
        }
    }
}