using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Responses;

public class Authentication : MonoBehaviour 
{
	public Text userIdText, connectionStatusText;
	public InputField usernameInput, passwordInput;
	public Button loginButton;
    
	void Start () 
	{
		// We won't immediately have connection, so at the start of the lobby we will set the connection status to show this
        connectionStatusText.text = "No Connection..."; 

        GS.GameSparksAvailable += (isAvailable) => 
        {
            if(isAvailable){
                connectionStatusText.text = "GameSparks Connected...";
            } else {
                connectionStatusText.text = "GameSparks Disconnected...";
            }
        };		

		// we won't start with a user logged in so lets show this also
        userIdText.text = "No User Logged In..."; 
        
		// 	Login button listener. So we don't need to create extra methods //
        loginButton.onClick.AddListener (() => 
        {
            MultiplayerNetworkingManager.Instance().AuthenticateUser(usernameInput.text, passwordInput.text, OnRegistration, OnAuthentication);
        });
	}
	
	/// <summary>
    /// this is called when a player is registered
    /// </summary>
    /// <param name="_resp">Resp.</param>
    private void OnRegistration(RegistrationResponse _resp)
	{
        userIdText.text = "User ID: "+_resp.UserId;
        connectionStatusText.text = "New User Registered...";
        OnAuthenticationSuccessful();
    }
	
    /// <summary>
    /// This is called when a player is authenticated
    /// </summary>
    /// <param name="_resp">Resp.</param>
    private void OnAuthentication(AuthenticationResponse _resp)
	{
        userIdText.text = "User ID: "+_resp.UserId;
        connectionStatusText.text = "User Authenticated...";
        OnAuthenticationSuccessful();
    }

    public void OnAuthenticationSuccessful () 
	{
        UIManager.Instance().ShowAuthenticationPanel(false);
    }
}
