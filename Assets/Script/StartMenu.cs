using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    public GameObject menu;
    public GameObject _Loading;

    #region Load
    public GameObject load;

    public InputField _input;

    public Text _Message;
    public Text _List;
    #endregion Load
    public Animator menuAnim;

    public AudioSource menuAudio;


    private int scaleWidth = 0;
    private int scaleHeight = 0;

    void setScreenScale()
    {
        scaleWidth = 1920;
        scaleHeight = 1080;
        Screen.SetResolution(scaleWidth, scaleHeight, true);
    }

    private void Awake()
    {
        setScreenScale();
        Screen.fullScreen = false;
        load.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        AddListener();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddListener()
    {
        Button[] buttons = menu.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject _diff = buttons[i].gameObject;
            buttons[i].onClick.AddListener(delegate () { ClickLevelButton(_diff); });
        }

        Button[] loadButtons = load.GetComponentsInChildren<Button>();
        for (int i = 0; i < loadButtons.Length; i++)
        {
            GameObject btn = loadButtons[i].gameObject;
            loadButtons[i].onClick.AddListener(delegate () { ClickLoadButton(btn); });
        }
    }

    void ClickLevelButton(GameObject _diff)
    {
        switch (_diff.name)
        {
            case "Easy":
                StaticValue.Get()._Difficult = StaticValue.Easy;
                break;
            case "Normal":
                StaticValue.Get()._Difficult = StaticValue.Normal;
                break;
            case "Hard":
                StaticValue.Get()._Difficult = StaticValue.Hard;
                break;
            case "Lunatic":
                StaticValue.Get()._Difficult = StaticValue.Lunatic;
                break;
            case "Empty":
                StaticValue.Get()._Difficult = StaticValue.Empty;
                break;
            case "Load":
                StaticValue.Get()._Difficult = StaticValue.Load;
                ClickLoad();
                return;
            //break;

            case "Exit":
                Application.Quit();
                return;
            //UnityEditor.EditorApplication.isPlaying = false;

            default:
                Debug.Log("Can't match any difficult level!");
                break;
        }
        Invoke("OpenScene", 1.0f);
    }

    void OpenScene()
    {
        _Loading.SetActive(true);
        SceneManager.LoadScene("Sudoku");
    }

    List<string> FilenameList;

    void ClickLoad()
    {
        menu.SetActive(false);
        load.SetActive(true);
        _List.text = "";
        FilenameList = new List<string>();

        string path = Application.persistentDataPath + "/Save";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var txtFiles = Directory.EnumerateFiles(path, "*.svdata");

        int Num = 1;

        foreach (string currentFile in txtFiles)
        {
            string fileName = currentFile.Substring(path.Length + 1);
            _List.text += Num.ToString() + ": " + fileName + "\n";
            FilenameList.Add(fileName);
            Num++;
        }

    }

    void ClickLoadButton(GameObject obj)
    {
        switch (obj.name)
        {
            case "Back":
                menu.SetActive(true);
                load.SetActive(false);
                break;
            case "OK":
                LoadFileGame();
                break;
            default:
                break;
        }
    }

    void LoadFileGame()
    {
        int num;
        try
        {
            num = int.Parse(_input.text);
        }
        catch (System.Exception)
        {
            _Message.text = "Invalid Input!";
            return;
        }
        if (num - 1 < 0 || num - 1 >= FilenameList.Count)
        {
            _Message.text = "Invalid Input!";
            return;
        }
        Debug.Log("num = " + num.ToString());
        string path = Application.persistentDataPath;
        if (!Directory.Exists(path + "/Save"))
        {
            Directory.CreateDirectory(path + "/Save");
        }

        string fullname = path + "/Save" + "/" + FilenameList[num - 1];

        Debug.Log(fullname);

        if (File.Exists(fullname))
        {
            StaticValue.Get()._Filename = FilenameList[num - 1];
            OpenScene();
        }
        else
        {
            _Message.text = "Invalid Input!";
        }

    }

}
