using Cupboard;

namespace Sandbox
{
    public sealed class Chocolatey : Manifest
    {
        public override void Execute(ManifestContext context)
        {
            // Do not run this in sandbox
            if (context.Facts["windows"]["sandbox"])
            {
                return;
            }

            // Download
            context.Resource<Download>("https://chocolatey.org/install.ps1")
                .ToFile("~/install-chocolatey.ps1");

            // Set execution policy
            context.Resource<RegistryKey>("Set execution policy")
                .Path(@"HKLM:\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell\ExecutionPolicy")
                .Value("Unrestricted", RegistryKeyValueKind.String);

            // Install
            context.Resource<PowerShell>("Install Chocolatey")
                .Script("~/install-chocolatey.ps1")
                .Flavor(PowerShellFlavor.PowerShell)
                .RequireAdministrator()
                .Unless("if (Test-Path \"$($env:ProgramData)/chocolatey/choco.exe\") { exit 1 }")
                .After<RegistryKey>("Set execution policy")
                .After<Download>("https://chocolatey.org/install.ps1");
        }
    }
}
