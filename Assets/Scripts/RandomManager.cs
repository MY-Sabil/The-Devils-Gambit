using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleJSON;
using System.IO;
using System.Linq;
using UnityEngine.Networking;


public class RandomManager : MonoBehaviour
{
    [HideInInspector] public int[,] ran_num;
    [HideInInspector] public List<int> NumList;

    string[] colors = { "#ff6666", "#e6d81c", "#33d6ff", "#ff66ef" };

    public ArrayLayout middle_grid_txt;
    [SerializeField] private TextMeshProUGUI[] right_grid_txt;
    [SerializeField] private TextMeshProUGUI[] operator_grid_txt;

    int ArraySizeInJSON;

    GameManager gameManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();

        RandomNumberGen();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                string color = RandomColorGen();
                Color kalar = new Color(255, 0, 0, 1);
                ColorUtility.TryParseHtmlString(color, out kalar);
                middle_grid_txt.rows[i].row[j].text = "<color=" + color + ">" + ran_num[i, j].ToString();
                middle_grid_txt.rows[i].row[j].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
            }

        }
        for (int k = 0; k < 8; k++)
        {
            string color = RandomColorGen();
            Color kalar = new Color(255, 0, 0, 1);
            ColorUtility.TryParseHtmlString(color, out kalar);
            right_grid_txt[k].text = "<color=" + color + ">" + NumList[0].ToString();
            NumList.RemoveAt(0);
            right_grid_txt[k].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
        }
    }

    void RandomNumberGen()
    {
        ran_num = new int[4,4];
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                ran_num[i, j] = Random.Range(1, 89);

                #if UNITY_STANDALONE
                    string path = "Assets/Resources/result.json";
                    jsonString = File.ReadAllText(path);
                #endif
                
                #if UNITY_WEBGL
                    string jsonString = FindObjectOfType<JsonLoader>().jsonString;
                #endif
                JSONObject jsonFile = (JSONObject)JSON.Parse(jsonString);

                int curr_num = ran_num[i, j];
                ArraySizeInJSON = jsonFile[curr_num].Count;
                int rnd_num = Random.Range(0, ArraySizeInJSON);
                var value = jsonFile[curr_num][rnd_num];
                NumList.Add(value[0]);
                NumList.Add(value[1]);
                NumList.Add(value[2]);
            }
        }
    }

    public string RandomColorGen()
    {
        int color = Random.Range(0, 4);
        return colors[color];
    }

    public void RerollNumbers(GameObject[] eqSlots)
    {
        GameObject[] nums = new GameObject[3];
        nums[0] = eqSlots[0];
        nums[1] = eqSlots[2];
        nums[2] = eqSlots[4];

        GameObject prefab = gameManager.numPrefab;
        Transform parent = gameManager.numParent;

        for (int i = 0; i < 3; i++)
        {
            ItemSlot slot = nums[i].GetComponent<ItemSlot>();
            slot.DestroyItem();

            var obj = Instantiate(prefab);
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<RectTransform>().anchoredPosition = slot.itemPos;

            string color = RandomColorGen();
            Color kalar = new Color(255, 0, 0, 1);
            ColorUtility.TryParseHtmlString(color, out kalar);
            obj.GetComponent<TextMeshProUGUI>().text = "<color=" + color + ">" + NumList[0].ToString();
            obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;

            ClearSlotData(slot);
            NumList.RemoveAt(0);
        }
        var op1 = eqSlots[1].GetComponent<ItemSlot>();
        op1.DestroyItem(); ;
        ClearSlotData(op1);

        var op2 = eqSlots[3].GetComponent<ItemSlot>();
        op2.DestroyItem();
        ClearSlotData(op2);
    }

    void ClearSlotData(ItemSlot slot)
    {
        slot.hasItem = false;
        slot.itemPos = new Vector2(0, 0);
        slot.ItemText = "";
    }


}
