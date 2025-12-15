using UnityEngine;//TextAsset
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;//정규 표현식 (Regex)을 사용

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    static char COMMENT_CHAR = '#'; //주석행 추가

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();

        string fullPath = "B/" + file; //폴더 경로 B

        TextAsset data = Resources.Load(fullPath) as TextAsset;

        if (data == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + fullPath);
            return list;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            string line = lines[i]; // 현재 줄을 변수에 저장

            // 추가된 로직: 현재 줄이 주석 문자로 시작하는지 확인
            if (line.Length > 0 && line.TrimStart().StartsWith(COMMENT_CHAR.ToString()))
            {
                continue; // 주석이면 다음 반복으로 넘어갑니다.
            }

            var values = Regex.Split(line, SPLIT_RE);//lines[i]=> line
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
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
}
