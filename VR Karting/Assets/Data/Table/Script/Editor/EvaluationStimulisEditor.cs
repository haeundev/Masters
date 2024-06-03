using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(EvaluationStimulis))]
    public class EvaluationStimulisEditor : DataScriptEditor
    {
        public override string FileID => "1xSdGn05B5qV5CUv0TKsH114OySjuw_oy1IfLBaW2_bg";
        public override string SheetName => "EvaluationStimuli";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(EvaluationStimuli);

        public override void SetAssetData(string json)
        {
            var obj = target as EvaluationStimulis;
            obj.Values = DataScript.MakeJsonListObjectString<EvaluationStimuli>(json).values;
        }
    }
}

