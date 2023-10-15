//using OpenCover.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GameObjectsManager : MonoBehaviour
{
    public static GameObjectsManager _instance;
   
    public static GameObjectsManager m_instance;

    private List<GameObject> _allObjects = new List<GameObject>();
    private List<GameObject> _playerGrid = new List<GameObject>();

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

    #region WorkWithGrid
    //ìåòîäû êàñàþùèåñÿ ñåòêè ñïàâíà èãðîêà

    public static void RegisterGrid(GameObject gameObject)
    {
        if (!_instance._playerGrid.Contains(gameObject))
            _instance._playerGrid.Add(gameObject);
    }

    public static void UnregisterGrid(GameObject gameObject)
    {
        if (_instance._playerGrid.Contains(gameObject))
            _instance._playerGrid.Remove(gameObject);
    }

    public static int TakeGridCount()
    {
        List<GameObject> items = new List<GameObject>();
        foreach (var entry in _instance._playerGrid)
        {
            items.Add(entry);
        }

        Array _items = items.ToArray();

        return _items.Length;
    }

    public static GameObject[] GetNPCGridByName(string _gridTeam, string _gridPos)//есть на моей линии енеми или нет
    {
        List<GameObject> _itemsLine = new List<GameObject>();
        List<GameObject> _itemsNotLine = new List<GameObject>();

        foreach (var entry in _instance._playerGrid)
        {
            PlayerGrid _entryGridFunction = entry.gameObject.GetComponent<PlayerGrid>();

            if (_entryGridFunction._teamNum != _gridTeam && _entryGridFunction._xPos == _gridPos)
            {
                List<GameObject> _npcGameObject = _entryGridFunction._NPCGrid;

                foreach (var npc in _npcGameObject)
                {
                    _itemsLine.Add(npc);
                }
            }

            else if (_entryGridFunction._teamNum != _gridTeam && _entryGridFunction._xPos != _gridPos)
            {
                List<GameObject> _npcGameObject = _entryGridFunction._NPCGrid;

                foreach (var npc in _npcGameObject)
                {
                    _itemsNotLine.Add(npc);
                }
            }
        }

        if (_itemsLine.Count > 0)
        {
            return _itemsLine.ToArray();
        }
        else
        {
            return _itemsNotLine.ToArray();
        }
    }

    #endregion
}
