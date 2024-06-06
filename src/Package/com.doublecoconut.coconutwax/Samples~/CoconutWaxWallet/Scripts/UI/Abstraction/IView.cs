namespace Samples.CoconutWaxWallet.Scripts.UI.Abstraction
{
    /// <summary>
    /// The IView interface provides a contract for all view classes in the application.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Initializes the view with the given initialization arguments.
        /// </summary>
        void Initialize(IInitializationArgs args);
        /// <summary>
        /// Closes the view.
        /// </summary>
        void Close();
    }
}