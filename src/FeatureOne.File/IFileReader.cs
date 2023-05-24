using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace FeatureOne.File
{
    internal interface IFileReader
    {
        FileState Read();
    }
}