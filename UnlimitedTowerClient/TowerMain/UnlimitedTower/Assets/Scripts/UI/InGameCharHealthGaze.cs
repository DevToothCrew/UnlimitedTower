using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCharHealthGaze : MonoBehaviour {

    #region UI Shader
    private readonly int _GRID_DIVIDOR = 4;
    private readonly float _CONVERGENCE_FACTOR = 2.0f;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    private float _nextHealthValue = 1.0f;
    private float _currHealthValue = 1.0f;

    private int _maxHealth = 10;
    private int _currHealth = 10;

    #endregion

    #region UI transform
    private float _DISTANCE_MARGIN = 0.3f;
    private float _distanceFromChar = 0.7f;

    #endregion

    private void Awake()
    {
        //this value is needed whenever it has made
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    // Use this for initialization
    private void Start()
    {
        //this value is needed after first frame
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }


    // Update is called once per frame
    void Update () {

        // MARK : Renderer update
        _currHealthValue = Mathf.Lerp(_currHealthValue, _nextHealthValue, Time.deltaTime * _CONVERGENCE_FACTOR);

        materialPropertyBlock.SetFloat("_CurrValue", _currHealthValue);
        materialPropertyBlock.SetFloat("_NextValue", _nextHealthValue);

        meshRenderer.SetPropertyBlock(materialPropertyBlock);

        // MARK : UI Transform update
        transform.localPosition = Vector3.up * (_distanceFromChar + _DISTANCE_MARGIN);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
    }
    
    public void SetMaxHealth(int v)
    {
        _maxHealth = v;
        materialPropertyBlock.SetFloat("_MaxHealthGrid", (float) v / _GRID_DIVIDOR);
    }

    public void SetHealth(int v)
    {
        _currHealth = v;
        _nextHealthValue = (float)_currHealth / _maxHealth;
    }

    public void SetDistanceFromChar(float v)
    {
        _distanceFromChar = v;
    }




}
