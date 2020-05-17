#if UNITY_EDITOR || RUNTIME_CSG

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sabresaurus.SabreCSG.Importers.ValveMapFormat2006
{
    /// <summary>
    /// Converts a Hammer Map to SabreCSG Brushes.
    /// </summary>
    public static class VmfWorldConverter
    {
        private const float inchesInMeters = 0.0254f;//0.03125f; // 1/32

        /// <summary>
        /// Imports the specified world into the SabreCSG model.
        /// </summary>
        /// <param name="model">The model to import into.</param>
        /// <param name="world">The world to be imported.</param>
        /// <param name="scale">The scale modifier.</param>
        public static void Import(CSGModelBase model, VmfWorld world)
        {
            try
            {
                model.BeginUpdate();

                // create a material searcher to associate materials automatically.
                MaterialSearcher materialSearcher = new MaterialSearcher();

                // group all the brushes together.
                GroupBrush groupBrush = new GameObject("Source Engine Map").AddComponent<GroupBrush>();
                groupBrush.transform.SetParent(model.transform);

                // iterate through all world solids.
                for (int i = 0; i < world.Solids.Count; i++)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.DisplayProgressBar("SabreCSG: Importing Source Engine Map", "Converting Hammer Solids To SabreCSG Brushes (" + (i + 1) + " / " + world.Solids.Count + ")...", i / (float)world.Solids.Count);
#endif
                    VmfSolid solid = world.Solids[i];

                    // don't add triggers to the scene.
                    if (solid.Sides.Count > 0 && IsSpecialMaterial(solid.Sides[0].Material))
                        continue;

                    // build a very large cube brush.
                    var go = model.CreateBrush(PrimitiveBrushType.Cube, Vector3.zero);
                    var pr = go.GetComponent<PrimitiveBrush>();
                    BrushUtility.Resize(pr, new Vector3(8192, 8192, 8192));

                    // clip all the sides out of the brush.
                    for (int j = solid.Sides.Count; j-- > 0;)
                    {
                        VmfSolidSide side = solid.Sides[j];
                        Plane clip = new Plane(pr.transform.InverseTransformPoint(new Vector3(side.Plane.P1.X, side.Plane.P1.Z, side.Plane.P1.Y) * inchesInMeters), pr.transform.InverseTransformPoint(new Vector3(side.Plane.P2.X, side.Plane.P2.Z, side.Plane.P2.Y) * inchesInMeters), pr.transform.InverseTransformPoint(new Vector3(side.Plane.P3.X, side.Plane.P3.Z, side.Plane.P3.Y) * inchesInMeters));
                        ClipUtility.ApplyClipPlane(pr, clip, false);

                        // find the polygons associated with the clipping plane.
                        // the normal is unique and can never occur twice as that wouldn't allow the solid to be convex.
                        var polygons = pr.GetPolygons().Where(p => p.Plane.normal.EqualsWithEpsilonLower3(clip.normal));
                        foreach (var polygon in polygons)
                        {
                            // detect excluded polygons.
                            if (IsExcludedMaterial(side.Material))
                                polygon.UserExcludeFromFinal = true;
                            // detect collision-only brushes.
                            if (IsInvisibleMaterial(side.Material))
                                pr.IsVisible = false;
                            // find the material in the unity project automatically.
                            Material material;
                            // try finding the fully qualified texture name with '/' replaced by '.' so 'BRICK.BRICKWALL052D'.
                            string materialName = side.Material.Replace("/", ".");
                            if (materialName.Contains('.'))
                            {
                                // try finding both 'BRICK.BRICKWALL052D' and 'BRICKWALL052D'.
                                string tiny = materialName.Substring(materialName.LastIndexOf('.') + 1);
                                material = materialSearcher.FindMaterial(new string[] { materialName, tiny });
                                if (material == null)
                                    Debug.Log("SabreCSG: Tried to find material '" + materialName + "' and also as '" + tiny + "' but it couldn't be found in the project.");
                            }
                            else
                            {
                                // only try finding 'BRICKWALL052D'.
                                material = materialSearcher.FindMaterial(new string[] { materialName });
                                if (material == null)
                                    Debug.Log("SabreCSG: Tried to find material '" + materialName + "' but it couldn't be found in the project.");
                            }
                            polygon.Material = material;
                            // calculate the texture coordinates.
                            int w = 256;
                            int h = 256;
                            if (polygon.Material != null && polygon.Material.mainTexture != null)
                            {
                                w = polygon.Material.mainTexture.width;
                                h = polygon.Material.mainTexture.height;
                            }
                            CalculateTextureCoordinates(pr, polygon, w, h, side.UAxis, side.VAxis);
                        }
                    }

                    // add the brush to the group.
                    pr.transform.SetParent(groupBrush.transform);
                }

                // iterate through all entities.
                for (int e = 0; e < world.Entities.Count; e++)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.DisplayProgressBar("SabreCSG: Importing Source Engine Map", "Converting Hammer Entities To SabreCSG Brushes (" + (e + 1) + " / " + world.Entities.Count + ")...", e / (float)world.Entities.Count);
#endif
                    VmfEntity entity = world.Entities[e];

                    // skip entities that sabrecsg can't handle.
                    switch (entity.ClassName)
                    {
                        case "func_areaportal":
                        case "func_areaportalwindow":
                        case "func_capturezone":
                        case "func_changeclass":
                        case "func_combine_ball_spawner":
                        case "func_dustcloud":
                        case "func_dustmotes":
                        case "func_nobuild":
                        case "func_nogrenades":
                        case "func_occluder":
                        case "func_precipitation":
                        case "func_proprespawnzone":
                        case "func_regenerate":
                        case "func_respawnroom":
                        case "func_smokevolume":
                        case "func_viscluster":
                            continue;
                    }

                    /*if(entity.ClassName.Contains("info_player"))
                    {
                        var Asset = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(@"Assets\GameRes\SDK\Prefabs\SpawnT.prefab", typeof(GameObject));
                        var EntityObject = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(Asset);
                        EntityObject.name = entity.ClassName;
                        EntityObject.transform.position = entity.Origin * inchesInMeters;
                        EntityObject.transform.rotation = Quaternion.Euler(entity.Angles.x, -90 + entity.Angles.z, entity.Angles.y);
                    }

                    if (entity.ClassName.Contains("weapon_"))
                    {
                        var Asset = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(@"Assets\GameRes\SDK\Prefabs\Weapons\rif_ak47.prefab", typeof(GameObject));
                        var EntityObject = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(Asset);
                        EntityObject.name = entity.ClassName;
                        EntityObject.transform.position = entity.Origin * inchesInMeters;
                        Quaternion QAngle = Quaternion.Euler(entity.Angles);
                        entity.VectorAngles(Vector3.forward, Vector3.down, ref QAngle);
                        EntityObject.transform.rotation = QAngle;//Quaternion.Euler(entity.Angles.x, entity.Angles.y - 90, entity.Angles.z);
                    }

                    if (entity.ClassName.Contains("light_environment"))
                    {
                        var EntityObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        EntityObject.name = entity.ClassName;
                        EntityObject.transform.position = entity.Origin * inchesInMeters;
                        Vector3 Angles = Vector3.zero;
                        entity.SetupLightNormalFromProps(Quaternion.Euler(entity.Angles), 0, -45, ref Angles);//Quaternion.Euler(entity.Angles.x, -90 + entity.Angles.z, entity.Angles.y);
                        EntityObject.transform.eulerAngles = Angles;
                    }

                    if (entity.ClassName.Contains("shadow_control"))
                    {
                        var EntityObjet = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //GameObject cvikBlyat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        EntityObjet.name = entity.ClassName;
                        EntityObjet.transform.position = entity.Origin * inchesInMeters;
                        EntityObjet.transform.rotation = Quaternion.Euler(entity.Angles.x * -Mathf.PI / 2.0f, entity.Angles.y, entity.Angles.z);
                    }*/

                    // iterate through all entity solids.
                    for (int i = 0; i < entity.Solids.Count; i++)
                    {
                        VmfSolid solid = entity.Solids[i];

                        // don't add triggers to the scene.
                        if (solid.Sides.Count > 0 && IsSpecialMaterial(solid.Sides[0].Material))
                            continue;

                        // build a very large cube brush.
                        var go = model.CreateBrush(PrimitiveBrushType.Cube, Vector3.zero);
                        var pr = go.GetComponent<PrimitiveBrush>();
                        BrushUtility.Resize(pr, new Vector3(8192, 8192, 8192));

                        // clip all the sides out of the brush.
                        for (int j = solid.Sides.Count; j-- > 0;)
                        {
                            VmfSolidSide side = solid.Sides[j];
                            Plane clip = new Plane(pr.transform.InverseTransformPoint(new Vector3(side.Plane.P1.X, side.Plane.P1.Z, side.Plane.P1.Y) * inchesInMeters), pr.transform.InverseTransformPoint(new Vector3(side.Plane.P2.X, side.Plane.P2.Z, side.Plane.P2.Y) * inchesInMeters), pr.transform.InverseTransformPoint(new Vector3(side.Plane.P3.X, side.Plane.P3.Z, side.Plane.P3.Y) * inchesInMeters));
                            ClipUtility.ApplyClipPlane(pr, clip, false);

                            // find the polygons associated with the clipping plane.
                            // the normal is unique and can never occur twice as that wouldn't allow the solid to be convex.
                            var polygons = pr.GetPolygons().Where(p => p.Plane.normal.EqualsWithEpsilonLower3(clip.normal));
                            foreach (var polygon in polygons)
                            {
                                // detect excluded polygons.
                                if (IsExcludedMaterial(side.Material))
                                    polygon.UserExcludeFromFinal = true;
                                // detect collision-only brushes.
                                if (IsInvisibleMaterial(side.Material))
                                    pr.IsVisible = false;
                                // find the material in the unity project automatically.
                                Material material;
                                // try finding the fully qualified texture name with '/' replaced by '.' so 'BRICK.BRICKWALL052D'.
                                string materialName = side.Material.Replace("/", ".");
                                if (materialName.Contains('.'))
                                {
                                    // try finding both 'BRICK.BRICKWALL052D' and 'BRICKWALL052D'.
                                    string tiny = materialName.Substring(materialName.LastIndexOf('.') + 1);
                                    material = materialSearcher.FindMaterial(new string[] { materialName, tiny });
                                    if (material == null)
                                        Debug.Log("SabreCSG: Tried to find material '" + materialName + "' and also as '" + tiny + "' but it couldn't be found in the project.");
                                }
                                else
                                {
                                    // only try finding 'BRICKWALL052D'.
                                    material = materialSearcher.FindMaterial(new string[] { materialName });
                                    if (material == null)
                                        Debug.Log("SabreCSG: Tried to find material '" + materialName + "' but it couldn't be found in the project.");
                                }
                                polygon.Material = material;
                                // calculate the texture coordinates.
                                int w = 256;
                                int h = 256;
                                if (polygon.Material != null && polygon.Material.mainTexture != null)
                                {
                                    w = polygon.Material.mainTexture.width;
                                    h = polygon.Material.mainTexture.height;
                                }
                                CalculateTextureCoordinates(pr, polygon, w, h, side.UAxis, side.VAxis);
                            }
                        }

                        // detail brushes that do not affect the CSG world.
                        if (entity.ClassName == "func_detail")
                            pr.IsNoCSG = true;
                        // collision only brushes.
                        if (entity.ClassName == "func_vehicleclip")
                            pr.IsVisible = false;

                        // add the brush to the group.
                        pr.transform.SetParent(groupBrush.transform);
                    }
                }

#if UNITY_EDITOR
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                model.EndUpdate();
            }
        }

        // shoutouts to Aleksi Juvani for your vmf importer giving me a clue on why my textures were misaligned.
        // had to add the world space position of the brush to the calculations! https://github.com/aleksijuvani
        private static void CalculateTextureCoordinates(PrimitiveBrush pr, Polygon polygon, int textureWidth, int textureHeight, VmfAxis UAxis, VmfAxis VAxis)
        {
            UAxis.Translation = UAxis.Translation % textureWidth;
            VAxis.Translation = VAxis.Translation % textureHeight;

            if (UAxis.Translation < -textureWidth / 2f)
                UAxis.Translation += textureWidth;

            if (VAxis.Translation < -textureHeight / 2f)
                VAxis.Translation += textureHeight;

            // calculate texture coordinates.
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                var vertex = pr.transform.position + polygon.Vertices[i].Position;

                Vector3 uaxis = new Vector3(UAxis.Vector.X, UAxis.Vector.Z, UAxis.Vector.Y);
                Vector3 vaxis = new Vector3(VAxis.Vector.X, VAxis.Vector.Z, VAxis.Vector.Y);

                var u = Vector3.Dot(vertex, uaxis) / (textureWidth * (UAxis.Scale * inchesInMeters)) + UAxis.Translation / textureWidth;
                var v = Vector3.Dot(vertex, vaxis) / (textureHeight * (VAxis.Scale * inchesInMeters)) + VAxis.Translation / textureHeight;

                polygon.Vertices[i].UV.x = u;
                polygon.Vertices[i].UV.y = -v;
            }
        }

        /// <summary>
        /// Determines whether the specified name is an excluded material.
        /// </summary>
        /// <param name="name">The name of the material.</param>
        /// <returns><c>true</c> if the specified name is an excluded material; otherwise, <c>false</c>.</returns>
        private static bool IsExcludedMaterial(string name)
        {
            switch (name)
            {
                case "TOOLS/TOOLSNODRAW":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified name is an invisible material.
        /// </summary>
        /// <param name="name">The name of the material.</param>
        /// <returns><c>true</c> if the specified name is an invisible material; otherwise, <c>false</c>.</returns>
        private static bool IsInvisibleMaterial(string name)
        {
            switch (name)
            {
                case "TOOLS/TOOLSCLIP":
                case "TOOLS/TOOLSNPCCLIP":
                case "TOOLS/TOOLSPLAYERCLIP":
                case "TOOLS/TOOLSGRENDADECLIP":
                case "TOOLS/TOOLSSTAIRS":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified name is a special material, these brush will not be
        /// imported into SabreCSG.
        /// </summary>
        /// <param name="name">The name of the material.</param>
        /// <returns><c>true</c> if the specified name is a special material; otherwise, <c>false</c>.</returns>
        private static bool IsSpecialMaterial(string name)
        {
            switch (name)
            {
                case "TOOLS/TOOLSTRIGGER":
                case "TOOLS/TOOLSBLOCK_LOS":
                case "TOOLS/TOOLSBLOCKBULLETS":
                case "TOOLS/TOOLSBLOCKBULLETS2":
                case "TOOLS/TOOLSBLOCKSBULLETSFORCEFIELD": // did the wiki have a typo or is BLOCKS truly plural?
                case "TOOLS/TOOLSBLOCKLIGHT":
                case "TOOLS/TOOLSCLIMBVERSUS":
                case "TOOLS/TOOLSHINT":
                case "TOOLS/TOOLSINVISIBLE":
                case "TOOLS/TOOLSINVISIBLENONSOLID":
                case "TOOLS/TOOLSINVISIBLELADDER":
                case "TOOLS/TOOLSINVISMETAL":
                case "TOOLS/TOOLSNODRAWROOF":
                case "TOOLS/TOOLSNODRAWWOOD":
                case "TOOLS/TOOLSNODRAWPORTALABLE":
                case "TOOLS/TOOLSSKIP":
                case "TOOLS/TOOLSFOG":
                case "TOOLS/TOOLSSKYBOX":
                case "TOOLS/TOOLS2DSKYBOX":
                case "TOOLS/TOOLSSKYFOG":
                case "TOOLS/TOOLSFOGVOLUME":
                    return true;
            }
            return false;
        }
    }
}

#endif