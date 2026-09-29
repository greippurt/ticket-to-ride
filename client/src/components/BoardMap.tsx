import { Card, CardContent } from '@/components/ui/card'
import type { Board, City, GameStateDto, Route, TrainColor } from '@/types/game'

const ROUTE_COLORS: Record<TrainColor, string> = {
  red: '#ef4444',
  blue: '#3b82f6',
  green: '#16a34a',
  yellow: '#facc15',
  black: '#171717',
  white: '#ffffff',
  orange: '#f97316',
  purple: '#a855f7',
  locomotive: '#ec4899',
  grey: '#9ca3af',
}

// Claimed routes are drawn in the owner's color, by player order
const PLAYER_COLORS = ['#dc2626', '#2563eb', '#059669', '#d97706', '#7c3aed']

const CITY_RADIUS = 14
const SEGMENT_GAP = 6

// One small line per train car between the two cities
function RouteSegments({ from, to, route, color }: { from: City; to: City; route: Route; color: string }) {
  const dx = to.x - from.x
  const dy = to.y - from.y
  const distance = Math.hypot(dx, dy)
  const ux = dx / distance
  const uy = dy / distance
  const segment = (distance - 2 * CITY_RADIUS) / route.length

  return Array.from({ length: route.length }, (_, i) => {
    const start = CITY_RADIUS + i * segment + SEGMENT_GAP / 2
    const end = start + segment - SEGMENT_GAP
    const x1 = from.x + ux * start
    const y1 = from.y + uy * start
    const x2 = from.x + ux * end
    const y2 = from.y + uy * end
    return (
      <g key={i}>
        <line x1={x1} y1={y1} x2={x2} y2={y2} stroke="#404040" strokeWidth={14} />
        <line x1={x1} y1={y1} x2={x2} y2={y2} stroke={color} strokeWidth={10} />
      </g>
    )
  })
}

export function BoardMap({ board, state }: { board: Board; state: GameStateDto }) {
  const citiesById = new Map(board.cities.map((c) => [c.id, c]))
  const playerColor = (index: number) => PLAYER_COLORS[index % PLAYER_COLORS.length]

  const ownerColor = (routeId: string) => {
    const claimed = state.claimedRoutes.find((c) => c.routeId === routeId)
    if (!claimed) return null
    return playerColor(state.players.findIndex((p) => p.id === claimed.playerId))
  }

  return (
    <Card>
      <CardContent className="flex flex-col gap-3">
        <div className="flex flex-wrap gap-4 text-sm">
          {state.players.map((player, index) => (
            <span key={player.id} className="flex items-center gap-2">
              <span className="size-3 rounded-full" style={{ backgroundColor: playerColor(index) }} />
              <span className={player.id === state.currentPlayerId ? 'font-semibold' : undefined}>
                {player.name}: {player.score} pts, {player.trainsRemaining} trains
              </span>
            </span>
          ))}
        </div>

        <svg viewBox="0 0 1000 600" className="w-full rounded-md bg-sky-50">
          {board.routes.map((route) => {
            const from = citiesById.get(route.fromCityId)
            const to = citiesById.get(route.toCityId)
            if (!from || !to) return null

            return (
              <RouteSegments
                key={route.id}
                from={from}
                to={to}
                route={route}
                color={ownerColor(route.id) ?? ROUTE_COLORS[route.color]}
              />
            )
          })}

          {board.cities.map((city) => (
            <g key={city.id}>
              <circle cx={city.x} cy={city.y} r={CITY_RADIUS} fill="#fef3c7" stroke="#404040" strokeWidth={3} />
              <text x={city.x} y={city.y - CITY_RADIUS - 6} textAnchor="middle" fontSize={20} fontWeight={600}>
                {city.name}
              </text>
            </g>
          ))}
        </svg>
      </CardContent>
    </Card>
  )
}
