using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//Временные баффы/дебаффы
public class TemporaryBuff : IBuff<IStats>
{
    //Тот, на кого вешаем бафф и его BaseStats и CurrentStats
    private readonly IBuffable _owner;
    //Бафф который вешаем
    private readonly IBuff<IStats> _coreBuff;
    //Время жизни баффа
    private readonly int _lifeTime;
    //Нужно для остановки async через который реализован Timer
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public TemporaryBuff(IBuffable owner, IBuff<IStats> coreBuff, int lifeTime)
    {
        //Ну тут понятно что в конструкторе все присваеваем по своим местам
        _owner = owner;
        _coreBuff = coreBuff;
        _lifeTime = lifeTime;
    }

    public IStats ApplyBuff(IStats baseStats)
    {
        //Навешиваем временный бафф
        var newStats = _coreBuff.ApplyBuff(baseStats);
        //Запускаем Timer
        Timer(_lifeTime);
        //Вот тут возвращаем новые статы, но!
        //Так как код дойдет сюда только после окончания asnyc, то новые статы будут как бы старыми статами до использования временного бафа
        //Иначе говоря тут кончается временный бафф
        return newStats;
    }

    private async void Timer(int lifeTime)
    {
        //Проверяем жив ли токен, если нет, то не запускаем Timer
        if (_cts.Token.IsCancellationRequested)
            return;
        //Тут была проблема с async, если мы не трогали токен вообще и никак его не проверяли,
        //то async работал всегда, а не заданное время
        //Если не использовали метод ниже, то он работал 2 раза или ни одного раза
        //Метод ниже говорит, что токен мы убиваем, но при мертвом токене 1 раз коду ниже можно отработать
        _cts.Cancel(true);
        await Task.Delay(lifeTime);
        //Удаляем временный бафф из листа, а значит удаляем и с чара
        _owner.RemoveBuff(_coreBuff);
    }
}

public class Character : IBuffable
{
    //BaseStats постоянно хранит изначальные статы персонажа выданные ему на начало боя
    public CharacterStats BaseStats { get; }
    //Хранит текущие статы персонажа после всех бафов/дебафов
    public IStats CurrentStats { get; private set; }
    //Хранит текущие действующие бафы/дебафы
    private readonly List<IBuff<IStats>> _buffs = new List<IBuff<IStats>>();

    public Character(CharacterStats baseStats)
    {
        //Изначально в момент объявления текущие и базовые статы равны
        BaseStats = baseStats;
        CurrentStats = baseStats;
    }

    //Метод для навешивания баффов/дебаффов
    public void AddBuff(IBuff<IStats> buff)
    {
        //Добавляем бафф в лист
        _buffs.Add(buff);
        //Применяем бафф на персонажа
        ApplyBuffs();
        Debug.Log($"Buff added: {buff}");
    }

    public void RemoveBuff(IBuff<IStats> buff)
    {
        //Ремув тот же самый Add, только из листа удаляем и навешиваем остатки
        _buffs.Remove(buff);

        ApplyBuffs();
        Debug.Log($"Buff removed: {buff}");
    }

    private void ApplyBuffs()
    {
        //Приравниваем текущие статы к базовым чтобы потом применить все баффы из листа
        CurrentStats = BaseStats;
        //Пробегаемся по всему листу и применяем каждый бафф
        //Судя по комментам к коду откуда я это спиздил большого выделения памяти не происходит
        //А значит похуй что мы бегаем циклом, на производительность не влияет
        foreach (var buff in _buffs)
        {
            //У каждого баффа из листа вызываем его реализацию метода унаследованного из интерфейса
            //Собственно для этого и затевался generic в том числе, CurrentStatc поле типа IStats
            //А все конкретные баффы наследуются от интерфейса работающем с типом данных равным типу данных CurrentStats
            CurrentStats = buff.ApplyBuff(CurrentStats);
        }
    }
}

public class DamageBuff : IBuff<IStats>
{
    private float _damageBonus;

    public DamageBuff(float damageBonus)
    {
        //При создании экзмепляра класса присваиваем значение
        //Положительные - бафф
        //Отрицательные - дебафф
        _damageBonus = damageBonus;
    }

    public IStats ApplyBuff(IStats baseStats)
    {
        //Ну тут просто навешиваем количество дамага
        //BaseStats содержит изначально сколько было урона у чара
        //Если у нас в листе лежит 5 раз по +10 дама при изначальных 2 единицах урона, то результат будет 52 урона у чара
        //На шестой раз когда мы нажмем накинуть баф +10 дамага, то в листе будет храниться что мы шесть раз накинули бафф
        //Следовательно в начале выполнения в классе Character СurrentStats мы прировняли к BaseStats
        //На каждой итерации увеличили дамаг на 10 и так шесть раз
        //Результатом будет 62 единицы дамага
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
    //Создаем переменную для последующего создания экземпляра класса чтобы из этого класса применять любой бафф
    private Character _myCharacter;
    //Создаем переменную чтобы в последуюем заполнить структуру статми чара
    public CharacterStats _characterStats;

    private void Start()
    {
        //Заполняем чара статами
        var stats = new CharacterStats
        {
            health = 10f,
            damage = 15f,
            armor = 20f,
            isImmortal = false
        };
        //Создаем экзмепляр класса и через конструктор внутри класса заполняем BaseStats и CurrentStats
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
            //Создаем экземпляр класса который в будущем будет использоваться в качестве временного баффа
            var immortalityBuff = new ImmortalityBuff();
            //Передаем в класс временного баффа на кого, что и на сколько будем навешивать, а так же кешируем этот готовый класс чтобы навесить бафф
            var tempImmortalityBuff = new TemporaryBuff(_myCharacter, immortalityBuff, 3000);

            //Добавляем временный бафф в лист
            _myCharacter.AddBuff(tempImmortalityBuff);
            Debug.Log($"Temporal Immortality Enabled: {_myCharacter.CurrentStats.IsImmortal}");
        }
    }

    public void Init(Character character)
    {
        _myCharacter = character;
    }


}