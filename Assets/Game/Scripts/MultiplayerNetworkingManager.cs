using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.Api.Messages;
using GameSparks.RT;

public class MultiplayerNetworkingManager : MonoBehaviour 
{
	#region MultiplayerNetworkingManager Singleton
	private static MultiplayerNetworkingManager instance = null;
	public static MultiplayerNetworkingManager Instance(){
		if (instance != null) {
			return instance;
		} else {
			Debug.LogError ("GSM| GameSparksManager Not Initialized...");
		}
		return null;
	}
    
    void Awake() {
		instance = this; // if not, give it a reference to this class
		DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
	}
    #endregion

    #region RT Session
	/// <summary>Need the GameSparksRTUnity class to create a real-time session</summary>
	private GameSparksRTUnity gameSparksRTUnity;	
	public GameSparksRTUnity GetRTSession(){ return gameSparksRTUnity; }

	/// <summary>Local private variable so we can access the RTSessionInfo used to create this session</summary>
	private RTSessionInfo sessionInfo;	
    public RTSessionInfo GetSessionInfo(){ return sessionInfo; }
    #endregion

	#region Login & Registration
	//	We are using an adaptable login method so that it removes the need for an extra registration screen upon login.
	//	This lets the developer automatically create a new user even if the user hasn't registered yet. 

	public delegate void AuthCallback(AuthenticationResponse _authresp2);
	public delegate void RegCallback(RegistrationResponse _authResp);
	//	Using a callback to allow the lobby manager to update the user's ID and connection-status 
	//	only when we have received a valid response. The reason for the two callbacks is that 
	//	the RegistrationResponse and AuthenticationResponse are slightly different.

	/// <summary>
	/// Sends an authentication request or registration request to GS.
	/// </summary>
	/// <param name="_callback1">Auth-Response</param>
	/// <param name="_callback2">Registration-Response</param>
	public void AuthenticateUser (string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback) {
        new GameSparks.Api.Requests.RegistrationRequest()
        // this login method first attempts a registration //
        // if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check
        // for this example we use the user-name was the display name also //
				.SetDisplayName(_userName)
				.SetUserName(_userName)
				.SetPassword(_password)
				.Send((registrationResponse) => {
					if (!registrationResponse.HasErrors) { // if we get the response back with no errors then the registration was successful
						Debug.Log("GSM| Registration Successful..."); 
						_regcallback(registrationResponse); 
					} else {
						// if we receive errors in the response, then the first thing we check is if the player is new or not
						if (!(bool)registrationResponse.NewPlayer) // player already registered, lets authenticate instead
						{
							Debug.LogWarning("GSM| Existing User, Switching to Authentication");
							new GameSparks.Api.Requests.AuthenticationRequest()
								.SetUserName(_userName)
								.SetPassword(_password)
								.Send((authenticationResponse) => {
									if (!authenticationResponse.HasErrors) {
										Debug.Log("Authentication Successful...");
										_authcallback(authenticationResponse);
									} else {
										Debug.LogWarning("GSM| Error Authenticating User \n"+authenticationResponse.Errors.JSON);
									}
								});
						} 
                        else 
                        {
                            // if there is another error, then the registration must have failed
                            Debug.LogWarning("GSM| Error Authenticating User \n"+registrationResponse.Errors.JSON); 
						}
					}
				});
		}
	#endregion

    #region Matchmaking Request
	/// <summary>
    /// This will request a pvp match between as many players you have set in the match. Parameters are set by GameManager.
    /// </summary>
    public void SendMatchmakingRequest(string _matchShortCode, long _matchSkill, string _matchGroup) 
    {
        Debug.Log ("GSM| Attempting Matchmaking...");

        new GameSparks.Api.Requests.MatchmakingRequest ()
            .SetMatchShortCode (_matchShortCode) 
            .SetSkill (_matchSkill)
            .SetMatchGroup (_matchGroup) // set the match group (this is set by the CloudAnchorController during hosting a room)
            .Send ((response) => 
            {
                // Check for errors
                if(response.HasErrors)
                { 
                    Debug.LogError("GSM| MatchMakingRequest Error \n" + response.Errors.JSON);
                }
            });
    }
    #endregion


    #region Start New RT Session
	/// <summary>Create a new session by passing in the match details</summary>
	public void StartNewRTSession(RTSessionInfo _info)
	{
        Debug.Log ("GSM| Creating New RT Session Instance...");

        sessionInfo = _info;
        gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game


        // In order to create a new RT game we need a 'FindMatchResponse' //
        // This would usually come from the server directly after a successful MatchmakingRequest //
        // However, in our case, we want the game to be created only when the first player decides using a button //
        // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
        // is passed in. //
        GSRequestData mockedResponse = new GSRequestData()
                                            .AddNumber("port", (double)_info.GetPortID())
                                            .AddString("host", _info.GetHostURL())
                                            .AddString("accessToken", _info.GetAccessToken()); // construct a dataset from the game-details

        FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
        // So in the game-config method we pass in the response which gives the instance its connection settings //
        // In this example, I use a lambda expression to pass in actions for 
        // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
        // These methods are self-explanatory, but the important one is the OnPacket Method //
        // this gets called when a packet is received //
        

        /// <summary>Reference gameSparksRTUnity in order to receive RTData</summary>
        gameSparksRTUnity.Configure(response, 
            (peerId) =>  {    OnPlayerConnected(peerId);  },
            (peerId) => {    OnPeerDisconnected(peerId);    },
            (ready) => {    OnRTReady(ready);    },
            (packet) => {    OnPacketReceived(packet);    });   //  When any packets are received, the OnPacketRecieved callback will be called and the details of that packet can be processed.
        gameSparksRTUnity.Connect(); // when the config is set, connect the game
    }
    #endregion

    private void OnRTReady(bool _isReady) {
        if (_isReady) {
            
            Debug.Log ("GSM| RT Session Connected...");

            // When the real-time session has successfully been configured between the participants in the MatchFoundMessage, set up the game. 
            GameManager.Instance().StartGame();
        }
    }

    private void OnPlayerConnected(int _peerId)
    {
        Debug.Log ("GSM| Player Connected, " + _peerId);

        GameManager.Instance().OnOpponentConnected(_peerId);
    }
    
    private void OnPeerDisconnected(int _peerId)
    {
        Debug.Log ("GSM| Player Disconnected, " + _peerId);

        GameManager.Instance().OnOpponentDisconnected(_peerId);
    }

    private void OnPacketReceived(RTPacket _packet)
    {
        Debug.Log("GSM| Packet received from " + _packet.Sender + " for OpCode " + _packet.OpCode);

        switch (_packet.OpCode) 
        {
            // Op-code 1 refers to 
            case 1:
                break;
            // Op-code 2 refers to 
            case 2:
                GameManager.Instance().UpdateOpponentCameraMovement(_packet);
                break;
            // Op-code 3 refers to 
            case 3:
                GameManager.Instance().UpdateOpponentAbility(_packet);
                break;
            case 4:
                GameManager.Instance().UpdateOpponentShield(_packet);
                break;
            case 5:
                GameManager.Instance().UpdateOpponentSpectatorMode(_packet);
                break;
            case 6:
                GameManager.Instance().UpdateOpponentProjectile(_packet);
                break;
        }
    }
}



public class RTSessionInfo 
{	
    private string hostURL;
    public string GetHostURL(){    return this.hostURL;    }
    private string acccessToken;
    public string GetAccessToken(){    return this.acccessToken;    }
    private int portID;
    public int GetPortID(){    return this.portID;    }
    private string matchID;
    public string GetMatchID(){    return this.matchID;    }

    private List<RTPlayer> playerList = new List<RTPlayer> ();
    public List<RTPlayer> GetPlayerList(){
        return playerList;
    }

    /// <summary>
    /// Creates a new RTSession object which is held until a new RT session is created
    /// </summary>
    public RTSessionInfo (MatchFoundMessage _message)
    {
        portID = (int)_message.Port;
        hostURL = _message.Host;
        acccessToken = _message.AccessToken;
        matchID = _message.MatchId;

        // we loop through each participant and get their peerId and display name //
        foreach(MatchFoundMessage._Participant player in _message.Participants)
        {
            playerList.Add(new RTPlayer(player.DisplayName, player.Id, (int)player.PeerId));
        }
    }

    public void UpdateRTSessionInfo (MatchUpdatedMessage _message) 
    {
        // The MatchUpdatedMessage's participants list contains the most recent list of players in the match, so we can reset the list
        playerList = new List<RTPlayer> ();
        foreach(MatchUpdatedMessage._Participant player in _message.Participants)
        {
            playerList.Add(new RTPlayer(player.DisplayName, player.Id, (int)player.PeerId));
        }
    }

    public class RTPlayer
    {
        public RTPlayer(string _displayName, string _id, int _peerId){
            this.displayName = _displayName;
            this.id = _id;
            this.peerId = _peerId;
        }

        public string displayName;
        public string id;
        public int peerId;
        public bool isOnline;
    }
}
