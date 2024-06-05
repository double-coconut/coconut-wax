using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace CoconutWax.Editor.Scripts
{
    public static class PackagePathLocator
    {
        private static ListRequest _listRequest;

        private static string _packageBasePath;
        private static TaskCompletionSource<string> _tcs;

        public static string PackageBasePath => _packageBasePath;

        public static async Task<string> RequestPackagePath(string packageName)
        {
            if (_tcs != null)
            {
                if (string.IsNullOrEmpty(_packageBasePath))
                {
                    _tcs = null;
                }
                else
                {
                    return await _tcs.Task;
                }
            }

            if (!string.IsNullOrEmpty(_packageBasePath))
            {
                return _packageBasePath;
            }

            _tcs = new TaskCompletionSource<string>();

            _listRequest = Client.List();
            while (!_listRequest.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
            }


            if (_listRequest.Status != StatusCode.Success)
            {
                _tcs.SetException(new Exception("Failed to locate package path."));
                return await _tcs.Task;
            }

            PackageInfo packageInfo =
                _listRequest.Result
                    .SingleOrDefault(info => info.name == packageName);

            if (packageInfo != null)
            {
                _packageBasePath = packageInfo.resolvedPath;
                _tcs.SetResult(_packageBasePath);
                return await _tcs.Task;
            }

            _tcs.SetException(new Exception("Unable to find package."));
            return await _tcs.Task;
        }
    }
}