//��������� ��� �������� ������ ������ ���� ������� ��������� ������
//����������� �� ���������� ����� � ������� ��� ���������� �� �������� �� ���� �����
public struct CharacterStats : IStats
{
    //���������� ����� ��� BaseStats � ������ Charcter
    public float health, armor, damage;
    public bool isImmortal;
    //���� ����� ��� CurrentStats � ������ Character
    float IStats.Health
    {
        get => this.health;
        set => this.health = value;
    }
    float IStats.Damage
    {
        get => this.damage;
        set => damage = value;
    }
    float IStats.Armor
    {
        get => this.armor;
        set => this.armor = value;
    }
    bool IStats.IsImmortal
    {
        get => this.isImmortal;
        set => this.isImmortal = value;
    }
}