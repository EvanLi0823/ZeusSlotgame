using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ConvertCSVToPlistXML : EditorWindow
{
    
    private string _filePath = "";
    private string _outPutPath = "";
    private bool _isValidPath = false;
    
    [MenuItem("Tools/ConvertCSVToPlistXML",false,11)]
    static void ShowChangeGUIDToolsWindow()
    {
        ConvertCSVToPlistXML editor = (ConvertCSVToPlistXML)GetWindowWithRect (typeof(ConvertCSVToPlistXML), new Rect (100, 100, 500, 800), false, "ConvertCSVToPlistXML");
        editor.Show ();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("文件路径输入", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // 拖拽区域
        Rect dragRect = EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("将文件拖拽到此处", EditorStyles.centeredGreyMiniLabel);
            
            // 实际拖拽处理
            HandleDragAndDrop(dragRect);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // 使用 EditorGUILayout 的文本字段
        _filePath = EditorGUILayout.TextField("CSV 文件路径:", _filePath);
        
        // 路径验证状态显示
        EditorGUILayout.LabelField("验证状态:", 
            _isValidPath ? "有效的 CSV 文件" : "无效的文件", 
            _isValidPath ? EditorStyles.boldLabel : new GUIStyle(EditorStyles.miniBoldLabel) { normal = { textColor = Color.red } });

        EditorGUILayout.Space();
        
        // 浏览按钮
        if (GUILayout.Button("选择 CSV 文件..."))
        {
            string path = EditorUtility.OpenFilePanel("选择 CSV 文件", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                _filePath = path;
                ValidatePath();
            }
        }
        // 自动验证路径
        if (GUI.changed)
        {
            ValidatePath();
        }
        
        //转换 csv to plist.xml文件
        if (GUILayout.Button("转换为 plist文件"))
        {
            string outputPath = Path.ChangeExtension(_filePath, ".plist.xml");
            ConvertCSVToPlist(_filePath,outputPath);
        }
    }
    
    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;
        
        switch (evt.type)
        {
            case EventType.DragUpdated:
                if (dropArea.Contains(evt.mousePosition))
                {
                    // 检查是否是CSV文件
                    bool hasCSV = false;
                    foreach (string path in DragAndDrop.paths)
                    {
                        if (Path.GetExtension(path).ToLower() == ".csv")
                        {
                            hasCSV = true;
                            break;
                        }
                    }
                    
                    DragAndDrop.visualMode = hasCSV ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                    evt.Use();
                }
                break;
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;
                
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    
                    // 只取第一个CSV文件
                    foreach (string path in DragAndDrop.paths)
                    {
                        if (Path.GetExtension(path).ToLower() == ".csv")
                        {
                            _filePath = path;
                            GUI.changed = true;
                            break;
                        }
                    }
                    evt.Use();
                }
                break;
        }
    }
    
    private void ValidatePath()
    {
        _isValidPath = false;
        if (!string.IsNullOrEmpty(_filePath))
        {
            // 检查文件扩展名
            bool isCSV = Path.GetExtension(_filePath).ToLower() == ".csv";
            
            // 检查文件是否存在
            bool fileExists = File.Exists(_filePath);
            
            _isValidPath = isCSV && fileExists;
        }
        Repaint();
    }

    private void ConvertCSVToPlist(string csvPath, string outputPath)
    {
        // 读取CSV数据
        var csvData = ReadCSV(csvPath);
        
        // 创建XML文档
        XmlDocument xmlDoc = new XmlDocument();
        
        // 添加XML声明
        XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(xmlDeclaration);
        
        // 添加DOCTYPE声明
        XmlDocumentType docType = xmlDoc.CreateDocumentType(
            "plist",
            "-//Apple//DTD PLIST 1.0//EN",
            "http://www.apple.com/DTDs/PropertyList-1.0.dtd",
            null);
        xmlDoc.AppendChild(docType);
        
        // 创建根元素
        XmlElement plistElement = xmlDoc.CreateElement("plist");
        plistElement.SetAttribute("version", "1.0");
        xmlDoc.AppendChild(plistElement);
        
        // 创建dict元素
        XmlElement dictElement = xmlDoc.CreateElement("dict");
        plistElement.AppendChild(dictElement);
        
        // 添加CommonTestResultsListData键
        XmlElement keyElement = xmlDoc.CreateElement("key");
        keyElement.InnerText = "CommonTestResultsListData";
        dictElement.AppendChild(keyElement);
        
        // 创建array元素
        XmlElement arrayElement = xmlDoc.CreateElement("array");
        dictElement.AppendChild(arrayElement);
        
        // 为每个CSV行创建dict元素
        foreach (var row in csvData)
        {
            XmlElement itemDictElement = xmlDoc.CreateElement("dict");
            arrayElement.AppendChild(itemDictElement);
            
            // 添加TestResultName
            XmlElement nameKeyElement = xmlDoc.CreateElement("key");
            nameKeyElement.InnerText = "TestResultName";
            itemDictElement.AppendChild(nameKeyElement);
            
            XmlElement nameValueElement = xmlDoc.CreateElement("string");
            nameValueElement.InnerText = row.TestResultName;
            itemDictElement.AppendChild(nameValueElement);
            
            // 添加TestResultData
            XmlElement dataKeyElement = xmlDoc.CreateElement("key");
            dataKeyElement.InnerText = "TestResultData";
            itemDictElement.AppendChild(dataKeyElement);
            
            XmlElement dataArrayElement = xmlDoc.CreateElement("array");
            itemDictElement.AppendChild(dataArrayElement);
            
            // 分割TestResultData并添加到array
            string[] dataItems = row.TestResultData.Split(';');
            foreach (string dataItem in dataItems)
            {
                XmlElement dataItemElement = xmlDoc.CreateElement("string");
                dataItemElement.InnerText = dataItem;
                dataArrayElement.AppendChild(dataItemElement);
            }
        }
        
        // 保存XML文件
        using (var writer = new XmlTextWriter(outputPath, Encoding.UTF8))
        {
            writer.Formatting = Formatting.Indented;
            xmlDoc.Save(writer);
        }
    }
    
    private List<CSVRow> ReadCSV(string csvPath)
    {
        var rows = new List<CSVRow>();
        
        if (!File.Exists(csvPath))
        {
            throw new FileNotFoundException("CSV文件未找到", csvPath);
        }
        
        string[] lines = File.ReadAllLines(csvPath);
        
        // 跳过标题行(如果有)
        int startLine = lines[0].StartsWith("TestResultName") ? 1 : 0;
        
        for (int i = startLine; i < lines.Length; i++)
        {
            string[] parts = SplitCSVLine(lines[i]);
            if (parts.Length >= 2)
            {
                rows.Add(new CSVRow
                {
                    TestResultName = parts[0].Trim(),
                    TestResultData = parts[1].Trim()
                });
            }
        }
        
        return rows;
    }
    
    private static string[] SplitCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        StringBuilder current = new StringBuilder();
        
        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        
        result.Add(current.ToString());
        return result.ToArray();
    }
    
    private class CSVRow
    {
        public string TestResultName { get; set; }
        public string TestResultData { get; set; }
    }
}
