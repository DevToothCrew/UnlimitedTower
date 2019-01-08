#pragma warning disable 0219

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Text;

public class ExelImportermaker_2 : EditorWindow
{
    private Vector2 curretScroll = Vector2.zero;

    void OnGUI()
    {
        GUILayout.Label("makeing importer", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField("class name", className);
        sepalateSheet = EditorGUILayout.Toggle("sepalate sheet", sepalateSheet);

        EditorPrefs.SetBool(s_key_prefix + fileName + ".separateSheet", sepalateSheet);

        if (GUILayout.Button("create"))
        {
            EditorPrefs.SetString(s_key_prefix + fileName + ".className", className);
            for (int i = 0; i < Sheet정보_list.Count; i++)
            {
                ExportEntity(Sheet정보_list[i]);
                ExportImporter(Sheet정보_list[i]);
            }
            
            AssetDatabase.ImportAsset(filePath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Close();
        }
    }

    public enum ValueType
    {
        BOOL,
        STRING,
        INT,
        FLOAT,
        DOUBLE,
    }

    private string fileName = string.Empty;
    private  string filePath = string.Empty;

    private bool sepalateSheet = false;
    private List<ExcelRowParameter> typeList = new List<ExcelRowParameter>();
    private List<ExcelSheetParameter> sheetList = new List<ExcelSheetParameter>();
    private string className = string.Empty;
    private static string s_key_prefix = "terasurware.exel-importer-maker.";

    private List<sheet별정보> Sheet정보_list = new List<sheet별정보>();

    // sheet별로 이제 typelist넣어주고, sheetname넣어주고, classname넣어주면됨.
    [MenuItem("Assets/엑셀쉬트별로파싱하기")]
    static void ExportExcelToAssetbundle()
    {
        foreach (Object obj in Selection.objects)
        {
            var window = ScriptableObject.CreateInstance<ExelImportermaker_2>();
            window.filePath = AssetDatabase.GetAssetPath(obj);
            window.fileName = Path.GetFileNameWithoutExtension(window.filePath);


            using (FileStream stream = File.Open(window.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book = null;
                if (Path.GetExtension(window.filePath) == ".xls")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }


                // sheet별로 
                for (int i = 0; i < book.NumberOfSheets; ++i)
                {
                    ISheet s = book.GetSheetAt(i);
                    ExcelSheetParameter sht = new ExcelSheetParameter();
                    sht.sheetName = s.SheetName;
                    sht.isEnable = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".sheet." + sht.sheetName, true);
                    window.sheetList.Add(sht);

                    sheet별정보 sheet_info = new sheet별정보();
                    window.Sheet정보_list.Add(sheet_info);
                    sheet_info.엑셀파일이름 = window.fileName;
                    sheet_info.sheetName = s.SheetName;
                    sheet_info.className = s.SheetName;


                    // 첫번째 행을 가져와서
                    IRow 변수타입row = s.GetRow(0);     // 첫번째행: 변수타입 지정
                    IRow 변수이름row = s.GetRow(1);     // 두번째행: 변수이름 지정
                    for (int kk = 0; kk < 변수타입row.LastCellNum; kk++)
                    {// 하나하나의 변수명과 타입을 지정해준다.
                        ExcelRowParameter lastParser = null;
                        ExcelRowParameter parser = new ExcelRowParameter();

                        if (변수이름row == null 
                            || 변수타입row == null 
                            || 변수이름row.GetCell(kk) == null 
                            || 변수타입row.GetCell(kk) == null )
                        {
                            break;
                        }
                        else
                        {
                            변수이름row.GetCell(kk).SetCellType(CellType.String);
                            변수타입row.GetCell(kk).SetCellType(CellType.String);
                        }

                        parser.name = 변수이름row.GetCell(kk). StringCellValue;
                        parser.typename = 변수타입row.GetCell(kk).StringCellValue;
                        if (parser.typename == "")
                            break;
                        Debug.Log("변수타입: "+ parser.typename + "  변수이름: " + parser.name);

                        sheet_info.typeList.Add(parser);
                        
                    }
                }


                // 여기부터는 1번째 sheet로 무언가를 하는것
                ISheet sheet = book.GetSheetAt(0);

                // 파일네임으로 클래스네임을 삼는다
                window.className = EditorPrefs.GetString(s_key_prefix + window.fileName + ".className", "Entity_" + sheet.SheetName);
                window.sepalateSheet = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".separateSheet");


                window.Show();
            }
        }
    }

    
    void ExportEntity(sheet별정보 sheet_info)
    {
        string templateFilePath =  "Assets/Terasurware/Editor/EntityTemplate2.txt" ;
        string entittyTemplate = File.ReadAllText(templateFilePath);
        entittyTemplate = entittyTemplate.Replace("\r\n", "\n").Replace("\n", System.Environment.NewLine);

        StringBuilder builder = new StringBuilder();
        foreach (ExcelRowParameter row in sheet_info.typeList)
        {
            builder.AppendLine();
            builder.AppendFormat("		public {0} {1};", row.typename, row.name);
        }

        entittyTemplate = entittyTemplate.Replace("$Types$", builder.ToString());
        entittyTemplate = entittyTemplate.Replace("$ExcelData$", sheet_info.className);

        Directory.CreateDirectory("Assets/Terasurware/Classes/" + sheet_info.엑셀파일이름);
        File.WriteAllText("Assets/Terasurware/Classes/" + sheet_info.엑셀파일이름 + "/" + sheet_info.className + ".cs", entittyTemplate);
    }
    
    void ExportImporter(sheet별정보 sheet_info)
    {
        string templateFilePath =  "Assets/Terasurware/Editor/ExportTemplate3.txt";
        string importerTemplate = File.ReadAllText(templateFilePath);
        
        StringBuilder builder = new StringBuilder();
        int rowCount = 0;
        string tab = "					";
        bool isInbetweenArray = false;
        
        
        foreach (ExcelRowParameter row in sheet_info.typeList)
        {
            builder.AppendLine();
            switch (row.typename.ToString())
            {
                case "bool":
                    builder.AppendFormat(tab + "cell = row.GetCell({1}); if(cell != null) cell.SetCellType(CellType.String);  if(cell != null && cell.StringCellValue != \"\") bool.TryParse(cell.StringCellValue, out p.{0});", row.name, rowCount);
                    break;
                case "double":
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\")double.TryParse( cell.StringCellValue, out p.{0});", row.name, rowCount);
                    break;
                case "int":
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\") int.TryParse( cell.StringCellValue, out p.{0});", row.name, rowCount);
                    break;
                case "float":
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\") float.TryParse( cell.StringCellValue, out p.{0});", row.name, rowCount);
                    break;
                case "string":
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\") p.{0} = cell.StringCellValue;", row.name, rowCount);
                    break;

                case "Passive_Class":
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\") DataAsset_Importer.PassiveParse_0(cell.StringCellValue, out p.{0} );", row.name, rowCount);
                    break;

                case "Auto_Translate_Class":
                    builder.AppendFormat(tab + "cell = row.GetCell({1}); int ak"+ rowCount+ "=0; if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\") int.TryParse( cell.StringCellValue, out ak" + rowCount + "); if(cell != null && cell.StringCellValue != \"\") p.{0} = new Auto_Translate_Class(ak" + rowCount + ");", row.name, rowCount);
                    break;

                case "Condition_Info_Class":
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\") p.{0} = DataAsset_Importer.Condition_Info_Class_Parse(cell.StringCellValue, '/');", row.name, rowCount);
                    break;

                default: // 이건 enum이라는것임
                    builder.AppendFormat(tab + "cell = row.GetCell({1});  if(cell != null) cell.SetCellType(CellType.String); if(cell != null && cell.StringCellValue != \"\")   p.{0} =  (" + row.typename + ")System.Enum.Parse(typeof(" + row.typename + "), cell.StringCellValue);", row.name, rowCount);
                    break;
            }
            #region MyRegion
            //foreach (ExcelRowParameter row in sheet_info.typeList)
            //{
            //    builder.AppendLine();
            //    switch (row.typename.ToString())
            //    {
            //        case "bool":
            //            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? false : cell.BooleanCellValue);", row.name, rowCount);
            //            break;
            //        case "double":
            //            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? 0.0 : cell.NumericCellValue);", row.name, rowCount);
            //            break;
            //        case "int":
            //            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (int)(cell == null ? 0 : cell.NumericCellValue);", row.name, rowCount);
            //            break;
            //        case "float":
            //            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (float)(cell == null ? 0 : cell.NumericCellValue);", row.name, rowCount);
            //            break;
            //        case "string":
            //            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? \"\" : cell.StringCellValue);", row.name, rowCount);
            //            break;

            //        default: // 이건 enum이라는것임
            //            break;
            //    } 
            #endregion

            rowCount += 1;
        }

        importerTemplate = importerTemplate.Replace("$IMPORT_PATH$", filePath); // 엑셀의 path
        importerTemplate = importerTemplate.Replace("$ExportAssetDirectry$", Path.GetDirectoryName(filePath).Replace("\\", "/")+"/" + fileName+"_Assets"); // 엑셀의path/엑셀의filename으로 해야한다
        importerTemplate = importerTemplate.Replace("$ExcelData$", sheet_info.className); // dz. 
        importerTemplate = importerTemplate.Replace("$SheetList$", "\"" + sheet_info.sheetName + "\""); // dz
        importerTemplate = importerTemplate.Replace("$EXPORT_DATA$", builder.ToString()); // dz
        importerTemplate = importerTemplate.Replace("$ExportTemplate$", sheet_info.className + "_importer"); // dz

        Directory.CreateDirectory("Assets/Terasurware/Classes/Editor/" +  sheet_info.엑셀파일이름);
        File.WriteAllText("Assets/Terasurware/Classes/Editor/" + sheet_info.엑셀파일이름 + "/" + sheet_info.className + "_importer.cs", importerTemplate);
    }

    public class ExcelSheetParameter
    {
        public string sheetName;
        public bool isEnable;
    }

    public class ExcelRowParameter
    {
        public ValueType type;
        public string typename;
        public string name;
        public ExcelRowParameter nextArrayItem;
    }

    public class sheet별정보
    {
        public string 엑셀파일이름;

        public string className;
        public string sheetName;
        public List<ExcelRowParameter> typeList = new List<ExcelRowParameter>();
    }
}