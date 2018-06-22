using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Workflow.ComponentModel.Serialization;

[assembly: AssemblyTitle("Commerce Library")]
[assembly: AssemblyDescription("Contains Commerce APIs")]
[assembly: AssemblyVersion("5.0.*")]
[assembly: XmlnsDefinition("http://schemas.mediachase.com/ecf/50/marketing", "Mediachase.Commerce.Marketing")]
[assembly: XmlnsDefinition("http://schemas.mediachase.com/ecf/50/marketing/objects", "Mediachase.Commerce.Marketing.Objects")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Mediachase.Commerce.UnitTests")]