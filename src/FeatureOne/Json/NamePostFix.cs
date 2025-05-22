using System;

namespace FeatureOne.Json
{
    public class NamePostFix
    {
        public string Name { get; private set; }

        public NamePostFix(string name, params string[] postFix)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var names = name.Split(postFix, StringSplitOptions.RemoveEmptyEntries);
            Name = names.Length >= 1
                ? $"{names[0]}{postFix[0]}" : $"{name}{postFix[0]}";
        }
    }
}