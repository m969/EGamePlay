using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    /// <summary>
    /// Create a new texture only once. Safe access to generated textures.
    /// </summary>
    /// <remarks>
    /// If </remarks>
    internal static class EditorTextures {
        private static Texture2D black;
        private static Texture2D white;
        private static Texture2D lightGray225;
        private static Texture2D lightGray235;
        private static Texture2D darkGray17;
        private static Texture2D darkGray30;

        // Texture2D.blackTexture doesn't render properly in Editor GUI.
        public static Texture2D Black {
            get {
                if (!black) {
                    black = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    
                    var pixels = black.GetPixels32();
                    for (var i = 0; i < pixels.Length; i++) {
                        pixels[i] = new Color32(0, 0, 0, byte.MaxValue);
                    }
                    black.SetPixels32(pixels);
                    black.Apply();
                }
                return black;
            }
        }
        
        // Texture2D.whiteTexture might not render properly in Editor GUI.
        public static Texture2D White {
            get {
                
                if (!white) {
                    white = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    
                    var pixels = white.GetPixels32();
                    for (var i = 0; i < pixels.Length; i++) {
                        pixels[i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                    }
                    white.SetPixels32(pixels);
                    white.Apply();
                }
                return white;
            }
        }

        public static Texture2D DarkGray17 {
            get {
                if (!darkGray17) {
                    darkGray17 = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    
                    var pixels = darkGray17.GetPixels32();
                    for (var i = 0; i < pixels.Length; i++) {
                        pixels[i] = new Color32(17, 17, 17, byte.MaxValue);
                    }
                    darkGray17.SetPixels32(pixels);
                    darkGray17.Apply();
                }
                return darkGray17;
            }
        }
        
        public static Texture2D DarkGray40 {
            get {
                if (!darkGray30) {
                    darkGray30 = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    
                    var pixels = darkGray30.GetPixels32();
                    for (var i = 0; i < pixels.Length; i++) {
                        pixels[i] = new Color32(40, 40, 40, byte.MaxValue);
                    }
                    darkGray30.SetPixels32(pixels);
                    darkGray30.Apply();
                }
                return darkGray30;
            }
        }

        public static Texture2D LightGray238 {
            get {
                if (!lightGray235) {
                    lightGray235 = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    
                    var pixels = lightGray235.GetPixels32();
                    for (var i = 0; i < pixels.Length; i++) {
                        pixels[i] = new Color32(238, 238, 238, byte.MaxValue);
                    }
                    lightGray235.SetPixels32(pixels);
                    lightGray235.Apply();
                }
                return lightGray235;
            }
        }

        public static Texture2D LightGray225 {
            get {
                if (!lightGray225) {
                    lightGray225 = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                    var pixels = lightGray225.GetPixels32();
                    for (var i = 0; i < pixels.Length; i++) {
                        pixels[i] = new Color32(225, 225, 225, byte.MaxValue);
                    }
                    lightGray225.SetPixels32(pixels);
                    lightGray225.Apply();
                }
                return lightGray225;
            }
        }
    }
}