using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class TemporaryBuff : IBuff
{
    private readonly IBuffable _owner;
    private readonly IBuff _coreBuff;
    private readonly int _lifeTime;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public TemporaryBuff(IBuffable owner, IBuff coreBuff, int lifeTime)
    {
        _owner = owner;
        _coreBuff = coreBuff;
        _lifeTime = lifeTime;
    }

    public CharacterStats ApplyBuff(CharacterStats baseStats)
    {
        var newStats = _coreBuff.ApplyBuff(baseStats);

        Timer(_lifeTime);

        return newStats;
    }

    private async void Timer(int lifeTime)
    {
        if(_cts.Token.IsCancellationRequested) 
            return;

        _cts.Cancel();
        await Task.Delay(lifeTime);
        _owner.RemoveBuff(_coreBuff);
    }
}

public class Character : IBuffable
{
    public CharacterStats BaseStats { get; }
    public CharacterStats CurrentStats { get; private set; }

    private readonly List<IBuff> _buffs = new List<IBuff>();

    public Character(CharacterStats baseStats)
    {
        BaseStats = baseStats;
        CurrentStats = baseStats;
    }

    public void AddBuff(IBuff buff)
    {
        _buffs.Add(buff);

        ApplyBuffs();
        Debug.Log($"Buff added: {buff}");
    }

    public void RemoveBuff(IBuff buff)
    {
        _buffs.Remove(buff);

        ApplyBuffs();
        Debug.Log($"Buff removed: {buff}");
    }

    private void ApplyBuffs()
    {
        CurrentStats = BaseStats;

        foreach (var buff in _buffs)
        {
            CurrentStats = buff.ApplyBuff(CurrentStats);
        }
    }
}

public class DamageBuff : IBuff
{
    private float _damageBonus;

    public DamageBuff(float damageBonus)
    {
        _damageBonus = damageBonus;
    }

    public CharacterStats ApplyBuff(CharacterStats baseStats)
    {
        var newStats = baseStats;
        newStats.Damage = Mathf.Max(newStats.Damage + _damageBonus, 0);

        return newStats;
    }
}

public class ImmortalityBuff : IBuff
{
    public CharacterStats ApplyBuff(CharacterStats baseStats)
    {
        var newStats = baseStats;
        newStats.IsImmortal = true;

        return newStats;
    }
}

public class Buffs : MonoBehaviour
{
    private Character _myCharacter;


    private void Start()
    {
        var stats = new CharacterStats
        {
            Health = 10f,
            Armor = 5f,
            Damage = 2f,
            IsImmortal = false
        };

        Init(new Character(stats));

        Debug.Log($"Charcter initialized. Damage: {_myCharacter.CurrentStats.Damage}, Is immortal: {_myCharacter.CurrentStats.IsImmortal}");
    }
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _myCharacter.AddBuff(new ImmortalityBuff());

            Debug.Log($"Immortality Enabled: {_myCharacter.CurrentStats.IsImmortal}");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _myCharacter.AddBuff(new DamageBuff(10f));

            Debug.Log($"Damage: {_myCharacter.CurrentStats.Damage}");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            var immortalityBuff = new ImmortalityBuff();
            var tempImmortalityBuff = new TemporaryBuff(_myCharacter, immortalityBuff, 3000);

            _myCharacter.AddBuff(tempImmortalityBuff);
            Debug.Log($"Temporal Immortality Enabled: {_myCharacter.CurrentStats.IsImmortal}");
        }
    }

    public void Init(Character character)
    {
        _myCharacter = character;   
    }

    
}
