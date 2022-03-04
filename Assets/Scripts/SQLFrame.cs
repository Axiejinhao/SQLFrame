using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;

public class SQLFrame
{
    #region Singleton
    private static SQLFrame instance;

    public static SQLFrame GetInstance()
    {
        if (instance == null)
        {
            instance = new SQLFrame();
        }
        return instance;
    }

    protected SQLFrame() { }
    #endregion

    private string databasePath;
    private SqliteConnection con;
    private SqliteCommand command;
    private SqliteDataReader reader;

    public void OpenDatabase(string databaseName)
    {
        if (!databaseName.EndsWith(".sqlite"))
        {
            databaseName += ".sqlite";
        }
#if UNITY_EDITOR
        databasePath = "Data Source = " + Application.streamingAssetsPath + "/" + databaseName;
#endif

        con = new SqliteConnection(databasePath);
        con.Open();

        command = con.CreateCommand();

    }

    public void CloseDatabase()
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
    /// 执行非查询语句
    /// </summary>
    /// <param name="query"></param>
    private int NonSelect(string query)
    {
        //赋值SQL语句
        command.CommandText = query;
        //执行SQL语句
        return command.ExecuteNonQuery();
    }

    public int InsertExe(string query)
    {
        return NonSelect(query);
    }

    public int UpdateExe(string query)
    {
        return NonSelect(query);
    }

    public int DeleteExe(string query)
    {
        return NonSelect(query);
    }

    /// <summary>
    /// 查询单个数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public object SelectSingleData(string query)
    {
        command.CommandText = query;
        return command.ExecuteScalar();
    }

    /// <summary>
    /// 查询多个数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public List<ArrayList> SelectMultipleData(string query)
    {
        command.CommandText = query;
        reader = command.ExecuteReader();
        //一行多列ArrayList
        //多行多列List<ArrayList>
        List<ArrayList> result = new List<ArrayList>();
        while (reader.Read())
        {
            //用ArrayList存储一行多列
            ArrayList currentRow = new ArrayList();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                //将当前列的数据添加到集合中
                currentRow.Add(reader.GetValue(i));
            }
            //将当前行的数据放到List中
            result.Add(currentRow);
        }
        reader.Close();
        return result;
    }
}
