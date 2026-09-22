export type TrainColor =
  | 'red'
  | 'blue'
  | 'green'
  | 'yellow'
  | 'black'
  | 'white'
  | 'orange'
  | 'purple'
  | 'locomotive'
  | 'grey'

export interface PlayerSummaryDto {
  id: string
  name: string
}

export interface CreateGameResultDto {
  gameId: string
  players: PlayerSummaryDto[]
}

export interface DestinationTicketDto {
  id: string
  fromCityId: string
  toCityId: string
  points: number
}

export interface ClaimedRouteDto {
  routeId: string
  playerId: string
}

export interface PlayerDto {
  id: string
  name: string
  trainsRemaining: number
  score: number
  trainCardCount: number
  destinationTicketCount: number
  trainCards: Partial<Record<TrainColor, number>> | null
  destinationTickets: DestinationTicketDto[] | null
}

export interface GameStateDto {
  id: string
  currentPlayerIndex: number
  currentPlayerId: string
  isGameOver: boolean
  isLastRound: boolean
  faceUpTrainCards: TrainColor[]
  trainDrawPileCount: number
  ticketDrawPileCount: number
  players: PlayerDto[]
  claimedRoutes: ClaimedRouteDto[]
}
