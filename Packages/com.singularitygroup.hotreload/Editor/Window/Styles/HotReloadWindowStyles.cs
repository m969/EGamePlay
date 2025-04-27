using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadWindowStyles {
        private static GUIStyle h1TitleStyle;
        private static GUIStyle h1TitleCenteredStyle;
        private static GUIStyle h2TitleStyle;
        private static GUIStyle h3TitleStyle;
        private static GUIStyle h3TitleWrapStyle;
        private static GUIStyle h4TitleStyle;
        private static GUIStyle h5TitleStyle;
        private static GUIStyle boxStyle;
        private static GUIStyle wrapStyle;
        private static GUIStyle noPaddingMiddleLeftStyle;
        private static GUIStyle middleLeftStyle;
        private static GUIStyle middleCenterStyle;
        private static GUIStyle mediumMiddleCenterStyle;
        private static GUIStyle textFieldWrapStyle;
        private static GUIStyle foldoutStyle;
        private static GUIStyle h3CenterTitleStyle;
        private static GUIStyle logoStyle;
        private static GUIStyle changelogPointersStyle;
        private static GUIStyle recompileButtonStyle;
        private static GUIStyle indicationIconStyle;
        private static GUIStyle indicationAlertIconStyle;
        private static GUIStyle startButtonStyle;
        private static GUIStyle stopButtonStyle;
        private static GUIStyle eventFilters;
        private static GUIStyle sectionOuterBoxCompactStyle;
        private static GUIStyle sectionInnerBoxStyle;
        private static GUIStyle sectionInnerBoxWideStyle;
        private static GUIStyle changelogSectionInnerBoxStyle;
        private static GUIStyle indicationBoxStyle;
        private static GUIStyle linkStyle;
        private static GUIStyle labelStyle;
        private static GUIStyle progressBarBarStyle;
        private static GUIStyle section;
        private static GUIStyle scroll;
        private static GUIStyle barStyle;
        private static GUIStyle barBgStyle;
        private static GUIStyle barChildStyle;
        private static GUIStyle barFoldoutStyle;
        private static GUIStyle timestampStyle;
        private static GUIStyle clickableLabelBoldStyle;
        private static GUIStyle _footerStyle;
        private static GUIStyle _emptyListText;
        private static GUIStyle _stacktraceTextAreaStyle;
        private static GUIStyle _customFoldoutStyle;
        private static GUIStyle _entryBoxStyle;
        private static GUIStyle _childEntryBoxStyle;
        private static GUIStyle _removeIconStyle;
        private static GUIStyle upgradeLicenseButtonStyle;
        private static GUIStyle upgradeLicenseButtonOverlayStyle;
        private static GUIStyle upgradeButtonStyle;
        private static GUIStyle hideButtonStyle;
        private static GUIStyle dynamicSection;
        private static GUIStyle dynamicSectionHelpTab;
        private static GUIStyle helpTabButton;
        private static GUIStyle indicationHelpBox;
        private static GUIStyle notificationsTitleStyle;
        
        private static Color32? darkModeLinkColor;
        private static Color32? lightModeModeLinkColor;
        
        public static bool IsDarkMode => EditorGUIUtility.isProSkin;
        public static int windowScreenWidth => HotReloadWindow.Current ? (int)HotReloadWindow.Current.position.width : Screen.width;
        public static int windowScreenHeight => HotReloadWindow.Current ? (int)HotReloadWindow.Current.position.height : Screen.height;
        public static GUIStyle H1TitleStyle {
            get {
                if (h1TitleStyle == null) {
                    h1TitleStyle = new GUIStyle(EditorStyles.label);
                    h1TitleStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    h1TitleStyle.fontStyle = FontStyle.Bold;
                    h1TitleStyle.fontSize = 16;
                    h1TitleStyle.padding.top = 5;
                    h1TitleStyle.padding.bottom = 5;
                }
                return h1TitleStyle;
            }
        }
        
        public static GUIStyle FooterStyle {
            get {
                if (_footerStyle == null) {
                    _footerStyle = new GUIStyle();
                    _footerStyle.fixedHeight = 28;
                }
                return _footerStyle;
            }
        }
        
        public static GUIStyle H1TitleCenteredStyle {
            get {
                if (h1TitleCenteredStyle == null) {
                    h1TitleCenteredStyle = new GUIStyle(H1TitleStyle);
                    h1TitleCenteredStyle.alignment = TextAnchor.MiddleCenter;
                }
                return h1TitleCenteredStyle;
            }
        }
        
        public static GUIStyle H2TitleStyle {
            get {
                if (h2TitleStyle == null) {
                    h2TitleStyle = new GUIStyle(EditorStyles.label);
                    h2TitleStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    h2TitleStyle.fontStyle = FontStyle.Bold;
                    h2TitleStyle.fontSize = 14;
                    h2TitleStyle.padding.top = 5;
                    h2TitleStyle.padding.bottom = 5;
                }
                return h2TitleStyle;
            }
        }
        
        public static GUIStyle H3TitleStyle {
            get {
                if (h3TitleStyle == null) {
                    h3TitleStyle = new GUIStyle(EditorStyles.label);
                    h3TitleStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    h3TitleStyle.fontStyle = FontStyle.Bold;
                    h3TitleStyle.fontSize = 12;
                    h3TitleStyle.padding.top = 5;
                    h3TitleStyle.padding.bottom = 5;
                }
                return h3TitleStyle;
            }
        }
                
        public static GUIStyle NotificationsTitleStyle {
            get {
                if (notificationsTitleStyle == null) {
                    notificationsTitleStyle = new GUIStyle(HotReloadWindowStyles.H3TitleStyle);
                    notificationsTitleStyle.padding.bottom = 0;
                    notificationsTitleStyle.padding.top = 0;
                }
                return notificationsTitleStyle;
            }
        }
        
        public static GUIStyle H3TitleWrapStyle {
            get {
                if (h3TitleWrapStyle == null) {
                    h3TitleWrapStyle = new GUIStyle(H3TitleStyle);
                    h3TitleWrapStyle.wordWrap = true;
                }
                return h3TitleWrapStyle;
            }
        }
        
        public static GUIStyle H3CenteredTitleStyle {
            get {
                if (h3CenterTitleStyle == null) {
                    h3CenterTitleStyle = new GUIStyle(EditorStyles.label);
                    h3CenterTitleStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    h3CenterTitleStyle.fontStyle = FontStyle.Bold;
                    h3CenterTitleStyle.alignment = TextAnchor.MiddleCenter;
                    h3CenterTitleStyle.fontSize = 12;
                }
                return h3CenterTitleStyle;
            }
        }

        public static GUIStyle H4TitleStyle {
            get {
                if (h4TitleStyle == null) {
                    h4TitleStyle = new GUIStyle(EditorStyles.label);
                    h4TitleStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    h4TitleStyle.fontStyle = FontStyle.Bold;
                    h4TitleStyle.fontSize = 11;
                }
                return h4TitleStyle;
            }
        }

        public static GUIStyle H5TitleStyle {
            get {
                if (h5TitleStyle == null) {
                    h5TitleStyle = new GUIStyle(EditorStyles.label);
                    h5TitleStyle.normal.textColor = EditorStyles.label.normal.textColor;
                    h5TitleStyle.fontStyle = FontStyle.Bold;
                    h5TitleStyle.fontSize = 10;
                }
                return h5TitleStyle;
            }
        }
        
        public static GUIStyle LabelStyle {
            get {
                if (labelStyle == null) {
                    labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.fontSize = 12;
                    labelStyle.clipping = TextClipping.Clip;
                    labelStyle.wordWrap = true;
                }
                return labelStyle;
            }
        }
        
        public static GUIStyle BoxStyle {
            get {
                if (boxStyle == null) {
                    boxStyle = new GUIStyle(EditorStyles.helpBox);
                    boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                    boxStyle.fontStyle = FontStyle.Bold;
                    boxStyle.alignment = TextAnchor.UpperLeft;
                }
                if (!IsDarkMode) {
                    boxStyle.normal.background = Texture2D.blackTexture;
                }
                return boxStyle;
            }
        }

        public static GUIStyle WrapStyle {
            get {
                if (wrapStyle == null) {
                    wrapStyle = new GUIStyle(EditorStyles.label);
                    wrapStyle.fontStyle = FontStyle.Normal;
                    wrapStyle.wordWrap = true;
                }
                return wrapStyle;
            }
        }

        public static GUIStyle NoPaddingMiddleLeftStyle {
            get {
                if (noPaddingMiddleLeftStyle == null) {
                    noPaddingMiddleLeftStyle = new GUIStyle(EditorStyles.label);
                    noPaddingMiddleLeftStyle.normal.textColor = GUI.skin.label.normal.textColor;
                    noPaddingMiddleLeftStyle.padding = new RectOffset();
                    noPaddingMiddleLeftStyle.margin = new RectOffset();
                    noPaddingMiddleLeftStyle.alignment = TextAnchor.MiddleLeft;
                }
                return noPaddingMiddleLeftStyle;
            }
        }

        public static GUIStyle MiddleLeftStyle {
            get {
                if (middleLeftStyle == null) {
                    middleLeftStyle = new GUIStyle(EditorStyles.label);
                    middleLeftStyle.fontStyle = FontStyle.Normal;
                    middleLeftStyle.alignment = TextAnchor.MiddleLeft;
                }

                return middleLeftStyle;
            }
        }

        public static GUIStyle MiddleCenterStyle {
            get {
                if (middleCenterStyle == null) {
                    middleCenterStyle = new GUIStyle(EditorStyles.label);
                    middleCenterStyle.fontStyle = FontStyle.Normal;
                    middleCenterStyle.alignment = TextAnchor.MiddleCenter;
                }
                return middleCenterStyle;
            }
        }
        
        public static GUIStyle MediumMiddleCenterStyle {
            get {
                if (mediumMiddleCenterStyle == null) {
                    mediumMiddleCenterStyle = new GUIStyle(EditorStyles.label);
                    mediumMiddleCenterStyle.fontStyle = FontStyle.Normal;
                    mediumMiddleCenterStyle.fontSize = 12;
                    mediumMiddleCenterStyle.alignment = TextAnchor.MiddleCenter;
                }
                return mediumMiddleCenterStyle;
            }
        }

        public static GUIStyle TextFieldWrapStyle {
            get {
                if (textFieldWrapStyle == null) {
                    textFieldWrapStyle = new GUIStyle(EditorStyles.textField);
                    textFieldWrapStyle.wordWrap = true;
                }
                return textFieldWrapStyle;
            }
        }

        public static GUIStyle FoldoutStyle {
            get {
                if (foldoutStyle == null) {
                    foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    foldoutStyle.normal.textColor = GUI.skin.label.normal.textColor;
                    foldoutStyle.alignment = TextAnchor.MiddleLeft;
                    foldoutStyle.fontStyle = FontStyle.Bold;
                    foldoutStyle.fontSize = 12;
                }
                return foldoutStyle;
            }
        }
        
        public static GUIStyle LogoStyle {
            get {
                if (logoStyle == null) {
                    logoStyle = new GUIStyle();
                    logoStyle.margin = new RectOffset(6, 6, 0, 0);
                    logoStyle.padding = new RectOffset(16, 16, 0, 0);
                }
                return logoStyle;
            }
        }
        
        public static GUIStyle ChangelogPointerStyle {
            get {
                if (changelogPointersStyle == null) {
                    changelogPointersStyle = new GUIStyle(EditorStyles.label);
                    changelogPointersStyle.wordWrap = true;
                    changelogPointersStyle.fontSize = 12;
                    changelogPointersStyle.padding.left = 20;
                }
                return changelogPointersStyle;
            }
        }
        
        public static GUIStyle IndicationIcon {
            get {
                if (indicationIconStyle == null) {
                    indicationIconStyle = new GUIStyle(H2TitleStyle);
                    indicationIconStyle.fixedHeight = 20;
                }
                indicationIconStyle.padding = new RectOffset(left: windowScreenWidth > Constants.IndicationTextHideWidth ? 7 : 5, right: windowScreenWidth > Constants.IndicationTextHideWidth ? 0 : -10, top: 1, bottom: 1);
                return indicationIconStyle;
            }
        }
        
        public static GUIStyle IndicationAlertIcon {
            get {
                if (indicationAlertIconStyle == null) {
                    indicationAlertIconStyle = new GUIStyle(H2TitleStyle);
                    indicationAlertIconStyle.padding = new RectOffset(left: 5, right: -7, top: 1, bottom: 1);
                    indicationAlertIconStyle.fixedHeight = 20;
                }
                return indicationAlertIconStyle;
            }
        }
        
        public static GUIStyle RecompileButton {
            get {
                if (recompileButtonStyle == null) {
                    recompileButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    recompileButtonStyle.margin.top = 17;
                    recompileButtonStyle.fixedHeight = 25;
                    recompileButtonStyle.margin.right = 5;
                }
                recompileButtonStyle.fixedWidth = windowScreenWidth > Constants.RecompileButtonTextHideWidth ? 95 : 30;
                return recompileButtonStyle;
            }
        }
        
        public static GUIStyle StartButton {
            get {
                if (startButtonStyle == null) {
                    startButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    startButtonStyle.fixedHeight = 25;
                    startButtonStyle.padding.top = 6;
                    startButtonStyle.padding.bottom = 6;
                    startButtonStyle.margin.top = 17;
                }
                startButtonStyle.fixedWidth = windowScreenWidth > Constants.StartButtonTextHideWidth ? 70 : 30;
                return startButtonStyle;
            }
        }
        
        public static GUIStyle StopButton {
            get {
                if (stopButtonStyle == null) {
                    stopButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    stopButtonStyle.fixedHeight = 25;
                    stopButtonStyle.margin.top = 17;
                }
                stopButtonStyle.fixedWidth = HotReloadWindowStyles.windowScreenWidth > Constants.StartButtonTextHideWidth ? 70 : 30;
                return stopButtonStyle;
            }
        }
        
        internal static GUIStyle EventFiltersStyle {
            get {
                if (eventFilters == null) {
                    eventFilters = new GUIStyle(EditorStyles.toolbarButton);
                    eventFilters.fontSize = 13;
                    // gets overwritten to content size
                    eventFilters.fixedHeight = 26; 
                    eventFilters.fixedWidth = 50; 
                    eventFilters.margin = new RectOffset(0, 0, 0, 0);
                    eventFilters.padding = new RectOffset(0, 0, 6, 6);
                }
                return eventFilters;
            }
        }

        private static Texture2D _clearBackground;
        private static Texture2D clearBackground {
            get {    
                    if (_clearBackground == null) {
                        _clearBackground = new Texture2D(1, 1);
                        _clearBackground.SetPixel(0, 0, Color.clear);
                        _clearBackground.Apply();
                    }
                    return _clearBackground;
                    
            }
        }

        public static GUIStyle SectionOuterBoxCompact {
            get {
                if (sectionOuterBoxCompactStyle == null) {
                    sectionOuterBoxCompactStyle = new GUIStyle();
                    sectionOuterBoxCompactStyle.padding.top = 10;
                    sectionOuterBoxCompactStyle.padding.bottom = 10;
                }
                // Looks better without a background
                sectionOuterBoxCompactStyle.normal.background = clearBackground;
                return sectionOuterBoxCompactStyle;
            }
        }
        
        public static GUIStyle SectionInnerBox {
            get {
                if (sectionInnerBoxStyle == null) {
                    sectionInnerBoxStyle = new GUIStyle();
                }
                sectionInnerBoxStyle.padding = new RectOffset(left: 0, right: 0, top: 15, bottom: 0);
                return sectionInnerBoxStyle;
            }
        }
        
        public static GUIStyle SectionInnerBoxWide {
            get {
                if (sectionInnerBoxWideStyle == null) {
                    sectionInnerBoxWideStyle = new GUIStyle(EditorStyles.helpBox);
                    sectionInnerBoxWideStyle.padding.top = 15;
                    sectionInnerBoxWideStyle.padding.bottom = 15;
                    sectionInnerBoxWideStyle.padding.left = 10;
                    sectionInnerBoxWideStyle.padding.right = 10;
                }
                return sectionInnerBoxWideStyle;
            }
        }
        
        public static GUIStyle DynamiSection {
            get {
                if (dynamicSection == null) {
                    dynamicSection = new GUIStyle();
                }
                var defaultPadding = 13;
                if (windowScreenWidth > 600) {
                    var dynamicPadding = (windowScreenWidth - 600) / 2;
                    dynamicSection.padding.left = defaultPadding + dynamicPadding;
                    dynamicSection.padding.right = defaultPadding + dynamicPadding;
                } else if (windowScreenWidth < Constants.IndicationTextHideWidth) {
                    dynamicSection.padding.left = 0;
                    dynamicSection.padding.right = 0;
                } else {
                    dynamicSection.padding.left = 13;
                    dynamicSection.padding.right = 13;
                }
                return dynamicSection;
            }
        }
        
        public static GUIStyle DynamicSectionHelpTab {
            get {
                if (dynamicSectionHelpTab == null) {
                    dynamicSectionHelpTab = new GUIStyle(DynamiSection);
                }
                dynamicSectionHelpTab.padding.left = DynamiSection.padding.left - 3;
                dynamicSectionHelpTab.padding.right = DynamiSection.padding.right - 3;
                return dynamicSectionHelpTab;
            }
        }

        public static GUIStyle ChangelogSectionInnerBox {
            get {
                if (changelogSectionInnerBoxStyle == null) {
                    changelogSectionInnerBoxStyle = new GUIStyle(EditorStyles.helpBox);
                    changelogSectionInnerBoxStyle.margin.bottom = 10;
                    changelogSectionInnerBoxStyle.margin.top = 10;
                }
                return changelogSectionInnerBoxStyle;
            }
        }

        public static GUIStyle IndicationBox {
            get {
                if (indicationBoxStyle == null) {
                    indicationBoxStyle = new GUIStyle();
                }
                indicationBoxStyle.margin.bottom = windowScreenWidth < 141 ? 0 : 10;
                return indicationBoxStyle;
            }
        }
        
        
        public static GUIStyle LinkStyle {
            get {
                if (linkStyle == null) {
                    linkStyle = new GUIStyle(EditorStyles.label);
                    linkStyle.fontStyle = FontStyle.Bold;
                }
                var color = IsDarkMode ? DarkModeLinkColor : LightModeModeLinkColor;
                linkStyle.normal.textColor = color;
                return linkStyle;
            }
        }
        
        private static Color32 DarkModeLinkColor {
            get {
                if (darkModeLinkColor == null) {
                    darkModeLinkColor = new Color32(0x3F, 0x9F, 0xFF, 0xFF);
                }
                return darkModeLinkColor.Value;
            }
        }
        
        
        private static Color32 LightModeModeLinkColor {
            get {
                if (lightModeModeLinkColor == null) {
                    lightModeModeLinkColor = new Color32(0x0F, 0x52, 0xD7, 0xFF);
                }
                return lightModeModeLinkColor.Value;
            }
        }
        public static GUIStyle ProgressBarBarStyle {
            get {
                if (progressBarBarStyle != null) {
                    return progressBarBarStyle;
                }
                var styles = (EditorStyles)typeof(EditorStyles)
                    .GetField("s_Current", BindingFlags.Static | BindingFlags.NonPublic)
                    ?.GetValue(null);
                var style = styles?.GetType()
                    .GetField("m_ProgressBarBar", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(styles);
                progressBarBarStyle = style != null ? (GUIStyle)style : GUIStyle.none;
                return progressBarBarStyle;
            }
        }
        
        internal static GUIStyle Section {
            get {
                if (section == null) {
                    section = new GUIStyle(EditorStyles.helpBox);
                    section.padding = new RectOffset(left: 10, right: 10, top: 10, bottom: 10);
                    section.margin = new RectOffset(left: 0, right: 0, top: 0, bottom: 0);
                }
                return section;
            }
        }
        internal static GUIStyle Scroll {
            get {
                if (scroll == null) {
                    scroll = new GUIStyle(EditorStyles.helpBox);
                }
                if (IsDarkMode) {
                    scroll.normal.background = GUIHelper.ConvertTextureToColor(new Color(0,0,0,0.05f));
                } else {
                    scroll.normal.background = GUIHelper.ConvertTextureToColor(new Color(0,0,0,0.03f));
                }
                return scroll;
            }
        }
        
        internal static GUIStyle BarStyle {
            get {
                if (barStyle == null) {
                    barStyle = new GUIStyle(GUI.skin.label);
                    barStyle.fontSize = 12;
                    barStyle.alignment = TextAnchor.MiddleLeft;
                    barStyle.fixedHeight = 20;
                    barStyle.padding = new RectOffset(10, 5, 2, 2);
                }
                return barStyle;
            }
        }
        
        internal static GUIStyle BarBackgroundStyle {
            get {
                if (barBgStyle == null) {
                    barBgStyle = new GUIStyle();
                }
                barBgStyle.normal.background = GUIHelper.ConvertTextureToColor(Color.clear);
                barBgStyle.hover.background = GUIHelper.ConvertTextureToColor(new Color(0, 0, 0, 0.1f));
                barBgStyle.focused.background = GUIHelper.ConvertTextureToColor(Color.clear);
                barBgStyle.active.background = null;
                return barBgStyle;
            }
        }
        
        internal static GUIStyle ChildBarStyle {
            get {
                if (barChildStyle == null) {
                    barChildStyle = new GUIStyle(BarStyle);
                    barChildStyle.padding = new RectOffset(43, barChildStyle.padding.right, barChildStyle.padding.top, barChildStyle.padding.bottom);
                }
                return barChildStyle;
            }
        }
        
        internal static GUIStyle FoldoutBarStyle {
            get {
                if (barFoldoutStyle == null) {
                    barFoldoutStyle = new GUIStyle(BarStyle);
                    barFoldoutStyle.padding = new RectOffset(23, barFoldoutStyle.padding.right, barFoldoutStyle.padding.top, barFoldoutStyle.padding.bottom);
                }
                return barFoldoutStyle;
            }
        }
        
        public static GUIStyle TimestampStyle {
            get {
                if (timestampStyle == null) {
                    timestampStyle = new GUIStyle(GUI.skin.label);
                }
                if (IsDarkMode) {
                    timestampStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                } else {
                    timestampStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                }
                timestampStyle.hover = timestampStyle.normal;
                return timestampStyle;
            }
        }
        
        internal static GUIStyle ClickableLabelBoldStyle {
            get {
                if (clickableLabelBoldStyle == null) {
                    clickableLabelBoldStyle = new GUIStyle(LabelStyle);
                    clickableLabelBoldStyle.fontStyle = FontStyle.Bold;
                    clickableLabelBoldStyle.fontSize = 14;
                    clickableLabelBoldStyle.margin.left = 17;
                    clickableLabelBoldStyle.active.textColor = clickableLabelBoldStyle.normal.textColor;
                }
                return clickableLabelBoldStyle;
            }
        }
        
        internal static GUIStyle EmptyListText {
            get {
                if (_emptyListText == null) {
                    _emptyListText = new GUIStyle();
                    _emptyListText.fontSize = 11;
                    _emptyListText.padding.left = 15;
                    _emptyListText.padding.top = 10;
                    _emptyListText.alignment = TextAnchor.MiddleCenter;
                    _emptyListText.normal.textColor = Color.gray;
                }

                return _emptyListText;
            }
        }
        
        internal static GUIStyle StacktraceTextAreaStyle {
            get {
                if (_stacktraceTextAreaStyle == null) {
                    _stacktraceTextAreaStyle = new GUIStyle(EditorStyles.textArea);
                    _stacktraceTextAreaStyle.border = new RectOffset(0, 0, 0, 0);
                }
                return _stacktraceTextAreaStyle;
            }
        }
        
        internal static GUIStyle EntryBoxStyle {
            get {
                if (_entryBoxStyle == null) {
                    _entryBoxStyle = new GUIStyle();
                    _entryBoxStyle.margin.left = 30;
                }
                return _entryBoxStyle;
            }
        }
        
        internal static GUIStyle ChildEntryBoxStyle {
            get {
                if (_childEntryBoxStyle == null) {
                    _childEntryBoxStyle = new GUIStyle();
                    _childEntryBoxStyle.margin.left = 45;
                }
                return _childEntryBoxStyle;
            }
        }
        
        internal static GUIStyle CustomFoldoutStyle {
            get {
                if (_customFoldoutStyle == null) {
                    _customFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                    _customFoldoutStyle.margin.top = 4;
                    _customFoldoutStyle.margin.left = 0;
                    _customFoldoutStyle.padding.left = 0;
                    _customFoldoutStyle.fixedWidth = 100;
                }
                return _customFoldoutStyle;
            }
        }
        
        internal static GUIStyle RemoveIconStyle {
            get {
                if (_removeIconStyle == null) {
                    _removeIconStyle = new GUIStyle();
                    _removeIconStyle.margin.top = 5;
                    _removeIconStyle.fixedWidth = 17;
                    _removeIconStyle.fixedHeight = 17;
                }
                return _removeIconStyle;
            }
        }
        
        internal static GUIStyle UpgradeLicenseButtonStyle {
            get {
                if (upgradeLicenseButtonStyle == null) {
                    upgradeLicenseButtonStyle = new GUIStyle(GUI.skin.button);
                    upgradeLicenseButtonStyle.padding = new RectOffset(5, 5, 0, 0);
                }
                return upgradeLicenseButtonStyle;
            }
        }
        
        internal static GUIStyle UpgradeLicenseButtonOverlayStyle {
            get {
                if (upgradeLicenseButtonOverlayStyle == null) {
                    upgradeLicenseButtonOverlayStyle = new GUIStyle(UpgradeLicenseButtonStyle);
                }
                return upgradeLicenseButtonOverlayStyle;
            }
        }
        
        internal static GUIStyle UpgradeButtonStyle {
            get {
                if (upgradeButtonStyle == null) {
                    upgradeButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    upgradeButtonStyle.fontStyle = FontStyle.Bold;
                    upgradeButtonStyle.fontSize = 14;
                    upgradeButtonStyle.fixedHeight = 24;
                }
                return upgradeButtonStyle;
            }
        }
        
        internal static GUIStyle HideButtonStyle {
            get {
                if (hideButtonStyle == null) {
                    hideButtonStyle = new GUIStyle(GUI.skin.button);
                }
                return hideButtonStyle;
            }
        }
        
        internal static GUIStyle HelpTabButton {
            get {
                if (helpTabButton == null) {
                    helpTabButton = new GUIStyle(GUI.skin.button);
                    helpTabButton.alignment = TextAnchor.MiddleLeft;
                    helpTabButton.padding.left = 10;
                }
                return helpTabButton;
            }
        }
        
        internal static GUIStyle IndicationHelpBox {
            get {
                if (indicationHelpBox == null) {
                    indicationHelpBox = new GUIStyle(EditorStyles.helpBox);
                    indicationHelpBox.margin.right = 0;
                    indicationHelpBox.margin.left = 0;
                }
                return indicationHelpBox;
            }
        }
    }
}
