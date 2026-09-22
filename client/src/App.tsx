import { useState } from 'react'
import { CreateGameForm } from '@/components/CreateGameForm'
import { GameLobby } from '@/components/GameLobby'
import { GameStateView } from '@/components/GameStateView'
import { PlayerSwitcher } from '@/components/PlayerSwitcher'
import { Toaster } from '@/components/ui/sonner'
import { GameConnectionProvider, useGameConnection } from '@/hooks/useGameConnection'
import type { CreateGameResultDto } from '@/types/game'

type Screen = { name: 'create' } | { name: 'lobby'; result: CreateGameResultDto } | { name: 'game' }

function GameScreen() {
  const [screen, setScreen] = useState<Screen>({ name: 'create' })
  const { gameState, playerId } = useGameConnection()

  if (screen.name === 'create') {
    return <CreateGameForm onCreated={(result) => setScreen({ name: 'lobby', result })} />
  }

  if (screen.name === 'lobby') {
    return <GameLobby result={screen.result} onJoined={() => setScreen({ name: 'game' })} />
  }

  if (!gameState || !playerId) {
    return <p className="text-muted-foreground text-sm">Loading game…</p>
  }

  return (
    <div className="flex w-full max-w-3xl flex-col gap-3">
      <PlayerSwitcher state={gameState} playerId={playerId} />
      <GameStateView state={gameState} playerId={playerId} />
    </div>
  )
}

function App() {
  return (
    <GameConnectionProvider>
      <div className="flex min-h-svh items-center justify-center p-4">
        <GameScreen />
      </div>
      <Toaster />
    </GameConnectionProvider>
  )
}

export default App
