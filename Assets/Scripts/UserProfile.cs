using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserProfile : MonoBehaviour
{
    public static UserProfile Instance;
    
    public static UnityEvent<ProfileData> OnProfileDataUpdated = new UnityEvent<ProfileData> ();
    
    [SerializeField] ProfileData profileData;
    public float xpThreshold = 1000;

    void Awake()
    {
        Instance = this; 
    }

    void OnEnable()
    {

        PlayFabManager.OnUserDataRetrieved.AddListener(UserDataRetrieved);
    }

    void OnDisable()
    {
        PlayFabManager.OnUserDataRetrieved.RemoveListener(UserDataRetrieved);
    }

    [ContextMenu("Get Profile Data")]

    public void GetUserData()
    {
        PlayFabManager.Instance.GetUserData("ProfileData");
    }

    void UserDataRetrieved(string key, string value)
    {
        if(key == "ProfileData")
        {
            profileData = JsonUtility.FromJson<ProfileData>(value);
            OnProfileDataUpdated.Invoke(profileData);
        }
    }

    [ContextMenu("Set Profile Data")]
    void SetUserData(UnityAction onSuccess = null)
    {
        PlayFabManager.Instance.SetUserData("ProfileData", JsonUtility.ToJson(profileData), onSuccess);
    }

    public void AddXp()
    {
        profileData.level += Random.Range(0f, 1f);
        SetUserData(GetUserData);
    }

    public void SetPlayerName(string playerName)
    {
        profileData.playerName = playerName;
        SetUserData(GetUserData);
    }

}

[System.Serializable]
public class ProfileData
{
    public string playerName;
    public float level;
}
