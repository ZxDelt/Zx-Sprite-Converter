using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteConverter : EditorWindow
{
    private Object folderObject;
    private List<Texture2D> imageList = new List<Texture2D>();
    private Vector2 scrollPosition;

    [MenuItem("ZxTools/Sprite Converter")]
    private static void Init()
    {
        SpriteConverter window = (SpriteConverter)EditorWindow.GetWindow(typeof(SpriteConverter));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Folder", EditorStyles.boldLabel);
        folderObject = EditorGUILayout.ObjectField("Folder", folderObject, typeof(DefaultAsset), true);

        if (GUILayout.Button("Search and Convert Images"))
        {
            imageList.Clear();
            string folderPath = AssetDatabase.GetAssetPath(folderObject);

            if (!string.IsNullOrEmpty(folderPath))
            {
                string[] imagePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
                foreach (string imagePath in imagePaths)
                {
                    if (imagePath.EndsWith(".jpg") || imagePath.EndsWith(".png") || imagePath.EndsWith(".jpeg"))
                    {
                        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
                        imageList.Add(texture);
                    }
                }

                Debug.Log("Image count: " + imageList.Count);
            }
        }

        if (GUILayout.Button("Convert Images to Sprites"))
        {
            foreach (Texture2D texture in imageList)
            {
                string texturePath = AssetDatabase.GetAssetPath(texture);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);

                if (sprite == null)
                {
                    TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 100;
                    importer.spritePivot = new Vector2(0.5f, 0.5f);
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.filterMode = FilterMode.Point;
                    importer.maxTextureSize = 2048;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.isReadable = true;

                    AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                }
            }

            Debug.Log("All images converted to sprites.");
        }

        GUILayout.Label("Image List " + "Total Images : " + imageList.Count, EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (Texture2D image in imageList)
        {
            EditorGUILayout.ObjectField(image, typeof(Texture2D), false);
        }
        EditorGUILayout.EndScrollView();
    }
}
