//Интерфейс необходимый для того чтобы IBuff был generic и работал не с конкретным типом статы
//а с полем типа IStats
public interface IStats
{
    float Health { get; set; }
    float Damage { get; set; }
    float Armor { get; set; }
    bool IsImmortal { get; set; }
}
