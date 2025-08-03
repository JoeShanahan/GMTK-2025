using System.Collections.Generic;
using UnityEngine;

namespace Gmtk2025
{
    public class InventoryBar : MonoBehaviour
    {
        private InventoryPickerButtonBase[] _allButtons;
        [SerializeField] private GameObject _templateObject;

        private void Start()
        {
            _allButtons = GetComponentsInChildren<InventoryPickerButtonBase>();
        }
        
        public void OnSelectButton(InventoryPickerButtonBase btn)
        {
            foreach (var childButton in _allButtons)
            {
                if (childButton != btn)
                {
                    childButton.Deselect();
                }
            }
            
            btn?.Select();
        }
        
        public void SetInventory(List<float> loops, List<ConnectorItem> connectors)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (float loopRadius in loops)
            {
                GameObject newObj = Instantiate(_templateObject, transform);
                newObj.GetComponent<InventoryPickerButtonGame>().SetLoop(loopRadius);
            }
            
            foreach (var conInfo in connectors)
            {
                GameObject newObj = Instantiate(_templateObject, transform);
                newObj.GetComponent<InventoryPickerButtonGame>().SetConnector(conInfo.Type, conInfo.Value);
            }
            
            _allButtons = GetComponentsInChildren<InventoryPickerButtonBase>();
        }
    }
}
