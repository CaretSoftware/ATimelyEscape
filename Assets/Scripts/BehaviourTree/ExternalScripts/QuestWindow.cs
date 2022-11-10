using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private Transform goalsContent;
    [SerializeField] private Text xpText;
    [SerializeField] private Text coinsText;

    public void Initialize(Quest quest)
    {
        titleText.text = quest.Information.name;
        descriptionText.text = quest.Information.description;

        foreach(var goal in quest.Goals)
        {
            GameObject goalObj = Instantiate(goalPrefab, goalsContent);
            goalObj.transform.Find("Text").GetComponent<Text>().text = goal.GetDescription();

            GameObject countObj = goalObj.transform.Find("Count").gameObject;
            GameObject skipObj = goalObj.transform.Find("Skip").gameObject;

            if (goal.Completed)
            {
                countObj.SetActive(false);
                skipObj.SetActive(false);
                goalObj.transform.Find("Done").gameObject.SetActive(true);
            }
            else
            {
                countObj.GetComponent<Text>().text = goal.CurrentAmount + "/" + goal.requiredAmount;
                /*
                skipObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                goal.Skip();
                countObj.SetActive(false);
                goalObj.transform.Find("Done").gameObject.SetActive(true);
                });
                */
            }
        }
        //xpText.text = quest.Reward.XP.ToString();
        //coinsText.text = quest.Reward.Currency.ToString();
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < goalsContent.childCount; i++)
            Destroy(goalsContent.GetChild(i).gameObject);
    }
}
