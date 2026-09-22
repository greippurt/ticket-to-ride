import * as signalR from '@microsoft/signalr'
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from 'react'
import { toast } from 'sonner'
import type {
  CreateGameResultDto,
  DestinationTicketDto,
  GameStateDto,
  TrainColor,
} from '@/types/game'

const HUB_URL = import.meta.env.VITE_HUB_URL ?? 'http://localhost:5074/hubs/game'

type ConnectionStatus = 'disconnected' | 'connecting' | 'connected'

interface GameConnectionContextValue {
  status: ConnectionStatus
  gameState: GameStateDto | null
  playerId: string | null
  createGame: (playerNames: string[]) => Promise<CreateGameResultDto>
  joinGame: (gameId: string, playerId: string) => Promise<GameStateDto>
  switchPlayer: (playerId: string) => Promise<GameStateDto>
  drawTrainCardFromDeck: () => Promise<void>
  drawFaceUpTrainCard: (color: TrainColor) => Promise<void>
  claimRoute: (routeId: string, color: TrainColor) => Promise<void>
  drawDestinationTickets: () => Promise<DestinationTicketDto[]>
  chooseDestinationTickets: (ticketIds: string[]) => Promise<void>
}

const GameConnectionContext = createContext<GameConnectionContextValue | null>(null)

function getErrorMessage(error: unknown): string {
  return error instanceof Error ? error.message : 'Something went wrong'
}

export function GameConnectionProvider({ children }: { children: ReactNode }) {
  const connectionRef = useRef<signalR.HubConnection | null>(null)
  const [status, setStatus] = useState<ConnectionStatus>('disconnected')
  const [gameState, setGameState] = useState<GameStateDto | null>(null)
  const [playerId, setPlayerId] = useState<string | null>(null)
  const gameIdRef = useRef<string | null>(null)

  const ensureConnection = useCallback(async () => {
    let connection = connectionRef.current
    if (!connection) {
      connection = new signalR.HubConnectionBuilder().withUrl(HUB_URL).withAutomaticReconnect().build()

      connection.on('GameStateUpdated', (dto: GameStateDto) => setGameState(dto))
      connection.onreconnecting(() => setStatus('connecting'))
      connection.onreconnected(() => setStatus('connected'))
      connection.onclose(() => setStatus('disconnected'))

      connectionRef.current = connection
    }

    if (connection.state === signalR.HubConnectionState.Disconnected) {
      setStatus('connecting')
      await connection.start()
      setStatus('connected')
    }

    return connection
  }, [])

  useEffect(() => {
    return () => {
      connectionRef.current?.stop()
    }
  }, [])

  const invoke = useCallback(
    async <T,>(method: string, ...args: unknown[]) => {
      const connection = await ensureConnection()
      try {
        return await connection.invoke<T>(method, ...args)
      } catch (error) {
        toast.error(getErrorMessage(error))
        throw error
      }
    },
    [ensureConnection],
  )

  const createGame = useCallback(
    (playerNames: string[]) => invoke<CreateGameResultDto>('CreateGame', playerNames),
    [invoke],
  )

  const joinGame = useCallback(
    async (gameId: string, joiningPlayerId: string) => {
      const dto = await invoke<GameStateDto>('JoinGame', gameId, joiningPlayerId)
      gameIdRef.current = gameId
      setGameState(dto)
      setPlayerId(joiningPlayerId)
      return dto
    },
    [invoke],
  )

  const switchPlayer = useCallback(
    async (nextPlayerId: string) => {
      if (!gameIdRef.current) {
        throw new Error('Cannot switch player before joining a game')
      }
      return joinGame(gameIdRef.current, nextPlayerId)
    },
    [joinGame],
  )

  const drawTrainCardFromDeck = useCallback(
    () => invoke<void>('DrawTrainCardFromDeck'),
    [invoke],
  )
  const drawFaceUpTrainCard = useCallback(
    (color: TrainColor) => invoke<void>('DrawFaceUpTrainCard', color),
    [invoke],
  )
  const claimRoute = useCallback(
    (routeId: string, color: TrainColor) => invoke<void>('ClaimRoute', routeId, color),
    [invoke],
  )
  const drawDestinationTickets = useCallback(
    () => invoke<DestinationTicketDto[]>('DrawDestinationTickets'),
    [invoke],
  )
  const chooseDestinationTickets = useCallback(
    (ticketIds: string[]) => invoke<void>('ChooseDestinationTickets', ticketIds),
    [invoke],
  )

  const value = useMemo<GameConnectionContextValue>(
    () => ({
      status,
      gameState,
      playerId,
      createGame,
      joinGame,
      switchPlayer,
      drawTrainCardFromDeck,
      drawFaceUpTrainCard,
      claimRoute,
      drawDestinationTickets,
      chooseDestinationTickets,
    }),
    [
      status,
      gameState,
      playerId,
      createGame,
      joinGame,
      switchPlayer,
      drawTrainCardFromDeck,
      drawFaceUpTrainCard,
      claimRoute,
      drawDestinationTickets,
      chooseDestinationTickets,
    ],
  )

  return <GameConnectionContext.Provider value={value}>{children}</GameConnectionContext.Provider>
}

export function useGameConnection() {
  const context = useContext(GameConnectionContext)
  if (!context) {
    throw new Error('useGameConnection must be used within a GameConnectionProvider')
  }
  return context
}
