import type { Spot } from './spots'

export interface Collection {
  id: number
  name: string
  subtitle: string
  spotCount: number
  swatches: { light: string; dark: string }[]
}

export interface CollectionDetail {
  id: number
  name: string
  subtitle: string
  spots: Spot[]
}

const BASE = (import.meta.env.VITE_API_URL as string | undefined) ?? ''

export async function fetchCollections(): Promise<Collection[]> {
  const res = await fetch(`${BASE}/api/collections`)
  if (!res.ok) throw new Error(`Failed to load collections (${res.status})`)
  return res.json()
}

export async function fetchCollectionDetail(id: number): Promise<CollectionDetail> {
  const res = await fetch(`${BASE}/api/collections/${id}`)
  if (!res.ok) throw new Error(`Failed to load collection (${res.status})`)
  const data = await res.json()
  return {
    ...data,
    spots: data.spots.map((s: any) => ({ ...s, kind: (s.kind as string).toLowerCase() })),
  }
}

export async function createCollection(name: string, subtitle: string): Promise<Collection> {
  const res = await fetch(`${BASE}/api/collections`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, subtitle }),
  })
  if (!res.ok) throw new Error(`Failed to create collection (${res.status})`)
  return res.json()
}

export async function addSpotToCollection(collectionId: number, spotId: number): Promise<void> {
  const res = await fetch(`${BASE}/api/collections/${collectionId}/spots/${spotId}`, { method: 'POST' })
  // 409 = already in collection — treat as success
  if (!res.ok && res.status !== 409) throw new Error(`Failed to add spot (${res.status})`)
}

export async function removeSpotFromCollection(collectionId: number, spotId: number): Promise<void> {
  const res = await fetch(`${BASE}/api/collections/${collectionId}/spots/${spotId}`, { method: 'DELETE' })
  if (!res.ok) throw new Error(`Failed to remove spot (${res.status})`)
}
