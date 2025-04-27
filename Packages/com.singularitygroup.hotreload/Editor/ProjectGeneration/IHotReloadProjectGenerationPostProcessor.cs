namespace SingularityGroup.HotReload.Editor.ProjectGeneration {
    /// <summary>
    /// Allows to post process Hot Reload's project generation.
    /// This should only be needed if you tinker with Unity's project generation as well.
    /// Types that inherit from this interface will get created automatically whenever Hot Reload generates project files.
    /// Types that implement this interface need to have a public parameterless default constructor.
    /// </summary>
    public interface IHotReloadProjectGenerationPostProcessor {
        
        /// <summary>
        /// Specifies the ordering of the post processor.
        /// Post processors with lower callback order get executed first.
        /// </summary>
        int CallbackOrder { get; }
        
        /// <summary>
        /// Use this method to set up state you need for the project generation.
        /// Calls to unity API need to happen here and it's values need to be cached.
        /// This is the only method that will get executed on the main thread.
        /// </summary>
        void InitializeOnMainThread();
        
        /// <summary>
        /// Gets called whenever Hot Reload generated a project file.
        /// <param name="path">The destination file path for the .csproj file</param>
        /// <param name="contents">The file contents of the .csproj file</param>
        /// </summary>
        string OnGeneratedCSProjectThreaded(string path, string contents);
        
        /// <summary>
        /// Gets called whenever Hot Reload generated a solution file.
        /// <param name="path">The destination file path for the .sln file</param>
        /// <param name="contents">The file contents of the .sln file</param>
        /// </summary>
        string OnGeneratedSlnSolutionThreaded(string path, string contents);
        
        /// <summary>
        /// Gets called after Hot Reload project generation is finished.
        /// </summary>
        void OnGeneratedCSProjectFilesThreaded();
    }
}

