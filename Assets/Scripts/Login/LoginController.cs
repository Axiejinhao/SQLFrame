using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;

public class LoginController : MonoBehaviour
{
    private InputField unameInput;
    private InputField pwdInput;
    private Toggle rememberMeToggle;
    private Button loginButton;

    //数据库路径
    private string dataBasePath;
    //数据库连接对象
    private SqliteConnection con;
    //数据库指令对象
    private SqliteCommand command;
    //数据库读取对象
    private SqliteDataReader reader;
    //指令
    private string commandText;

    private bool rememberMe = false;

    private void Awake()
    {
        unameInput = transform.GetChild(1).GetComponent<InputField>();
        pwdInput = transform.GetChild(2).GetComponent<InputField>();
        rememberMeToggle = transform.GetChild(3).GetComponent<Toggle>();
        loginButton = transform.GetChild(4).GetComponent<Button>();
    }

    private void Start()
    {
        //绑定回调
        rememberMeToggle.onValueChanged.AddListener(OnRememberMeToggleValueChange);
        loginButton.onClick.AddListener(OnLoginButtonClick);

        //数据库初始化
        DataBaseInit();
        //加载用户及密码
        LoadRememberUser();
    }

    /// <summary>
    /// 加载记住的用户和密码
    /// </summary>
    private void LoadRememberUser()
    {
        if (PlayerPrefs.HasKey("uname"))
        {
            unameInput.text = PlayerPrefs.GetString("uname");
            pwdInput.text = PlayerPrefs.GetString("pwd");
            rememberMeToggle.isOn = true;
        }
    }

    /// <summary>
    /// 数据库初始化封装
    /// </summary>
    private void DataBaseInit()
    {
        //设置连接路径
        dataBasePath = "Data Source = " + Application.streamingAssetsPath + "/Userdatabase.sqlite";
        //实例化连接对象
        con = new SqliteConnection(dataBasePath);
        con.Open();

        command = con.CreateCommand();

    }

    private void DataBaseDispose()
    {
        if (con != null)
        {
            con.Close();
            con = null;
        }
        if (command != null)
        {
            command.Dispose();
            command = null;
        }
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
    }

    /// <summary>
    /// 记住密码的变值回调
    /// </summary>
    /// <param name="isOn"></param>
    private void OnRememberMeToggleValueChange(bool isOn)
    {
        //记录是否记住密码
        rememberMe = isOn;
        if (!isOn)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    private void OnLoginButtonClick()
    {
        if (unameInput.text == "" || pwdInput.text == "")
        {
            Debug.Log("用户名或密码不能为空");
            return;
        }
        commandText = "select pwd from UserTable where uname ='" + unameInput.text + "';";
        command.CommandText = commandText;

        object pwd = command.ExecuteScalar();
        if (pwd == null)
        {
            Debug.Log("该用户不存在");
            return;
        }

        if (pwd.ToString() == pwdInput.text)
        {
            if (rememberMe)
            {
                PlayerPrefs.SetString("uname", unameInput.text);
                PlayerPrefs.SetString("pwd", pwdInput.text);
            }
            Debug.Log("登录成功");
        }
        else
        {
            Debug.Log("用户名或密码错误");
        }
    }

    private void OnApplicationQuit()
    {
        DataBaseDispose();
    }
}
