namespace Assets.FantasyInventory.Scripts.Enums
{
    /// <summary>
    /// Item tags can be used for implementing custom logic (special cases).
    /// Use constant integer values for enums to avoid data distortion when adding/removing new values.
    /// </summary>
    public enum ItemTag
    {
        Undefined   = 0,
        NotForSale  = 1,
        Gun         = 2,
        Consumable  = 3,
        Armor       = 4,
        Artifact    = 5,
    }
}