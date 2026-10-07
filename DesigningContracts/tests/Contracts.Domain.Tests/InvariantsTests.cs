using Contracts.Domain;
using NUnit.Framework;
namespace Contracts.Domain.Tests;

[TestFixture]
public class InvariantsTests
{
    [Test]
    public void Workshop_Sequence_MaintainsInvariantAtEveryStep()
    {
        var w = new WorkshopReservation(20);
        void Check(int reserved)
        {
            Assert.Multiple(() =>
            {
                Assert.That(w.Capacity, Is.EqualTo(20));
                Assert.That(w.ReservedSeats, Is.EqualTo(reserved).And.InRange(0, 20));
                Assert.That(w.AvailableSeats, Is.EqualTo(20 - reserved).And.GreaterThanOrEqualTo(0));
                Assert.That(w.AvailableSeats + w.ReservedSeats, Is.EqualTo(w.Capacity));
            });
        }
        Check(0);
        w.Reserve(8); Check(8);
        w.Reserve(12); Check(20);
        Assert.Throws<InvalidOperationException>(() => w.Reserve(1)); Check(20);
        w.Cancel(5); Check(15);
        Assert.Throws<InvalidOperationException>(() => w.Cancel(16)); Check(15);
        w.Cancel(15); Check(0);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Equipment_Sequence_MaintainsInvariantAtEveryStep(bool logged)
    {
        IEquipmentLoan loan = logged ? new LoggedEquipmentLoan(10) : new EquipmentLoan(10);
        void Check(int borrowed)
        {
            Assert.Multiple(() =>
            {
                Assert.That(loan.TotalStock, Is.EqualTo(10));
                Assert.That(loan.BorrowedStock, Is.EqualTo(borrowed).And.GreaterThanOrEqualTo(0));
                Assert.That(loan.AvailableStock, Is.EqualTo(10 - borrowed).And.GreaterThanOrEqualTo(0));
                Assert.That(loan.AvailableStock + loan.BorrowedStock, Is.EqualTo(loan.TotalStock));
            });
        }
        Check(0);
        loan.Borrow(4); Check(4);
        loan.Borrow(6); Check(10);
        Assert.Throws<InvalidOperationException>(() => loan.Borrow(1)); Check(10);
        loan.Return(3); Check(7);
        Assert.Throws<InvalidOperationException>(() => loan.Return(8)); Check(7);
        loan.Return(7); Check(0);
    }

    [TestCase(typeof(WorkshopReservation))]
    [TestCase(typeof(EquipmentLoan))]
    [TestCase(typeof(LoggedEquipmentLoan))]
    public void PublicProperties_HaveNoPublicSetter(Type type)
    {
        foreach (var property in type.GetProperties())
            Assert.That(property.GetSetMethod(), Is.Null, property.Name);
    }

    [Test]
    public void Workshop_MaximumCapacity_DoesNotOverflow()
    {
        var w = new WorkshopReservation(int.MaxValue);
        w.Reserve(int.MaxValue);
        Assert.That(w.AvailableSeats, Is.Zero);
        w.Cancel(int.MaxValue);
        Assert.That(w.ReservedSeats, Is.Zero);
        Assert.That(w.AvailableSeats, Is.EqualTo(int.MaxValue));
    }
}
