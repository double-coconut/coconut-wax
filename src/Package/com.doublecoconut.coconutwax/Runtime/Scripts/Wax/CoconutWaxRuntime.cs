using UnityEngine;

namespace Wax
{
    /// <summary>
    /// The CoconutWaxRuntime class is a MonoBehaviour that manages a CoconutWax instance.
    /// </summary>
    public class CoconutWaxRuntime : MonoBehaviour
    {
        /// <summary>
        /// Port number for the CoconutWax instance.
        /// </summary>
        [SerializeField] private uint port = 2023;

        /// <summary>
        /// User agent for the CoconutWax instance.
        /// </summary>
        [SerializeField] private string userAgent = CoconutWax.DefaultUserAgent;

        /// <summary>
        /// The CoconutWax instance managed by this MonoBehaviour.
        /// </summary>
        public CoconutWax CoconutWax { get; private set; }

        /// <summary>
        /// Singleton instance of the CoconutWaxRuntime.
        /// </summary>
        private static CoconutWaxRuntime _instance;

        /// <summary>
        /// Accessor for the singleton instance.
        /// </summary>
        public static CoconutWaxRuntime Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CoconutWaxRuntime>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// On start, a new CoconutWax instance is created.
        /// </summary>
        private void Start()
        {
            CoconutWax = new CoconutWax(port, userAgent);
        }

        /// <summary>
        /// On destruction, the CoconutWax instance is disposed of.
        /// </summary>
        private void OnDestroy()
        {
            CoconutWax?.Dispose();
        }
    }
}