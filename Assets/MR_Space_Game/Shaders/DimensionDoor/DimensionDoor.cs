using TMPro;
using UnityEngine;

public class DimensionDoor : MonoBehaviour
{
    private readonly int strengthPro = Shader.PropertyToID("_AlphaThreshold");
    private readonly int colorPro = Shader.PropertyToID("_EdgeColor");
    private readonly int dimensionDoorStrengthPro = Shader.PropertyToID("_DimensionDoorStrength");

    private Material _stencilMaterial;
    private Material _maskMaterial;

    [SerializeField] private bool _deforming = false;
    public bool Deforming { get => _deforming; set => _deforming = value; }

    public GameObject stencilLayer;
    public GameObject edgeDissolvedLayer;

    public float speed = 0.0f;
    public Color edgeColor;
    [Range(0, 1)] public float alphaThreshold = 0.0f;
    [Range(0, 0.1f)] public float edgeWidth = 0.01f;



    void Start()
    {
        init();
    }

    public void init(MeshFilter meshFilter = null)
    {
        if (meshFilter != null) {
            //stencilLayer.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
            //edgeDissolvedLayer.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
            stencilLayer.GetComponent<MeshFilter>().mesh = meshFilter.sharedMesh;
            edgeDissolvedLayer.GetComponent<MeshFilter>().mesh = meshFilter.sharedMesh;
        }
        _stencilMaterial = stencilLayer.GetComponent<MeshRenderer>().material;
        _maskMaterial = edgeDissolvedLayer.GetComponent<MeshRenderer>().material;

        _stencilMaterial.SetFloat(strengthPro, alphaThreshold);

        _maskMaterial.SetColor(colorPro, edgeColor);
        _maskMaterial.SetFloat(dimensionDoorStrengthPro, alphaThreshold);

        edgeDissolvedLayer.SetActive(false);
    }


    void Update()
    {
        if (Deforming)
        {
            edgeDissolvedLayer.SetActive(true);
            alphaThreshold += (Time.deltaTime * speed);

            if (alphaThreshold <= 1.0f)
            {              
                //if (alphaThreshold >= 1.0f)
                //    Deforming = false;

                _stencilMaterial.SetFloat(strengthPro, alphaThreshold);
                _maskMaterial.SetFloat(dimensionDoorStrengthPro, Mathf.Clamp01(alphaThreshold - edgeWidth));
            }
        
        }
        else
        {
            alphaThreshold = 0;
            _stencilMaterial.SetFloat(strengthPro, 0);
            _maskMaterial.SetFloat(dimensionDoorStrengthPro, 0);
        }
    }

    private void OnDestroy()
    {

        if (_maskMaterial == null) return;
        _maskMaterial.SetFloat(dimensionDoorStrengthPro, 0);
        _stencilMaterial.SetFloat(strengthPro, 0);
    }
}