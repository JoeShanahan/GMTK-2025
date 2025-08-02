using UnityEngine;

namespace Gmtk2025
{
    public class FlagButton : MonoBehaviour
    {
        [SerializeField] private LevelController TheController;

        public void GoGoGadgetFlag()
        {
            if (TheController == null)
            {
                Debug.LogWarning("A FlagButton has a null LevelController reference");
                return;
            }

            if (TheController.IsPlaying)
                TheController.GoProjectileFlags();
            else
                TheController.ClearProjectileFlags();
        }
    }
}
