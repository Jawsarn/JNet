using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHosting : MonoBehaviour {

    public Text m_textHud;

    Dictionary<string, uint> m_test = new Dictionary<string, uint>();

    public void OnPressHost()
    {
        m_textHud.text = "Hosting..";
        JNet.StartHosting(JNet.LobbyType.Public);
    }

    public void OnPressJoin()
    {
        m_textHud.text = "Joining..";
        JNet.JoinRandomLobby();
    }

    private void Update()
    {
        
    }
}
