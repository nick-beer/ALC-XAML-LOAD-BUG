using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Markup;
using System.Xml;

namespace XamlLoad
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var forceOtherAssemblyIntoDefaultContext = typeof(Library.Properties) != null;
            var onlyIsolateOurAssemblies = false;

            var crash = forceOtherAssemblyIntoDefaultContext && !onlyIsolateOurAssemblies;
            var cxt = crash
                ? (AssemblyLoadContext)new LoadAllAssembliesContext()
                : new LoadOnlyOurAssembliesContext();

            var assembly = cxt.LoadFromAssemblyPath(typeof(Program).Assembly.Location);
            var type = assembly.GetType(typeof(Program).FullName);
            type.GetMethod(nameof(Program.DoXamlLoad)).Invoke(null, null);
        }

        public static void DoXamlLoad()
        {
            var xaml = @"
<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:p=""clr-namespace:Library;assembly=Library"">

    <Style x:Key=""ButtonWithName"" TargetType=""{x:Type Button}"">
        <Setter Property=""p:Properties.ButtonName"" Value=""Frank""/>
    </Style>

</ResourceDictionary>
";
            var reader = new StringReader(xaml);
            var xml = XmlReader.Create(reader);
            XamlReader.Load(xml);
        }
    }

    public sealed class LoadAllAssembliesContext : AssemblyLoadContext
    {
        public LoadAllAssembliesContext() { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            return LoadFromAssemblyPath(assembly.Location);
        }
    }

    public sealed class LoadOnlyOurAssembliesContext : AssemblyLoadContext
    {
        public LoadOnlyOurAssembliesContext() { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == "XamlLoad" || assemblyName.Name == "Library")
            {
                var assemblyInDefaultContext = Assembly.Load(assemblyName);
                var assemblyInThisContext = LoadFromAssemblyPath(assemblyInDefaultContext.Location);
                return assemblyInThisContext;
            }
            return null;
        }
    }
}