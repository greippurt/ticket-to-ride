using core.Enums;

namespace core.Models;

public record Route(string Id, string FromCityId, string ToCityId, int Length, TrainColor Color);
