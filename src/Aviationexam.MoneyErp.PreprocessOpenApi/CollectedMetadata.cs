using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using ZLinq;
using ParameterType = (string Type, string? Format);

namespace Aviationexam.MoneyErp.PreprocessOpenApi;

public sealed partial class CollectedMetadata(
    ISet<string> ignoredPaths
)
{
    [GeneratedRegex(@"\{([^}]+)\}", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 100)]
    private static partial Regex UrlParamRegex();

    private Version? _targetApiVersion;
    private readonly Dictionary<string, ICollection<string>> _knownPaths = [];
    private readonly HashSet<(string Path, string Method)> _ignoredPathMethods = [];
    private readonly HashSet<PathParameter> _pathParameters = [];
    private readonly HashSet<(string pathName, string methodName, ISchema schema)> _paginatedResponse = [];

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

        if (ignoredPaths.Contains(pathString))
        {
            return true;
        }

        return _knownPaths[pathString].AsValueEnumerable().All(x => _ignoredPathMethods.Contains((pathString, x)));
    }

    public bool IsPathMethodIgnored(
        string path,
        ReadOnlySpan<byte> methodName
    ) => IsPathMethodIgnored(path, Encoding.UTF8.GetString(methodName));

    public bool IsPathMethodIgnored(
        string path,
        string methodName
    ) => _ignoredPathMethods.Contains((path, methodName));

    public bool ModifyPathParameterType(string pathName, string parameterName, [NotNullWhen(true)] out ParameterType? type)
    {
        var shortestPathWithParameter = GetShortestPathWithParameter(pathName, parameterName);

        Span<char> templatePathBuilder = stackalloc char[shortestPathWithParameter.Length + 10];

        GetTemplatePath(
            templatePathBuilder,
            shortestPathWithParameter,
            parameterName,
            out var currentParameterId
        );

        if (!currentParameterId.HasValue)
        {
            throw new Exception("Parameter ID is not set. This should never happen.");
        }

        var templatePath = templatePathBuilder.ToString();

        var pathParameters = _pathParameters.AsValueEnumerable().Where(x =>
            templatePath.StartsWith(x.TemplatePath)
            && x.ParameterId == currentParameterId
        ).ToList();

        if (pathParameters.Count > 1)
        {
            type = GetPathParameterType(pathParameters);
            return true;
        }

        type = null;
        return false;
    }

    private ParameterType GetPathParameterType(IEnumerable<PathParameter> pathParameters)
    {
        ParameterType? type = null;
        foreach (var pathParameter in pathParameters)
        {
            if (type is null)
            {
                type = (pathParameter.Type, pathParameter.Format);
                continue;
            }

            if (type.Value.Type != pathParameter.Type)
            {
                type = GetNewType(type.Value, pathParameter);
            }
        }

        if (type is not null)
        {
            return type.Value;
        }

        return (string.Empty, null);
    }

    private ParameterType GetNewType(ParameterType currentType, PathParameter anotherType)
    {
        if (currentType is { Type: "string", Format: null })
        {
            return currentType;
        }

        if (currentType is { Type: "string" } && anotherType is { Type: "integer" })
        {
            return currentType with { Format = null };
        }

        throw new NotSupportedException(
            $"Cannot merge path parameter types: '{currentType}' and '{anotherType}'."
        );
    }

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

    private string? _parameterName;
    private string? _parameterIn;
    private string? _parameterType;
    private string? _parameterFormat;

    public void AddParameterName(ReadOnlySpan<byte> value)
    {
        _parameterName = Encoding.UTF8.GetString(value);
    }

    public void AddParameterIn(ReadOnlySpan<byte> value)
    {
        _parameterIn = Encoding.UTF8.GetString(value);
    }

    public void AddParameterType(ReadOnlySpan<byte> value)
    {
        _parameterType = Encoding.UTF8.GetString(value);
    }

    public void AddParameterStringFormat(ReadOnlySpan<byte> value)
    {
        _parameterFormat = Encoding.UTF8.GetString(value);
    }

    public void CommitParameter(string pathName)
    {
        if (
            _parameterName is not null
            && _parameterIn is OpenApiPreprocessor.Path
            && _parameterType is not null
        )
        {
            var shortestPathWithParameter = GetShortestPathWithParameter(pathName, _parameterName);

            Span<char> templatePathBuilder = stackalloc char[shortestPathWithParameter.Length + 10];

            GetTemplatePath(
                templatePathBuilder,
                shortestPathWithParameter,
                _parameterName,
                out var currentParameterId
            );

            if (!currentParameterId.HasValue)
            {
                throw new Exception("Parameter ID is not set. This should never happen.");
            }

            var templatePath = templatePathBuilder.ToString();

            var pathParameter = new PathParameter(
                templatePath,
                pathName,
                currentParameterId.Value,
                _parameterName,
                _parameterType,
                _parameterFormat
            );

            var contains = _pathParameters.AsValueEnumerable().Any(x =>
                x.TemplatePath == templatePath
                && x.Name == pathParameter.Name
                && x.Type == pathParameter.Type
                && x.Format == pathParameter.Format
            );

            if (!contains)
            {
                _pathParameters.Add(pathParameter);
            }
        }

        _parameterName = null;
        _parameterIn = null;
        _parameterType = null;
        _parameterFormat = null;
    }

    private static void GetTemplatePath(
        Span<char> templatePath,
        ReadOnlySpan<char> pathName,
        string parameterName, out int? currentParameterId
    )
    {
        var lastMatch = 0;
        var writeCursor = 0;
        var parameterId = 0;
        currentParameterId = null;
        foreach (var match in UrlParamRegex().EnumerateMatches(pathName))
        {
            var literal = pathName.Slice(lastMatch, match.Index - lastMatch);
            var parameter = pathName.Slice(match.Index + 1, match.Length - 2);
            lastMatch = match.Index + match.Length;

            if (parameter.SequenceEqual(parameterName))
            {
                currentParameterId = parameterId;
            }

            literal.CopyTo(templatePath.Slice(writeCursor, literal.Length));
            writeCursor += literal.Length;
            templatePath[writeCursor] = '{';
            writeCursor++;
            parameterId.TryFormat(templatePath[writeCursor..], out var charsWritten);
            writeCursor += charsWritten;
            templatePath[writeCursor] = '}';
            writeCursor++;

            parameterId++;
        }
    }

    private ReadOnlySpan<char> GetShortestPathWithParameter(ReadOnlySpan<char> pathName, ReadOnlySpan<char> parameterName)
    {
        ReadOnlySpan<char> parameterPlaceholder =
        [
            '{',
            .. parameterName,
            '}',
        ];
        var idx = pathName.IndexOf(parameterPlaceholder, StringComparison.Ordinal);
        return idx >= 0 ? pathName[..(idx + parameterPlaceholder.Length)] : pathName;
    }

    public void AddPaginatedResponse(
        string pathName, string methodName, ReadOnlySpan<byte> itemsRef
    ) => _paginatedResponse.Add((pathName, methodName, new RefSchema(Encoding.UTF8.GetString(itemsRef))));

    public void AddPaginatedResponse(
        string pathName, string methodName, string type, string? format
    ) => _paginatedResponse.Add((pathName, methodName, new InlineSchema(type, format)));

    public bool IsPaginatedResponse(
        string pathName, string methodName, out ISchema? itemsRef
    )
    {
        itemsRef = _paginatedResponse
            .AsValueEnumerable()
            .Where(x => x.pathName == pathName && x.methodName == methodName)
            .Select(x => x.schema)
            .FirstOrDefault();

        return itemsRef is not null;
    }
}
