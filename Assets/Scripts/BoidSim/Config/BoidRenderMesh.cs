
using Unity.Rendering;
using UnityEngine;

namespace BoidSim {
    [CreateAssetMenu(fileName = "BoidRenderMesh", menuName = "Create new boid render mesh", order = 1)]
    public class BoidRenderMesh : ScriptableObject {
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField] private float scale;

        public float Scale => scale;

        public RenderMeshDescription RenderMeshDescription {
            get {
                var renderMesh = new RenderMeshDescription(mesh, material);
                renderMesh.RenderMesh.receiveShadows = true;
                renderMesh.RenderMesh.castShadows = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                return renderMesh;
            }
        }
    }

}
