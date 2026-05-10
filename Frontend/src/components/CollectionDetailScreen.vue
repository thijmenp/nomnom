<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Polaroid from './Polaroid.vue'
import { fetchCollectionDetail, removeSpotFromCollection, type Collection } from '../api/collections'
import { photoUrl, type Spot } from '../api/spots'
import { KIND_META } from '../data/spots'

const props = defineProps<{ collection: Collection }>()
const emit  = defineEmits<{ back: []; openSpot: [spot: Spot] }>()

const spots   = ref<Spot[]>([])
const loading = ref(true)
const error   = ref<string | null>(null)

const removingId  = ref<number | null>(null)
const removingErr = ref<string | null>(null)
let resetTimer: ReturnType<typeof setTimeout> | null = null

onMounted(async () => {
  try {
    const detail = await fetchCollectionDetail(props.collection.id)
    spots.value = detail.spots
  } catch {
    error.value = 'Could not load entries.'
  } finally {
    loading.value = false
  }
})

function requestRemove(spot: Spot) {
  removingErr.value = null
  if (removingId.value === spot.id) {
    performRemove(spot)
    return
  }
  if (resetTimer) clearTimeout(resetTimer)
  removingId.value = spot.id
  resetTimer = setTimeout(() => { removingId.value = null }, 3000)
}

async function performRemove(spot: Spot) {
  if (resetTimer) clearTimeout(resetTimer)
  try {
    await removeSpotFromCollection(props.collection.id, spot.id)
    spots.value = spots.value.filter(s => s.id !== spot.id)
    removingId.value = null
  } catch {
    removingErr.value = 'Could not remove — please try again.'
    removingId.value = null
  }
}

const TILTS = [-2, 1.5, -1, 2, -1.5, 1]

function formatDate(iso: string): string {
  const [y, m, d] = iso.split('-').map(Number)
  return new Date(y, m - 1, d).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
}

function ratingParts(r: number) {
  const [w, f] = r.toFixed(1).split('.')
  return { w, f }
}
</script>

<template>
  <div class="pb-tab-bar lg:pb-12 min-h-full">

    <!-- ── Header ───────────────────────────────────────────── -->
    <div class="sticky top-0 z-10 bg-paper border-b border-rule-soft px-gutter lg:px-9 py-3.5 flex items-center gap-3">
      <button
        @click="emit('back')"
        class="w-8 h-8 rounded-full bg-paper-warm border border-rule flex items-center justify-center cursor-pointer shrink-0"
      >
        <svg width="12" height="12" viewBox="0 0 16 16" fill="none">
          <path d="M10 3l-5 5 5 5" stroke="#2A1F18" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      <div class="min-w-0">
        <div class="font-display text-[22px] text-ink font-medium leading-tight truncate">{{ collection.name }}</div>
        <div v-if="collection.subtitle" class="font-display italic text-[13px] text-cocoa-500 leading-tight truncate">
          {{ collection.subtitle }}
        </div>
      </div>
      <div class="caption ml-auto shrink-0">{{ spots.length }} entries</div>
    </div>

    <!-- ── Loading / error ──────────────────────────────────── -->
    <div v-if="loading" class="px-gutter py-12 text-center font-display italic text-[15px] text-cocoa-400">
      Loading…
    </div>
    <div v-else-if="error" class="px-gutter py-12 text-center font-display italic text-[15px] text-paprika">
      {{ error }}
    </div>

    <!-- ── Empty state ───────────────────────────────────────── -->
    <div v-else-if="spots.length === 0" class="px-gutter py-12 text-center">
      <div class="font-display italic text-[17px] text-cocoa-400">Nothing here yet.</div>
      <div class="font-display italic text-[13px] text-cocoa-300 mt-1">Open a place and save it to this collection.</div>
    </div>

    <!-- ── Spot list ─────────────────────────────────────────── -->
    <div v-else class="px-gutter lg:px-9 pt-1">
      <div v-if="removingErr" class="font-display italic text-[13px] text-paprika py-3">{{ removingErr }}</div>

      <div
        v-for="(spot, i) in spots" :key="spot.id"
        class="flex gap-3.5 py-5"
        style="border-bottom: 0.5px dashed rgba(60,40,20,0.18)"
      >
        <!-- Polaroid — tappable -->
        <div class="shrink-0 cursor-pointer" @click="emit('openSpot', spot)">
          <Polaroid
            :src="spot.photoPath ? photoUrl(spot.photoPath) : undefined"
            :c1="spot.swatchLight" :c2="spot.swatchDark"
            :dish="spot.dish ?? ''" :glyph="KIND_META[spot.kind].glyph"
            :size="88" :rotate="TILTS[i % 6]"
          />
        </div>

        <!-- Meta -->
        <div class="flex-1 min-w-0 pt-0.5 cursor-pointer" @click="emit('openSpot', spot)">
          <div class="flex items-center gap-1.5 font-mono text-[9px] text-cocoa-500 uppercase tracking-[0.8px] mb-1">
            <span>{{ formatDate(spot.visitedOn) }}</span>
            <span class="opacity-40">·</span>
            <span>{{ KIND_META[spot.kind].label }}</span>
            <span class="opacity-40">·</span>
            <span>
              <span v-for="n in 4" :key="n" :style="{ opacity: n <= spot.price ? '1' : '0.25' }">€</span>
            </span>
          </div>
          <div class="font-display text-h2 text-ink font-medium leading-tight">{{ spot.name }}</div>
          <div class="font-display italic text-meta text-cocoa-500 mb-2">
            {{ spot.cuisine }} · {{ spot.neighborhood.split(',')[0] }}
          </div>
          <div class="flex items-center gap-2.5">
            <div class="inline-flex items-baseline gap-[1px] font-display text-ink leading-none">
              <span class="text-[22px] font-medium italic">{{ ratingParts(spot.rating).w }}</span>
              <span class="text-[13px] text-cocoa-500">.{{ ratingParts(spot.rating).f }}</span>
              <span class="font-mono text-[9px] text-cocoa-300 ml-[3px] tracking-[0.5px]">/10</span>
            </div>
            <span v-if="spot.returnable" class="stamp">Going back</span>
          </div>
        </div>

        <!-- Remove button -->
        <div class="flex flex-col items-center justify-center shrink-0 pl-1">
          <button
            @click="requestRemove(spot)"
            class="w-7 h-7 rounded-full flex items-center justify-center border cursor-pointer transition-colors"
            :class="removingId === spot.id
              ? 'bg-paprika border-paprika text-paper'
              : 'bg-transparent border-rule text-cocoa-400 hover:border-paprika hover:text-paprika'"
            :title="removingId === spot.id ? 'Tap again to remove' : 'Remove from collection'"
          >
            <svg width="10" height="10" viewBox="0 0 16 16" fill="none">
              <path d="M3 3l10 10M13 3L3 13" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/>
            </svg>
          </button>
          <div
            v-if="removingId === spot.id"
            class="font-mono text-[8px] text-paprika uppercase tracking-[0.5px] mt-1 text-center"
            style="max-width: 32px; line-height: 1.2"
          >again</div>
        </div>
      </div>
    </div>

  </div>
</template>
