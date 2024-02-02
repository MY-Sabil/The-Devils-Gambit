using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using SimpleJSON;
using System.IO;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public GameObject[] opPrefabs;
    public GameObject numPrefab;
    public Transform opParent;
    public Transform numParent;

    public GameObject[] eqSlots;
    bool[] isAllFilled;
    string[] eqNums;
    string[] colors;

    [HideInInspector] public int ans = 0;
    [HideInInspector] public string ansColor = "";

    RandomManager randomManager;
    ArrayLayout m_grid;

    public int Score = 0;
    [SerializeField] TextMeshProUGUI score_txt;

    int combCount = 0;

    [SerializeField] ParticleSystem ps;
    [SerializeField] Timer timer;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pauseMenu;


    void Start()
    {
        Score = 100;

        isAllFilled = new bool[5];
        eqNums = new string[5];
        colors = new string[5];

        randomManager = GetComponent<RandomManager>();
        m_grid = randomManager.middle_grid_txt;
    }

    bool isHard = false;
    bool isHarder = false;

    bool isGameOver = false;

    void Update()
    {
        score_txt.text = "$" + Score.ToString();

        if (Score <= 0)
        {
            isGameOver = true;
            GameOver();
        }

        if(Score >= 200 && Score < 380 && !isHard)
        {
            FindObjectOfType<SoundManager>().Play("laugh");
            MakeGameHarder();
        }

        else if(Score >= 380 && !isHarder)
        {
            FindObjectOfType<SoundManager>().Play("laugh");
            MakeItEvenHarder();
        }

        ans = CheckEquationFilled();
        if(ans != 0)
        {
            if(ans > 99 || ans < 0) { eqSlots[6].GetComponent<TextMeshProUGUI>().text = "∞"; return; }
            eqSlots[6].GetComponent<TextMeshProUGUI>().text = ansColor + ans.ToString();
            Color kalar = new Color(255, 0, 0, 1);
            string color = ansColor[7..];
            ColorUtility.TryParseHtmlString(color[..7], out kalar);
            eqSlots[6].transform.GetChild(0).gameObject.SetActive(true);
            eqSlots[6].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
            CheckGridForAns();
        }
        else
        {
            eqSlots[6].GetComponent<TextMeshProUGUI>().text = "";
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            pauseMenu.SetActive(true);
            timer.timerPaused = true;
        }
    }

    public void UnPauseGame()
    {
        pauseMenu.SetActive(false);
        timer.timerPaused = false;
    }

    int CheckEquationFilled()
    {
        for (int i = 0; i < 5; i++)
        {
            ItemSlot slot = eqSlots[i].GetComponent<ItemSlot>();
            isAllFilled[i] = slot.hasItem;
            if (isAllFilled[i])
            {
                if (slot.isOp)
                {
                    eqNums[i] = slot.ItemText;
                    continue;
                }
                eqNums[i] = slot.ItemText[15..];
                colors[i] = slot.ItemText[..15];
            }
        }
        if (!isAllFilled.Contains(false))
        {
            ansColor = ColorForAns(colors);
            return SolveEquation(eqNums);
        }
        eqSlots[6].transform.GetChild(0).gameObject.SetActive(false);
        return 0;
    }

    int SolveEquation(string[] eqNums)
    {
        int[] nums = new int[3];
        string[] ops = new string[2];

        nums[0] = int.Parse(eqNums[0]);
        nums[1] = int.Parse(eqNums[2]);
        nums[2] = int.Parse(eqNums[4]);

        ops[0] = eqNums[1];
        ops[1] = eqNums[3];

        int ans = 0;

        if(ops[0] == "+")
        {
            ans = nums[0] + nums[1];
        }
        else if (ops[0] == "-")
        {
            ans = nums[0] - nums[1];
        }
        else if (ops[0] == "x")
        {
            ans = nums[0] * nums[1];
        }
        else if (ops[0] == "/")
        {
            ans = nums[0] / nums[1];
        }

        if (ops[1] == "+")
        {
            ans += nums[2];
        }
        else if (ops[1] == "-")
        {
            ans -= nums[2];
        }
        else if (ops[1] == "x")
        {
            ans *= nums[2];
        }
        else if (ops[1] == "/")
        {
            ans /= nums[2];
        }

        return ans;
    }

    string ColorForAns(string[] colors)
    {
        string color = colors.Where(c => !string.IsNullOrEmpty(c)).GroupBy(a => a).OrderByDescending(b => b.Key[1].ToString()).First().Key;
        return color;
    }

    bool comboAlreadyPlayedSound = false;

    void CheckGridForAns()
    {
        bool found = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                string m_grid_txt = m_grid.rows[i].row[j].text[15..];
                string m_grid_txt_color = m_grid.rows[i].row[j].text[..15];

                string txt_color = m_grid_txt_color[7..];

                if (m_grid_txt == ans.ToString() && !found)
                {
                    Debug.Log("Solved One!");

                    DoAnimation(m_grid.rows[i].row[j], txt_color[..7]);

                    bool alreadyComboPlayed = false;
                    if(m_grid_txt_color == ansColor)
                    {
                        int combScore = CheckForEpicComb(i, j, m_grid_txt_color);
                        if (!comboAlreadyPlayedSound)
                        {
                            FindObjectOfType<SoundManager>().Play("combo1");
                        }
                        alreadyComboPlayed = true;

                        Score += 10 + combScore;
                    }
                    if (!alreadyComboPlayed)
                    {
                        FindObjectOfType<SoundManager>().Play("combo0");
                    }
                    Score += 20;

                    ReRollNum(m_grid.rows[i].row[j]);
                    found = true;
                    isHard = false;
                    break;
                }
            }
        }
    }

    int CheckForEpicComb(int row, int col, string color)
    {
        int score = 0;
        combCount = 0;

        if(col + 1 <= 3)
        {
            if(m_grid.rows[row].row[col+1].text[..15] == color)
            {
                score += 10;
                combCount += 1;

                string text = m_grid.rows[row].row[col + 1].text[..15];
                string num = m_grid.rows[row].row[col + 1].text[15..];
                //DoAnimation(m_grid.rows[row].row[col + 1], text);

                string newColor = randomManager.RandomColorGen();
                Color kalar = new Color(255, 0, 0, 1);
                ColorUtility.TryParseHtmlString(color, out kalar);
                text = "<color=" + newColor + ">";
                m_grid.rows[row].row[col + 1].text = text + num;
                m_grid.rows[row].row[col + 1].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
            }
        }
        if (col - 1 >= 0)
        {
            if (m_grid.rows[row].row[col - 1].text[..15] == color)
            {
                score += 10;
                combCount += 1;

                string text = m_grid.rows[row].row[col - 1].text[..15];
                string num = m_grid.rows[row].row[col - 1].text[15..];
                //DoAnimation(m_grid.rows[row].row[col - 1], text);

                string newColor = randomManager.RandomColorGen();
                Color kalar = new Color(255, 0, 0, 1);
                ColorUtility.TryParseHtmlString(color, out kalar);
                text = "<color=" + newColor + ">";
                m_grid.rows[row].row[col - 1].text = text + num;
                m_grid.rows[row].row[col - 1].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
            }
        }
        if (row + 1 <= 3)
        {
            if (m_grid.rows[row + 1].row[col].text[..15] == color)
            {
                score += 10;
                combCount += 1;

                string text = m_grid.rows[row + 1].row[col].text[..15];
                string num = m_grid.rows[row + 1].row[col].text[15..];
                //DoAnimation(m_grid.rows[row +1].row[col], text);

                string newColor = randomManager.RandomColorGen();
                Color kalar = new Color(255, 0, 0, 1);
                ColorUtility.TryParseHtmlString(color, out kalar);
                text = "<color=" + newColor + ">";
                m_grid.rows[row + 1].row[col].text = text + num;
                m_grid.rows[row + 1].row[col].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
            }
        }
        if (row - 1 >= 0)
        {
            if (m_grid.rows[row - 1].row[col].text[..15] == color)
            {
                score += 10;
                combCount += 1;

                string text = m_grid.rows[row - 1].row[col].text[..15];
                string num = m_grid.rows[row - 1].row[col].text[15..];
                //DoAnimation(m_grid.rows[row - 1].row[col], text);

                string newColor = randomManager.RandomColorGen();
                Color kalar = new Color(255, 0, 0, 1);
                ColorUtility.TryParseHtmlString(color, out kalar);
                text = "<color=" + newColor + ">";
                m_grid.rows[row - 1].row[col].text = text + num;
                m_grid.rows[row - 1].row[col].transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;
            }
        }
        if(score > 0)
        {
            FindObjectOfType<SoundManager>().Play("combo2");
        }
        return score;
    }

    void ReRollNum(TextMeshProUGUI oldNum)
    {
        int newNum = Random.Range(1, 89);
        string color = randomManager.RandomColorGen();
        Color kalar = new Color(255, 0, 0, 1);
        ColorUtility.TryParseHtmlString(color, out kalar);
        oldNum.text = "<color=" + color + ">" + newNum;
        oldNum.transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;

        #if UNITY_STANDALONE
            string path = "Assets/Resources/result.json";
            string jsonString = File.ReadAllText(path);
        #endif
        #if UNITY_WEBGL
            string jsonString = FindObjectOfType<JsonLoader>().jsonString;
        #endif
        
        JSONObject jsonFile = (JSONObject)JSON.Parse(jsonString);

        int ArraySizeInJSON = jsonFile[newNum].Count;
        int rnd_num = Random.Range(0, ArraySizeInJSON);
        var value = jsonFile[newNum][rnd_num];
        randomManager.NumList.Add(value[0]);
        randomManager.NumList.Add(value[1]);
        randomManager.NumList.Add(value[2]);

        randomManager.RerollNumbers(eqSlots);
    }

    void DoAnimation(TextMeshProUGUI text, string color)
    {
        ps.GetComponent<Transform>().localPosition = text.GetComponent<RectTransform>().anchoredPosition;
        ps.gameObject.SetActive(true);

        Gradient grad = new Gradient();
        Color col = Color.green;
        ColorUtility.TryParseHtmlString(color, out col);
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(col, 0.0f), new GradientColorKey(col, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        var colOverLifetime = ps.colorOverLifetime;
        colOverLifetime.color = grad;

        ps.Play();
    }

    void GameOver()
    {
        timer.timerPaused = true;
        gameOverPanel.SetActive(true);
    }

    [SerializeField] RectTransform opPos1, opPos2;

    void MakeGameHarder()
    {
        int opIndex = Random.Range(0, 4);
        RectTransform[] pos = { opPos1, opPos2 };
        int posIndex = Random.Range(0, 2);

        var obj = Instantiate(opPrefabs[opIndex]);
        obj.transform.SetParent(opParent);
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.GetComponent<DragNDrop>().cannotDrag = true;
        obj.GetComponent<RectTransform>().anchoredPosition = pos[posIndex].anchoredPosition;

        pos[posIndex].GetComponent<ItemSlot>().ItemText = obj.GetComponent<TextMeshProUGUI>().text;
        pos[posIndex].GetComponent<ItemSlot>().hasItem = true;
        pos[posIndex].GetComponent<ItemSlot>().item = obj;

        isHard = true;
    }

    void MakeItEvenHarder()
    {
        int opIndex1 = Random.Range(0, 4);
        int opIndex2 = Random.Range(0, 4);

        var obj = Instantiate(opPrefabs[opIndex1]);
        obj.transform.SetParent(opParent);
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.GetComponent<DragNDrop>().cannotDrag = true;
        obj.GetComponent<RectTransform>().anchoredPosition = opPos1.anchoredPosition;

        opPos1.GetComponent<ItemSlot>().ItemText = obj.GetComponent<TextMeshProUGUI>().text;
        opPos1.GetComponent<ItemSlot>().hasItem = true;
        opPos1.GetComponent<ItemSlot>().item = obj;

        var obj2 = Instantiate(opPrefabs[opIndex2]);
        obj2.transform.SetParent(opParent);
        obj2.transform.localScale = new Vector3(1, 1, 1);
        obj2.GetComponent<DragNDrop>().cannotDrag = true;
        obj2.GetComponent<RectTransform>().anchoredPosition = opPos2.anchoredPosition;

        opPos2.GetComponent<ItemSlot>().ItemText = obj2.GetComponent<TextMeshProUGUI>().text;
        opPos2.GetComponent<ItemSlot>().hasItem = true;
        opPos2.GetComponent<ItemSlot>().item = obj2;

        isHarder = true;
    }
}
