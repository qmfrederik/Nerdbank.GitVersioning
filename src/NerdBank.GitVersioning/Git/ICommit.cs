using System;
using System.Collections.Generic;
using System.IO;

namespace NerdBank.GitVersioning.Git
{
    public interface ICommit
    {
        string Sha { get; }
        byte[] RawId { get; }
        DateTimeOffset When { get; }
        IGitRepository Repository { get; }
        IEnumerable<ICommit> Parents { get; }

        Stream GetBlobStream(string path);
        bool IsRelevant(Func<IEnumerable<LibGit2Sharp.TreeEntryChanges>, bool> containsRelevantChanges, List<string> diffInclude);
    }
}
