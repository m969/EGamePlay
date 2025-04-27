using UnityEngine;
using System.Collections.Generic;

namespace SingularityGroup.HotReload.Editor {
    internal enum InvertibleIcon {
        BugReport,
        Events,
        EventsNew,
        Recompile,
        Logo,
        Close,
        FoldoutOpen,
        FoldoutClosed,
        Spinner,
        Stop,
        Start,
    }
    
    internal static class GUIHelper {
        private static readonly Dictionary<InvertibleIcon, string> supportedInvertibleIcons = new Dictionary<InvertibleIcon, string> {
            { InvertibleIcon.BugReport, "report_bug" },
            { InvertibleIcon.Events, "events" },
            { InvertibleIcon.Recompile, "refresh" },
            { InvertibleIcon.Logo, "logo" },
            { InvertibleIcon.Close, "close" },
            { InvertibleIcon.FoldoutOpen, "foldout_open" },
            { InvertibleIcon.FoldoutClosed, "foldout_closed" },
            { InvertibleIcon.Spinner, "icon_loading_star_light_mode_96" },
            { InvertibleIcon.Stop, "Icn_Stop" },
            { InvertibleIcon.Start, "Icn_play" },
        };
        
        private static readonly Dictionary<InvertibleIcon, Texture2D> invertibleIconCache = new Dictionary<InvertibleIcon, Texture2D>();
        private static readonly Dictionary<InvertibleIcon, Texture2D> invertibleIconInvertedCache = new Dictionary<InvertibleIcon, Texture2D>();
        private static readonly Dictionary<string, Texture2D> iconCache = new Dictionary<string, Texture2D>();
        
        internal static Texture2D InvertTextureColor(Texture2D originalTexture) {
            if (!originalTexture) {
                return originalTexture;
            }
            // Get the original pixels from the texture
            Color[] originalPixels = originalTexture.GetPixels();

            // Create a new array for the inverted colors
            Color[] invertedPixels = new Color[originalPixels.Length];

            // Iterate through the pixels and invert the colors while preserving the alpha channel
            for (int i = 0; i < originalPixels.Length; i++) {
                Color originalColor = originalPixels[i];
                Color invertedColor = new Color(1 - originalColor.r, 1 - originalColor.g, 1 - originalColor.b, originalColor.a);
                invertedPixels[i] = invertedColor;
            }

            // Create a new texture and set its pixels
            Texture2D invertedTexture = new Texture2D(originalTexture.width, originalTexture.height);
            invertedTexture.SetPixels(invertedPixels);

            // Apply the changes to the texture
            invertedTexture.Apply();

            return invertedTexture;
        }

        internal static Texture2D GetInvertibleIcon(InvertibleIcon invertibleIcon) {
            Texture2D iconTexture;
            var cache = HotReloadWindowStyles.IsDarkMode ? invertibleIconInvertedCache : invertibleIconCache;
           
            if (!cache.TryGetValue(invertibleIcon, out iconTexture) || !iconTexture) {
                var type = invertibleIcon == InvertibleIcon.EventsNew ? InvertibleIcon.Events : invertibleIcon;
                iconTexture = Resources.Load<Texture2D>(supportedInvertibleIcons[type]);
                
                // we assume icons are for light mode by default
                // therefore if its dark mode we should invert them
                if (HotReloadWindowStyles.IsDarkMode) {
                    iconTexture = InvertTextureColor(iconTexture);
                }

                cache[type] = iconTexture;

                // we combine dot image with Events icon to create a new alert version
                if (invertibleIcon == InvertibleIcon.EventsNew) {
                    var redDot = Resources.Load<Texture2D>("red_dot");
                    iconTexture = CombineImages(iconTexture, redDot);
                    cache[InvertibleIcon.EventsNew] = iconTexture;
                }
            }
            return cache[invertibleIcon];
        }
        
        internal static Texture2D GetLocalIcon(string iconName) {
            Texture2D iconTexture;
            if (!iconCache.TryGetValue(iconName, out iconTexture) || !iconTexture) {
                iconTexture = Resources.Load<Texture2D>(iconName);
                iconCache[iconName] = iconTexture;
            }
            return iconTexture;
        }
        
        static Texture2D CombineImages(Texture2D image1, Texture2D image2) {
            if (!image1 || !image2) {
                return image1;
            }
            var combinedImage = new Texture2D(Mathf.Max(image1.width, image2.width), Mathf.Max(image1.height, image2.height));

            for (int y = 0; y < combinedImage.height; y++) {
                for (int x = 0; x < combinedImage.width; x++) {
                    Color color1 = x < image1.width && y < image1.height ? image1.GetPixel(x, y) : Color.clear;
                    Color color2 = x < image2.width && y < image2.height ? image2.GetPixel(x, y) : Color.clear;
                    combinedImage.SetPixel(x, y, Color.Lerp(color1, color2, color2.a));
                }
            }
            combinedImage.Apply();
            return combinedImage;
        }
        
        private static readonly Dictionary<Color, Texture2D> textureColorCache = new Dictionary<Color, Texture2D>();
        internal static Texture2D ConvertTextureToColor(Color color) {
            Texture2D texture;
            if (!textureColorCache.TryGetValue(color, out texture) || !texture) {
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, color);
                texture.Apply();
                textureColorCache[color] = texture;
            }
            return texture;
        }
        
        private static readonly Dictionary<string, Texture2D> grayTextureCache = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Color> colorFactor = new Dictionary<string, Color> {
            { "error", new Color(0.6f, 0.587f, 0.114f) },
        };
        
        internal static Texture2D ConvertToGrayscale(string localIcon) {
            Texture2D _texture;
            if (!grayTextureCache.TryGetValue(localIcon, out _texture) || !_texture) {
                var icon = GUIHelper.GetLocalIcon(localIcon);
                // Create a copy of the texture
                Texture2D copiedTexture = new Texture2D(icon.width, icon.height, TextureFormat.RGBA32, false);

                // Convert the copied texture to grayscale
                Color[] pixels = icon.GetPixels();
                for (int i = 0; i < pixels.Length; i++) {
                    Color pixel = pixels[i];
                    Color factor;
                    if (!colorFactor.TryGetValue(localIcon, out factor)) {
                        factor = new Color(0.299f, 0.587f, 0.114f);
                    }
                    float grayscale = factor.r * pixel.r + factor.g * pixel.g + factor.b * pixel.b;
                    pixels[i] = new Color(grayscale, grayscale, grayscale, pixel.a);  // Preserve alpha channel
                }
                copiedTexture.SetPixels(pixels);
                copiedTexture.Apply();

                // Store the grayscale texture in the cache
                grayTextureCache[localIcon] = copiedTexture;

                return copiedTexture;
            }
            return _texture;
        }
    }
}
