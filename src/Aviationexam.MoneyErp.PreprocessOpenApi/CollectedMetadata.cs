using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public sealed class CollectedMetadata
{
    private Version? _targetApiVersion;
    private readonly Dictionary<string, ICollection<string>> _knownPaths = [];
    private readonly HashSet<(string Path, string Method)> _ignoredPathMethods = [];

    public void AddVersion(ReadOnlySpan<byte> target)
    {
        var versionString = Encoding.UTF8.GetString(target);

        if (versionString.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        {
            versionString = versionString[1..];
        }

        _targetApiVersion = Version.Parse(versionString);
    }

    public Version? GetVersion() => _targetApiVersion;

    public bool IsPathIgnored(
        ReadOnlySpan<byte> path
    )
    {
        var pathString = Encoding.UTF8.GetString(path);

        return _knownPaths[pathString].All(x => _ignoredPathMethods.Contains((pathString, x)));
    }

    public bool IsPathMethodIgnored(
        string path,
        ReadOnlySpan<byte> methodName
    ) => IsPathMethodIgnored(path, Encoding.UTF8.GetString(methodName));

    public bool IsPathMethodIgnored(
        string path,
        string methodName
    ) => _ignoredPathMethods.Contains((path, methodName));

    public void AddKnownPathMethod(string path, string methodName)
    {
        if (!_knownPaths.TryAdd(path, [methodName]))
        {
            _knownPaths[path].Add(methodName);
        }
    }

    public void SkipPathMethod(string path, string methodName)
    {
        _ignoredPathMethods.Add((path, methodName));
    }
}
