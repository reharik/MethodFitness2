namespace ConcatAndCompressTester
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ConcatAndCompress;
    using NUnit.Framework;
    using MethodFitness.Tests;
    using System.Linq;

    public class GetFIlesTester
    {
    }

    [TestFixture]
    public class when_recursing_directories
    {
        private ConcatenateFiles _SUT;
        private List<FileInfo> _result;
        private DirectoryInfo _rootDir  ;

        [SetUp]
        public void Setup()
        {
            _SUT = new ConcatenateFiles();
            _rootDir = _SUT.GetRootDir();
        }

        [Test]
        public void should_return_fileInofs_for_root()
        {
            _result.FirstOrDefault(x=>x.Name=="MF.App.js").ShouldNotBeNull();
        }

        [Test]
        public void should_return_some_of_the_files_down_at_the_base()
        {
            _result.FirstOrDefault(x => x.Name == "topLeft.js").ShouldNotBeNull();
        }

    }
}