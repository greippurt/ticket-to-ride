namespace core.Models;

public record Route(int Id, string fromCityId, string toCityId, int Length, TrainColor Color);
