using System;

namespace NerdBank.GitVersioning.Git
{
    public interface IGitRepository : IDisposable
    {
        string WorkingDirectory { get; }
        string Path { get; }
        ICommit Head { get; }
        string HeadCanonicalName { get; }
        bool IsBare { get; }

        string ShortenObjectId(ICommit commit, int minLength);
        T? GetConfig<T>(string key) where T : struct;
    }
}
