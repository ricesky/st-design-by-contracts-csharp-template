namespace Contracts.Domain;

public class WorkshopReservation
{
    public int Capacity { get; }
    public int ReservedSeats { get; private set; }
    public int AvailableSeats => Capacity - ReservedSeats;

    public WorkshopReservation(int capacity)
    {
        // TODO: tambahkan implementasi disini
        throw new NotImplementedException();
    }

    public void Reserve(int quantity)
    {
        // TODO: tambahkan implementasi disini
        throw new NotImplementedException();
    }

    public void Cancel(int quantity)
    {
        // TODO: tambahkan implementasi disini
        throw new NotImplementedException();
    }
}
