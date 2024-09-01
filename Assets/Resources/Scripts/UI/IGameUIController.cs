using Heroicsolo.DI;
using System;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public interface IGameUIController : ISystem
    {
        void ShowWorldItemDesc(Guid itemID, string desc, Vector3 worldPos);
        void HideWorldItemDesc(Guid itemID);
        void OnHealthChanged(float oldVal, float newVal, float maxVal);
        void OnStaminaChanged(float oldVal, float newVal, float maxVal);
        void OnPlayerLevelUp(int level);
        void SetAmmo(int ammo);
        void RemoveAmmo();
        void SetCursorState(CursorState cursorState, bool forced = false);
        void SetUIElementSelected(bool value);
        void HideIngameUI();
        void ShowIngameUI();
    }

    public enum CursorState
    {
        Aim = 0,
        Targeted = 1,
        PickUp = 2,
        Default = 3
    }
}