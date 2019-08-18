using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Markup;
using System.Xml;
using Library;

namespace XamlLoad
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            // force the Library assembly into the current context/app domain;
            Debug.Assert(typeof(Properties) != null);

            var cxt = new LoadAllAssembliesContext();
            var assembly = cxt.LoadFromAssemblyPath(typeof(Program).Assembly.Location);
            var type = assembly.GetType(typeof(Program).FullName);
            using (cxt.EnterContextualReflection())
            {
                type.GetMethod(nameof(Program.DoXamlLoad)).Invoke(null, null);
            }
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
            var assemblyInDefaultContext = Default.LoadFromAssemblyName(assemblyName);
            var location = assemblyInDefaultContext.Location;
            var assemblyInThisContext = LoadFromAssemblyPath(location);
            return assemblyInThisContext;
        }
    }
}