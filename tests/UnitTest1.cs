namespace tests;
using cli_life;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Program.Reset();
        Board result = Program.board;
        Board expected = new Board(50, 20, 1, 0.5);
        Assert.Equal(expected.Height, result.Height);
        Assert.Equal(expected.CellSize, result.CellSize);
        Assert.Equal(expected.Columns, result.Columns);
        Assert.Equal(expected.Rows, result.Rows);
        Assert.Equal(expected.Width, result.Width);
    }
    [Fact]
    public void Test2()
    {
        Program.Reset();
        Program.load("out2.txt");
        Assert.Equal(Program.find("block"),2);
    }
    [Fact]
    public void Test3()
    {
        Program.Reset();
        Program.load("out2.txt");
        Assert.Equal(Program.find("box"),1);
    }
    [Fact]
    public void Test4()
    {
        Program.Reset();
        Program.load("out2.txt");
        Assert.Equal(Program.symmetry_horizontally(),false);
    }
    [Fact]
    public void Test5()
    {
        Program.Reset();
        Program.load("out2.txt");
        Assert.Equal(Program.symmetry_vertically(),false);
    }
}
