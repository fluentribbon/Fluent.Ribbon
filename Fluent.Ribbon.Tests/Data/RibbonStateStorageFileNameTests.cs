namespace Fluent.Tests.Data;

using System;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

/// <summary>
/// The saved state of a ribbon is stored in a file whose name contains a hash of the window and ribbon names.
/// </summary>
[TestFixture]
public class RibbonStateStorageFileNameTests
{
    /// <summary>
    /// Guard: the file name is the same as in earlier versions, so a state saved by an earlier version is still found.
    /// </summary>
    [TestCase("", "Fluent.Ribbon.State.D98C1DD4")]
    [TestCase("MainRibbon", "Fluent.Ribbon.State.4498E39")]
    public void File_name_is_the_same_as_in_earlier_versions(string ribbonName, string earlierFileName)
    {
        var ribbon = new Ribbon { Name = ribbonName };
        var storage = new FileNameStorage(ribbon);

        var stringForHash = ribbonName.Length > 0 ? "." + ribbonName : string.Empty;

        using (var md5 = MD5.Create())
        {
            var expected = "Fluent.Ribbon.State." + BitConverter.ToInt32(md5.ComputeHash(Encoding.Default.GetBytes(stringForHash)), 0).ToString("X");

            Assert.That(storage.FileName, Is.EqualTo(expected));
            Assert.That(storage.FileName, Is.EqualTo(earlierFileName), "The name an earlier version gave this ribbon.");
        }
    }

    /// <summary>
    /// The hash used when Windows only allows FIPS validated algorithms (MD5 can't be created then on .NET Framework)
    /// must give the same name on every run, otherwise the saved state would never be found again.
    /// </summary>
    [Test]
    public void Fallback_hash_is_stable()
    {
        // Known FNV-1a values.
        Assert.That(RibbonStateStorage.GetStableHash(Array.Empty<byte>()), Is.EqualTo(unchecked((int)0x811C9DC5)));
        Assert.That(RibbonStateStorage.GetStableHash(Encoding.ASCII.GetBytes("a")), Is.EqualTo(unchecked((int)0xE40C292C)));
        Assert.That(RibbonStateStorage.GetStableHash(Encoding.ASCII.GetBytes("foobar")), Is.EqualTo(unchecked((int)0xBF9CF968)));
    }

    [Test]
    public void Fallback_hash_differs_for_different_names()
    {
        var first = RibbonStateStorage.GetStableHash(Encoding.Default.GetBytes(".Fluent.Tests.MainWindow.Ribbon1"));
        var second = RibbonStateStorage.GetStableHash(Encoding.Default.GetBytes(".Fluent.Tests.MainWindow.Ribbon2"));

        Assert.That(first, Is.Not.EqualTo(second));
    }

    private sealed class FileNameStorage : RibbonStateStorage
    {
        public FileNameStorage(Ribbon ribbon)
            : base(ribbon)
        {
        }

        public string FileName => this.IsolatedStorageFileName;
    }
}
