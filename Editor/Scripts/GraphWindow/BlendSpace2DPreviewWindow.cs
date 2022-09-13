using System.Collections.Generic;
using GBG.Puppeteer.Editor.Elements;
using GBG.Puppeteer.ThirdParties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Editor.BlendSpace
{
    public class BlendSpace2DPreviewWindow : EditorWindow
    {
        [MenuItem("Puppeteer/Test/BlendSpace2D Previewer Test")]
        private static void TestOpen()
        {
            var vertices = new Vector2[] // Test
            {
                new Vector2(0.00f, 0.00f),
                new Vector2(0.00f, 0.20f),
                new Vector2(0.00f, 0.40f),
                new Vector2(0.00f, -0.20f),
                new Vector2(0.00f, -0.40f),
                new Vector2(-0.20f, 0.20f),
                new Vector2(-0.20f, 0.40f),
                new Vector2(0.20f, 0.20f),
                new Vector2(0.20f, 0.40f),
                new Vector2(-0.20f, -0.20f),
                new Vector2(-0.20f, -0.40f),
                new Vector2(0.20f, -0.20f),
                new Vector2(0.20f, -0.40f),
            };

            var triangles = new Delaunay2D.Triangle[] // Test
            {
                new Delaunay2D.Triangle(1, 0, 5),
                new Delaunay2D.Triangle(2, 1, 6),
                new Delaunay2D.Triangle(1, 5, 6),
                new Delaunay2D.Triangle(0, 1, 7),
                new Delaunay2D.Triangle(1, 2, 8),
                new Delaunay2D.Triangle(7, 1, 8),
                new Delaunay2D.Triangle(0, 3, 9),
                new Delaunay2D.Triangle(5, 0, 9),
                new Delaunay2D.Triangle(3, 4, 10),
                new Delaunay2D.Triangle(9, 3, 10),
                new Delaunay2D.Triangle(3, 0, 11),
                new Delaunay2D.Triangle(0, 7, 11),
                new Delaunay2D.Triangle(4, 3, 12),
                new Delaunay2D.Triangle(3, 11, 12),
            };

            Open("BlendSpace Preview", vertices, triangles);
        }

        public static void Open(string name, IEnumerable<Vector2> vertices, IEnumerable<Delaunay2D.Triangle> triangles)
        {
            Assert.IsTrue(vertices != null);
            Assert.IsTrue(triangles != null);

            var window = GetWindow<BlendSpace2DPreviewWindow>(name);
            window.Initialize(vertices, triangles);
        }


        private BlendSpace2DPreviewer _previewer;

        private GUIStyle _lockAspectButtonStyle;

        private GUIContent _lockAspectButtonContent;


        /// <summary>
        /// Draw buttons on toolbar.
        /// Automatically called by unity.
        /// </summary>
        /// <param name="buttonPosition"></param>
        private void ShowButton(Rect buttonPosition)
        {
            if (_previewer == null)
            {
                return;
            }

            // Button style
            if (_lockAspectButtonStyle == null)
            {
                _lockAspectButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    // Remove paddings
                    padding = new RectOffset()
                };
            }

            if (_lockAspectButtonContent == null)
            {
                UpdateLockAspectButtonContent();
            }

            // Draw lock aspect button
            if (GUI.Button(buttonPosition, _lockAspectButtonContent, _lockAspectButtonStyle))
            {
                _previewer.LockGraphAspect = !_previewer.LockGraphAspect;
                UpdateLockAspectButtonContent();
            }

            void UpdateLockAspectButtonContent()
            {
                _lockAspectButtonContent = _previewer.LockGraphAspect
                    ? EditorGUIUtility.IconContent("LockIcon-On")
                    : EditorGUIUtility.IconContent("LockIcon");
                _lockAspectButtonContent.tooltip = _previewer.LockGraphAspect
                    ? "Graph aspect locked"
                    : "Graph aspect unlocked";
            }
        }

        private void Initialize(IEnumerable<Vector2> vertices, IEnumerable<Delaunay2D.Triangle> triangles)
        {
            _previewer = new BlendSpace2DPreviewer(vertices, triangles);
            rootVisualElement.Add(_previewer);
        }
    }
}
