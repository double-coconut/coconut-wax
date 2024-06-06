namespace Server.FileSystem
{
    public static class FileHandlerFactory
    {
        /// <summary>
        /// Creates an instance of IFileHandler.
        /// </summary>
        /// <returns>An instance of IFileHandler.</returns>
        public static IFileHandler CreateFileHandler()
        {
#if UNITY_EDITOR
            return new FileSystemFileHandler();
#elif UNITY_ANDROID || UNITY_WEBGL
            return new UnityWebRequestFileHandler();
#else
            return new FileSystemFileHandler();
#endif
        }
    }
}