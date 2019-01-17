using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StatEntity_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/Resources/ErdExcels/Item.xlsx";
    private static readonly string[] sheetNames = { "StatEntity" };
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (!filePath.Equals(asset))
                continue;

            using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}

                foreach (string sheetName in sheetNames)
                {
                    var exportPath = "Assets/Resources/ErdExcels/Item_Assets/" + sheetName + ".asset";
                    
                    // check scriptable object
                    var data = (StatEntity)AssetDatabase.LoadAssetAtPath(exportPath, typeof(StatEntity));
                    if (data == null)
                    {
					     //���丮�� ���ٸ� ������ش�
                        if (!Directory.Exists(Path.GetDirectoryName(exportPath).Replace("\\", "/")))
                            Directory.CreateDirectory(Path.GetDirectoryName(exportPath).Replace("\\", "/"));

                        data = ScriptableObject.CreateInstance<StatEntity>();
                        AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
                        //data.hideFlags = HideFlags.NotEditable;
                    }
                    data.param.Clear();

					// check sheet
                    var sheet = book.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        ////////debug.logError("[QuestData] sheet not found:" + sheetName);
                        continue;
                    }


					bool isNormalDataend = false;
                	// add infomation
                    for (int i=2; i<= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
						if (row == null)
                            break;
						if (row.GetCell(0) != null )
                        {
                            row.GetCell(0).SetCellType(CellType.String);
                            if (row.GetCell(0).StringCellValue == "DATAEND")
                            {
                                isNormalDataend = true;
                                break;
                            }
                        }
                        ICell cell = null;

                        var p = new StatEntity.Param();
			
					cell = row.GetCell(0);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "")   p.statType =  (StatType)System.Enum.Parse(typeof(StatType), cell.StringCellValue);
					cell = row.GetCell(1);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "") p.statName = cell.StringCellValue;

                        data.param.Add(p);
                    }
                    
					// error alarm
                    if (data.param.Count < 1 || isNormalDataend == false)
                    {
                        //debug.log("Reimport fail:" + filePath);
                        EditorUtility.DisplayDialog("Error File Detected!", filePath, "ok");
                    }

                    // save scriptable object
                    ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
                    EditorUtility.SetDirty(obj);
                }
            }

        }
    }
}