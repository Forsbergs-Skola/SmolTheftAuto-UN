using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Events;

public class QuestPanel : MonoBehaviour
{
    private const float SPACING = 320f;

    [SerializeField] private GameObject sunglassesPrefab;
    [SerializeField] private GameObject gasCanPrefab;
    [SerializeField] private GameObject matchesPrefab;
    [SerializeField] private GameObject finalPrefab;

    List<QuestItem> activeQuests = new List<QuestItem>();


    public void StartQuest(EnumQuest quest)
    {
        foreach(QuestItem _quest in activeQuests)
        {
            if (_quest.QuestName == quest)
            {
                Debug.LogWarning($"{quest} is already started");
                return;
            }
        }
        GameObject questObj = null;
        switch (quest)
        {
            case EnumQuest.SUNGLASSES:
                questObj = Instantiate(sunglassesPrefab);
                break;
            case EnumQuest.MATCHES:
                questObj = Instantiate(matchesPrefab);
                break;
            case EnumQuest.GAS_CAN:
                questObj = Instantiate(gasCanPrefab);
                break;
            case EnumQuest.FINAL:
                questObj = Instantiate(finalPrefab);
                break;
            default:
                return;
        }
        questObj.transform.SetParent(transform);
        RectTransform rectXForm = questObj.GetComponent<RectTransform>();
        int order = activeQuests.Count;
        PositionRectTransform(rectXForm, order);
        QuestItem questItem = questObj.GetComponent<QuestItem>();
        activeQuests.Add(questItem);
    }

    public void FinishQuest(EnumQuest quest)
    {
        QuestItem finishedItem = null;
        foreach (QuestItem _quest in activeQuests)
        {
            if (_quest.QuestName == quest)
            {
                finishedItem = _quest;
                break;
            }
        }
        if (finishedItem == null)
        {
            Debug.LogError("Quest not started");
            return;
        }
        finishedItem.StrikethoughText();
    }

    public void ResetQuestUI()
    {
        List<QuestItem> freshList = new List<QuestItem>();
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestItem thisItem = activeQuests[i];
            activeQuests.RemoveAt(i);
            Destroy(thisItem.gameObject);
        }
        activeQuests = freshList;

    }

    public void InitializeQuestUI(PlayerData playerdata, QuestStartedData questStartedData)
    {
        if (playerdata.hasSunglasses)
        {
            StartQuest(EnumQuest.SUNGLASSES);
            QuestItem q = GetItemFromActiveList(EnumQuest.SUNGLASSES);
            if (q != null) { q.StrikethoughText(); }
        }
        if (playerdata.hasMatches)
        {
            StartQuest(EnumQuest.MATCHES);
            QuestItem q = GetItemFromActiveList(EnumQuest.MATCHES);
            if (q != null) { q.StrikethoughText(); }
        }
        if (playerdata.hasGasCan)
        {
            StartQuest(EnumQuest.GAS_CAN);
            QuestItem q = GetItemFromActiveList(EnumQuest.GAS_CAN);
            if (q != null) { q.StrikethoughText(); }
        }

        if (questStartedData.sunglasses && !playerdata.hasSunglasses)
        {
            StartQuest(EnumQuest.SUNGLASSES);
        }
        if (questStartedData.matches && !playerdata.hasMatches)
        {
            StartQuest(EnumQuest.MATCHES);
        }
        if (questStartedData.gasCan && !playerdata.hasGasCan)
        {
            StartQuest(EnumQuest.GAS_CAN);
        }
        if (questStartedData.final)
        {
            StartQuest(EnumQuest.FINAL);
        }
    }

    private QuestItem? GetItemFromActiveList(EnumQuest activeQuest)
    {
        foreach (QuestItem qItem in activeQuests)
        {
            if (qItem.QuestName == activeQuest) { return qItem; }
        }
        return null;
    }


    private void PositionRectTransform(RectTransform rectXForm, int order)
    {
        if (order < 0) { order = 0; }
        if (order > 3) { order = 3; }

        float magicNumber = -SPACING - ((float)order * SPACING);

        rectXForm.anchorMin = new Vector2(0f, 1f);
        rectXForm.anchorMax = new Vector2(1f, 1f);
        rectXForm.offsetMax = new Vector2(0f, magicNumber);
        rectXForm.offsetMin = new Vector2(0f, 0f);

    }
}
