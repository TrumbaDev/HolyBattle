using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//��������� �����/�������
public class TemporaryBuff : IBuff<IStats>
{
    //���, �� ���� ������ ���� � ��� BaseStats � CurrentStats
    private readonly IBuffable _owner;
    //���� ������� ������
    private readonly IBuff<IStats> _coreBuff;
    //����� ����� �����
    private readonly int _lifeTime;
    //����� ��� ��������� async ����� ������� ���������� Timer
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public TemporaryBuff(IBuffable owner, IBuff<IStats> coreBuff, int lifeTime)
    {
        //�� ��� ������� ��� � ������������ ��� ����������� �� ����� ������
        _owner = owner;
        _coreBuff = coreBuff;
        _lifeTime = lifeTime;
    }

    public IStats ApplyBuff(IStats baseStats)
    {
        //���������� ��������� ����
        var newStats = _coreBuff.ApplyBuff(baseStats);
        //��������� Timer
        Timer(_lifeTime);
        //��� ��� ���������� ����� �����, ��!
        //��� ��� ��� ������ ���� ������ ����� ��������� asnyc, �� ����� ����� ����� ��� �� ������� ������� �� ������������� ���������� ����
        //����� ������ ��� ��������� ��������� ����
        return newStats;
    }

    private async void Timer(int lifeTime)
    {
        //��������� ��� �� �����, ���� ���, �� �� ��������� Timer
        if (_cts.Token.IsCancellationRequested)
            return;
        //��� ���� �������� � async, ���� �� �� ������� ����� ������ � ����� ��� �� ���������,
        //�� async ������� ������, � �� �������� �����
        //���� �� ������������ ����� ����, �� �� ������� 2 ���� ��� �� ������ ����
        //����� ���� �������, ��� ����� �� �������, �� ��� ������� ������ 1 ��� ���� ���� ����� ����������
        _cts.Cancel(true);
        await Task.Delay(lifeTime);
        //������� ��������� ���� �� �����, � ������ ������� � � ����
        _owner.RemoveBuff(_coreBuff);
    }
}

public class Character : IBuffable
{
    //BaseStats ��������� ������ ����������� ����� ��������� �������� ��� �� ������ ���
    public CharacterStats BaseStats { get; }
    //������ ������� ����� ��������� ����� ���� �����/�������
    public IStats CurrentStats { get; private set; }
    //������ ������� ����������� ����/������
    private readonly List<IBuff<IStats>> _buffs = new List<IBuff<IStats>>();

    public Character(CharacterStats baseStats)
    {
        //���������� � ������ ���������� ������� � ������� ����� �����
        BaseStats = baseStats;
        CurrentStats = baseStats;
    }

    //����� ��� ����������� ������/��������
    public void AddBuff(IBuff<IStats> buff)
    {
        //��������� ���� � ����
        _buffs.Add(buff);
        //��������� ���� �� ���������
        ApplyBuffs();
        Debug.Log($"Buff added: {buff}");
    }

    public void RemoveBuff(IBuff<IStats> buff)
    {
        //����� ��� �� ����� Add, ������ �� ����� ������� � ���������� �������
        _buffs.Remove(buff);

        ApplyBuffs();
        Debug.Log($"Buff removed: {buff}");
    }

    private void ApplyBuffs()
    {
        //������������ ������� ����� � ������� ����� ����� ��������� ��� ����� �� �����
        CurrentStats = BaseStats;
        //����������� �� ����� ����� � ��������� ������ ����
        //���� �� ��������� � ���� ������ � ��� ������� �������� ��������� ������ �� ����������
        //� ������ ����� ��� �� ������ ������, �� ������������������ �� ������
        foreach (var buff in _buffs)
        {
            //� ������� ����� �� ����� �������� ��� ���������� ������ ��������������� �� ����������
            //���������� ��� ����� � ��������� generic � ��� �����, CurrentStatc ���� ���� IStats
            //� ��� ���������� ����� ����������� �� ���������� ���������� � ����� ������ ������ ���� ������ CurrentStats
            CurrentStats = buff.ApplyBuff(CurrentStats);
        }
    }
}

public class DamageBuff : IBuff<IStats>
{
    private float _damageBonus;

    public DamageBuff(float damageBonus)
    {
        //��� �������� ���������� ������ ����������� ��������
        //������������� - ����
        //������������� - ������
        _damageBonus = damageBonus;
    }

    public IStats ApplyBuff(IStats baseStats)
    {
        //�� ��� ������ ���������� ���������� ������
        //BaseStats �������� ���������� ������� ���� ����� � ����
        //���� � ��� � ����� ����� 5 ��� �� +10 ���� ��� ����������� 2 �������� �����, �� ��������� ����� 52 ����� � ����
        //�� ������ ��� ����� �� ������ �������� ��� +10 ������, �� � ����� ����� ��������� ��� �� ����� ��� �������� ����
        //������������� � ������ ���������� � ������ Character �urrentStats �� ���������� � BaseStats
        //�� ������ �������� ��������� ����� �� 10 � ��� ����� ���
        //����������� ����� 62 ������� ������
        var newStats = baseStats;
        newStats.Damage = Mathf.Max(newStats.Damage + _damageBonus, 0);

        return newStats;
    }
}

public class ImmortalityBuff : IBuff<IStats>
{
    public IStats ApplyBuff(IStats baseStats)
    {
        var newStats = baseStats;
        newStats.IsImmortal = true;

        return newStats;
    }
}

public class Buffs : MonoBehaviour
{
    //������� ���������� ��� ������������ �������� ���������� ������ ����� �� ����� ������ ��������� ����� ����
    private Character _myCharacter;
    //������� ���������� ����� � ���������� ��������� ��������� ������ ����
    public CharacterStats _characterStats;

    private void Start()
    {
        //��������� ���� �������
        var stats = new CharacterStats
        {
            health = 10f,
            damage = 15f,
            armor = 20f,
            isImmortal = false
        };
        //������� ��������� ������ � ����� ����������� ������ ������ ��������� BaseStats � CurrentStats
        Init(new Character(stats));
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
            //������� ��������� ������ ������� � ������� ����� �������������� � �������� ���������� �����
            var immortalityBuff = new ImmortalityBuff();
            //�������� � ����� ���������� ����� �� ����, ��� � �� ������� ����� ����������, � ��� �� �������� ���� ������� ����� ����� �������� ����
            var tempImmortalityBuff = new TemporaryBuff(_myCharacter, immortalityBuff, 3000);

            //��������� ��������� ���� � ����
            _myCharacter.AddBuff(tempImmortalityBuff);
            Debug.Log($"Temporal Immortality Enabled: {_myCharacter.CurrentStats.IsImmortal}");
        }
    }

    public void Init(Character character)
    {
        _myCharacter = character;
    }


}