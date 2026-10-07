using Contracts.Domain;
using NUnit.Framework;
namespace Contracts.Domain.Tests;
[TestFixture(false)]
[TestFixture(true)]
public class SubstitutionTests
{
    private readonly bool _logged;
    public SubstitutionTests(bool logged) => _logged = logged;
    private IEquipmentLoan Create(int total) => _logged
        ? new LoggedEquipmentLoan(total) : new EquipmentLoan(total);

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

    [Test]
    public void History_RecordsOnlySuccessfulOperationsInOrder()
    {
        var loan = new LoggedEquipmentLoan(10);
        Assert.That(loan.History, Is.Not.Null.And.Empty);
        loan.Borrow(3);
        Assert.That(loan.History, Is.EqualTo(new[] { "BORROW:3" }));
        loan.Return(2);
        var before = loan.History.ToArray();
        Assert.That(before, Is.EqualTo(new[] { "BORROW:3", "RETURN:2" }));
        Assert.Throws<InvalidOperationException>(() => loan.Borrow(10));
        Assert.That(loan.History, Is.EqualTo(before));
        Assert.Throws<InvalidOperationException>(() => loan.Return(2));
        Assert.That(loan.History, Is.EqualTo(before));
        Assert.Throws<ArgumentOutOfRangeException>(() => loan.Borrow(0));
        Assert.That(loan.History, Is.EqualTo(before));
        Assert.Throws<ArgumentOutOfRangeException>(() => loan.Return(-1));
        Assert.That(loan.History, Is.EqualTo(before));
    }

    [Test]
    public void History_CannotBeUsedToMutateInternalRecords()
    {
        var loan = new LoggedEquipmentLoan(10);
        loan.Borrow(3);
        if (loan.History is IList<string> list)
        {
            try { list[0] = "CHANGED"; } catch (NotSupportedException) { }
            try { list.Add("EXTRA"); } catch (NotSupportedException) { }
            try { list.Clear(); } catch (NotSupportedException) { }
        }
        Assert.That(loan.History, Is.EqualTo(new[] { "BORROW:3" }));
    }

    [Test]
    public void History_UsesInvariantCulture()
    {
        var old = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            var culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "NEG";
            System.Globalization.CultureInfo.CurrentCulture = culture;
            var loan = new LoggedEquipmentLoan(2000);
            loan.Borrow(1000);
            loan.Return(1000);
            Assert.That(loan.History, Is.EqualTo(new[] { "BORROW:1000", "RETURN:1000" }));
        }
        finally { System.Globalization.CultureInfo.CurrentCulture = old; }
    }
}
