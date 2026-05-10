import type { Kind } from '../data/spots'

export interface Spot {
  id: number
  name: string
  kind: Kind
  cuisine: string
  neighborhood: string
  visitedOn: string       // "YYYY-MM-DD"
  rating: number
  price: number
  returnable: boolean
  note: string | null
  swatchLight: string
  swatchDark: string
  dish: string | null
  latitude: number | null
  longitude: number | null
  photoPath: string | null
}

export interface CreateSpotPayload {
  name: string
  kind: string
  cuisine: string
  neighborhood: string
  latitude: number | null
  longitude: number | null
  visitedOn: string
  dish: string | null
  note: string | null
  rating: number
  price: number
  returnable: boolean
  swatchLight: string
  swatchDark: string
  photoPath: string | null
}

export async function uploadPhoto(file: File): Promise<string> {
  const form = new FormData()
  form.append('file', file)
  const res = await fetch(`${BASE}/api/uploads`, { method: 'POST', body: form })
  if (!res.ok) throw new Error(`Upload failed (${res.status})`)
  const { path } = await res.json()
  return path as string
}

export function photoUrl(path: string): string {
  return `${BASE}${path}`
}

// Empty in Docker (nginx proxies /api and /uploads to the backend).
// Set VITE_API_URL=http://localhost:8080 only when running the frontend
// outside both Docker and the Vite dev server.
const BASE = (import.meta.env.VITE_API_URL as string | undefined) ?? ''

export async function fetchSpots(kind?: Kind): Promise<Spot[]> {
  const qs = kind ? `?kind=${encodeURIComponent(kind)}` : ''
  const res = await fetch(`${BASE}/api/spots${qs}`)
  if (!res.ok) throw new Error(`Failed to load spots (${res.status})`)
  const data: any[] = await res.json()
  return data.map(normalise)
}

export async function deleteSpot(id: number): Promise<void> {
  const res = await fetch(`${BASE}/api/spots/${id}`, { method: 'DELETE' })
  if (!res.ok) throw new Error(`Failed to delete spot (${res.status})`)
}

export async function createSpot(payload: CreateSpotPayload): Promise<Spot> {
  const res = await fetch(`${BASE}/api/spots`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
  if (!res.ok) throw new Error(`Failed to create spot (${res.status})`)
  return normalise(await res.json())
}

// Backend returns "Coffee" / "Lunch" / "Dinner" — normalise to lowercase Kind
function normalise(raw: any): Spot {
  return { ...raw, kind: (raw.kind as string).toLowerCase() as Kind }
}
