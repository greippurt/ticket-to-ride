using core.Models;

namespace core.Data;

public static class PlaceholderBoard
{
    public static readonly List<City> Cities = new List<City>
    {
        new City("newYork", "New York", 880, 140),
        new City("losAngeles", "Los Angeles", 110, 420),
        new City("chicago", "Chicago", 650, 180),
        new City("houston", "Houston", 560, 500),
        new City("phoenix", "Phoenix", 330, 420),
        new City("denver", "Denver", 400, 250),
        new City("seattle", "Seattle", 90, 60),
        new City("miami", "Miami", 870, 540),
    };

    public static readonly List<Route> Routes = new List<Route>
    {
        new Route("newYork-losAngeles", "newYork", "losAngeles", 5, Enums.TrainColor.Red),
        new Route("newYork-chicago", "newYork", "chicago", 4, Enums.TrainColor.Blue),
        new Route("losAngeles-houston", "losAngeles", "houston", 6, Enums.TrainColor.Green),
        new Route("chicago-phoenix", "chicago", "phoenix", 3, Enums.TrainColor.Yellow),
        new Route("houston-phoenix", "houston", "phoenix", 2, Enums.TrainColor.Black),
        new Route("chicago-denver", "chicago", "denver", 3, Enums.TrainColor.White),
        new Route("denver-seattle", "denver", "seattle", 5, Enums.TrainColor.Purple),
        new Route("seattle-losAngeles", "seattle", "losAngeles", 4, Enums.TrainColor.Orange),
        new Route("denver-phoenix", "denver", "phoenix", 3, Enums.TrainColor.Grey),
        new Route("newYork-miami", "newYork", "miami", 5, Enums.TrainColor.Red),
        new Route("miami-houston", "miami", "houston", 4, Enums.TrainColor.Blue),
    };

    public static readonly List<DestinationTicket> DestinationTickets = new List<DestinationTicket>
    {
        new DestinationTicket("newYork-losAngeles", "newYork", "losAngeles", 21),
        new DestinationTicket("newYork-chicago", "newYork", "chicago", 12),
        new DestinationTicket("losAngeles-houston", "losAngeles", "houston", 16),
        new DestinationTicket("chicago-phoenix", "chicago", "phoenix", 11),
        new DestinationTicket("houston-phoenix", "houston", "phoenix", 7),
        new DestinationTicket("chicago-denver", "chicago", "denver", 8),
        new DestinationTicket("denver-seattle", "denver", "seattle", 13),
        new DestinationTicket("seattle-losAngeles", "seattle", "losAngeles", 14),
        new DestinationTicket("denver-phoenix", "denver", "phoenix", 9),
        new DestinationTicket("newYork-miami", "newYork", "miami", 15),
        new DestinationTicket("miami-houston", "miami", "houston", 10),
    };
}
