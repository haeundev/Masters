using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(EvaluationInfos))]
    public class EvaluationInfosEditor : DataScriptEditor
    {
        public override string FileID => "1mBm88XtB5kGGC1-cgTf7Ql18e0V6uFjbQsshTDk3cWM";
        public override string SheetName => "EvaluationInfo";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(EvaluationInfo);

        public override void SetAssetData(string json)
        {
            var obj = target as EvaluationInfos;
            obj.Values = DataScript.MakeJsonListObjectString<EvaluationInfo>(json).values;
        }
    }
}

