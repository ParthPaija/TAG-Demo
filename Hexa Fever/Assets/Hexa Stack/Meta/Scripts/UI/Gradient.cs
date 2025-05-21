using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.UI
{
    [AddComponentMenu("Tag/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        #region PRIVATE_VARS
        [SerializeField] private Type _gradientType;

        [SerializeField] private Blend _blendMode = Blend.Multiply;

        [Range(-1, 1)]
        [SerializeField] private float _offset = 0f;

        [SerializeField]
        private UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0), new GradientColorKey(Color.blue, 1)
            }
        };
        #endregion

        #region PUBLIC_METHODS
        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive() || helper.currentVertCount == 0)
                return;

            List<UIVertex> _vertexList = new List<UIVertex>();

            helper.GetUIVertexStream(_vertexList);

            int nCount = _vertexList.Count;
            switch (_gradientType)
            {
                case Type.Horizontal:
                    {
                        float right = _vertexList[0].position.x;
                        float left = _vertexList[0].position.x;
                        for (int i = nCount - 1; i >= 1; --i)
                        {
                            float x = _vertexList[i].position.x;
                            if (x > right) 
                                right = x;
                            else if (x < left) 
                                left = x;
                        }

                        float width = 1f / (right - left);
                        UIVertex vertex = new UIVertex();

                        for (int i = 0; i < helper.currentVertCount; i++)
                        {
                            helper.PopulateUIVertex(ref vertex, i);

                            vertex.color = BlendColor(vertex.color, _effectGradient.Evaluate((vertex.position.x - left) * width - _offset));

                            helper.SetUIVertex(vertex, i);
                        }
                    }
                    break;

                case Type.Vertical:
                    {
                        float top = _vertexList[0].position.y;
                        float bottom = _vertexList[0].position.y;
                        for (int i = nCount - 1; i >= 1; --i)
                        {
                            float y = _vertexList[i].position.y;
                            if (y > top) 
                                top = y;
                            else if 
                                (y < bottom) bottom = y;
                        }

                        float height = 1f / (top - bottom);
                        UIVertex vertex = new UIVertex();

                        for (int i = 0; i < helper.currentVertCount; i++)
                        {
                            helper.PopulateUIVertex(ref vertex, i);

                            vertex.color = BlendColor(vertex.color, _effectGradient.Evaluate((vertex.position.y - bottom) * height - _offset));

                            helper.SetUIVertex(vertex, i);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region PRIVATE_VARS
        private Color BlendColor(Color colorA, Color colorB)
        {
            switch (_blendMode)
            {
                case Blend.Add: 
                    return colorA + colorB;
                
                case Blend.Multiply: 
                    return colorA * colorB;
                
                default: 
                    return colorB;
            }
        }
        #endregion
    }

    public enum Type
    {
        Horizontal,
        Vertical
    }

    public enum Blend
    {
        Override,
        Add,
        Multiply
    }
}
