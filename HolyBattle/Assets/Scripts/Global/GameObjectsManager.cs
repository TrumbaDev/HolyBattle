using OpenCover.Framework.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GameObjectsManager : MonoBehaviour
{
    public static GameObjectsManager _instance;
   
    public static GameObjectsManager m_instance;

    private List<GameObject> _allObjects = new List<GameObject>();

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }

    }

    public static void Register(GameObject gameObject)
    {
        if (!_instance._allObjects.Contains(gameObject))
            _instance._allObjects.Add(gameObject);
    }

    public static void Unregister(GameObject gameObject)
    {
        if (_instance._allObjects.Contains(gameObject))
            _instance._allObjects.Remove(gameObject);
    }

    public static GameObject[] GetGameObjects<T>()
    {
        List<GameObject> items = new List<GameObject>();

        foreach (var entry in _instance._allObjects)
        {
            if (entry.GetType() == typeof(T))
                items.Add(entry);
        }

        return items.ToArray();
    }

    public static GameObject[] GetGameObjectByTag(string tagName)
    {
        List<GameObject> items = new List<GameObject>();
        foreach (var entry in _instance._allObjects)
        {
            if (entry.gameObject.tag == tagName)
                items.Add(entry);
        }

        return items.ToArray();
    }

    public static GameObject[] GetGameObjectByName(string name)
    {
        List<GameObject> items = new List<GameObject>();
        foreach (var entry in _instance._allObjects)
        {
            if (entry.gameObject.name == name)
                items.Add(entry);
        }

        return items.ToArray();
    }

}
