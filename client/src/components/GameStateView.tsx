import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { cn } from '@/lib/utils'
import type { GameStateDto, PlayerDto, TrainColor } from '@/types/game'

const TRAIN_COLOR_CLASSES: Record<TrainColor, string> = {
  red: 'bg-red-500 text-white',
  blue: 'bg-blue-500 text-white',
  green: 'bg-green-600 text-white',
  yellow: 'bg-yellow-400 text-black',
  black: 'bg-black text-white',
  white: 'bg-white text-black border border-border',
  orange: 'bg-orange-500 text-white',
  purple: 'bg-purple-500 text-white',
  locomotive: 'bg-gradient-to-r from-pink-500 to-violet-500 text-white',
  grey: 'bg-gray-400 text-black',
}

function TrainColorBadge({ color }: { color: TrainColor }) {
  return <Badge className={cn('capitalize', TRAIN_COLOR_CLASSES[color])}>{color}</Badge>
}

function PlayerCard({ player, isCurrentTurn, isYou }: { player: PlayerDto; isCurrentTurn: boolean; isYou: boolean }) {
  return (
    <Card className={cn(isCurrentTurn && 'border-primary')}>
      <CardHeader>
        <CardTitle className="flex items-center gap-2 text-base">
          {player.name}
          {isYou && <Badge variant="secondary">You</Badge>}
          {isCurrentTurn && <Badge>Current turn</Badge>}
        </CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col gap-2 text-sm">
        <p>Trains remaining: {player.trainsRemaining}</p>
        <p>Score: {player.score}</p>

        {player.trainCards ? (
          <div className="flex flex-wrap gap-1">
            {Object.entries(player.trainCards)
              .filter(([, count]) => (count ?? 0) > 0)
              .map(([color, count]) => (
                <Badge key={color} variant="outline" className="capitalize">
                  {color}: {count}
                </Badge>
              ))}
          </div>
        ) : (
          <p>Train cards: {player.trainCardCount}</p>
        )}

        {player.destinationTickets ? (
          <div className="flex flex-col gap-1">
            {player.destinationTickets.map((ticket) => (
              <p key={ticket.id} className="text-muted-foreground">
                {ticket.fromCityId} → {ticket.toCityId} ({ticket.points} pts)
              </p>
            ))}
          </div>
        ) : (
          <p>Destination tickets: {player.destinationTicketCount}</p>
        )}
      </CardContent>
    </Card>
  )
}

export function GameStateView({ state, playerId }: { state: GameStateDto; playerId: string }) {
  return (
    <div className="flex w-full flex-col gap-4">
      <Card>
        <CardHeader>
          <CardTitle className="flex flex-wrap items-center gap-2">
            Game {state.id}
            {state.isGameOver && <Badge variant="destructive">Game over</Badge>}
            {state.isLastRound && !state.isGameOver && <Badge variant="secondary">Last round</Badge>}
          </CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-3 text-sm">
          <div className="flex flex-wrap items-center gap-4">
            <span>Train draw pile: {state.trainDrawPileCount}</span>
            <span>Ticket draw pile: {state.ticketDrawPileCount}</span>
            <span>Claimed routes: {state.claimedRoutes.length}</span>
          </div>
          <Separator />
          <div>
            <p className="text-muted-foreground mb-1">Face-up train cards</p>
            <div className="flex flex-wrap gap-1">
              {state.faceUpTrainCards.map((color, index) => (
                <TrainColorBadge key={`${color}-${index}`} color={color} />
              ))}
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        {state.players.map((player) => (
          <PlayerCard
            key={player.id}
            player={player}
            isCurrentTurn={player.id === state.currentPlayerId}
            isYou={player.id === playerId}
          />
        ))}
      </div>
    </div>
  )
}
