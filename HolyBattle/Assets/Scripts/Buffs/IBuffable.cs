//интерфейс нужный для реализации каждого конкретного бафа (к примеру баф дамага в классе DamageBuff)
public interface IBuffable
{
    void AddBuff(IBuff<IStats> buff);
    void RemoveBuff(IBuff<IStats> buff);
}