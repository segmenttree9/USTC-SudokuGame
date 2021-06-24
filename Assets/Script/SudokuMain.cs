
/***********************************
Sudoku by Yang Jiaqi
************************************/

/***********************************
        注释

    数组从0开始编号！！！
    [9]是从[0]到[8]！！！

    数组从上往下0到8行(line)
        从左往右0到8列(column)
        宫(grid)编号如下
        0  1  2
        3  4  5
        6  7  8



************************************/

//开启调试
#define DEBUG_SUDOKU

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class SudokuMain : MonoBehaviour
{
    //数独！！！
    private SudokuManager Sudoku;

    //小方格的预制体(Prefab)
    public GameObject _SudokuCell;

    //绘制数独的载体
    public GameObject _SudokuPanel;

    //填数的panel
    public GameObject _NumberPanel;

    //撤销的Panel
    public GameObject _OKPanel;

    //背景Panel
    public GameObject _BackGround;

    //菜单Panel
    public GameObject _MenuPanel;
    public GameObject HintButton;

    //存储
    GameSaveManager saveManager;
    public SudokuInfo _SaveData;
    public GameObject _SaveGame;
    public Text SaveMessage;
    public InputField _Input;

    //文本
    public Text _TitleText;
    public Text _TimeText;
    public Text _MessageText;

    //每一个格子的信息
    public GameObject[] CellObject;

    //当前活动的格子
    GameObject activeCell;

    //用这个作为范围限制
    const int Nine = 9;

    // Start is called before the first frame update
    void Start()
    {
        InitUI();
        if (StaticValue.Get()._Difficult != StaticValue.Load)
        {
            InitSudokuCell();
        }
        else
        {
            LoadSudokuCell();
        }
        AddListener();
        _SaveGame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        CountTime();
    }

    //初始化游戏界面
    void InitUI()
    {
        _TimeText.text = "Time: 0s";
        _MessageText.text = "";
        switch (StaticValue.Get()._Difficult)
        {
            case StaticValue.Easy:
                _TitleText.text = "Easy";
                break;

            case StaticValue.Normal:
                _TitleText.text = "Normal";
                break;
            case StaticValue.Hard:
                _TitleText.text = "Hard";
                break;
            case StaticValue.Lunatic:
                _TitleText.text = "Lunatic";
                break;
            case StaticValue.Empty:
                _TitleText.text = "Edit";
                break;
            case StaticValue.Load:
                _TitleText.text = "Load Game";
                break;
        }

    }

    //添加各种监听
    void AddListener()
    {
        //添加数字Panel监听
        Button[] nums = _NumberPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < nums.Length; ++i)
        {
            GameObject temp = nums[i].gameObject;
            nums[i].onClick.AddListener(delegate () { ClickNumberPanel(temp); });
        }

        //添加撤销Panel监听
        Button No = _OKPanel.GetComponentInChildren<Button>();
        GameObject tempNo = No.gameObject;
        No.onClick.AddListener(delegate () { ClickNo(tempNo); });

        //添加背景监听
        _BackGround.GetComponent<Button>().onClick.AddListener(ClickBackGround);

        //添加菜单按钮监听
        Button[] menuButtons = _MenuPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < menuButtons.Length; ++i)
        {
            GameObject temp = menuButtons[i].gameObject;
            menuButtons[i].onClick.AddListener(delegate () { ClickMenuPanel(temp); });
            if (temp.name == "Hint")
            {
                HintButton = temp;
                HintButton.SetActive(false);
            }
        }

        Button[] saveButtons = _SaveGame.GetComponentsInChildren<Button>();
        for (int i = 0; i < saveButtons.Length; ++i)
        {
            GameObject temp = saveButtons[i].gameObject;
            saveButtons[i].onClick.AddListener(delegate () { ClickSavePanel(temp); });
        }
    }

    //初始化数独
    void InitSudokuCell()
    {
        if (Sudoku == null)
        {
            Sudoku = new SudokuManager();
        }
        Sudoku.GenerateSudoku();
        CellObject = new GameObject[9 * 9];
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; ++j)
            {
                //复制prefab对象
                CellObject[i * 9 + j] = Instantiate(_SudokuCell);
                GameObject cell = CellObject[i * 9 + j];
                cell.transform.SetParent(_SudokuPanel.transform);
                cell.transform.localScale = Vector3.one;
                cell.GetComponent<SudokuCell>().X = i;
                cell.GetComponent<SudokuCell>().Y = j;

                //该格为初始数字
                if (Sudoku.cell[i, j].isOrigin)
                {
                    cell.GetComponent<Button>().enabled = false;
                    cell.transform.GetComponentInChildren<Text>().text =
                        Sudoku.cell[i, j].value.ToString();
                    cell.transform.GetComponentInChildren<Text>().color =
                        Color.black;
                }
                else
                {
                    cell.transform.GetComponentInChildren<Text>().text = "";
                }
                //添加填数Panel监听
                cell.GetComponent<Button>().onClick.AddListener(delegate () { ClickEmptyCell(cell); });
            }
        }

        _NumberPanel.SetActive(false);

    }

    //点击数独空格
    void ClickEmptyCell(GameObject now)
    {
        _NumberPanel.SetActive(false);
        _OKPanel.SetActive(false);
        HintButton.SetActive(false);

        activeCell = now;

        Text nowText = now.transform.GetComponentInChildren<Text>();

        //尚未填数
        if (nowText.text == "")
        {
            _NumberPanel.transform.position = Input.mousePosition;
            _NumberPanel.SetActive(true);
            HintButton.SetActive(true);
        }
        //已经填数
        else
        {
            _OKPanel.transform.position = Input.mousePosition;
            _OKPanel.SetActive(true);
        }

    }

    //点击数字Panel，将数字写到数独中
    void ClickNumberPanel(GameObject now)
    {
        //填写数字
        if (activeCell == null)
        {
            _NumberPanel.SetActive(false);
            return;
        }

        SudokuCell tempCell = activeCell.GetComponent<SudokuCell>();
        int thisNum = int.Parse(now.GetComponentInChildren<Text>().text);


        //是否行列宫对角线有相同数字
        Vector2Int SameNum = Sudoku.CheckSudokunumber(tempCell.X, tempCell.Y, thisNum);

        //填入数字
        if (SameNum == Vector2Int.down)
        {
            Debug.Log("OK");

            activeCell.GetComponentInChildren<Text>().text = now.GetComponentInChildren<Text>().text;
            activeCell.GetComponentInChildren<Text>().color = now.GetComponent<Image>().color;
            Sudoku.AddSudokuNumber(tempCell.X, tempCell.Y, thisNum);

            if (StaticValue.Get()._Difficult != StaticValue.Empty && Sudoku.isFinish())
            {
                FinishAnim();
            }

        }
        else
        {
            Debug.Log("not OK");
            CellObject[SameNum.x * 9 + SameNum.y].GetComponent<SudokuCell>().PlaySame();
        }

        _NumberPanel.SetActive(false);
        activeCell = null;
        HintButton.SetActive(false);
    }


    //撤销
    void ClickNo(GameObject no)
    {
        if (activeCell == null)
        {
            return;
        }


        activeCell.GetComponentInChildren<Text>().text = "";
        activeCell.GetComponentInChildren<Text>().color = Color.white;

        SudokuCell tempCell = activeCell.GetComponent<SudokuCell>();
        Sudoku.RemoveSudokuNumber(tempCell.X, tempCell.Y);

        _OKPanel.SetActive(false);
        _MessageText.text = "";
        activeCell = null;
        HintButton.SetActive(false);
    }

    void ClickHint()
    {
        if (activeCell == null)
        {
            return;
        }
        SudokuCell tempCell = activeCell.GetComponent<SudokuCell>();
        int num = Sudoku.GetHintNumber(tempCell.X, tempCell.Y);
        if (num == 0)
        {
            Debug.Log("No legal number");
            ShowMessage("No legal number!");
            Invoke("ClearMessage", 1.2f);
        }
        else
        {
            Debug.Log("Get hint");
            activeCell.GetComponentInChildren<Text>().text = num.ToString();
            activeCell.GetComponentInChildren<Text>().color = Color.gray;

        }
        _OKPanel.SetActive(false);
        _NumberPanel.SetActive(false);
        activeCell = null;
        HintButton.SetActive(false);
    }

    void ClickSolve()
    {
        bool isOK = Sudoku.SolveSudoku();
        if (isOK)
        {
            for (int i = 0; i < CellObject.Length; ++i)
            {
                GameObject now = CellObject[i];
                if (now.GetComponentInChildren<Text>().text == "")
                {
                    now.GetComponentInChildren<Text>().text =
                        Sudoku.cell[now.GetComponent<SudokuCell>().X, now.GetComponent<SudokuCell>().Y].value.ToString();
                    now.GetComponentInChildren<Text>().color = Color.gray;
                }
            }
        }
        else
        {
            Debug.Log("No valid solution!!!");
            ShowMessage("No valid solution!!!");
            Invoke("ClearMessage", 2.0f);
        }

        _OKPanel.SetActive(false);
        _NumberPanel.SetActive(false);
        activeCell = null;
        HintButton.SetActive(false);
    }

    //点击背景
    void ClickBackGround()
    {
        _NumberPanel.SetActive(false);
        _OKPanel.SetActive(false);
        activeCell = null;
        HintButton.SetActive(false);
    }

    //点击存储
    void ClickSave()
    {
        _SaveGame.SetActive(true);
    }

    void ClickSavePanel(GameObject go)
    {
        switch (go.name)
        {
            case "Back":
                _SaveGame.SetActive(false);
                break;
            case "OK":
                ClickOKButton();
                break;
        }
    }

    void ClickOKButton()
    {
        string name = _Input.text;
        if (name == "")
        {
            return;
        }
        string path = Application.persistentDataPath + "/Save";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fullname = path + "/" + name + ".svdata";

        if (File.Exists(fullname))
        {
            SaveMessage.text = "This name already Exists!";
            return;
        }

        StaticValue.Get()._Filename = name + ".svdata";

        if (saveManager == null)
        {
            saveManager = new GameSaveManager();
        }
        saveManager.SaveGame(this);
        _SaveGame.SetActive(false);
        ShowMessage("Save Successed!");
        Invoke("ClearMessage", 2.0f);
    }

    //将游戏数据存储到SudokuInfo中
    public SudokuInfo StoreInfo()
    {
        _SaveData = new SudokuInfo();
        _SaveData.cellColor = new Color[9 * 9];
        _SaveData.cellNum = new int[9 * 9];
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; j++)
            {
                _SaveData.cellColor[i * 9 + j] = CellObject[i * 9 + j].transform.GetComponentInChildren<Text>().color;
                _SaveData.cellNum[i * 9 + j] = Sudoku.cell[i, j].value;
            }
        }
        return _SaveData;
    }


    //加载数独
    void LoadSudokuCell()
    {
        if (Sudoku == null)
        {
            Sudoku = new SudokuManager();
        }

        GameSaveManager manager = new GameSaveManager();
        manager.LoadGame();

        Sudoku.InitSudokuInfo();

        for (int i = 0; i < Nine; ++i)
        {
            for (int j = 0; j < Nine; ++j)
            {
                Sudoku.cell[i, j].value = manager.info.cellNum[i * 9 + j];
                if (Sudoku.cell[i, j].value != 0 && manager.info.cellColor[i * 9 + j] == Color.black)
                {
                    Sudoku.cell[i, j].isOrigin = true;
                }
                else
                {
                    Sudoku.cell[i, j].isOrigin = false;
                }
            }
        }

        CellObject = new GameObject[9 * 9];
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; ++j)
            {
                //复制prefab对象
                CellObject[i * 9 + j] = Instantiate(_SudokuCell);
                GameObject cell = CellObject[i * 9 + j];
                cell.transform.SetParent(_SudokuPanel.transform);
                cell.transform.localScale = Vector3.one;
                cell.GetComponent<SudokuCell>().X = i;
                cell.GetComponent<SudokuCell>().Y = j;

                //该格为初始数字
                if (Sudoku.cell[i, j].isOrigin)
                {
                    cell.GetComponent<Button>().enabled = false;
                    cell.transform.GetComponentInChildren<Text>().text =
                        Sudoku.cell[i, j].value.ToString();
                    cell.transform.GetComponentInChildren<Text>().color =
                        Color.black;
                }
                else if (Sudoku.cell[i, j].value != 0)
                {
                    cell.transform.GetComponentInChildren<Text>().text =
                        Sudoku.cell[i, j].value.ToString();
                    cell.transform.GetComponentInChildren<Text>().color =
                        manager.info.cellColor[i * 9 + j];
                }
                else
                {
                    cell.transform.GetComponentInChildren<Text>().text = "";
                }
                //添加填数Panel监听
                cell.GetComponent<Button>().onClick.AddListener(delegate () { ClickEmptyCell(cell); });
            }
        }

        _NumberPanel.SetActive(false);

    }

    //点击菜单Panel
    void ClickMenuPanel(GameObject now)
    {
        switch (now.name)
        {
            case "Exit":
                Application.Quit();
                break;

            case "Home":
                SceneManager.LoadScene("Start");
                break;

            case "Hint":
                Debug.Log("Hint");
                ClickHint();
                break;

            case "Solve":
                ClickSolve();
                break;
            case "Save":
                ClickSave();
                break;
            default:
                break;
        }
    }
    private float time;

    void CountTime()
    {
        time += Time.fixedDeltaTime;
        _TimeText.text = "Time: " + (int)(time) + "s";
    }

    void FinishAnim()
    {
        _MessageText.text = "You Win!\nTime: " + ((int)time).ToString() + "s";
        for (int i = 0; i < CellObject.Length; i++)
        {
            CellObject[i].GetComponent<SudokuCell>().PlaySame();
        }
    }

    void ShowMessage(string str)
    {
        _MessageText.text = str;
    }

    void ClearMessage()
    {
        _MessageText.text = "";
    }

}
