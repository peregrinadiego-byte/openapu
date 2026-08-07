using OpenAPU.Api.Configuration;

namespace OpenAPU.Api.Tests;

public sealed class DatabaseStartupValidatorTests
{
    [Fact]
    public void Writable_directory_is_accepted()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"openapu-startup-{Guid.NewGuid():N}");

        var databasePath = Path.Combine(
            directory,
            "openapu.db");

        try
        {
            var result =
                DatabaseStartupValidator.Validate(databasePath);

            Assert.True(result.DirectoryExists);
            Assert.True(result.DirectoryWritable);
            Assert.Equal(
                Path.GetFullPath(databasePath),
                result.Path);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
