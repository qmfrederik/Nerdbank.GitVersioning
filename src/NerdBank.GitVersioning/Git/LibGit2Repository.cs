using System;

namespace NerdBank.GitVersioning.Git
{
    public class LibGit2Repository : IGitRepository
    {
        private readonly LibGit2Sharp.Repository repository;

        public LibGit2Repository(LibGit2Sharp.Repository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public static LibGit2Repository Create(LibGit2Sharp.Repository repository)
        {
            if (repository == null)
            {
                return null;
            }

            return new LibGit2Repository(repository);
        }

        public bool IsBare => this.repository.Info.IsBare;

        public string WorkingDirectory => this.repository.Info?.WorkingDirectory;

        public string Path => this.repository.Info?.Path;

        public string HeadCanonicalName => this.repository.Head.CanonicalName;

        public ICommit Head => LibGit2Commit.Create(this.repository.Head.Tip, this);

        public T? GetConfig<T>(string key) where T : struct
        {
            return this.repository.Config.Get<T>(key)?.Value;
        }

        public string ShortenObjectId(ICommit commit, int minLength)
        {
            return this.repository.ObjectDatabase.ShortenObjectId(((LibGit2Commit)commit).Commit, minLength);
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}
