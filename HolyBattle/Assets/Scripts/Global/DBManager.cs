using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
public class UserData
{
    public Player playerData = new Player();
    //Создали экземпляр класса чтобы отсюда потом выдать NPC
    public NPCData npcData = new NPCData();
    public Error error = new Error();
}

public class Error
{
    public string errorText;
    public bool isError;
}

public class Player
{
    public int id;
    public string login;
    public float power, dexterity, intelligence, health, base_damage, attack_speed, armor, magic_resistance, move_speed;
    //public string NameLvlNPC1, NameLvlNPC2, NameLvlNPC3, NameLvlNPC4;
    //public NPC[] npcData;

    //public Player(string nick, float power, float dexterity, float intelligence, float health, float base_damage, float attack_speed, float armor, float magic_resistance, float move_speed)
    //{
    //    login = nick;
        //this.power = power;
        //this.dexterity = dexterity;
        //this.intelligence = intelligence;
        //this.health = health;
        //this.base_damage = base_damage;
        //this.attack_speed = attack_speed;
        //this.armor = armor;
        //this.magic_resistance = magic_resistance;
        //this.move_speed = move_speed;
    //}
    //public void SetNickname(string name) => login = name;
    //public void SetPower(float power) => this.power = power;
    //public void SetDexterity(float dexterity) => this.dexterity = dexterity;
    //public void SetIntelligence(float intelligence) => this.intelligence = intelligence;
    //public void SetHealth(float health) => this.health = health;
    //public void SetBaseDamage(float base_damage) => this.base_damage = base_damage;
    //public void SetAttackSpeed(float attack_speed) => this.attack_speed = attack_speed;
    //public void SetArmor(float armor) => this.armor = armor;
    //public void SetMagicResistance(float magic_resistance) => this.magic_resistance = magic_resistance;
    //public void SetMoveSpeed(float move_speed) => this.move_speed = move_speed;
}

//Создали класс для хранения переменных которые придут с PHP
public class NPCData
{
    public int num, PlayerID, Lvl, Rare, Position;
    public string NameNPC;
}


//Класс необходимый для запуска www.SendWebRequest() при помощи await
//Т.к. класс DBManager не унаследован от MonoBehaviour, то Coroutine запустить не представляется возможным. А по умолчанию метод www.SendWebRequest() не содержит определения GetAwaiter()
//Без определения GetAwaiter() нельзя ассинхронно запускать методы
public static class UnityWebRequestExtension
{
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
    {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new();
        reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

        if (reqOp.isDone)
            tsc.TrySetResult(reqOp.webRequest.result);

        return tsc.Task.GetAwaiter();
    }
}

public class DBManager
{
    private UserData _userData = new UserData();
    private string _targetUrl = "http://localhost/HolyBattle/index.php";

    private enum RequestType
    {
        logging, register, save, get_stats
    }

    private string GetUserData(UserData data)
    {
        return JsonUtility.ToJson(data);
    }

    private UserData SetUserData(string data)
    {
        //Принятый ответ в формате JSON преобразовали в переменные которые находятся в классах
        return JsonUtility.FromJson<UserData>(data);
    }

    public void Login(string login, string password)
    {
        if (IsValidStr(login) && IsValidStr(password))
        {
            Logging(login, password);
        }
        else
        {
            //_userData.error.errorText = "To small length";
        }
    }

    private void Logging(string login, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("type", RequestType.logging.ToString());
        form.AddField("login", login);
        form.AddField("password", password);
        SendData(form, RequestType.logging);
    }

    public void Registration(string login, string password, string passwordConfirm, string email)
    {

        if (IsValidStr(login) && IsValidStr(password) && IsValidStr(passwordConfirm) && IsValidEmail(email))
        {
            if (password == passwordConfirm)
                Registering(login, password, passwordConfirm, email);
            else
                _userData.error.errorText = "Пароли не совпадают";
        }
        else
        {
            //userData.error.errorText = "To small length";
        }
    }

    private void Registering(string login, string password, string passwordConfirm, string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("type", RequestType.register.ToString());
        form.AddField("login", login);
        form.AddField("password", password);
        form.AddField("password_confirm", passwordConfirm);
        form.AddField("email", email);
        SendData(form, RequestType.register);
    }

    public void SaveData(int id)
    {
        SaveProgress(id);
    }

    private void SaveProgress(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("type", RequestType.save.ToString());
        form.AddField("id", id);
        SendData(form, RequestType.save);
    }

    public void GetUserStats()
    {
        //Заполнили форму которая улетит в PHP скрипт для получения данных
        //_userData.playerData = new Player() { id = 1 };
        WWWForm form = new WWWForm();
        form.AddField("type", RequestType.get_stats.ToString());
       // form.AddField("userID", 1);// _userData.playerData.id);
        SendData(form, RequestType.get_stats);
    }

    private bool IsValidStr(string str, int characters = 256)
    {
        if (str.Length > characters)
        {
            //_userData.error = new Error() { errorText = "Слишком много символов", isError = true };
            _userData.error.errorText = "Слишком много символов";
            _userData.error.isError = true;
            return false;
        }

        if (str.Contains(" "))
        {
            _userData.error = new Error() { errorText = "В строке есть пробелы", isError = true };
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        if (email.Contains(" "))
        {
            _userData.error = new Error() { errorText = "Email contains space symbol", isError = true };
            return false;
        }

        string trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            _userData.error = new Error() { errorText = "Wrong Email", isError = true };
            return false;
        }
    }

    private async void SendData(WWWForm form, RequestType type)
    {
        //Бросили запрос к php скрипту
        UnityWebRequest www = UnityWebRequest.Post(_targetUrl, form);
        //Подождали ответ
        await www.SendWebRequest();
        //Проверили ответ на успешность
        if (www.result != UnityWebRequest.Result.Success)
        {
            _userData.error.errorText = www.error;
            _userData.error.isError = true;
            return;
        }
        //Преобразовали из JSON в обычные переменные которые в классах
        UserData data = SetUserData(www.downloadHandler.text);
        //В зависимости от запроса обработали данные
        switch (type)
        {
            case RequestType.save:
                break;
            case RequestType.logging:
                _userData = data;
                break;
            case RequestType.register:
                _userData = data;
                break;
            case RequestType.get_stats:
                _userData = data;
                //Кидаем инвок к eventу чтобы классы которые участвуют в игре узнали об ответе
                GameEventManager.OnGetStats?.Invoke(_userData);
                break;
            default:
                _userData.error.isError = true;
                _userData.error.errorText = "Undefined RequestType";
                break;
        }
    }

}
