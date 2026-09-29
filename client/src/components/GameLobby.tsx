import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Separator } from '@/components/ui/separator'
import type { CreateGameResultDto } from '@/types/game'

function LinkRow({ label, href }: { label: string; href: string }) {
  return (
    <div className="flex flex-col">
      <a href={href} target="_blank" rel="noreferrer" className="font-medium underline">
        {label}
      </a>
      <span className="text-muted-foreground break-all text-xs">{href}</span>
    </div>
  )
}

// Each player opens their own link on their own device; the board link goes on a shared screen
export function GameLobby({ result }: { result: CreateGameResultDto }) {
  const base = `${window.location.origin}/?game=${result.gameId}`

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle>Share the links</CardTitle>
      </CardHeader>
      <CardContent className="flex flex-col gap-3 text-sm">
        <LinkRow label="Board (shared screen)" href={`${base}&board`} />
        <Separator />
        {result.players.map((player) => (
          <LinkRow key={player.id} label={player.name} href={`${base}&player=${player.id}`} />
        ))}
      </CardContent>
    </Card>
  )
}
