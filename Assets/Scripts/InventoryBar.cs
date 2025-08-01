using UnityEngine;

namespace Gmtk2025
{
    public class InventoryBar : MonoBehaviour
    {
        [SerializeField]
        private InventoryPickerButton[] _allButtons;
        
        public void OnSelectButton(InventoryPickerButton btn)
        {
            foreach (var childButton in _allButtons)
            {
                if (childButton == btn)
                {
                    childButton.Select();
                }
                else
                {
                    childButton.Deselect();
                }
            }
        }
    }
}
