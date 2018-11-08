using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };
	
	public static List<Dictionary<string, object>> Read(string file)
	{
		var list = new List<Dictionary<string, object>>();
		TextAsset data = Resources.Load (file) as TextAsset;
		
		var lines = Regex.Split (data.text, LINE_SPLIT_RE);
		
		if(lines.Length <= 1) return list;
		
		var header = Regex.Split(lines[0], SPLIT_RE);
		for(var i=1; i < lines.Length; i++) {
			
			var posOffsets = Regex.Split(lines[i], SPLIT_RE);
			if(posOffsets.Length == 0 ||posOffsets[0] == "") continue;
			
			var entry = new Dictionary<string, object>();
			for(var j=0; j < header.Length && j < posOffsets.Length; j++ ) {
				string posOffset = posOffsets[j];
				posOffset = posOffset.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				object finalposOffset = posOffset;
				int n;
				float f;
				if(int.TryParse(posOffset, out n)) {
					finalposOffset = n;
				} else if (float.TryParse(posOffset, out f)) {
					finalposOffset = f;
				}
				entry[header[j]] = finalposOffset;
			}
			list.Add (entry);
		}
		return list;
	}
}
