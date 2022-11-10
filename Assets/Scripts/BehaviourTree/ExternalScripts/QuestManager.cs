using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private GameObject questHolder;
    [SerializeField] private Transform questsContent;

    public List<Quest> CurrentQuests;

    private void Awake()
    {
        foreach(var quest in CurrentQuests)
        {
            quest.Initialize();
            quest.QuestCompleted.AddListener(OnQuestCompleted);

            GameObject questObj = Instantiate(questPrefab, questsContent);
            questObj.transform.Find("Icon").GetComponent<Image>().sprite = quest.Information.icon;
            questObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate 
            {
                questHolder.GetComponent<QuestWindow>().Initialize(quest);
                questHolder.SetActive(true); 
            });
        }
    }

    public void Create(string objName)
    {
       // EventManager.Instance.QueueEvent(new ObjectiveEvent(objName));
    }

    private void OnQuestCompleted(Quest quest)
    {
        questsContent.GetChild(CurrentQuests.IndexOf(quest)).Find("Checkmark").gameObject.SetActive(true);
    }
}
