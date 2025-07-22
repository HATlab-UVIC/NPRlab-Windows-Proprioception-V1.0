using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Lobbies;

public class NetworkManagerWindows : MonoBehaviour
{
    private ISession _activeSession;
    private string playerNamePropertyKey = "playerName";

    ISession _ActiveSession
    {
        get => _activeSession;
        set
        {
            _activeSession = value;
            Debug.Log($"Active session: {_activeSession}");
        }
    }

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sign in anonymously successed! Player ID: {AuthenticationService.Instance.PlayerId}");

            StartSessionAsHost();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties() {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey,  playerNameProperty } };
    }

    private async void StartSessionAsHost() {
        var playerProperties = await GetPlayerProperties();

        var options = new SessionOptions
        {
            MaxPlayers = 2,
            IsLocked = false,
            IsPrivate = false,
            PlayerProperties = playerProperties,
        }.WithRelayNetwork();

        _ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {_ActiveSession.Id} created! Join code: {_ActiveSession.Code}");
        NetworkDebugConsole.Singleton.SetDebugString($"Session {NetworkManager.Singleton.LocalClientId} created! Join code: {_ActiveSession.Code}");
        NetworkDebugConsole.Singleton.SetJoingCode(_ActiveSession.Code);
    }

    private async Task LeaveSession() {
        if ( _ActiveSession != null )
        {
            try
            {
                await _ActiveSession.LeaveAsync();
            }
            catch
            {
                // Ignored as we are exiting the game
            }
            finally
            {
                _ActiveSession = null;
            }
        }
    }

    private void OnDestroy() {
    }
}
