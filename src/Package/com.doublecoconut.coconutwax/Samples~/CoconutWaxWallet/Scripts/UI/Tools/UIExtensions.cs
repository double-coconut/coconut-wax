using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;

namespace Samples.CoconutWaxWallet.Scripts.UI.Tools
{
    /// <summary>
    /// Provides extension methods for the IInitializationArgs interface.
    /// </summary>
    public static class UIExtensions
    {
        /// <summary>
        /// Casts the IInitializationArgs instance to the specified type.
        /// </summary>
        /// <typeparam name="T">The target type to cast to. Must be a class that implements IInitializationArgs.</typeparam>
        /// <param name="args">The IInitializationArgs instance to cast.</param>
        /// <returns>The casted instance if the cast is successful, otherwise null.</returns>
        public static T CastTo<T>(this IInitializationArgs args) where T : class, IInitializationArgs
        {
            return args as T;
        }
    }
}