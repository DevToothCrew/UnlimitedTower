using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class MountItemEntity_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/Resources/ErdExcels/Item.xlsx";
    private static readonly string[] sheetNames = { "MountItemEntity" };
    
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
                    var data = (MountItemEntity)AssetDatabase.LoadAssetAtPath(exportPath, typeof(MountItemEntity));
                    if (data == null)
                    {
					     //�����丮�� ���ٸ� �������ش�
                        if (!Directory.Exists(Path.GetDirectoryName(exportPath).Replace("\\", "/")))
                            Directory.CreateDirectory(Path.GetDirectoryName(exportPath).Replace("\\", "/"));

                        data = ScriptableObject.CreateInstance<MountItemEntity>();
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

                        var p = new MountItemEntity.Param();
			
					cell = row.GetCell(0);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "") int.TryParse( cell.StringCellValue, out p.mountItemNum);
					cell = row.GetCell(1);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "")   p.mountitemType =  (MountitemType)System.Enum.Parse(typeof(MountitemType), cell.StringCellValue);
					cell = row.GetCell(2);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "") p.itemName = cell.StringCellValue;
					cell = row.GetCell(3); if(cell != null) cell.SetCellType(CellType.String);  if(cell != null && cell.StringCellValue != "") bool.TryParse(cell.StringCellValue, out p.isSpeJobPossible);
					cell = row.GetCell(4);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "") int.TryParse( cell.StringCellValue, out p.possibleJobNum);
					cell = row.GetCell(5);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "") int.TryParse( cell.StringCellValue, out p.teerNum);
					cell = row.GetCell(6);  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != "") int.TryParse( cell.StringCellValue, out p.gradeNum);

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
