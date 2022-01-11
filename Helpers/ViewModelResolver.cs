using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Helpers
{
    public static 
    class ViewModelResolver
    {
        private static Dictionary<string, ObservableObject> ViewModelDictionary = new Dictionary<string, ObservableObject>();

        public static ObservableObject? GetViewModel(string key)
        {
            if (ViewModelResolver.ViewModelDictionary.ContainsKey(key)) {
                return ViewModelResolver.ViewModelDictionary[key];
            }
            return null;
        }

        public static void AddViewModel(string key, ObservableObject viewModel)
        {
            ViewModelResolver.ViewModelDictionary.Add(key, viewModel);
        }

        public static void RemoveViewModel(string key)
        {
            ViewModelResolver.ViewModelDictionary.Remove(key);
        }
    }
}
