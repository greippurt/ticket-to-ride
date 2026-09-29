import { useEffect, useState } from 'react'
import type { Board } from '@/types/game'

// The board is served by the same server as the hub
const API_URL = new URL(import.meta.env.VITE_HUB_URL ?? 'http://localhost:5074/hubs/game').origin

export function useBoard() {
  const [board, setBoard] = useState<Board | null>(null)

  useEffect(() => {
    fetch(`${API_URL}/api/board`)
      .then((response) => response.json())
      .then(setBoard)
  }, [])

  return board
}
