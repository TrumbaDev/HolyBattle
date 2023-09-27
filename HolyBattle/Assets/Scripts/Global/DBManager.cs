using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

[System.Serializable]
public class UserData
{
    public Player playerData;
    public Error error = new Error();
    public NPCData npcData;
}
[System.Serializable]
public class Error
{
    public string errorText;
    public bool isError;
}
[System.Serializable]
public class Player
{
    public int id;
    public string login;

    public Player(string nick)
    {
        login = nick;
    }
    public void SetNickname(string name) => login = name;
}

public class NPCData
{
    public string NameLvlNpc1, NameLvlNpc2, NameLvlNpc3, NameLvlNpc4;
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
    private UnityEvent OnLogged, OnRegistered, OnError;

    private enum RequestType
    {
        logging, register, save, takeNPCData
    }

    private string GetUserData(UserData data)
    {
        return JsonUtility.ToJson(data);
    }

    private UserData SetUserData(string data)
    {
        return JsonUtility.FromJson<UserData>(data);
    }

    private NPCData SetNPCData(string data)
    {
        return JsonUtility.FromJson<NPCData>(data);
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
            OnError.Invoke();
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
            OnError.Invoke();
        }
    }

    public void Registering(string login, string password, string passwordConfirm, string email)
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

    public void SaveProgress(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("type", RequestType.save.ToString());
        form.AddField("id", id);
        SendData(form, RequestType.save);
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
        UnityWebRequest www = UnityWebRequest.Post(_targetUrl, form);
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            _userData.error.errorText = www.error;
            _userData.error.isError = true;
            return;
        }
        UserData data = SetUserData(www.downloadHandler.text);

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
            default:
                _userData.error.isError = true;
                _userData.error.errorText = "Undefined RequestType";
                break;
        }
    }


    public UnityWebRequest ConnectToDB(string requestType, string[] filedsName, string[] values)
    {
        if (requestType == null || requestType == "")
            return null;

        if (filedsName.Length == 0 || values.Length == 0)
            return null;

        WWWForm form = new WWWForm();
        for (int i = 0; i < filedsName.Length; i++)
        {
            if (values[i] == null)
                values[i] = "";

            form.AddField(filedsName[i], values[i]);
        }

        UnityWebRequest rq = UnityWebRequest.Post("http://localhost/HolyBattle/index.php", form);

        return rq;
    }

}
