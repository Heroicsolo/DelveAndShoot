using Heroicsolo.DI;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Inventory
{
    public interface ILootManager : ISystem
    {
        List<LootInfo> GetLootInfos();
        LootInfo GetLootInfo(string lootId);
        void GenerateRandomDrop(LootInfo lootInfo, Vector3 worldPosition);
        void GenerateRandomDrop(string lootId, Vector3 worldPosition);
    }
}