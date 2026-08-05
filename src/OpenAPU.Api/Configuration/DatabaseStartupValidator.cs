namespace OpenAPU.Api.Configuration;

public static class DatabaseStartupValidator
{
    public static DatabaseStartupStatus Validate(
        string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new InvalidOperationException(
                "Database path is required.");
        }

        var fullPath = Path.GetFullPath(databasePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                "Database directory could not be resolved.");
        }

        Directory.CreateDirectory(directory);

        var probePath = Path.Combine(
            directory,
            $".openapu-write-{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllText(probePath, "openapu");
            File.Delete(probePath);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"Database directory is not writable: {directory}",
                exception);
        }

        return new DatabaseStartupStatus(
            fullPath,
            directory,
            Directory.Exists(directory),
            true);
    }
}
