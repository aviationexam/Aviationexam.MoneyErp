using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aviationexam.MoneyErp.Tests.Infrastructure;

public static class Loader
{
    public static void LoadEnvFile(
        string envFileName, [CallerFilePath] string filePath = ""
    )
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        FileInfo fileInfo = new(filePath);
        var envFileFullPath = Path.Join(fileInfo.Directory!.Parent!.Parent!.FullName, envFileName);

        if (!File.Exists(envFileFullPath))
        {
            return;
        }

        foreach (var line in File.ReadAllLines(envFileFullPath))
        {
            var parts = line.Split(
                '=',
                2,
                StringSplitOptions.RemoveEmptyEntries
            );

            Environment.SetEnvironmentVariable(parts[0], parts.Length > 1 ? parts[1] : null, EnvironmentVariableTarget.Process);
        }
    }
}
