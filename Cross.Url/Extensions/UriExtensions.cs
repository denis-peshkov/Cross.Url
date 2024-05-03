namespace Cross.Url.Extensions;

public static class UriExtensions
{
    public static Uri Combine(this Uri path, string[] paths)
    {
        var t = new UriBuilder(path);

        var basePath = t.Path.RemoveLastSlash();

        foreach (var p in paths)
        {
            var partPath = p;
            partPath = partPath
                .ReplaceBackSlashToForwardSlash()
                .RemoveLastSlash();

            if (partPath.Length != 0)
            {
                basePath = partPath.StartsWith("/")
                    ? basePath + partPath
                    : basePath + "/" + partPath;
            }
        }

        var pos = basePath.IndexOf('?');
        if (pos != -1)
        {
            var query = basePath.Substring(pos);
            t.Query = query;
            basePath = basePath.Remove(pos);
        }

        t.Path = basePath;

        return t.Uri;
    }

    public static Uri? Combine(this Uri? path, string path2)
    {
        if (path == null && string.IsNullOrEmpty(path2))
        {
            throw new ArgumentException("Not possible to create Uri with provided params!");
        }

        if (path == null && path2.StartsWith("http"))
        {
            return new Uri(path2, UriKind.Absolute);
        }

        return path?.Combine(new[] { path2 });
    }

    public static Uri Combine(this Uri path, Uri[] paths)
        => path.Combine(paths.Select(x => x.LocalPath).ToArray());

    public static Uri Combine(this Uri path, Uri path2)
        => path.Combine(path2.LocalPath);

    public static Uri Combine(this string path, string path2)
        => new Uri(path).Combine(path2);

    public static string RemoveLastSlash(this string path)
        => path.EndsWith('/') ? path.Remove(path.Length - 1, 1) : path;

    public static Uri? CreateUri(this string? uri)
        => string.IsNullOrEmpty(uri) ? null : new System.Uri(uri, UriKind.RelativeOrAbsolute);

    public static string ReplaceBackSlashToForwardSlash(this string path)
        => path.Replace("\\", "/");

    public static string RemoveLeadingSlash(this string path)
        => path.StartsWith("/") ? path.Substring(1) : path;

    public static string NormalizePath(this string path)
        => path.ReplaceBackSlashToForwardSlash().RemoveLeadingSlash();

    public static string SanitizeName(this string path)
    {
        var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)|\s+", invalidChars);
        var parts = path.ReplaceBackSlashToForwardSlash().Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts is { Length: > 0 })
        {
            parts[^1] = Regex.Replace(parts[^1], invalidRegStr, "_");
        }

        return Path.Combine(parts);
    }
}
