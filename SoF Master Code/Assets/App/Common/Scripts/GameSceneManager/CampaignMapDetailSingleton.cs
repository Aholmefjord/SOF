using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignMapDetailSingleton
{
    /*
    #region Singleton
    private static CampaignMapDetailSingleton instance = null;

    private CampaignMapDetailSingleton() { }

    public static CampaignMapDetailSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CampaignMapDetailSingleton();
                instance.Init();
            }

            return instance;
        }
    }

    #endregion

    int mTotalLessonCount;
    List<MapIcon> mMapIconList;
    GameSceneManager mGameSceneManager;

    public void Init()
    {
        //Make sure Game Scene Manager is loaded
        mGameSceneManager = GameSceneManager.getInstance();

        mTotalLessonCount = JULESTech.DataStore.Instance.sofLessonPlanData.GetInt("total_lesson");

        mMapIconList = new List<MapIcon>();

        for (int i = 1; i <= mTotalLessonCount; ++i)
        {
            MapIcon icon = new MapIcon();

            icon.type = JULESTech.DataStore.Instance.sofLessonPlanData.GetString("lesson_" + i);
            icon.name = JULESTech.DataStore.Instance.sofLessonPlanData.GetString("lesson_" + i + "_name");
            icon.description = JULESTech.DataStore.Instance.sofLessonPlanData.GetString("lesson_" + i + "_description");
            icon.command = JULESTech.DataStore.Instance.sofLessonPlanData.GetString("lesson_" + i + "_cmd");

            Debug.Log("Campaign Map Detail Load Map Icon - name:" + icon.type + " name:" + icon.name + " command: " + icon.command);

            mMapIconList.Add(icon);
        }
        Debug.Log(mMapIconList);
    }

    public MapIcon GetMapIcon(int _index)
    {
        if (mMapIconList.Count <= _index)
        {
            Debug.Log("CampaignMapDetail Get Map Icon, index is invalid:" + _index);
            return new global::MapIcon();
        }

        return mMapIconList[_index];
    }

    public void CleanMap()
    {
        mMapIconList.Clear();
    }

    public int GetTotalLessonCount()
    {
        return mTotalLessonCount;
    }

    public int GetTotalTerm()
    {
        return JULESTech.DataStore.Instance.schoolData.GetInt("total_terms");
    }

    /// <summary>
    /// Get term lesson start number, stored in the school data.
    /// </summary>
    /// <param name="_termNumber">Term Number</param>
    /// <returns>Return the first lesson number for the term. Return 0 is error.</returns>
    public int GetTermStartLessonNumber(int _termNumber)
    {
        if (GetTotalTerm() < _termNumber)
        {
            Debug.Log("Campaign Map Detail Singleton, get term start lesson number, term number is less than total term");
            return 0;
        }

        return JULESTech.DataStore.Instance.schoolData.GetInt("term_" + _termNumber + "_start"); 
    }

    /// <summary>
    /// Get term lesson end number, stored in the school data.
    /// </summary>
    /// <param name="_termNumber">Term Number</param>
    /// <returns>Return the last lesson number for the term. Return 0 is error.</returns>
    public int GetTermEndLessonNumber(int _termNumber)
    {
        if (GetTotalTerm() < _termNumber)
        {
            Debug.Log("Campaign Map Detail Singleton, get term end lesson number, term number is less than total term");
            return 0;
        }

        return JULESTech.DataStore.Instance.schoolData.GetInt("term_" + _termNumber + "_end");
    }

    /*
    /// <summary>
    /// Issue command to game scene manager based on the map icon information.
    /// </summary>
    /// <param name="_index">Index number of the game icon in the map, stats off as 1 not 0</param>
    public void LaunchLesson(int _index)
    {
        MapIcon icon = GetMapIcon(_index-1);

        string command = icon.command;

        //break the string into half
        string lessString = command.Substring(1, command.IndexOf("_") - 1);

        Debug.Log("Lesson Number get: " + lessString);

        string lessNumString = icon.command.Substring(icon.command.IndexOf("_") + 2);

        Debug.Log("Lesson Part Number get: " + lessNumString);

        int lessonNumber = int.Parse(lessString);
        int lessonPartNumber = int.Parse(lessNumString);

        mGameSceneManager.SetCurrentLessonNumber(lessonNumber);
        mGameSceneManager.SetCurrentLessonPartNumber(lessonPartNumber);
        mGameSceneManager.TriggerLessonWithCurrent();
    }
    */
}

/*
public struct MapIcon
{
    public string type;
    public string name;
    public string command;
    public string description;
}
*/