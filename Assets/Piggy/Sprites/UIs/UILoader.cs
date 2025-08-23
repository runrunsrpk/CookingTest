using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoader : MonoBehaviour
{
    public static UILoader Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public GameObject LoadUI(string name)
    {
        string path = $"Prefabs/UIs/{name}";

        GameObject loadedPrefab = Resources.Load<GameObject>(path);
        
        if (loadedPrefab != null)
        {
            return Instantiate(loadedPrefab, transform);
        }
        else
        {
            Debug.LogError("Failed to load prefab at path: " + path);
            return null;
        }
    }

    public GameObject GetLoadedUI()
    {
        if(transform.childCount > 0)
        {
            return transform.GetChild(0).gameObject;
        }

        return null;
    }
}
