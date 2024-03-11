// using DataTables;

using DataTables;
using UnityEngine;

namespace LiveLarson.DataTableManagement
{
    [ExecuteAlways]
    public class DataTableManager : MonoBehaviour
    {
        [SerializeField] private Stimulis stimulis;

        public static DataTableManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            
            Debug.Log("[DataTableManager]  Awake!");
        }

        private void OnDestroy()
        {
            Debug.Log("[DataTableManager]  OnDestroy!");

            Instance = default;
        }
        
        public static Stimulis Stimulis => Instance.stimulis;
    }
}