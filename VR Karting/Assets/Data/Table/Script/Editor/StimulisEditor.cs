using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(Stimulis))]
    public class StimulisEditor : DataScriptEditor
    {
        public override string FileID => "1xSdGn05B5qV5CUv0TKsH114OySjuw_oy1IfLBaW2_bg";
        public override string SheetName => "Stimuli";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(Stimuli);

        public override void SetAssetData(string json)
        {
            var obj = target as Stimulis;
            obj.Values = DataScript.MakeJsonListObjectString<Stimuli>(json).values;
        }
    }
}

