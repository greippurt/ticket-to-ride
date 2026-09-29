import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { useGameConnection } from '@/hooks/useGameConnection'
import type { Board, GameStateDto, Route, TrainColor } from '@/types/game'

function ClaimPanel({ route, cards, onDone }: {
  route: Route
  cards: Partial<Record<TrainColor, number>>
  onDone: () => void
}) {
  const { claimRoute } = useGameConnection()

  // A grey route can be paid with any color, other routes only with their own
  const colorOptions: TrainColor[] =
    route.color === 'grey'
      ? (Object.keys(cards) as TrainColor[]).filter((c) => c !== 'locomotive' && (cards[c] ?? 0) > 0)
      : [route.color]

  const [color, setColor] = useState<TrainColor | undefined>(colorOptions[0])
  const [locomotives, setLocomotives] = useState(0)

  const handleClaim = async () => {
    try {
      // Paying with locomotives only still needs a color, the route color works for that
      await claimRoute(route.id, color ?? route.color, locomotives)
      onDone()
    } catch {
      // The error is already shown as a toast
    }
  }

  return (
    <div className="flex flex-wrap items-center gap-2 pb-2">
      {colorOptions.map((option) => (
        <Button
          key={option}
          size="sm"
          variant={option === color ? 'default' : 'outline'}
          className="capitalize"
          onClick={() => setColor(option)}
        >
          {option} ({cards[option] ?? 0})
        </Button>
      ))}
      <label className="flex items-center gap-1">
        Locomotives ({cards.locomotive ?? 0})
        <Input
          type="number"
          min={0}
          max={route.length}
          value={locomotives}
          onChange={(e) => setLocomotives(Number(e.target.value))}
          className="w-16"
        />
      </label>
      <Button size="sm" onClick={handleClaim}>
        Claim
      </Button>
      <Button size="sm" variant="ghost" onClick={onDone}>
        Cancel
      </Button>
    </div>
  )
}

export function ClaimRouteList({ board, state, playerId }: { board: Board; state: GameStateDto; playerId: string }) {
  const [selectedRouteId, setSelectedRouteId] = useState<string | null>(null)
  const cityName = (id: string) => board.cities.find((c) => c.id === id)?.name ?? id
  const cards = state.players.find((p) => p.id === playerId)?.trainCards ?? {}
  const isMyTurn = state.currentPlayerId === playerId
  const openRoutes = board.routes.filter((r) => !state.claimedRoutes.some((c) => c.routeId === r.id))

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">Claim a route</CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col text-sm">
        {openRoutes.map((route) => (
          <div key={route.id} className="border-b last:border-b-0">
            <div className="flex items-center justify-between gap-2 py-2">
              <span>
                {cityName(route.fromCityId)} → {cityName(route.toCityId)}{' '}
                <span className="text-muted-foreground capitalize">
                  ({route.length} {route.color})
                </span>
              </span>
              {selectedRouteId !== route.id && (
                <Button size="sm" variant="outline" disabled={!isMyTurn} onClick={() => setSelectedRouteId(route.id)}>
                  Claim
                </Button>
              )}
            </div>
            {selectedRouteId === route.id && (
              <ClaimPanel route={route} cards={cards} onDone={() => setSelectedRouteId(null)} />
            )}
          </div>
        ))}
      </CardContent>
    </Card>
  )
}
