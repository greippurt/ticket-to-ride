import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useGameConnection } from '@/hooks/useGameConnection'
import type { CreateGameResultDto } from '@/types/game'

const MIN_PLAYERS = 2
const MAX_PLAYERS = 5

export function CreateGameForm({ onCreated }: { onCreated: (result: CreateGameResultDto) => void }) {
  const { createGame } = useGameConnection()
  const [names, setNames] = useState(['', ''])
  const [isSubmitting, setIsSubmitting] = useState(false)

  const updateName = (index: number, value: string) => {
    setNames((prev) => prev.map((name, i) => (i === index ? value : name)))
  }

  const addPlayer = () => {
    if (names.length < MAX_PLAYERS) setNames((prev) => [...prev, ''])
  }

  const removePlayer = (index: number) => {
    if (names.length > MIN_PLAYERS) setNames((prev) => prev.filter((_, i) => i !== index))
  }

  const trimmedNames = names.map((name) => name.trim())
  const canSubmit = trimmedNames.every((name) => name.length > 0)

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    if (!canSubmit) return

    setIsSubmitting(true)
    try {
      const result = await createGame(trimmedNames)
      onCreated(result)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Ticket to Ride</CardTitle>
      </CardHeader>
      <CardContent>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          {names.map((name, index) => (
            <div key={index} className="flex flex-col gap-1.5">
              <Label htmlFor={`player-${index}`}>Player {index + 1}</Label>
              <div className="flex gap-2">
                <Input
                  id={`player-${index}`}
                  value={name}
                  onChange={(event) => updateName(index, event.target.value)}
                  placeholder="Name"
                />
                {names.length > MIN_PLAYERS && (
                  <Button
                    type="button"
                    variant="ghost"
                    onClick={() => removePlayer(index)}
                    aria-label={`Remove player ${index + 1}`}
                  >
                    ✕
                  </Button>
                )}
              </div>
            </div>
          ))}

          {names.length < MAX_PLAYERS && (
            <Button type="button" variant="outline" onClick={addPlayer}>
              Add player
            </Button>
          )}

          <Button type="submit" disabled={!canSubmit || isSubmitting} className="mt-2 w-full">
            {isSubmitting ? 'Creating…' : 'Create game'}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
