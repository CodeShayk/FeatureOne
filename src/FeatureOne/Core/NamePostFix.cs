namespace FeatureOne.Core
{
    public class NamePostFix
    {
        public string Name { get; private set; }
  
        public NamePostFix(string name, string postFix)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var names = name.Split(postFix);
            Name = names.Length >= 1
                ? $"{names[0]}{postFix}" : $"{name}{postFix}";
        }
    }
}