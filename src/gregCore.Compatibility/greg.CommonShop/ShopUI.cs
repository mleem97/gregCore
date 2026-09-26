using System;
using System.Linq;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using greg.Logging;

namespace greg.CommonShop
{
    /// <summary>
    /// Shop layout helpers: real category/sub-category containers plus grid
    /// height repair. Best-effort everywhere — falls back to the vanilla
    /// parent so injection never breaks the shop.
    /// </summary>
    internal static partial class ShopUI
    {
        private static readonly GregModLogger _log = new GregModLogger("CommonShop");
    }
}
