using System.IO;
using JavaCompiler;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnitTests;

namespace UnitTests
{
    public class Tests
    {
        private Scanner _scanner;
        [SetUp]
        public void Setup()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "prog2.java");
            _scanner = new Scanner(path);
        }

        [Test]
        public void Test1()
        {
            _scanner.GetNextToken();
        }
}