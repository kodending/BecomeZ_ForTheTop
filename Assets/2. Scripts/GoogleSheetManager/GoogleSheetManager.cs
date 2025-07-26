using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Linq;

//등록해야됨 각 시트에 들어갈놈들을

public class GoogleSheetManager : MonoBehaviour
{
    public static GoogleSheetManager instance;

    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    const string defaultURL = "https://docs.google.com/spreadsheets/d/1o9i-q5HB_8lupPaUWiR-juc7r1KPvmNyTRwFhUWPSsc/export?format=csv&gid=";

    [Tooltip("구글 데이터 딕셔너리")]
    public Dictionary<GOOGLEDATALOADTYPE, GoogleGameData> m_dicGoogleData = new Dictionary<GOOGLEDATALOADTYPE, GoogleGameData>();

    public static List<Dictionary<string, object>> m_runeInfo;
    public static List<Dictionary<string, object>> m_consumeInfo;
    public static List<Dictionary<string, object>> m_skillInfo;
    public static List<Dictionary<string, object>> m_RankStatInfo;
    public static List<Dictionary<string, object>> m_enemyInfo;
    public static List<Dictionary<string, object>> m_classInfo;
    public static List<Dictionary<string, object>> m_levelDesignInfo;
    public static List<Dictionary<string, object>> m_enemySkillInfo;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(instance);
    }

    IEnumerator Start()
    {
        //먼저 타입안에 들어온 enum문들을 정리한다. dic으로
        Dictionary<int, GOOGLEDATALOADTYPE> dicGoogleSheetId = new Dictionary<int, GOOGLEDATALOADTYPE>();

        int cnt = 0;

        foreach(GOOGLEDATALOADTYPE type in Enum.GetValues(typeof(GOOGLEDATALOADTYPE)))
        {
            dicGoogleSheetId.Add(cnt, type);
            cnt++;
        }


        for (int i = 0; i < dicGoogleSheetId.Count; i++)
        {
            Int64 num = (Int64)dicGoogleSheetId[i];
            string id = num.ToString();

            UnityWebRequest www = UnityWebRequest.Get(defaultURL + id);
            yield return www.SendWebRequest();

            string data = www.downloadHandler.text;

            GoogleGameData gData = new GoogleGameData();
            m_dicGoogleData.Add(dicGoogleSheetId[i], gData);
            m_dicGoogleData[dicGoogleSheetId[i]].recordDataList = Read(data);
            m_dicGoogleData[dicGoogleSheetId[i]].recordTitleList = ReadTitle(data);
        }

        m_runeInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.RUNEINFO].recordDataList;
        m_consumeInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.CONSUMEINFO].recordDataList;
        m_skillInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.SKILLINFO].recordDataList;
        m_RankStatInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.RANKSTATINFO].recordDataList;
        m_enemyInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.ENEMYINFO].recordDataList;
        m_classInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.CLASSINFO].recordDataList;
        m_levelDesignInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.LEVELDESIGN].recordDataList;
        m_enemySkillInfo = m_dicGoogleData[GOOGLEDATALOADTYPE.ENEMYATTACKINFO].recordDataList;

        yield return null;
    }

    static List<Dictionary<string, object>> Read(string data)
    {
        if (data == "") return null;

        var list = new List<Dictionary<string, object>>();
        string[] lines;
        string source;

        StringReader sr = new StringReader(data);
        source = sr.ReadToEnd();
        sr.Close();

        lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                value = value.Replace("<c>", ",");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }

        return list;
    }

    static List<Dictionary<int, object>> ReadTitle(string data)
    {
        var list = new List<Dictionary<int, object>>();
        string[] lines;
        string source;

        StringReader sr = new StringReader(data);
        source = sr.ReadToEnd();
        sr.Close();

        lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 0; i < 1; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<int, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                value = value.Replace("<c>", ",");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[j] = finalvalue;
            }
            list.Add(entry);
        }

        return list;
    }

    public static List<string> ParseAvailableClassType(string raw)
    {
        return raw.Split(',')
                  .Select(cls => cls.Trim())
                  .ToList();
    }
}
 