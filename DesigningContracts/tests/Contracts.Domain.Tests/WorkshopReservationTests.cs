using Contracts.Domain;
using NUnit.Framework;

namespace Contracts.Domain.Tests;

[TestFixture]
public class WorkshopReservationTests
{
    [TestCase(1)]
    [TestCase(30)]
    [TestCase(int.MaxValue)]
    public void Constructor_Positive_SetsInitialState(int capacity)
    {
        var w = new WorkshopReservation(capacity);
        State(w, capacity, 0);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void Constructor_NotPositive_Throws(int capacity)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new WorkshopReservation(capacity));
        Assert.That(ex!.ParamName, Is.EqualTo("capacity"));
    }

    [TestCase(0, 1)]
    [TestCase(0, 5)]
    [TestCase(0, 30)]
    [TestCase(10, 20)]
    public void Reserve_Valid_UpdatesState(int reserved, int quantity)
    {
        var w = new WorkshopReservation(30);
        if (reserved > 0) w.Reserve(reserved);
        w.Reserve(quantity);
        State(w, 30, reserved + quantity);
    }

    [TestCase(0, 0)]
    [TestCase(0, -1)]
    [TestCase(30, 0)]
    [TestCase(30, int.MinValue)]
    public void Reserve_NotPositive_ThrowsAndPreservesState(int reserved, int quantity)
    {
        var w = new WorkshopReservation(30);
        if (reserved > 0) w.Reserve(reserved);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => w.Reserve(quantity));
        Assert.That(ex!.ParamName, Is.EqualTo("quantity"));
        State(w, 30, reserved);
    }

    [TestCase(0, 31)]
    [TestCase(10, 21)]
    [TestCase(30, 1)]
    [TestCase(1, int.MaxValue)]
    public void Reserve_Insufficient_ThrowsAndPreservesState(int reserved, int quantity)
    {
        var w = new WorkshopReservation(30);
        if (reserved > 0) w.Reserve(reserved);
        Assert.Throws<InvalidOperationException>(() => w.Reserve(quantity));
        State(w, 30, reserved);
    }

    [TestCase(10, 1)]
    [TestCase(10, 4)]
    [TestCase(10, 10)]
    public void Cancel_Valid_UpdatesState(int reserved, int quantity)
    {
        var w = new WorkshopReservation(30);
        w.Reserve(reserved);
        w.Cancel(quantity);
        State(w, 30, reserved - quantity);
    }

    [TestCase(0, 0)]
    [TestCase(0, -1)]
    [TestCase(10, 0)]
    [TestCase(10, int.MinValue)]
    public void Cancel_NotPositive_ThrowsAndPreservesState(int reserved, int quantity)
    {
        var w = new WorkshopReservation(30);
        if (reserved > 0) w.Reserve(reserved);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => w.Cancel(quantity));
        Assert.That(ex!.ParamName, Is.EqualTo("quantity"));
        State(w, 30, reserved);
    }

    [TestCase(0, 1)]
    [TestCase(10, 11)]
    public void Cancel_TooMany_ThrowsAndPreservesState(int reserved, int quantity)
    {
        var w = new WorkshopReservation(30);
        if (reserved > 0) w.Reserve(reserved);
        Assert.Throws<InvalidOperationException>(() => w.Cancel(quantity));
        State(w, 30, reserved);
    }

    private static void State(WorkshopReservation w, int capacity, int reserved)
    {
        Assert.Multiple(() =>
        {
            Assert.That(w.Capacity, Is.EqualTo(capacity));
            Assert.That(w.ReservedSeats, Is.EqualTo(reserved));
            Assert.That(w.AvailableSeats, Is.EqualTo(capacity - reserved));
        });
    }
}
