using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

public class EventManagerEditor : EditorWindow
{
    private string targetFolder;
    private string relativeTargetFolder; // From assets
    private string currentFilepath;
    private string filename = "EventManager.cs";
    private List<EventModuleData> eventData = new List<EventModuleData>();

    [MenuItem("Window/Event Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EventManagerEditor));
    }

    void Awake()
    {
        if (SearchFileRecursive(Application.dataPath, filename, out currentFilepath))
        {
            targetFolder = currentFilepath;
        }
        else
        {
            targetFolder = Application.dataPath;
        }
    }


    void OnGUI()
    {
        if (GUILayout.Button("Select target folder"))
        {
            targetFolder = EditorUtility.OpenFolderPanel("Target folder", targetFolder, "");
            StringBuilder sb = new StringBuilder();
            string[] splitPath = targetFolder.Split('/');
            for (int i = 0; i < splitPath.Length; i++)
            {
                if (splitPath[i] == "Assets")
                {
                    sb.Append('/');
                    for (int p = i; p < splitPath.Length; p++)
                    {
                        sb.Append(splitPath[p]);
                        sb.Append('/');
                    }
                    break;
                }
            }
            relativeTargetFolder = sb.ToString();
        }
        EditorGUILayout.LabelField(relativeTargetFolder);

        if (GUILayout.Button("Generate sample code"))
        {
            //GenerateEventClassFile(targetFolder, null);
        }
    }

    private class EventModuleData
    {
        public EventModuleData()
        {

        }

        public EventModuleData(string name, string returnType, string[] paramTypes, string comment)
        {
            this.name = name;
            this.returnType = returnType;
            this.paramTypes = paramTypes;
            this.comment = comment;
        }

        public string name;
        public string returnType;
        public string[] paramTypes;
        public string comment;
    }

    private void UpdateExistingEventData(string filepath)
    {
        CodeCompileUnit targetUnit = GetExistingCode(filepath);
        if (targetUnit != null)
        {
            CodeNamespaceCollection namespaceCol = targetUnit.Namespaces;
            if (namespaceCol.Count == 1)
            {
                CodeTypeDeclarationCollection typeDecCol = namespaceCol[0].Types;
                if (typeDecCol.Count == 1 && typeDecCol[0].Name == filename.Split('/')[0])
                {
                    CodeTypeMemberCollection memberCol = typeDecCol[0].Members;
                    List<EventModuleData> eventDataCol = new List<EventModuleData>();
                    CodeTypeDelegate currentDelegate = null;
                    
                    for (int i = 0; i < memberCol.Count; i++)
                    {
                        Type type = memberCol[i].GetType();
                        if (type == typeof(CodeTypeDelegate))
                        {
                            currentDelegate = (CodeTypeDelegate)memberCol[i];
                        }
                        else if (type == typeof(CodeMemberEvent) && currentDelegate != null)
                        {
                            eventDataCol.Add(ReadEventModuleData(currentDelegate, (CodeMemberEvent)memberCol[i]));
                            currentDelegate = null;
                        }
                    }

                    eventData.Clear();
                    eventData.AddRange(eventDataCol);
                }
            }
        }
    }

    private EventModuleData ReadEventModuleData(CodeTypeDelegate eventDelegate, CodeMemberEvent eventField)
    {
        EventModuleData data = new EventModuleData();
        data.name = eventField.Name;
        data.returnType = eventDelegate.ReturnType.BaseType;
        data.paramTypes = new string[eventDelegate.Parameters.Count];
        for (int i = 0; i < eventDelegate.Parameters.Count; i++)
        {
            data.paramTypes[i] = eventDelegate.Parameters[i].Type.BaseType;
        }
        if (eventField.Comments.Count > 0)
        {
            data.comment = eventField.Comments[0].Comment.Text;
        }
        return data;
    }

    private void CreateCode()
    {
        string existingFilepath;
        if (SearchFileRecursive(Application.dataPath, filename, out existingFilepath))
        {
            File.Delete(existingFilepath + filename);
        }

        CodeCompileUnit targetUnit = new CodeCompileUnit();

        CodeNamespace targetNamespace = new CodeNamespace();
        targetNamespace.Imports.Add(new CodeNamespaceImport("System"));
        targetNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));

        CodeTypeDeclaration targetClass = new CodeTypeDeclaration("EventManager");
        targetClass.IsClass = true;
        targetClass.TypeAttributes = TypeAttributes.Public;

        targetUnit.Namespaces.Add(targetNamespace);
        targetNamespace.Types.Add(targetClass);

        foreach (EventModuleData dataItem in eventData)
        {
            AddEventModule(targetClass, dataItem);
        }

        GenerateCodeFile(targetUnit);
    }

    private void AddEventModule(CodeTypeDeclaration targetClass, EventModuleData data)
    {
        string capitalizedName = CapitalizeString(data.name);
        string delegateName = capitalizedName + "Delegate";

        CodeTypeDelegate delegateField = new CodeTypeDelegate(delegateName);
        delegateField.Attributes = MemberAttributes.Public;
        delegateField.ReturnType = new CodeTypeReference(data.returnType);
        for (int i = 0; i < data.paramTypes.Length; i++)
        {
            delegateField.Parameters.Add(new CodeParameterDeclarationExpression(data.paramTypes[i], "a" + i));
        }

        CodeMemberEvent eventField = new CodeMemberEvent();
        eventField.Attributes = MemberAttributes.Public;
        eventField.Name = data.name;
        eventField.Type = new CodeTypeReference(delegateName);

        targetClass.Members.Add(delegateField);
        targetClass.Members.Add(eventField);
    }

    private void GenerateCodeFile(CodeCompileUnit targetUnit)
    {
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(currentFilepath))
        {
            provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
        }
    }

    private CodeCompileUnit GetExistingCode(string filepath)
    {
        if (filepath != "" && File.Exists(filepath))
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeCompileUnit targetUnit = new CodeCompileUnit();

            using (StreamReader sourceReader = new StreamReader(filepath))
            {
                targetUnit = provider.Parse(sourceReader);
            }
            return targetUnit;
        }
        return null;
    }

    private static bool SearchFileRecursive(string root, string filename, out string resultFilepath)
    {
        resultFilepath = root;
        if (File.Exists(resultFilepath))
        {
            return true;
        }
        else
        {
            string[] subDir = Directory.GetDirectories(root);
            for (int i = 0; i < subDir.Length; i++)
            {
                if (SearchFileRecursive(subDir[i], filename, out resultFilepath))
                {
                    return true;
                }
            }
        }
        resultFilepath = "";
        return false;
    }

    private string CapitalizeString(string input)
    {
        if (input.Length > 0)
        {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
        return input;
    }

    /*
    private EventConstructionData ReadEventData(CodeMemberEvent eventField)
    {
        EventConstructionData eventData = new EventConstructionData();
        eventField.Type.BaseType.GetType().;
        eventData.name = eventField.Name;
    }
    */
    /*
    private CodeMemberEvent CreateEventData(EventConstructionData data)
    {
        CodeMemberEvent eventField = new CodeMemberEvent();
        eventField.Attributes = MemberAttributes.Public;
        eventField.Name = data.name;
        CodeMem
        CodeTypeReference eventFieldDelegate = new CodeTypeReference(data.eventType);
        foreach (Type arg in data.eventGenericTypeArguments)
        {
            eventFieldDelegate.TypeArguments.Add(arg);
        }
        eventField.Type = eventFieldDelegate;
        return eventField;
    }
    */
}
/*
private EventConstructionData[] GetEventManagerReconstructionData()
{
    Type type = Type.GetType("EventManager", false, false);
    if (type != null)
    {
        EventInfo[] eventInfo = type.GetEvents();
        List<EventConstructionData> dataCollection = new List<EventConstructionData>();
        foreach (EventInfo item in eventInfo)
        {
            EventConstructionData data = new EventConstructionData();
            data.eventType = item.EventHandlerType.GetGenericTypeDefinition();
            data.eventGenericTypeArguments = item.EventHandlerType.GetGenericArguments();
            data.eventName = item.Name;
        }
        return dataCollection.ToArray();
    }
    else
    {
        return new EventConstructionData[0];
    }
}
*/
/*
public class RuntimeCodeEditor
{


    /// <summary>
    /// Adds two fields to the class.
    /// </summary>
    public void AddFields()
    {
        // Declare the widthValue field.
        CodeMemberField widthValueField = new CodeMemberField();
        widthValueField.Attributes = MemberAttributes.Private;
        widthValueField.Name = "widthValue";
        widthValueField.Type = new CodeTypeReference(typeof(System.Double));
        widthValueField.Comments.Add(new CodeCommentStatement(
            "The width of the object."));
        targetClass.Members.Add(widthValueField);
        // Declare the heightValue field
        CodeMemberField heightValueField = new CodeMemberField();
        heightValueField.Attributes = MemberAttributes.Private;
        heightValueField.Name = "heightValue";
        heightValueField.Type =
            new CodeTypeReference(typeof(System.Double));
        heightValueField.Comments.Add(new CodeCommentStatement(
            "The height of the object."));
        targetClass.Members.Add(heightValueField);
    }
    /// <summary>
    /// Add three properties to the class.
    /// </summary>
    public void AddProperties()
    {
        // Declare the read-only Width property.
        CodeMemberProperty widthProperty = new CodeMemberProperty();
        widthProperty.Attributes =
            MemberAttributes.Public | MemberAttributes.Final;
        widthProperty.Name = "Width";
        widthProperty.HasGet = true;
        widthProperty.Type = new CodeTypeReference(typeof(System.Double));
        widthProperty.Comments.Add(new CodeCommentStatement(
            "The Width property for the object."));
        widthProperty.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "widthValue")));
        targetClass.Members.Add(widthProperty);

        // Declare the read-only Height property.
        CodeMemberProperty heightProperty = new CodeMemberProperty();
        heightProperty.Attributes =
            MemberAttributes.Public | MemberAttributes.Final;
        heightProperty.Name = "Height";
        heightProperty.HasGet = true;
        heightProperty.Type = new CodeTypeReference(typeof(System.Double));
        heightProperty.Comments.Add(new CodeCommentStatement(
            "The Height property for the object."));
        heightProperty.GetStatements.Add(new CodeMethodReturnStatement(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "heightValue")));
        targetClass.Members.Add(heightProperty);

        // Declare the read only Area property.
        CodeMemberProperty areaProperty = new CodeMemberProperty();
        areaProperty.Attributes =
            MemberAttributes.Public | MemberAttributes.Final;
        areaProperty.Name = "Area";
        areaProperty.HasGet = true;
        areaProperty.Type = new CodeTypeReference(typeof(System.Double));
        areaProperty.Comments.Add(new CodeCommentStatement(
            "The Area property for the object."));

        // Create an expression to calculate the area for the get accessor 
        // of the Area property.
        CodeBinaryOperatorExpression areaExpression =
            new CodeBinaryOperatorExpression(
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "widthValue"),
            CodeBinaryOperatorType.Multiply,
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "heightValue"));
        areaProperty.GetStatements.Add(
            new CodeMethodReturnStatement(areaExpression));
        targetClass.Members.Add(areaProperty);
    }

    /// <summary>
    /// Adds a method to the class. This method multiplies values stored 
    /// in both fields.
    /// </summary>
    public void AddMethod()
    {
        // Declaring a ToString method
        CodeMemberMethod toStringMethod = new CodeMemberMethod();
        toStringMethod.Attributes =
            MemberAttributes.Public | MemberAttributes.Override;
        toStringMethod.Name = "ToString";
        toStringMethod.ReturnType =
            new CodeTypeReference(typeof(System.String));

        CodeFieldReferenceExpression widthReference =
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "Width");
        CodeFieldReferenceExpression heightReference =
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "Height");
        CodeFieldReferenceExpression areaReference =
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "Area");

        // Declaring a return statement for method ToString.
        CodeMethodReturnStatement returnStatement =
            new CodeMethodReturnStatement();

        // This statement returns a string representation of the width,
        // height, and area.
        string formattedOutput = "The object:" + Environment.NewLine +
            " width = {0}," + Environment.NewLine +
            " height = {1}," + Environment.NewLine +
            " area = {2}";
        returnStatement.Expression =
            new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression("System.String"), "Format",
            new CodePrimitiveExpression(formattedOutput),
            widthReference, heightReference, areaReference);
        toStringMethod.Statements.Add(returnStatement);
        targetClass.Members.Add(toStringMethod);
    }
    /// <summary>
    /// Add a constructor to the class.
    /// </summary>
    public void AddConstructor()
    {
        // Declare the constructor
        CodeConstructor constructor = new CodeConstructor();
        constructor.Attributes =
            MemberAttributes.Public | MemberAttributes.Final;

        // Add parameters.
        constructor.Parameters.Add(new CodeParameterDeclarationExpression(
            typeof(System.Double), "width"));
        constructor.Parameters.Add(new CodeParameterDeclarationExpression(
            typeof(System.Double), "height"));

        // Add field initialization logic
        CodeFieldReferenceExpression widthReference =
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "widthValue");
        constructor.Statements.Add(new CodeAssignStatement(widthReference,
            new CodeArgumentReferenceExpression("width")));
        CodeFieldReferenceExpression heightReference =
            new CodeFieldReferenceExpression(
            new CodeThisReferenceExpression(), "heightValue");
        constructor.Statements.Add(new CodeAssignStatement(heightReference,
            new CodeArgumentReferenceExpression("height")));
        targetClass.Members.Add(constructor);
    }

    /// <summary>
    /// Add an entry point to the class.
    /// </summary>
    public void AddEntryPoint()
    {
        CodeEntryPointMethod start = new CodeEntryPointMethod();
        CodeObjectCreateExpression objectCreate =
            new CodeObjectCreateExpression(
            new CodeTypeReference("CodeDOMCreatedClass"),
            new CodePrimitiveExpression(5.3),
            new CodePrimitiveExpression(6.9));

        // Add the statement:
        // "CodeDOMCreatedClass testClass = 
        //     new CodeDOMCreatedClass(5.3, 6.9);"
        start.Statements.Add(new CodeVariableDeclarationStatement(
            new CodeTypeReference("CodeDOMCreatedClass"), "testClass",
            objectCreate));

        // Creat the expression:
        // "testClass.ToString()"
        CodeMethodInvokeExpression toStringInvoke =
            new CodeMethodInvokeExpression(
            new CodeVariableReferenceExpression("testClass"), "ToString");

        // Add a System.Console.WriteLine statement with the previous 
        // expression as a parameter.
        start.Statements.Add(new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression("System.Console"),
            "WriteLine", toStringInvoke));
        targetClass.Members.Add(start);
    }
    /// <summary>
    /// Generate CSharp source code from the compile unit.
    /// </summary>
    /// <param name="filename">Output file name</param>
    public void GenerateCSharpCode(string fileName)
    {
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(fileName))
        {
            provider.GenerateCodeFromCompileUnit(
                targetUnit, sourceWriter, options);
        }
    }

    /// <summary>
    /// Create the CodeDOM graph and generate the code.
    /// </summary>
    public static void TestGeneration()
    {
        RuntimeCodeEditor sample = new RuntimeCodeEditor();
        sample.AddFields();
        sample.AddProperties();
        sample.AddMethod();
        sample.AddConstructor();
        sample.AddEntryPoint();
        sample.GenerateCSharpCode(outputFileName);
    }
}
*/
