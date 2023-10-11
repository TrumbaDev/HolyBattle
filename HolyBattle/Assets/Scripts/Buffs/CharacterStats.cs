//—труктура дл€ хранени€ статов любого чара включа€ основного игрока
//Ќаследуетс€ от интерфейса чтобы в будущем при реализации не зависить от типа статы
public struct CharacterStats : IStats
{
    //переменные нужны дл€ BaseStats в классе Charcter
    public float health, armor, damage;
    public bool isImmortal;
    //пол€ нужны дл€ CurrentStats в классе Character
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