using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GlowObject : MonoBehaviour
{
    public Color GlowColor;
    public float LerpFactor = 10;
    int handsColliding;

    public Renderer[] Renderers
    {
        get;
        private set;
    }

    public Color CurrentColor
    {
        get { return _currentColor; }
    }

    private List<Material> _materials = new List<Material>();
    private Color _currentColor;
    private Color _targetColor;

    void Start()
    {
        Renderers = GetComponentsInChildren<Renderer>();
        handsColliding = 0;
        foreach (var renderer in Renderers)
        {
            _materials.AddRange(renderer.materials);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            handsColliding++;
            _targetColor = GlowColor;
            StopCoroutine(DisableGlow());
            StartCoroutine(EnableGlow());
            enabled = true;
        }
    }


    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            handsColliding--;
            if (handsColliding < 1)
            {
                _targetColor = Color.black;
                StopCoroutine(EnableGlow());
                StartCoroutine(DisableGlow());
                enabled = true;
            }
        }
    }

    IEnumerator DisableGlow()
    {
        while (_currentColor.a > 0.1f)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                _currentColor = Color.Lerp(_currentColor, new Color(0, 0, 0, 0), Time.deltaTime * LerpFactor);
                _materials[i].SetColor("_GlowColor", _currentColor);
            }
            yield return new WaitForEndOfFrame();
        }
        _materials[0].SetColor("_GlowColor", Color.black);
    }

    IEnumerator EnableGlow()
    {
        while (_currentColor.a < 0.9f)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);
                _materials[i].SetColor("_GlowColor", _currentColor);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
