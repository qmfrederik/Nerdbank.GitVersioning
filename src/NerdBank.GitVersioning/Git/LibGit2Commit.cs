using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nerdbank.GitVersioning;

namespace NerdBank.GitVersioning.Git
{
    public class LibGit2Commit : ICommit
    {
        private readonly LibGit2Sharp.Commit commit;

        public LibGit2Commit(LibGit2Sharp.Commit commit, IGitRepository repository)
        {
            this.commit = commit ?? throw new ArgumentNullException(nameof(commit));
            this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public static ICommit Create(LibGit2Sharp.Commit commit, IGitRepository repository)
        {
            if (commit == null)
            {
                return null;
            }

            return new LibGit2Commit(commit, repository);
        }

        public LibGit2Sharp.Commit Commit => this.commit;

        public Stream GetBlobStream(string path)
        {
            var blob = this.commit.Tree[path]?.Target as LibGit2Sharp.Blob;
            return blob?.GetContentStream();
        }

        public string Sha => this.commit.Sha;
        public byte[] RawId => this.commit.Id.RawId;

        public DateTimeOffset When => this.commit.Author.When;

        public IGitRepository Repository { get; }

        public IEnumerable<ICommit> Parents
        {
            get
            {
                foreach (var parent in this.commit.Parents)
                {
                    yield return new LibGit2Commit(parent, this.Repository);
                }
            }
        }

        public bool IsRelevant(Func<IEnumerable<LibGit2Sharp.TreeEntryChanges>, bool> containsRelevantChanges, List<string> diffInclude)
        {
            return this.commit.Parents.Any()
                ? this.commit.Parents.Any(parent => containsRelevantChanges(this.commit.GetRepository().Diff
                    .Compare<LibGit2Sharp.TreeChanges>(parent.Tree, this.commit.Tree, diffInclude)))
                : containsRelevantChanges(this.commit.GetRepository().Diff
                    .Compare<LibGit2Sharp.TreeChanges>(null, this.commit.Tree, diffInclude));
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var commit = obj as ICommit;

            if (commit != null)
            {
                return string.Equals(commit.Sha, this.Sha, StringComparison.OrdinalIgnoreCase);
            }

            var nativeCommit = obj as LibGit2Sharp.Commit;

            if (nativeCommit != null)
            {
                return string.Equals(nativeCommit.Sha, this.Sha, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
