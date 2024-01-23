using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScribbleHunter
{
    class WaveEntity
    {
        private Vector2 startPosition;
        private List<Vector2> routePoints = new List<Vector2>();

        public WaveEntity(Vector2 startPosition)
        {
            this.startPosition = startPosition;
        }

        public void AddRoutePoint(Vector2 point)
        {
            this.routePoints.Add(point);
        }

        public Vector2 StartPosition
        {
            get
            {
                return this.startPosition;
            }
        }

        public List<Vector2> RoutePoints
        {
            get
            {
                return this.routePoints;
            }
        }
    }
}