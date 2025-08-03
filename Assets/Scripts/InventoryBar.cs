using UnityEngine;

namespace Gmtk2025
{
    public class InventoryBar : MonoBehaviour
    {
        private InventoryPickerButtonBase[] _allButtons;

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
    }
}
