using UnityEngine;

namespace Samples.CoconutWaxWallet.Scripts.UI.Abstraction
{
    /// <summary>
    /// The AbstractView class provides a base for all view classes in the application.
    /// It extends the MonoBehaviour class, allowing it to be attached to a GameObject in a Unity scene.
    /// It also implements the IView interface.
    /// </summary>
    public abstract class AbstractView : MonoBehaviour, IView
    {
        /// <summary>
        /// Initializes the view with the given initialization arguments.
        /// This method can be overridden by subclasses to provide custom initialization logic.
        /// </summary>
        public virtual void Initialize(IInitializationArgs args)
        {
        }
        /// <summary>
        /// Closes the view and destroys the associated GameObject.
        /// </summary>
        public void Close()
        {
            InternalClose();
            Destroy(gameObject);
        }
        /// <summary>
        /// Provides a hook for subclasses to add custom logic when the view is closed.
        /// This method is called by the Close method.
        /// </summary>
        protected virtual void InternalClose()
        {
        }
    }
}