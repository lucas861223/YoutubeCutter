using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Collections.Generic;
namespace YoutubeCutter.Helpers
{
    class ViewModelResolver
    {
        private static Dictionary<string, ObservableObject> ViewModelDictionary = new Dictionary<string, ObservableObject>();

        public static ObservableObject? GetViewModel(string key)
        {
            if (ViewModelDictionary.ContainsKey(key))
            {
                return ViewModelDictionary[key];
            }
            return null;
        }
        public static void AddViewModel(string key, ObservableObject viewModel)
        {
            ViewModelDictionary.Add(key, viewModel);
        }
        public static void RemoveViewModel(string key)
        {
            ViewModelDictionary.Remove(key);
        }
    }
}
