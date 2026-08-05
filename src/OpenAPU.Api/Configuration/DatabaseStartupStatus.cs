namespace OpenAPU.Api.Configuration;

public sealed record DatabaseStartupStatus(
    string Path,
    string Directory,
    bool DirectoryExists,
    bool DirectoryWritable);
