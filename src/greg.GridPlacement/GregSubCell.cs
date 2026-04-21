using UnityEngine;

namespace greg.GridPlacement
{
    public class GregSubCell
    {
        public Vector2Int SubCoord { get; private set; }
        public bool IsOccupied { get; set; }
        public string? OccupantType { get; set; }

        public GregSubCell(Vector2Int subCoord)
        {
            SubCoord = subCoord;
        }
    }
}
