using UnityEditor;
using UnityEngine;

public class AttackBehaviour : INPCBehaviour
{
    private NPC _npc;

    public void Enter()
    {
        _npc.SetAnimationNPC("bool", "Attack", true);
    }

    public void Exit()
    {
        _npc.SetAnimationNPC("bool", "Attack", false);
    }

    public void Update()
    {

    }

    public AttackBehaviour(NPC npc)
    {
        _npc = npc;
    }
}
