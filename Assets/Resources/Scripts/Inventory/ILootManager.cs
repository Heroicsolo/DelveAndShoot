using Heroicsolo.DI;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Inventory
{
    public interface ILootManager : ISystem
    {
        List<LootInfo> GetLootInfos();
        void GenerateRandomDrop(LootInfo lootInfo, Vector3 worldPosition);
    }
}