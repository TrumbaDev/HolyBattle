using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrentData : MonoBehaviour
{
    DBManager db;

    private Player _thisPlayer;

    private void Awake()
    {
        GameEventManager.OnGetStats += GetStatsHandler;
    }

    private void Start()
    {
        db = new DBManager();
        _thisPlayer = new Player();

        StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(2f);
        db.GetUserStats();
    }

    private void GetStatsHandler(UserData userData)
    {
        if (userData.error.isError)
        {
            Debug.Log(userData.error.errorText);
            return;
        }

        Debug.Log(userData.playerData.health);

        SetPlayerData(userData);
    }

    private void SetPlayerData(UserData userData)
    {
        _thisPlayer.armor = userData.playerData.armor;
        _thisPlayer.attack_speed = userData.playerData.attack_speed;
        _thisPlayer.base_damage = userData.playerData.base_damage;
        _thisPlayer.dexterity = userData.playerData.dexterity;
        _thisPlayer.health = userData.playerData.health;
        _thisPlayer.id = userData.playerData.id;
        _thisPlayer.intelligence = userData.playerData.intelligence;
        _thisPlayer.magic_resistance = userData.playerData.magic_resistance;
        _thisPlayer.move_speed = userData.playerData.move_speed;
        _thisPlayer.power = userData.playerData.power;

        SetNPCData(userData);
    }

    private void SetNPCData(UserData userData)
    {
        /*_thisPlayer.npcData.armor = userData.playerData.npcData.armor;
        _thisPlayer.npcData.attack_speed = userData.playerData.npcData.attack_speed;
        _thisPlayer.npcData.base_damage = userData.playerData.npcData.base_damage;
        _thisPlayer.npcData.dexterity = userData.playerData.npcData.dexterity;
        _thisPlayer.npcData.health = userData.playerData.npcData.health;
        _thisPlayer.npcData.intelligence = userData.playerData.npcData.intelligence;
        _thisPlayer.npcData.magic_resistance = userData.playerData.npcData.magic_resistance;
        _thisPlayer.npcData.move_speed = userData.playerData.npcData.move_speed;
        _thisPlayer.npcData.power = userData.playerData.npcData.power;*/
    }
}
