//��� ����� generic ������� ���������� ����� ���� ��� ����������� �� ��� ���� ������
public interface IBuff<T> where T : IStats
{
    T ApplyBuff(T baseStats);
}
