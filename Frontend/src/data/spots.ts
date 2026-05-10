export type Kind = 'coffee' | 'lunch' | 'dinner'

export const KIND_META: Record<Kind, { label: string; glyph: string }> = {
  coffee: { label: 'Coffee', glyph: '◔' },
  lunch:  { label: 'Lunch',  glyph: '◑' },
  dinner: { label: 'Dinner', glyph: '◕' },
}
