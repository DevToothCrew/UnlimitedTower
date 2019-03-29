using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader : MonoBehaviour
{
    static private CSVReader instance = null;

    static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static readonly char[] TRIM_CHARS = { '\"' };

    public class CSVPairs : Dictionary<string, object> { };
    public class CSVPairsList : List<CSVPairs> { };

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public static void ReadAsync(string file, Action<CSVPairsList> onSuccess = null, Action<float> onProgress = null, Action<string> onFailed = null)
    {
        instance.StartCoroutine(CoRead(file, onSuccess, onProgress, onFailed));
    }

    public static CSVPairsList Read(string file)
    {
        CSVPairsList list = new CSVPairsList();
        TextAsset data = Resources.Load<TextAsset>(file);

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            return list;
        }

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var posOffsets = Regex.Split(lines[i], SPLIT_RE);

            if (posOffsets.Length == 0 || string.IsNullOrWhiteSpace(posOffsets[0]))
                continue;

            var entry = new CSVPairs();
            for (var j = 0; j < header.Length && j < posOffsets.Length; j++)
            {
                string posOffset = posOffsets[j];
                posOffset = posOffset.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalposOffset = posOffset;
                int n;
                float f;
                if (int.TryParse(posOffset, out n))
                {
                    finalposOffset = n;
                }
                else if (float.TryParse(posOffset, out f))
                {
                    finalposOffset = f;
                }
                entry[header[j]] = finalposOffset;
            }
            list.Add(entry);
        }

        return list;
    }

    private static IEnumerator CoRead(string file, Action<CSVPairsList> onSuccess, Action<float> onProgress, Action<string> onFailed)
    {
        var list = new CSVPairsList();
        var req = Resources.LoadAsync<TextAsset>(file);
        yield return req;

        if (req.asset as TextAsset == null)
        {
            onFailed?.Invoke($"<color=red>{file}</color> is not existed.");
            yield break;
        }

        onProgress?.Invoke(0.5f);

        TextAsset data = req.asset as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            onFailed?.Invoke($"<color=red>{file}</color> is empty.");
            yield break;
        }

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            yield return null;

            var posOffsets = Regex.Split(lines[i], SPLIT_RE);

            if (posOffsets.Length == 0 || string.IsNullOrWhiteSpace(posOffsets[0]))
                continue;

            var entry = new CSVPairs();
            for (var j = 0; j < header.Length && j < posOffsets.Length; j++)
            {
                string posOffset = posOffsets[j];
                posOffset = posOffset.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalposOffset = posOffset;
                int n;
                float f;
                if (int.TryParse(posOffset, out n))
                {
                    finalposOffset = n;
                }
                else if (float.TryParse(posOffset, out f))
                {
                    finalposOffset = f;
                }
                entry[header[j]] = finalposOffset;
            }
            list.Add(entry);
        }

        onSuccess?.Invoke(list);
    }
}
