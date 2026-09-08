using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Helpers for working with the map's pathfinding nodes, connections and player
    /// spawn markers.
    /// </summary>
    public static class PathGridHelper
    {
        private const string SPAWN_MARKER_NAME = "SpawnPlayer";

        private static readonly PathNodeType[] _validNodeTypes = [
            PathNodeType.Ground,
            PathNodeType.Platform
        ];

        private static List<Vector2[]> _segments = null;

        /// <summary>
        /// Gets the segments of all valid path node connections, computed once and
        /// cached, as path nodes are static.
        /// </summary>
        private static List<Vector2[]> Segments
        {
            get
            {
                if (_segments is not null)
                {
                    return _segments;
                }

                _segments = [];

                // Get all path node connections
                foreach (IObjectPathNodeConnection conn in Game.GetObjects<IObjectPathNodeConnection>())
                {
                    // Skip invalid connections
                    if (!IsPathNodeConnectionValid(conn)) continue;

                    // Get connected nodes
                    IObjectPathNode nodeA = conn.GetPathNodeA();
                    IObjectPathNode nodeB = conn.GetPathNodeB();

                    // Skip connections with missing nodes
                    if (nodeA is null || nodeB is null) continue;

                    // Ensure nodes are valid
                    if (!IsPathNodeValid(nodeA) || !IsPathNodeValid(nodeB)) continue;

                    _segments.Add([
                        nodeA.GetWorldPosition(),
                        nodeB.GetWorldPosition()
                    ]);
                }

                return _segments;
            }
        }

        /// <summary>
        /// Returns a random position along a random valid path node connection, or
        /// <see cref="Vector2.Zero"/> when the map contains no valid connections.
        /// </summary>
        /// <remarks>
        /// A connection is valid when it is enabled, has the
        /// <see cref="PathNodeConnectionType.Default"/> type and links two enabled nodes
        /// of the <see cref="PathNodeType.Ground"/> or <see cref="PathNodeType.Platform"/>
        /// types. Connections with missing nodes are skipped, and the segments are
        /// computed once and cached, as path nodes are static. Every valid connection
        /// contributes the segment between its nodes, and a point is picked uniformly
        /// along the chosen segment.
        /// </remarks>
        public static Vector2 GetRandomPathGridPosition
        {
            get
            {
                List<Vector2[]> segments = Segments;

                if (segments.Count == 0) return Vector2.Zero;

                // Pick a random segment
                Vector2[] segment = segments[Random.Shared.Next(segments.Count)];

                // Calculate a random point on the segment
                float t = (float)Random.Shared.NextDouble();

                return segment[0] + (segment[1] - segment[0]) * t;
            }
        }

        /// <summary>
        /// Returns the position of a random player spawn marker, or
        /// <see cref="Vector2.Zero"/> when the map contains none.
        /// </summary>
        public static Vector2 GetRandomSpawnPosition
        {
            get
            {
                // Get all player spawn markers
                IObject[] spawns = Game.GetObjectsByName(SPAWN_MARKER_NAME);

                if (spawns.Length == 0) return Vector2.Zero;

                // Pick a random spawn marker
                return spawns[Random.Shared.Next(spawns.Length)].GetWorldPosition();
            }
        }

        /// <summary>
        /// Returns whether the node is enabled and of a valid type.
        /// </summary>
        private static bool IsPathNodeValid(IObjectPathNode node) => node.GetNodeEnabled() && _validNodeTypes.Any(v => node.GetPathNodeType() == v);

        /// <summary>
        /// Returns whether the connection is enabled and of the default type.
        /// </summary>
        private static bool IsPathNodeConnectionValid(IObjectPathNodeConnection conn) => conn.GetConnectionEnabled() &&
            conn.GetConnectionType() == PathNodeConnectionType.Default;
    }
}
