using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace com.tag.editor
{
    public class PresetCreator
    {
        //Follow the step :

        //Import font(.ttf) file
        //Convert into TMP Asset (Select->rigtClick->Create->TextMeshPro->FontAsset)
        //Create Dummy material then SetShader(TextMeshPro/Distance Field)
        //Select Dummy material And then Go to menu I2 Localize->Create Preset
        //Wait For Generate AutoMate Material Ane then set Material Value accordinng eglish matirial value 

        [MenuItem("I2 Languages/Creat Preset")]
        public static void LoadSaveDataToPrefabs()
        {
            Object[] materials = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);
            string mainMaterialPath = AssetDatabase.GetAssetPath(materials[0]);
            string presetPath = mainMaterialPath.Replace("/" + materials[0].name + ".mat", "");
            string fontName = "";
            Dictionary<string, MaterialRefConfig> materialRefConfigs = GetMaterialNames("Assets/Hexa Stack/Fonts/English", presetPath, out fontName);
            foreach (var m in materialRefConfigs)
            {
                if (m.Value.targetMat == null)
                {
                    CreateMaterial(mainMaterialPath, presetPath + "/" + fontName + m.Key + ".mat");
                    m.Value.targetMat = AssetDatabase.LoadAssetAtPath<Material>(presetPath + "/" + fontName + m.Key + ".mat");
                }
                if ((m.Value.targetMat != null && m.Value.referenceMat != null))
                {
                    Color c = m.Value.referenceMat.GetColor("_OutlineColor");
                    m.Value.targetMat.SetColor("_OutlineColor", c);
                    float f = m.Value.referenceMat.GetFloat("_OutlineWidth");
                    m.Value.targetMat.SetFloat("_OutlineWidth", f);

                    if (m.Value.referenceMat.IsKeywordEnabled(ShaderUtilities.Keyword_Underlay))
                    {
                        m.Value.targetMat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                    }
                    m.Value.targetMat.SetColor("_UnderlayColor", m.Value.referenceMat.GetColor("_UnderlayColor"));
                    m.Value.targetMat.SetFloat("_UnderlayOffsetX", m.Value.referenceMat.GetFloat("_UnderlayOffsetX"));
                    m.Value.targetMat.SetFloat("_UnderlayOffsetY", m.Value.referenceMat.GetFloat("_UnderlayOffsetY"));
                    m.Value.targetMat.SetFloat("_UnderlayDilate", m.Value.referenceMat.GetFloat("_UnderlayDilate"));
                    m.Value.targetMat.SetFloat("_UnderlaySoftness", m.Value.referenceMat.GetFloat("_UnderlaySoftness"));

                    if (m.Value.referenceMat.IsKeywordEnabled(ShaderUtilities.Keyword_Bevel))
                    {
                        m.Value.targetMat.EnableKeyword(ShaderUtilities.Keyword_Bevel);
                    }
                    m.Value.targetMat.SetFloat("_LightAngle", m.Value.referenceMat.GetFloat("_LightAngle"));
                    m.Value.targetMat.SetColor("_SpecularColor", m.Value.referenceMat.GetColor("_SpecularColor"));
                    m.Value.targetMat.SetFloat("_SpecularPower", m.Value.referenceMat.GetFloat("_SpecularPower"));
                    m.Value.targetMat.SetFloat("_Reflectivity", m.Value.referenceMat.GetFloat("_Reflectivity"));
                    m.Value.targetMat.SetFloat("_Diffuse", m.Value.referenceMat.GetFloat("_Diffuse"));
                    m.Value.targetMat.SetFloat("_Ambient", m.Value.referenceMat.GetFloat("_Ambient"));

                    m.Value.targetMat.SetFloat("_Bevel", m.Value.referenceMat.GetFloat("_Bevel"));
                    m.Value.targetMat.SetFloat("_BevelOffset", m.Value.referenceMat.GetFloat("_BevelOffset"));
                    m.Value.targetMat.SetFloat("_BevelWidth", m.Value.referenceMat.GetFloat("_BevelWidth"));
                    m.Value.targetMat.SetFloat("_BevelClamp", m.Value.referenceMat.GetFloat("_BevelClamp"));
                    m.Value.targetMat.SetFloat("_BevelRoundness", m.Value.referenceMat.GetFloat("_BevelRoundness"));


                }
                //m.Value.targetMat.CopyPropertiesFromMaterial(m.Value.referenceMat);
            }
        }

        private static Dictionary<string, MaterialRefConfig> GetMaterialNames(string referencePath, string targetPath, out string nameOfFont)
        {
            string[] paths = AssetDatabase.FindAssets("t:material", new[] { referencePath });
            string[] fontPath = AssetDatabase.FindAssets("t:font", new[] { referencePath });
            string fontName = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontPath[0])).name;

            Dictionary<string, MaterialRefConfig> materialMapping = new Dictionary<string, MaterialRefConfig>();

            for (int i = 0; i < paths.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(paths[i]);
                if (IsExtension(p, "mat"))
                {
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(p);
                    string matId = mat.name.Replace(fontName, "");
                    if (!materialMapping.ContainsKey(matId))
                        materialMapping.Add(matId, new MaterialRefConfig());
                    materialMapping[matId].referenceMat = mat;
                }
            }


            paths = AssetDatabase.FindAssets("t:material", new[] { targetPath });
            fontPath = AssetDatabase.FindAssets("t:font", new[] { targetPath });
            fontName = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(fontPath[0])).name;
            nameOfFont = fontName;
            for (int i = 0; i < paths.Length; i++)
            {
                string p = AssetDatabase.GUIDToAssetPath(paths[i]);
                if (IsExtension(p, "mat"))
                {
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(p);
                    Debug.Log("mat  1" + mat.name);
                    string matId = mat.name.Replace(fontName, "");
                    Debug.Log("matId 1" + matId);
                    if (!materialMapping.ContainsKey(matId))
                        materialMapping.Add(matId, new MaterialRefConfig());
                    materialMapping[matId].targetMat = mat;
                }
            }

            return materialMapping;
        }

        private static void CreateMaterial(string mainPath, string copyPath)
        {
            AssetDatabase.CopyAsset(mainPath, copyPath);
        }

        private static bool IsExtension(string path, string extension)
        {
            string ext = path.Split('.')[1];
            return ext.ToLower().Equals(extension);
        }

        public class MaterialRefConfig
        {
            public Material targetMat;
            public Material referenceMat;
        }
    }
}