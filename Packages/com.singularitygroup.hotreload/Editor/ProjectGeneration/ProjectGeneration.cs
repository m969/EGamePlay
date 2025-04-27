using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload;
using SingularityGroup.HotReload.Editor.Util;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using Assembly = UnityEditor.Compilation.Assembly;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
#if UNITY_2019_1_OR_NEWER
using System.Reflection;
#endif

namespace SingularityGroup.HotReload.Editor.ProjectGeneration {
    class ProjectGeneration {
        private enum ScriptingLanguage {
            None,
            CSharp
        }

        [Serializable]
        class Config {
            public string projectExclusionRegex;
            public HashSet<string> projectBlacklist;
            public HashSet<string> polyfillSourceFiles;
            public bool excludeAllAnalyzers;
            public bool useBuiltInProjectGeneration;
        }
        
        public static readonly string MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";

        /// <summary>
        /// Map source extensions to ScriptingLanguages
        /// </summary>
        private static readonly Dictionary<string, ScriptingLanguage> k_BuiltinSupportedExtensions =
            new Dictionary<string, ScriptingLanguage> {
                { "cs", ScriptingLanguage.CSharp },
                { "uxml", ScriptingLanguage.None },
                { "uss", ScriptingLanguage.None },
                { "shader", ScriptingLanguage.None },
                { "compute", ScriptingLanguage.None },
                { "cginc", ScriptingLanguage.None },
                { "hlsl", ScriptingLanguage.None },
                { "glslinc", ScriptingLanguage.None },
                { "template", ScriptingLanguage.None },
                { "raytrace", ScriptingLanguage.None },
                { "json", ScriptingLanguage.None },
                { "rsp", ScriptingLanguage.None },
                { "asmdef", ScriptingLanguage.None },
                { "asmref", ScriptingLanguage.None },
                { "xaml", ScriptingLanguage.None },
                { "tt", ScriptingLanguage.None },
                { "t4", ScriptingLanguage.None },
                { "ttinclude", ScriptingLanguage.None }
            };

        private string m_SolutionProjectEntryTemplate = string.Join(Environment.NewLine,
            @"Project(""{{{0}}}"") = ""{1}"", ""{2}"", ""{{{3}}}""",
            @"EndProject").Replace("    ", "\t");

        private string m_SolutionProjectConfigurationTemplate = string.Join(Environment.NewLine,
            @"        {{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
            @"        {{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU").Replace("    ", "\t");

        private string[] m_ProjectSupportedExtensions = new string[0];

        private readonly string m_ProjectName;
        private readonly string m_ProjectDirectory;
        private readonly string m_SolutionDirectory;
        private readonly IFileIO m_FileIOProvider;
        private readonly IGUIDGenerator m_GUIDGenerator;
        private static readonly SemaphoreSlim gate = new SemaphoreSlim(1, 1);

        private const string k_ToolsVersion = "4.0";
        private const string k_ProductVersion = "10.0.20506";
        private const string k_BaseDirectory = ".";

#if !UNITY_2020_2_OR_NEWER
    private const string k_TargetLanguageVersion = "latest";
#endif

        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly Dictionary<string, UnityEditor.PackageManager.PackageInfo> m_PackageInfoCache =
            new Dictionary<string, UnityEditor.PackageManager.PackageInfo>();

        private Assembly[] m_AllEditorAssemblies;

        private Assembly[] m_AllPlayerAssemblies;

        private string[] m_AllAssetPaths;

        private string m_EngineAssemblyPath;

        private string m_EditorAssemblyPath;

        private bool m_SuppressCommonWarnings;

        private string m_FallbackRootNamespace;

        private IHotReloadProjectGenerationPostProcessor[] m_PostProcessors;


        public static bool IsSyncing => gate.CurrentCount == 0;

        internal const string tempDir = PackageConst.LibraryCachePath + "/Solution";
        public static string GetUnityProjectDirectory(string dataPath) => new DirectoryInfo(dataPath).Parent.FullName;
        public static string GetSolutionFilePath(string dataPath) => Path.Combine(tempDir, Path.GetFileName(GetUnityProjectDirectory(dataPath)) + ".sln");

        public static Task GenerateSlnAndCsprojFiles(string dataPath) {
            if (!IsSyncing) {
                return GenerateAsync(dataPath);
            }
            return Task.CompletedTask;
        }

        public static Task EnsureSlnAndCsprojFiles(string dataPath) {
            if (File.Exists(GetSolutionFilePath(dataPath))) {
                return Task.CompletedTask;
            }

            return GenerateAsync(dataPath);
        }

        private static Task GenerateAsync(string dataPath) {
            Directory.CreateDirectory(tempDir);
            var gen = new ProjectGeneration(tempDir, GetUnityProjectDirectory(dataPath));
            return gen.Sync();
        }


        public ProjectGeneration(string solutionDirectory, string unityProjectDirectory) {
            m_ProjectDirectory = unityProjectDirectory;
            m_SolutionDirectory = solutionDirectory;
            m_ProjectName = Path.GetFileName(unityProjectDirectory);
            m_FileIOProvider = new FileIOProvider();
            m_GUIDGenerator = new GUIDProvider();
        }

        public async Task Sync() {
            await ThreadUtility.SwitchToThreadPool();
            var config = LoadConfig();
            if (config.useBuiltInProjectGeneration) {
                return;
            }
            
            await ThreadUtility.SwitchToMainThread();
            await gate.WaitAsync();
            try {
                //Cache all data that is accessed via unity API on the unity main thread.
                m_AllAssetPaths = AssetDatabase.GetAllAssetPaths();
                m_ProjectSupportedExtensions = EditorSettings.projectGenerationUserExtensions;
                m_EngineAssemblyPath = InternalEditorUtility.GetEngineAssemblyPath();
                m_EditorAssemblyPath = InternalEditorUtility.GetEditorAssemblyPath();
                m_FallbackRootNamespace = EditorSettings.projectGenerationRootNamespace;
                m_SuppressCommonWarnings =
#if UNITY_2020_1_OR_NEWER
                    PlayerSettings.suppressCommonWarnings;
#else
        false;
#endif

                //Do the remaining work on a separate thread
                await Task.WhenAll(
                    BuildPackageInfoCache(),
                    BuildEditorAssemblies(),
                    BuildPostProcessors()
                );
                await GenerateAndWriteSolutionAndProjects(config);
            } finally {
                gate.Release();
            }
        }

        private Config LoadConfig() {
            var configPath = Path.Combine(m_ProjectDirectory, PackageConst.ConfigFileName); 
            Config config;
            if(File.Exists(configPath)) {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            } else {
                config = new Config();
            }
            return config;
        }

        private bool ShouldFileBePartOfSolution(string file) {
            // Exclude files coming from packages except if they are internalized.
            if (IsInternalizedPackagePath(file)) {
                return false;
            }

            return HasValidExtension(file);
        }

        public bool HasValidExtension(string file) {
            var extension = Path.GetExtension(file);

            // Dll's are not scripts but still need to be included..
            if (file.Equals(".dll", StringComparison.OrdinalIgnoreCase))
                return true;

            return IsSupportedExtension(extension);
        }

        private bool IsSupportedExtension(string extension) {
            extension = extension.TrimStart('.');
            return k_BuiltinSupportedExtensions.ContainsKey(extension) || m_ProjectSupportedExtensions.Contains(extension);
        }

        async Task GenerateAndWriteSolutionAndProjects(Config config) {
            await ThreadUtility.SwitchToThreadPool();
            
            var projectExclusionRegex = config.projectExclusionRegex != null ? new Regex(config.projectExclusionRegex, RegexOptions.Compiled | RegexOptions.Singleline) : null;
            var projectBlacklist = config.projectBlacklist ?? new HashSet<string>();
            var polyfillSourceFiles = config.polyfillSourceFiles ?? new HashSet<string>();
            var filteredProjects = new HashSet<string>();
            var runtimeDependenciesBuilder = new List<string>();
            runtimeDependenciesBuilder.Add(typeof(HarmonyLib.DetourApi).Assembly.Location);
#           if UNITY_2019_4_OR_NEWER
                runtimeDependenciesBuilder.Add(typeof(Helper2019).Assembly.Location);
#endif
#           if UNITY_2020_3_OR_NEWER
                runtimeDependenciesBuilder.Add(typeof(Helper2020).Assembly.Location);
#endif
#           if UNITY_2022_2_OR_NEWER
                runtimeDependenciesBuilder.Add(typeof(Helper2022).Assembly.Location);
#endif
            var runtimeDependencies = runtimeDependenciesBuilder.ToArray();
            
            // Only synchronize islands that have associated source files and ones that we actually want in the project.
            // This also filters out DLLs coming from .asmdef files in packages.
            var assemblies = GetAssemblies(ShouldFileBePartOfSolution).ToArray();
            var projectParts = new List<ProjectPart>();
            foreach (var assembly in assemblies) {
                if(projectExclusionRegex != null && projectExclusionRegex.IsMatch(assembly.name)) {
                    filteredProjects.Add(assembly.name);
                    continue;
                }
                var part = new ProjectPart(assembly.name, assembly, "", m_FallbackRootNamespace, polyfillSourceFiles);
                string projectPath;
#               if (UNITY_2021_3_OR_NEWER)
                    projectPath = Path.GetRelativePath(m_ProjectDirectory, ProjectFile(part)).Replace('\\', '/');
#               else
                    projectPath = ProjectFile(part).Replace('\\', '/').Replace(m_ProjectDirectory.Replace('\\', '/'), "");
#endif
                if(projectBlacklist.Contains(projectPath)) {
                    filteredProjects.Add(assembly.name);
                    continue;
                }
                projectParts.Add(part);
            }

            SyncSolution(projectParts.ToArray());

            await ThreadUtility.SwitchToMainThread();
            var responseFiles = new List<ResponseFileData>[projectParts.Count];
            for (var i = 0; i < projectParts.Count; i++) {
                responseFiles[i] = projectParts[i].ParseResponseFileData(m_ProjectDirectory).ToList();
            }
            
            await ThreadUtility.SwitchToThreadPool();
            for (var i = 0; i < projectParts.Count; i++) {
                SyncProject(projectParts[i], responseFiles[i], filteredProjects, runtimeDependencies, config);
            }

            foreach (var pp in m_PostProcessors) {
                try {
                    pp.OnGeneratedCSProjectFilesThreaded();
                } catch (Exception ex) {
                    Log.Warning("Post processor '{0}' threw exception when calling OnGeneratedCSProjectFilesThreaded:\n{1}", pp, ex);
                }
            }
        }

        private void SyncProject(
            ProjectPart island, 
            List<ResponseFileData> responseFileData,
            HashSet<string> filteredProjects, 
            string[] runtimeDependencies,
            Config config) {
            
            SyncProjectFileIfNotChanged(
                ProjectFile(island),
                ProjectText(island, responseFileData, filteredProjects, runtimeDependencies, config));
        }

        private void SyncProjectFileIfNotChanged(string path, string newContents) {
            foreach (var pp in m_PostProcessors) {
                try {
                    newContents = pp.OnGeneratedCSProjectThreaded(path, newContents);
                } catch (Exception ex) {
                    Log.Warning("Post processor '{0}' failed when processing project '{1}':\n{2}", pp, path, ex);
                }
            }

            SyncFileIfNotChanged(path, newContents);
        }

        private void SyncSolutionFileIfNotChanged(string path, string newContents) {
            foreach (var pp in m_PostProcessors) {
                try {
                    newContents = pp.OnGeneratedSlnSolutionThreaded(path, newContents);
                } catch (Exception ex) {
                    Log.Warning("Post processor '{0}' failed when processing solution '{1}':\n{2}", pp, path, ex);
                }
            }

            SyncFileIfNotChanged(path, newContents);
        }


        private void SyncFileIfNotChanged(string path, string newContents) {
            try {
                if (m_FileIOProvider.Exists(path) && newContents == m_FileIOProvider.ReadAllText(path)) {
                    return;
                }
            } catch (Exception exception) {
                Log.Exception(exception);
            }

            m_FileIOProvider.WriteAllText(path, newContents);
        }

        private string ProjectText(ProjectPart assembly, List<ResponseFileData> responseFilesData, HashSet<string> filteredProjects, string[] runtimeDependencies, Config config) {
            var projectBuilder = new StringBuilder(ProjectHeader(assembly, responseFilesData, config));

            foreach (var file in assembly.SourceFiles) {
                var fullFile = m_FileIOProvider.EscapedRelativePathFor(file, m_SolutionDirectory);
                projectBuilder.Append("     <Compile Include=\"").Append(fullFile).Append("\" />").Append(Environment.NewLine);
            }

            projectBuilder.Append(assembly.AssetsProjectPart);

            var responseRefs = responseFilesData.SelectMany(x => x.FullPathReferences.Select(r => r));
            var internalAssemblyReferences = assembly.AssemblyReferences
                .Where(reference => filteredProjects.Contains(reference.name) || !reference.sourceFiles.Any(ShouldFileBePartOfSolution)).Select(i => i.outputPath);
            var allReferences =
                assembly.CompiledAssemblyReferences
                    .Union(responseRefs)
                    .Union(internalAssemblyReferences).ToArray();

            foreach (var reference in allReferences) {
                var fullReference = Path.IsPathRooted(reference) ? reference : Path.Combine(m_ProjectDirectory, reference);
                AppendReference(fullReference, projectBuilder);
            }
            foreach (var path in runtimeDependencies) {
                AppendReference(path, projectBuilder);
            }

            if (assembly.AssemblyReferences.Length > 0) {
                projectBuilder.Append("  </ItemGroup>").Append(Environment.NewLine);
                projectBuilder.Append("  <ItemGroup>").Append(Environment.NewLine);
                foreach (var reference in assembly.AssemblyReferences.Where(i => !filteredProjects.Contains(i.name) && i.sourceFiles.Any(ShouldFileBePartOfSolution))) {
                    var name = GetProjectName(reference.name, reference.defines);
                    projectBuilder.Append("    <ProjectReference Include=\"").Append(name).Append(".csproj").Append("\">")
                        .Append(Environment.NewLine);
                    projectBuilder.Append("      <Project>{").Append(ProjectGuid(name)).Append("}</Project>").Append(Environment.NewLine);
                    projectBuilder.Append("      <Name>").Append(name).Append("</Name>").Append(Environment.NewLine);
                    projectBuilder.Append("    </ProjectReference>").Append(Environment.NewLine);
                }
            }

            projectBuilder.Append(ProjectFooter());
            return projectBuilder.ToString();
        }

        private static void AppendReference(string fullReference, StringBuilder projectBuilder) {
            var escapedFullPath = SecurityElement.Escape(fullReference);
            escapedFullPath = escapedFullPath.NormalizePath();
            projectBuilder.Append("     <Reference Include=\"").Append(FileSystemUtil.FileNameWithoutExtension(escapedFullPath))
                .Append("\">").Append(Environment.NewLine);
            projectBuilder.Append("     <HintPath>").Append(escapedFullPath).Append("</HintPath>").Append(Environment.NewLine);
            projectBuilder.Append("     </Reference>").Append(Environment.NewLine);
        }

        private string ProjectFile(ProjectPart projectPart) {
            return Path.Combine(m_SolutionDirectory, $"{GetProjectName(projectPart.Name, projectPart.Defines)}.csproj");
        }

        public string SolutionFile() {
            return Path.Combine(m_SolutionDirectory, $"{m_ProjectName}.sln");
        }

        private string ProjectHeader(
            ProjectPart assembly,
            List<ResponseFileData> responseFilesData,
            Config config
        ) {
            var otherResponseFilesData = GetOtherArgumentsFromResponseFilesData(responseFilesData);
            var arguments = new object[] {
                k_ToolsVersion,
                k_ProductVersion,
                ProjectGuid(GetProjectName(assembly.Name, assembly.Defines)),
                m_EngineAssemblyPath,
                m_EditorAssemblyPath,
                string.Join(";", assembly.Defines.Concat(responseFilesData.SelectMany(x => x.Defines)).Distinct().ToArray()),
                MSBuildNamespaceUri,
                assembly.Name,
                assembly.OutputPath,
                assembly.RootNamespace,
                "",
                GenerateLangVersion(otherResponseFilesData["langversion"], assembly),
                k_BaseDirectory,
                assembly.CompilerOptions.AllowUnsafeCode | responseFilesData.Any(x => x.Unsafe),
                GenerateNoWarn(otherResponseFilesData["nowarn"].Distinct().ToList()),
                config.excludeAllAnalyzers ? "" : GenerateAnalyserItemGroup(RetrieveRoslynAnalyzers(assembly, otherResponseFilesData)),
                config.excludeAllAnalyzers ? "" : GenerateAnalyserAdditionalFiles(otherResponseFilesData["additionalfile"].SelectMany(x=>x.Split(';')).Distinct().ToArray()),
                config.excludeAllAnalyzers ? "" : GenerateRoslynAnalyzerRulesetPath(assembly, otherResponseFilesData),
                GenerateWarningLevel(otherResponseFilesData["warn"].Concat(otherResponseFilesData["w"]).Distinct()),
                GenerateWarningAsError(otherResponseFilesData["warnaserror"], otherResponseFilesData["warnaserror-"],
                    otherResponseFilesData["warnaserror+"]),
                GenerateDocumentationFile(otherResponseFilesData["doc"].ToArray()),
                GenerateNullable(otherResponseFilesData["nullable"])
            };

            try {
                return string.Format(GetProjectHeaderTemplate(), arguments);
            } catch (Exception) {
                throw new NotSupportedException(
                    "Failed creating c# project because the c# project header did not have the correct amount of arguments, which is " +
                    arguments.Length);
            }
        }

        string[] RetrieveRoslynAnalyzers(ProjectPart assembly, ILookup<string, string> otherResponseFilesData) {
            var otherAnalyzers = otherResponseFilesData["a"] ?? Array.Empty<string>();
        #if UNITY_2020_2_OR_NEWER
              return otherResponseFilesData["analyzer"].Concat(otherAnalyzers)
                .SelectMany(x=>x.Split(';'))
        // #if !ROSLYN_ANALYZER_FIX
        //         .Concat(GetRoslynAnalyzerPaths())
        // #else
                .Concat(assembly.CompilerOptions.RoslynAnalyzerDllPaths ?? Array.Empty<string>())
        // #endif
                .Select(MakeAbsolutePath)
                .Distinct()
                .ToArray();
        #else
              return otherResponseFilesData["analyzer"].Concat(otherAnalyzers)
                .SelectMany(x=>x.Split(';'))
                .Distinct()
                .Select(MakeAbsolutePath)
                .ToArray();
        #endif
        }

        private static string GenerateAnalyserItemGroup(string[] paths) {
            //   <ItemGroup>
            //      <Analyzer Include="..\packages\Comments_analyser.1.0.6626.21356\analyzers\dotnet\cs\Comments_analyser.dll" />
            //      <Analyzer Include="..\packages\UnityEngineAnalyzer.1.0.0.0\analyzers\dotnet\cs\UnityEngineAnalyzer.dll" />
            //  </ItemGroup>
            if (!paths.Any())
                return string.Empty;

            var analyserBuilder = new StringBuilder();
            analyserBuilder.AppendLine("  <ItemGroup>");
            foreach (var path in paths) {
                analyserBuilder.AppendLine($"    <Analyzer Include=\"{path.NormalizePath()}\" />");
            }

            analyserBuilder.AppendLine("  </ItemGroup>");
            return analyserBuilder.ToString();
        }

        private string GenerateRoslynAnalyzerRulesetPath(ProjectPart assembly, ILookup<string, string> otherResponseFilesData) {
#if UNITY_2020_2_OR_NEWER
            return GenerateAnalyserRuleSet(otherResponseFilesData["ruleset"].Append(assembly.CompilerOptions.RoslynAnalyzerRulesetPath)
                .Where(a => !string.IsNullOrEmpty(a)).Distinct().Select(x => MakeAbsolutePath(x).NormalizePath()).ToArray());
#else
            return GenerateAnalyserRuleSet(otherResponseFilesData["ruleset"].Distinct().Select(x => MakeAbsolutePath(x).NormalizePath()).ToArray());
#endif
        }

        private static string GenerateAnalyserRuleSet(string[] paths) {
            //<CodeAnalysisRuleSet>..\path\to\myrules.ruleset</CodeAnalysisRuleSet>
            if (!paths.Any())
                return string.Empty;

            return
                $"{Environment.NewLine}{string.Join(Environment.NewLine, paths.Select(a => $"    <CodeAnalysisRuleSet>{a}</CodeAnalysisRuleSet>"))}";
        }

        private static string MakeAbsolutePath(string path) {
            return Path.IsPathRooted(path) ? path : Path.GetFullPath(path);
        }

        private string GenerateNullable(IEnumerable<string> enumerable) {
            var val = enumerable.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(val))
                return string.Empty;

            return $"{Environment.NewLine}    <Nullable>{val}</Nullable>";
        }

        private static string GenerateDocumentationFile(string[] paths) {
            if (!paths.Any())
                return String.Empty;

            return $"{Environment.NewLine}{string.Join(Environment.NewLine, paths.Select(a => $"  <DocumentationFile>{a}</DocumentationFile>"))}";
        }

        private static string GenerateWarningAsError(IEnumerable<string> args, IEnumerable<string> argsMinus, IEnumerable<string> argsPlus) {
            var returnValue = String.Empty;
            var allWarningsAsErrors = false;
            var warningIds = new List<string>();

            foreach (var s in args) {
                if (s == "+" || s == string.Empty) allWarningsAsErrors = true;
                else if (s == "-") allWarningsAsErrors = false;
                else {
                    warningIds.Add(s);
                }
            }

            warningIds.AddRange(argsPlus);

            returnValue += $@"    <TreatWarningsAsErrors>{allWarningsAsErrors}</TreatWarningsAsErrors>";
            if (warningIds.Any()) {
                returnValue += $"{Environment.NewLine}    <WarningsAsErrors>{string.Join(";", warningIds)}</WarningsAsErrors>";
            }

            if (argsMinus.Any())
                returnValue += $"{Environment.NewLine}    <WarningsNotAsErrors>{string.Join(";", argsMinus)}</WarningsNotAsErrors>";

            return $"{Environment.NewLine}{returnValue}";
        }

        private static string GenerateWarningLevel(IEnumerable<string> warningLevel) {
            var level = warningLevel.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(level))
                return level;

            return 4.ToString();
        }

        private static string GetSolutionText() {
            return string.Join(Environment.NewLine,
                @"",
                @"Microsoft Visual Studio Solution File, Format Version {0}",
                @"# Visual Studio {1}",
                @"{2}",
                @"Global",
                @"    GlobalSection(SolutionConfigurationPlatforms) = preSolution",
                @"        Debug|Any CPU = Debug|Any CPU",
                @"    EndGlobalSection",
                @"    GlobalSection(ProjectConfigurationPlatforms) = postSolution",
                @"{3}",
                @"    EndGlobalSection",
                @"    GlobalSection(SolutionProperties) = preSolution",
                @"        HideSolutionNode = FALSE",
                @"    EndGlobalSection",
                @"EndGlobal",
                @"").Replace("    ", "\t");
        }

        private static string GetProjectFooterTemplate() {
            return string.Join(Environment.NewLine,
                @"  </ItemGroup>",
                @"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />",
                @"  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.",
                @"       Other similar extension points exist, see Microsoft.Common.targets.",
                @"  <Target Name=""BeforeBuild"">",
                @"  </Target>",
                @"  <Target Name=""AfterBuild"">",
                @"  </Target>",
                @"  -->",
                @"</Project>",
                @"");
        }

        private static string GetProjectHeaderTemplate() {
            var header = new[] {
                @"<?xml version=""1.0"" encoding=""utf-8""?>",
                @"<Project ToolsVersion=""{0}"" DefaultTargets=""Build"" xmlns=""{6}"">",
                @"  <PropertyGroup>",
                @"    <LangVersion>{11}</LangVersion>",
                @"    <_TargetFrameworkDirectories>non_empty_path_generated_by_unity.rider.package</_TargetFrameworkDirectories>",
                @"    <_FullFrameworkReferenceAssemblyPaths>non_empty_path_generated_by_unity.rider.package</_FullFrameworkReferenceAssemblyPaths>",
                @"    <DisableHandlePackageFileConflicts>true</DisableHandlePackageFileConflicts>{17}",
                @"  </PropertyGroup>",
                @"  <PropertyGroup>",
                @"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>",
                @"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>",
                @"    <ProductVersion>{1}</ProductVersion>",
                @"    <SchemaVersion>2.0</SchemaVersion>",
                @"    <RootNamespace>{9}</RootNamespace>",
                @"    <ProjectGuid>{{{2}}}</ProjectGuid>",
                @"    <ProjectTypeGuids>{{E097FAD1-6243-4DAD-9C02-E9B9EFC3FFC1}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>",
                @"    <OutputType>Library</OutputType>",
                @"    <AppDesignerFolder>Properties</AppDesignerFolder>",
                @"    <AssemblyName>{7}</AssemblyName>",
                @"    <TargetFrameworkVersion>v4.7.1</TargetFrameworkVersion>",
                @"    <FileAlignment>512</FileAlignment>",
                @"    <BaseDirectory>{12}</BaseDirectory>",
                @"  </PropertyGroup>",
                @"  <PropertyGroup>",
                @"    <DebugSymbols>true</DebugSymbols>",
                @"    <DebugType>full</DebugType>",
                @"    <Optimize>false</Optimize>",
                @"    <OutputPath>{8}</OutputPath>",
                @"    <DefineConstants>{5}</DefineConstants>",
                @"    <ErrorReport>prompt</ErrorReport>",
                @"    <WarningLevel>{18}</WarningLevel>",
                @"    <NoWarn>{14}</NoWarn>",
                @"    <AllowUnsafeBlocks>{13}</AllowUnsafeBlocks>{19}{20}{21}",
                @"  </PropertyGroup>"
            };

            var forceExplicitReferences = new[] {
                @"  <PropertyGroup>",
                @"    <NoConfig>true</NoConfig>",
                @"    <NoStdLib>true</NoStdLib>",
                @"    <AddAdditionalExplicitAssemblyReferences>false</AddAdditionalExplicitAssemblyReferences>",
                @"    <ImplicitlyExpandNETStandardFacades>false</ImplicitlyExpandNETStandardFacades>",
                @"    <ImplicitlyExpandDesignTimeFacades>false</ImplicitlyExpandDesignTimeFacades>",
                @"  </PropertyGroup>"
            };

            var footer = new[] {
                @"{15}{16}  <ItemGroup>",
                @""
            };

            var pieces = header.Concat(forceExplicitReferences).Concat(footer).ToArray();
            return string.Join(Environment.NewLine, pieces);
        }

        private void SyncSolution(ProjectPart[] islands) {
            SyncSolutionFileIfNotChanged(SolutionFile(), SolutionText(islands));
        }

        private string SolutionText(ProjectPart[] islands) {
            var fileversion = "11.00";
            var vsversion = "2010";

            var projectEntries = GetProjectEntries(islands);
            var projectConfigurations = string.Join(Environment.NewLine,
                islands.Select(i => GetProjectActiveConfigurations(ProjectGuid(GetProjectName(i.Name, i.Defines)))).ToArray());
            return string.Format(GetSolutionText(), fileversion, vsversion, projectEntries, projectConfigurations);
        }

        private static ILookup<string, string> GetOtherArgumentsFromResponseFilesData(List<ResponseFileData> responseFilesData) {
            var paths = responseFilesData.SelectMany(x => {
                    return x.OtherArguments
                        .Where(a => a.StartsWith("/", StringComparison.Ordinal) || a.StartsWith("-", StringComparison.Ordinal))
                        .Select(b => {
                            var index = b.IndexOf(":", StringComparison.Ordinal);
                            if (index > 0 && b.Length > index) {
                                var key = b.Substring(1, index - 1);
                                return new KeyValuePair<string, string>(key.ToLowerInvariant(), b.Substring(index + 1));
                            }

                            const string warnaserror = "warnaserror";
                            if (b.Substring(1).StartsWith(warnaserror, StringComparison.Ordinal)) {
                                return new KeyValuePair<string, string>(warnaserror, b.Substring(warnaserror.Length + 1));
                            }

                            const string nullable = "nullable";
                            if (b.Substring(1).StartsWith(nullable)) {
                                var res = b.Substring(nullable.Length + 1);
                                if (string.IsNullOrWhiteSpace(res) || res.Equals("+"))
                                    res = "enable";
                                else if (res.Equals("-"))
                                    res = "disable";
                                return new KeyValuePair<string, string>(nullable, res);
                            }

                            return default(KeyValuePair<string, string>);
                        });
                })
                .Distinct()
                .ToLookup(o => o.Key, pair => pair.Value);
            return paths;
        }

        private string GenerateLangVersion(IEnumerable<string> langVersionList, ProjectPart assembly) {
            var langVersion = langVersionList.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(langVersion))
                return langVersion;
#if UNITY_2020_2_OR_NEWER
            return assembly.CompilerOptions.LanguageVersion;
#else
            return k_TargetLanguageVersion;
#endif
        }

        private static string GenerateAnalyserAdditionalFiles(string[] paths) {
            if (!paths.Any())
                return string.Empty;

            var analyserBuilder = new StringBuilder();
            analyserBuilder.AppendLine("  <ItemGroup>");
            foreach (var path in paths) {
                analyserBuilder.AppendLine($"    <AdditionalFiles Include=\"{path}\" />");
            }

            analyserBuilder.AppendLine("  </ItemGroup>");
            return analyserBuilder.ToString();
        }

        public string GenerateNoWarn(List<string> codes) {
            if (m_SuppressCommonWarnings)
                codes.AddRange(new[] { "0169", "0649" });

            if (!codes.Any())
                return string.Empty;

            return string.Join(",", codes.Distinct());
        }

        private string GetProjectEntries(ProjectPart[] islands) {
            var projectEntries = islands.Select(i => string.Format(
                m_SolutionProjectEntryTemplate,
                SolutionGuidGenerator.GuidForSolution(),
                i.Name,
                Path.GetFileName(ProjectFile(i)),
                ProjectGuid(GetProjectName(i.Name, i.Defines))
            ));

            return string.Join(Environment.NewLine, projectEntries.ToArray());
        }

        /// <summary>
        /// Generate the active configuration string for a given project guid
        /// </summary>
        private string GetProjectActiveConfigurations(string projectGuid) {
            return string.Format(
                m_SolutionProjectConfigurationTemplate,
                projectGuid);
        }

        private static string ProjectFooter() {
            return GetProjectFooterTemplate();
        }


        private string ProjectGuid(string name) {
            return m_GUIDGenerator.ProjectGuid(m_ProjectName + name);
        }

        public ProjectGenerationFlag ProjectGenerationFlag => ProjectGenerationFlag.Local | ProjectGenerationFlag.Embedded;

        public string GetAssemblyNameFromScriptPath(string path) {
            return CompilationPipeline.GetAssemblyNameFromScriptPath(path);
        }

        public IEnumerable<Assembly> GetAssemblies(Func<string, bool> shouldFileBePartOfSolution) {
            return m_AllEditorAssemblies.Where(a => a.sourceFiles.Any(shouldFileBePartOfSolution));
        }

        private Task BuildEditorAssemblies() {
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Editor);
            return Task.Run(() => {
                var result = new Assembly[assemblies.Length];
                for (var i = 0; i < assemblies.Length; i++) {
                    var assembly = assemblies[i];
                    var outputPath = $@"Temp\Bin\Debug\{assembly.name}\";
                    result[i] = new Assembly(assembly.name, outputPath, assembly.sourceFiles,
                        assembly.defines,
                        assembly.assemblyReferences, assembly.compiledAssemblyReferences,
                        assembly.flags, assembly.compilerOptions
#if UNITY_2020_2_OR_NEWER
                        , assembly.rootNamespace
#endif
                    );
                }

                m_AllEditorAssemblies = result;
            });
        }

        public string GetProjectName(string name, string[] defines) {
            if (!ProjectGenerationFlag.HasFlag(ProjectGenerationFlag.PlayerAssemblies))
                return name;
            return !defines.Contains("UNITY_EDITOR") ? name + ".Player" : name;
        }

        private static string ResolvePotentialParentPackageAssetPath(string assetPath) {
            const string packagesPrefix = "packages/";
            if (!assetPath.StartsWith(packagesPrefix, StringComparison.OrdinalIgnoreCase)) {
                return null;
            }

            var followupSeparator = assetPath.IndexOf('/', packagesPrefix.Length);
            if (followupSeparator == -1) {
                return assetPath.ToLowerInvariant();
            }

            return assetPath.Substring(0, followupSeparator).ToLowerInvariant();
        }

        public UnityEditor.PackageManager.PackageInfo FindForAssetPath(string assetPath) {
            var parentPackageAssetPath = ResolvePotentialParentPackageAssetPath(assetPath);
            if (parentPackageAssetPath == null) {
                return null;
            }

            PackageInfo cachedPackageInfo;
            if (m_PackageInfoCache.TryGetValue(parentPackageAssetPath, out cachedPackageInfo)) {
                return cachedPackageInfo;
            }

            return null;
        }

        async Task BuildPackageInfoCache() {
#if UNITY_2019_4_OR_NEWER
            m_PackageInfoCache.Clear();
            var parentAssetPaths = new HashSet<string>();
            await Task.Run(() => {
                for (var i = 0; i < m_AllAssetPaths.Length; i++) {
                    if (string.IsNullOrWhiteSpace(m_AllAssetPaths[i])) {
                        continue;
                    }

                    var parentPackageAssetPath = ResolvePotentialParentPackageAssetPath(m_AllAssetPaths[i]);
                    if (parentPackageAssetPath == null) {
                        continue;
                    }

                    parentAssetPaths.Add(parentPackageAssetPath);
                }
            });
            foreach (var parentAssetPath in parentAssetPaths) {
                var result = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(parentAssetPath);
                m_PackageInfoCache.Add(parentAssetPath, result);
            }
#else
      //keep compiler happy
      await Task.CompletedTask;
#endif
        }

        async Task BuildPostProcessors() {
#if UNITY_2019_1_OR_NEWER
            var types = TypeCache.GetTypesDerivedFrom<IHotReloadProjectGenerationPostProcessor>();
            m_PostProcessors = await Task.Run(() => {
                var postProcessors = new List<IHotReloadProjectGenerationPostProcessor>(types.Count);
                foreach (var type in types) {
                    try {
                        var instance = (IHotReloadProjectGenerationPostProcessor)Activator.CreateInstance(type);
                        postProcessors.Add(instance);
                    } catch (MissingMethodException) {
                        Log.Warning("The type '{0}' was expected to have a public default constructor but it didn't", type.FullName);
                    } catch (TargetInvocationException ex) {
                        Log.Warning("Exception occurred when invoking default constructor of '{0}':\n{1}", type.FullName, ex.InnerException);
                    } catch (Exception ex) {
                        Log.Warning("Unknown exception encountered when trying to create post processor '{0}':\n{1}", type.FullName, ex);
                    }
                }

                postProcessors.Sort((x, y) => x.CallbackOrder.CompareTo(y.CallbackOrder));
                return postProcessors.ToArray();
            });
            foreach (var postProcessor in m_PostProcessors) {
                postProcessor.InitializeOnMainThread();
            }
#else
          m_PostProcessors = new IHotReloadProjectGenerationPostProcessor[0];
          //keep compiler happy
          await Task.CompletedTask;
#endif
        }

        public bool IsInternalizedPackagePath(string path) {
            if (string.IsNullOrWhiteSpace(path)) {
                return false;
            }

            var packageInfo = FindForAssetPath(path);
            if (packageInfo == null) {
                return false;
            }

            var packageSource = packageInfo.source;
            switch (packageSource) {
                case PackageSource.Embedded:
                case PackageSource.Local:
                    return false;
                default:
                    return true;
            }
        }
    }
}
