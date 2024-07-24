using Assets.Resources.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Utils
{
    public class HidebleFade : MonoBehaviour, IHideable
    {
        [SerializeField] Shader m_fadeShader;
        [SerializeField] bool m_proccessChildren = false;
        [Min(0)]
        [SerializeField] float m_fadeMultiplier = 1;

        private List<MeshRenderer> m_renderers;
        private float m_fadeValue = 0;
        private Dictionary<Material, Shader> m_initShaders;
        public float HidedPercentage { get => m_fadeValue; }

        private void Awake()
        {
            m_renderers = new List<MeshRenderer>();

            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();

            if (!rootRenderer) m_proccessChildren = true;

            if (m_proccessChildren)
                m_renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
            else
                m_renderers.Add(rootRenderer);

            m_initShaders = new Dictionary<Material, Shader>();

            foreach (var rend in m_renderers)
            {
                foreach (var mat in rend.materials)
                {
                    m_initShaders.Add(mat, rend.material.shader);
                }
            }
            Unhide();
        }

        public void Hide(float hideValue)
        {
            hideValue = Mathf.Clamp01(hideValue*m_fadeMultiplier);
            if (Mathf.Approximately(hideValue, m_fadeValue))
                return;
            m_fadeValue = (float)System.Math.Round(hideValue,2); 
            foreach (var rend in m_renderers)
            {
                foreach (var mat in rend.materials)
                {
                    mat.shader = m_fadeShader;
                    mat.ToFadeMode();
                    Color col = mat.color;
                    col.a = hideValue;
                    mat.color = col;
                }
            }
        }

        public void Unhide()
        {
            m_fadeValue = 0f;

            if (m_renderers == null)
            {
                return;
            }

            for (int i = 0; i < m_renderers.Count; i++)
            {
                foreach (var mat in m_renderers[i].materials)
                {
                    mat.shader = m_initShaders[mat];
                    mat.ToOpaqueMode();

                    if (mat.HasProperty("_Color"))
                    {
                        Color col = mat.color;
                        col.a = 1f;
                        mat.color = col;
                    }
                }
            }
        }

    }
}