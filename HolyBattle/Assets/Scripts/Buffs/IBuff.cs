//тот самый generic который активирует любой бафф вне зависимости от его типа данных
public interface IBuff<T> where T : IStats
{
    T ApplyBuff(T baseStats);
}
