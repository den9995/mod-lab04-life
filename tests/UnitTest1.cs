using cli_life;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test1()
        {
            Program.Reset();
            Board result = Program.board;
            Board expected = new Board(50, 20, 1, 0.5);
            Assert.AreEqual(expected.Height, result.Height);
            Assert.AreEqual(expected.CellSize, result.CellSize);
            Assert.AreEqual(expected.Columns, result.Columns);
            Assert.AreEqual(expected.Rows, result.Rows);
            Assert.AreEqual(expected.Width, result.Width);
        }
        [TestMethod]
        public void Test2()
        {
            Program.Reset();
            Program.load("out2.txt");
            Assert.AreEqual(Program.find("block"),2);
        }
        [TestMethod]
        public void Test3()
        {
            Program.Reset();
            Program.load("out2.txt");
            Assert.AreEqual(Program.find("box"),1);
        }
        [TestMethod]
        public void Test4()
        {
            Program.Reset();
            Program.load("out2.txt");
            Assert.AreEqual(Program.symmetry_horizontally(),false);
        }
        [TestMethod]
        public void Test5()
        {
            Program.Reset();
            Program.load("out2.txt");
            Assert.AreEqual(Program.symmetry_vertically(),false);
        }
    }
}
