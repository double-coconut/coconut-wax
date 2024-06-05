using System;
using System.Collections.Generic;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using Samples.CoconutWaxWallet.Scripts.UI.Popup;
using Samples.CoconutWaxWallet.Scripts.UI.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Samples.CoconutWaxWallet.Scripts.UI
{
    /// <summary>
    /// The UIManager class is used to manage the user interface (UI) of the application.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// The base path for loading view prefabs from resources.
        /// </summary>
        [SerializeField] private string viewBasePath = "UI/Views";
        /// <summary>
        /// The base path for loading popup prefabs from resources.
        /// </summary>
        [SerializeField] private string popupBasePath = "UI/Popups";
        /// <summary>
        /// The container for displaying views.
        /// </summary>
        [SerializeField] private RectTransform viewContainer;
        /// <summary>
        /// The container for displaying popups.
        /// </summary>
        [SerializeField] private RectTransform popupContainer;

        /// <summary>
        /// Singleton instance of the UIManager.
        /// </summary>
        private static UIManager _instance;
        /// <summary>
        /// Property to access the singleton instance of the UIManager.
        /// </summary>
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// The current view being displayed.
        /// </summary>
        private IView _currentView;
        /// <summary>
        /// Stack of popups being displayed.
        /// </summary>
        private readonly Stack<IView> _popupStack = new Stack<IView>();
        /// <summary>
        /// Dictionary mapping types of views and popups to their corresponding creation methods.
        /// </summary>
        private readonly IReadOnlyDictionary<Type, Func<string, RectTransform, IInitializationArgs, IView>> _views =
            new Dictionary<Type, Func<string, RectTransform, IInitializationArgs, IView>>
            {
                { typeof(AuthenticationView), CreateView<AuthenticationView> },
                { typeof(ProfileView), CreateView<ProfileView> },
                { typeof(TransferNFTPopup), CreatePopup<TransferNFTPopup> },
                { typeof(TransferTokenPopup), CreatePopup<TransferTokenPopup> },
                { typeof(TokenSettingsPopup), CreatePopup<TokenSettingsPopup> },
                { typeof(AlertPopup), CreatePopup<AlertPopup> },
            };
        /// <summary>
        /// Method to create a specific view
        /// </summary>
        private static TView CreateView<TView>(string basePath, RectTransform parent, IInitializationArgs args)
            where TView : Object, IView
        {
            TView prefab = Resources.Load<TView>($"{basePath}/{typeof(TView).Name}");
            TView view = Instantiate(prefab, parent);
            view.Initialize(args);
            return view;
        }
        /// <summary>
        /// Method to create a specific popup
        /// </summary>
        private static TPopup CreatePopup<TPopup>(string basePath, RectTransform parent, IInitializationArgs args)
            where TPopup : Object, IView
        {
            TPopup prefab = Resources.Load<TPopup>($"{basePath}/{typeof(TPopup).Name}");
            TPopup view = Instantiate(prefab, parent);
            view.Initialize(args);
            return view;
        }
        /// <summary>
        /// Displays a specific view.
        /// </summary>

        private TView ShowInternalView<TView>(IInitializationArgs args) where TView : Object, IView
        {
            Type type = typeof(TView);
            if (!_views.TryGetValue(type, out var view))
            {
                throw new Exception("View not found");
            }

            return view(viewBasePath, viewContainer, args) as TView;
        }
        /// <summary>
        /// Displays a specific popup.
        /// </summary>
        private TPopup ShowInternalPopup<TPopup>(IInitializationArgs args) where TPopup : Object, IView
        {
            Type type = typeof(TPopup);
            if (!_views.TryGetValue(type, out var popup))
            {
                throw new Exception("Popup not found");
            }

            return popup(popupBasePath, popupContainer, args) as TPopup;
        }

        /// <summary>
        /// Method to display a specific view, closing the current view if one exists.
        /// </summary>
        public TView ShowView<TView>(IInitializationArgs args = default) where TView : Object, IView
        {
            CloseView();
            TView view = ShowInternalView<TView>(args);
            _currentView = view;
            return view;
        }
        /// <summary>
        /// Method to display a specific popup, adding it to the popup stack.
        /// </summary>
        public TPopup ShowPopup<TPopup>(IInitializationArgs args = default) where TPopup : Object, IView
        {
            TPopup popup = ShowInternalPopup<TPopup>(args);
            _popupStack.Push(popup);
            return popup;
        }
        /// <summary>
        /// Method to close the current view.
        /// </summary>
        public void CloseView()
        {
            _currentView?.Close();
            _currentView = null;
        }

        /// <summary>
        /// Method to close the topmost popup in the stack.
        /// </summary>
        public void ClosePopup()
        {
            if (_popupStack.Count == 0) return;
            _popupStack.Pop().Close();
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}