//��������� ������ ��� ���������� ������� ����������� ���� (� ������� ��� ������ � ������ DamageBuff)
public interface IBuffable
{
    void AddBuff(IBuff<IStats> buff);
    void RemoveBuff(IBuff<IStats> buff);
}