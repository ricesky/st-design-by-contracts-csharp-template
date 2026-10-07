using Contracts.Domain;
using NUnit.Framework;
namespace Contracts.Domain.Tests;
[TestFixture]
public class EquipmentLoanTests
{
    private static IEquipmentLoan Create(int total) => new EquipmentLoan(total);

    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void Constructor_Positive_SetsInitialState(int total)
    {
        State(Create(total), total, 0);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void Constructor_NotPositive_Throws(int total)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Create(total));
        Assert.That(ex!.ParamName, Is.EqualTo("totalStock"));
    }

    [TestCase(0, 1)]
    [TestCase(0, 3)]
    [TestCase(0, 10)]
    [TestCase(4, 6)]
    public void Borrow_Valid_UpdatesState(int borrowed, int quantity)
    {
        var loan = Create(10);
        if (borrowed > 0) loan.Borrow(borrowed);
        loan.Borrow(quantity);
        State(loan, 10, borrowed + quantity);
    }

    [TestCase(0, 0)]
    [TestCase(0, -1)]
    [TestCase(10, 0)]
    [TestCase(10, int.MinValue)]
    public void Borrow_NotPositive_ThrowsAndPreservesState(int borrowed, int quantity)
    {
        var loan = Create(10);
        if (borrowed > 0) loan.Borrow(borrowed);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => loan.Borrow(quantity));
        Assert.That(ex!.ParamName, Is.EqualTo("quantity"));
        State(loan, 10, borrowed);
    }

    [TestCase(0, 11)]
    [TestCase(10, 1)]
    [TestCase(3, int.MaxValue)]
    public void Borrow_Insufficient_ThrowsAndPreservesState(int borrowed, int quantity)
    {
        var loan = Create(10);
        if (borrowed > 0) loan.Borrow(borrowed);
        Assert.Throws<InvalidOperationException>(() => loan.Borrow(quantity));
        State(loan, 10, borrowed);
    }

    [TestCase(6, 1)]
    [TestCase(6, 2)]
    [TestCase(6, 6)]
    public void Return_Valid_UpdatesState(int borrowed, int quantity)
    {
        var loan = Create(10);
        loan.Borrow(borrowed);
        loan.Return(quantity);
        State(loan, 10, borrowed - quantity);
    }

    [TestCase(0, 0)]
    [TestCase(0, -1)]
    [TestCase(6, 0)]
    [TestCase(6, int.MinValue)]
    public void Return_NotPositive_ThrowsAndPreservesState(int borrowed, int quantity)
    {
        var loan = Create(10);
        if (borrowed > 0) loan.Borrow(borrowed);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => loan.Return(quantity));
        Assert.That(ex!.ParamName, Is.EqualTo("quantity"));
        State(loan, 10, borrowed);
    }

    [TestCase(0, 1)]
    [TestCase(6, 7)]
    public void Return_TooMany_ThrowsAndPreservesState(int borrowed, int quantity)
    {
        var loan = Create(10);
        if (borrowed > 0) loan.Borrow(borrowed);
        Assert.Throws<InvalidOperationException>(() => loan.Return(quantity));
        State(loan, 10, borrowed);
    }

    [Test]
    public void Operations_MaximumStock_DoNotOverflow()
    {
        var loan = Create(int.MaxValue);
        loan.Borrow(int.MaxValue);
        State(loan, int.MaxValue, int.MaxValue);
        loan.Return(int.MaxValue);
        State(loan, int.MaxValue, 0);
    }

    private static void State(IEquipmentLoan loan, int total, int borrowed)
    {
        Assert.Multiple(() =>
        {
            Assert.That(loan.TotalStock, Is.EqualTo(total));
            Assert.That(loan.AvailableStock, Is.EqualTo(total - borrowed));
            Assert.That(loan.BorrowedStock, Is.EqualTo(borrowed));
        });
    }
}
