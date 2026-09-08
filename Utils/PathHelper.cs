using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Helpers for working with the map's pathfinding nodes and connections.
    /// </summary>
    public static class PathHelper
    {
        private static readonly PathNodeType[] _validNodeTypes = [
            PathNodeType.Ground,
            PathNodeType.Platform
        ];

        /// <summary>
        /// Returns a random position along a random valid path node connection, or
        /// <see cref="Vector2.Zero"/> when the map contains no valid connections.
        /// </summary>
        /// <remarks>
        /// A connection is valid when it is enabled, has the
        /// <see cref="PathNodeConnectionType.Default"/> type and links two enabled nodes
        /// of the <see cref="PathNodeType.Ground"/> or <see cref="PathNodeType.Platform"/>
        /// types. Every valid connection contributes the segment between its nodes, and a
        /// point is picked uniformly along the chosen segment.
        /// </remarks>
        public static Vector2 GetRandomPathGridPosition
        {
            get
            {
                List<Vector2[]> segments = [];

                // Get all path node connections
                foreach (IObjectPathNodeConnection conn in Game.GetObjects<IObjectPathNodeConnection>())
                {
                    // Skip invalid connections
                    if (!IsPathNodeConnectionValid(conn)) continue;

                    // Get connected nodes
                    IObjectPathNode nodeA = conn.GetPathNodeA();
                    IObjectPathNode nodeB = conn.GetPathNodeB();

                    // Ensure nodes are valid
                    if (!IsPathNodeValid(nodeA) || !IsPathNodeValid(nodeB)) continue;

                    // Get world positions
                    Vector2 posA = nodeA.GetWorldPosition();
                    Vector2 posB = nodeB.GetWorldPosition();

                    segments.Add([
                        posA,
                        posB
                    ]);
                }

                if (segments.Count == 0) return Vector2.Zero;

                // Pick a random segment
                Vector2[] segment = segments[Random.Shared.Next(segments.Count)];

                // Calculate a random point on the segment
                float t = (float)Random.Shared.NextDouble();

                return segment[0] + (segment[1] - segment[0]) * t;
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
