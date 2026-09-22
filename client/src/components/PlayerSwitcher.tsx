import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { useGameConnection } from '@/hooks/useGameConnection'
import type { GameStateDto } from '@/types/game'

export function PlayerSwitcher({ state, playerId }: { state: GameStateDto; playerId: string }) {
  const { switchPlayer } = useGameConnection()
  const [switchingTo, setSwitchingTo] = useState<string | null>(null)

  const handleSwitch = async (nextPlayerId: string) => {
    setSwitchingTo(nextPlayerId)
    try {
      await switchPlayer(nextPlayerId)
    } finally {
      setSwitchingTo(null)
    }
  }

  return (
    <div className="flex flex-wrap items-center gap-2 text-sm">
      <span className="text-muted-foreground">Viewing as:</span>
      {state.players.map((player) => (
        <Button
          key={player.id}
          size="sm"
          variant={player.id === playerId ? 'default' : 'outline'}
          disabled={switchingTo !== null}
          onClick={() => handleSwitch(player.id)}
        >
          {switchingTo === player.id ? 'Switching…' : player.name}
        </Button>
      ))}
    </div>
  )
}
