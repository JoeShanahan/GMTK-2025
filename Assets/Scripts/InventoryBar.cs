using UnityEngine;

namespace Gmtk2025
{
    public class InventoryBar : MonoBehaviour
    {
        private InventoryPickerButton[] _allButtons;

        private void Start()
        {
            _allButtons = GetComponentsInChildren<InventoryPickerButton>();
        }
        
        public void OnSelectButton(InventoryPickerButton btn)
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
