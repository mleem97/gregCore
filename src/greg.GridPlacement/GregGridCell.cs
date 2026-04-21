using UnityEngine;

namespace greg.GridPlacement
{
    public class GregGridCell
    {
        public Vector2Int Coord { get; private set; }
        public bool IsOccupied { get; set; }
        public GregPlaceableRack? Occupant { get; set; }
        public GregSubCell[] SubCells { get; private set; } = new GregSubCell[4];
        public bool IsBlocked { get; set; }

        public GregGridCell(Vector2Int coord)
        {
            Coord = coord;
            for (int i = 0; i < 4; i++)
            {
                SubCells[i] = new GregSubCell(new Vector2Int(i % 2, i / 2));
            }
        }
    }
}
