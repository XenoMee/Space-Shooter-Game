using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProfile : MonoBehaviour
{
    [SerializeField] InputField playerNameText;

    [SerializeField] Text playerLevelText;

    [SerializeField] Text playerXpText;

    [SerializeField] Image playerXpFill;

    void OnEnable()
    {
        UserProfile.OnProfileDataUpdated.AddListener(ProfileDataUpdated);
    }

    void OnDisable()
    {
        UserProfile.OnProfileDataUpdated.RemoveListener(ProfileDataUpdated);
    }

    void ProfileDataUpdated(ProfileData profileData)
    {
        float level = (Mathf.Floor(profileData.level));
        float xp = profileData.level - level;

        playerNameText.text = profileData.playerName;
        playerLevelText.text = level.ToString();
        playerXpText.text = ((int)(xp * UserProfile.Instance.xpThreshold)).ToString();
        playerXpFill.fillAmount = xp;
    }
}
