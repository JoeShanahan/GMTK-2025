

using UnityEngine;

namespace Gmtk2025
{
    public abstract class InventoryPickerButtonBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform _moverRect;
        [SerializeField] protected LevelCreator _levelCreator;
        [SerializeField] protected InventoryBar _parentBar;

        protected bool _isSelected;

        public virtual void ButtonPress()
        {

        }

        public virtual void Deselect()
        {

        }

        public virtual void Select()
        {

        }
    }
}