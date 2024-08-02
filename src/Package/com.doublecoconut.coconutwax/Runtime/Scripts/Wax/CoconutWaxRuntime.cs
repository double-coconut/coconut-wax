using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wax
{
    /// <summary>
    /// The CustomUserAgent class used to store a custom user agent for a specific platform.
    /// </summary>
    [Serializable]
    public struct CustomUserAgent
    {
        [field: SerializeField] public string UserAgent { get; private set; }
        [field: SerializeField] public RuntimePlatform Platform { get; private set; }

        public bool IsValid => !string.IsNullOrEmpty(UserAgent);
    }

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
        /// Enable user custom agent for the CoconutWax instance.
        /// </summary>
        [SerializeField] private bool useCustomUserAgent;

        /// <summary>
        /// User agent for the CoconutWax instance.
        /// </summary>
        [SerializeField] private List<CustomUserAgent> customUserAgents;

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
            if (useCustomUserAgent)
            {
                CustomUserAgent userAgent = customUserAgents.Find(x => x.Platform == Application.platform);
                if (userAgent.IsValid)
                {
                    CoconutWax = new CoconutWax(port, userAgent.UserAgent);
                    return;
                }
            }

            CoconutWax = new CoconutWax(port);
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