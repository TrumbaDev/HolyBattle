using System;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static Action<GameObject, GameObject> GridDestroy; //при смерти НПС, строкой, передаем имя нужной сетки, та удаляет НПС из своего листа и дестроится если лист стал пустым
    public static Action StartMovementAction; //объявляет о старте игры, меняет булева в Movement
    //public static Action <GameObject, GameObject> SpawnOnStartInGrid; //передает значения в GameObjectsManager об НПС которые появятся на сетке
    public static Action StopPrefabEvent;
}
