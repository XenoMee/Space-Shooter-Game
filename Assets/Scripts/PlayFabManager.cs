using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    [Header("UI")]
    public Text messageText;
    public InputField emailInput;
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;

    [Header("Windows")]
    public GameObject usernameWindow;
    public GameObject loginWindow;

    [Header("Leaderboard")]
    public GameObject rowPrefab;
    public Transform rowsParent;

    [HideInInspector]
    public static string loggedInPlayfabId;

    private UserProfile _userProfile;

    public static UnityEvent<string,string> OnUserDataRetrieved = new UnityEvent<string,string>();

    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password is too short!";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "User registered!";
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public IEnumerator LoginUsernameCooldown()
    {
        yield return new WaitForSeconds(1f);
        usernameWindow.SetActive(true);
        loginWindow.SetActive(false);
    }

    public IEnumerator LoginMainMenuCooldown()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "E7417"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    public void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = usernameInput.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name");
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset mail sent!";
    }

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        _userProfile = GameObject.Find("Canvas").GetComponent<UserProfile>();
    }

    void OnLoginSuccess(LoginResult result)
    {
        loggedInPlayfabId = result.PlayFabId;
        messageText.text = "Logged in!";
        _userProfile.GetUserData();
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        if (name == null)
        {
            StartCoroutine(LoginUsernameCooldown());
        }
        else 
        {
            StartCoroutine(LoginMainMenuCooldown());
        }
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "PlatformScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard sent!");
    }

    public void GetLeaderboardAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "PlatformScore",
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, OnError);
    }

    void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult result)
    {
        foreach (Transform item in rowsParent){
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard){

            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();

            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            if (item.PlayFabId == loggedInPlayfabId)
            {
                texts[0].color = Color.red;
                texts[1].color = Color.red;
                texts[2].color = Color.red;

                Debug.Log(string.Format("PLACE: {0} | ID: {1} | VALUE: {2}",
                item.Position, item.DisplayName, item.StatValue));
            }
        }
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlatformScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach(Transform item in rowsParent){
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            Debug.Log(string.Format("PLACE: {0} | ID: {1} | VALUE: {2}", 
                item.Position, item.DisplayName, item.StatValue));
        }
    }

    public void GetUserData(string key)
    {

        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            PlayFabId = loggedInPlayfabId,
            Keys = new List<string>() {
                key 
            }
        }, result => {
            Debug.Log("Successfull GetUserData");
            if (result.Data.ContainsKey(key))
            {
                OnUserDataRetrieved.Invoke(key, result.Data[key].Value);
            }
            else
            {
                OnUserDataRetrieved.Invoke(key, null);
            }
        }, OnError);
    }

    public void SetUserData(string key, string value, UnityAction onSuccess = null)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>()
            {
                {key, value}
            }
        }, result => {
            Debug.Log("Successfull SetUserData");
            onSuccess.Invoke();
        }, OnError);
    }


    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }
}