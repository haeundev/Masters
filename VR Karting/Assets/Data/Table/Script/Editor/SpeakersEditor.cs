using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(Speakers))]
    public class SpeakersEditor : DataScriptEditor
    {
        public override string FileID => "1mBm88XtB5kGGC1-cgTf7Ql18e0V6uFjbQsshTDk3cWM";
        public override string SheetName => "Speaker";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(Speaker);

        public override void SetAssetData(string json)
        {
            var obj = target as Speakers;
            obj.Values = DataScript.MakeJsonListObjectString<Speaker>(json).values;
        }
    }
}
