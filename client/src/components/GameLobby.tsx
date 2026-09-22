import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import { useGameConnection } from '@/hooks/useGameConnection'
import type { CreateGameResultDto } from '@/types/game'

export function GameLobby({
  result,
  onJoined,
}: {
  result: CreateGameResultDto
  onJoined: () => void
}) {
  const { joinGame } = useGameConnection()
  const [joiningId, setJoiningId] = useState<string | null>(null)

  const handleJoin = async (playerId: string) => {
    setJoiningId(playerId)
    try {
      await joinGame(result.gameId, playerId)
      onJoined()
    } finally {
      setJoiningId(null)
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Who are you?</CardTitle>
      </CardHeader>
      <CardContent>
        <p className="text-muted-foreground mb-3 text-sm">Game {result.gameId}</p>
        <Separator className="mb-3" />
        <div className="flex flex-col gap-2">
          {result.players.map((player) => (
            <Button
              key={player.id}
              variant="outline"
              disabled={joiningId !== null}
              onClick={() => handleJoin(player.id)}
            >
              {joiningId === player.id ? 'Joining…' : `Join as ${player.name}`}
            </Button>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
