using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DBManager
{
    public UnityWebRequest ConnectToDB(string[] forms)
    {
        WWWForm form = new WWWForm();
        foreach (string formValue in forms)
        {
            form.AddField(formValue, "1");
        }

        UnityWebRequest rq = UnityWebRequest.Post("http://localhost/HolyBattle/index.php", form);

        return rq;
    }

}
