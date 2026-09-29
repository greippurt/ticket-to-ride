import { useEffect, useRef, useState } from 'react'
import { BoardMap } from '@/components/BoardMap'
import { ClaimRouteList } from '@/components/ClaimRouteList'
import { CreateGameForm } from '@/components/CreateGameForm'
import { GameLobby } from '@/components/GameLobby'
import { GameStateView } from '@/components/GameStateView'
import { Toaster } from '@/components/ui/sonner'
import { useBoard } from '@/hooks/useBoard'
import { GameConnectionProvider, useGameConnection } from '@/hooks/useGameConnection'
import type { CreateGameResultDto } from '@/types/game'

// The link decides the view: ?game=..&player=.. is a player's hand, ?game=..&board is the shared map
const params = new URLSearchParams(window.location.search)
const gameId = params.get('game')
const linkPlayerId = params.get('player')
const isBoardView = params.has('board')

function CreateScreen() {
  const [result, setResult] = useState<CreateGameResultDto | null>(null)

  if (result) {
    return <GameLobby result={result} />
  }
  return <CreateGameForm onCreated={setResult} />
}

function BoardScreen({ gameId }: { gameId: string }) {
  const { gameState, watchGame } = useGameConnection()
  const board = useBoard()
  const started = useRef(false)

  useEffect(() => {
    if (started.current) return
    started.current = true
    watchGame(gameId)
  }, [gameId, watchGame])

  if (!gameState || !board) {
    return <p className="text-muted-foreground text-sm">Loading board…</p>
  }

  return (
    <div className="w-full max-w-6xl">
      <BoardMap board={board} state={gameState} />
    </div>
  )
}

function PlayerScreen({ gameId, playerId }: { gameId: string; playerId: string }) {
  const { gameState, joinGame } = useGameConnection()
  const board = useBoard()
  const started = useRef(false)

  useEffect(() => {
    if (started.current) return
    started.current = true
    joinGame(gameId, playerId)
  }, [gameId, playerId, joinGame])

  if (!gameState || !board) {
    return <p className="text-muted-foreground text-sm">Loading game…</p>
  }

  return (
    <div className="flex w-full max-w-3xl flex-col gap-3">
      <GameStateView state={gameState} playerId={playerId} />
      <ClaimRouteList board={board} state={gameState} playerId={playerId} />
    </div>
  )
}

function GameScreen() {
  if (gameId && isBoardView) return <BoardScreen gameId={gameId} />
  if (gameId && linkPlayerId) return <PlayerScreen gameId={gameId} playerId={linkPlayerId} />
  return <CreateScreen />
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
