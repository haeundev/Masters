using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(SessionInfos))]
    public class SessionInfosEditor : DataScriptEditor
    {
        public override string FileID => "1mBm88XtB5kGGC1-cgTf7Ql18e0V6uFjbQsshTDk3cWM";
        public override string SheetName => "SessionInfo";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(SessionInfo);

        public override void SetAssetData(string json)
        {
            var obj = target as SessionInfos;
            obj.Values = DataScript.MakeJsonListObjectString<SessionInfo>(json).values;
        }
    }
}

