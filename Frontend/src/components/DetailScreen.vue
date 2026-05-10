<script setup lang="ts">
import { ref } from 'vue'
import Polaroid from './Polaroid.vue'
import { deleteSpot, photoUrl, type Spot } from '../api/spots'
import { fetchCollections, addSpotToCollection, type Collection } from '../api/collections'
import { KIND_META } from '../data/spots'

const props = defineProps<{ spot: Spot }>()
const emit  = defineEmits<{ back: []; deleted: [] }>()

// ── Save-to-collection sheet ──────────────────────────────────
const sheetOpen        = ref(false)
const sheetLoading     = ref(false)
const sheetError       = ref<string | null>(null)
const sheetCollections = ref<Collection[]>([])
const addedIds         = ref<number[]>([])

async function openSheet() {
  sheetOpen.value    = true
  sheetLoading.value = true
  sheetError.value   = null
  try {
    sheetCollections.value = await fetchCollections()
  } catch {
    sheetError.value = 'Could not load collections.'
  } finally {
    sheetLoading.value = false
  }
}

function closeSheet() {
  sheetOpen.value = false
  addedIds.value  = []
}

async function saveToCollection(collection: Collection) {
  try {
    await addSpotToCollection(collection.id, props.spot.id)
    if (!addedIds.value.includes(collection.id))
      addedIds.value = [...addedIds.value, collection.id]
  } catch {
    // non-blocking — user can retry
  }
}

const confirming = ref(false)
const deleting   = ref(false)
const deleteError = ref<string | null>(null)
let resetTimer: ReturnType<typeof setTimeout> | null = null

function requestDelete() {
  if (!confirming.value) {
    confirming.value = true
    resetTimer = setTimeout(() => { confirming.value = false }, 3000)
    return
  }
  if (resetTimer) clearTimeout(resetTimer)
  performDelete()
}

async function performDelete() {
  deleting.value = true
  deleteError.value = null
  try {
    await deleteSpot(props.spot.id)
    emit('deleted')
  } catch {
    deleteError.value = 'Could not delete — please try again.'
    confirming.value = false
  } finally {
    deleting.value = false
  }
}

function ratingParts(r: number) {
  const [w, f] = r.toFixed(1).split('.')
  return { w, f }
}

function formatDate(iso: string): string {
  const [y, m, d] = iso.split('-').map(Number)
  return new Date(y, m - 1, d).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
}
</script>

<template>
  <div class="pb-tab-bar lg:pb-12 min-h-full">

    <!-- ── Header band ─────────────────────────────────────────── -->
    <div
      class="relative overflow-hidden"
      :style="{
        height: '200px',
        background: `linear-gradient(135deg, ${spot.swatchLight} 0%, ${spot.swatchDark} 100%)`,
      }"
    >
      <!-- stripe texture -->
      <div
        class="absolute inset-0"
        style="background-image: repeating-linear-gradient(135deg, transparent 0 8px, rgba(255,255,255,0.05) 8px 9px);"
      />

      <!-- Back button -->
      <button
        @click="emit('back')"
        class="absolute top-3.5 left-4 z-10 w-9 h-9 rounded-full bg-paper/90 border-none flex items-center justify-center cursor-pointer shadow-fab"
      >
        <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
          <path d="M10 3l-5 5 5 5" stroke="#2A1F18" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>

      <!-- Save to collection -->
      <button
        @click="openSheet"
        class="absolute top-3.5 right-4 z-10 w-9 h-9 rounded-full bg-paper/90 border-none flex items-center justify-center cursor-pointer shadow-fab"
      >
        <svg width="14" height="16" viewBox="0 0 14 16" fill="none">
          <path d="M1 1h12v13.5l-6-3.5-6 3.5V1z" stroke="#2A1F18" stroke-width="1.4" stroke-linejoin="round"/>
        </svg>
      </button>

      <!-- Polaroid stack peeking from bottom -->
      <div class="absolute left-1/2 -translate-x-1/2 flex" style="bottom: -30px;">
        <div style="margin-right: -16px; transform: rotate(-6deg);">
          <Polaroid
            :src="spot.photoPath ? photoUrl(spot.photoPath) : undefined"
            :c1="spot.swatchLight" :c2="spot.swatchDark"
            :dish="spot.dish ?? ''" :glyph="KIND_META[spot.kind].glyph"
            :size="88"
          />
        </div>
        <div style="margin-right: -16px; transform: rotate(2deg) translateY(-6px);">
          <Polaroid
            :c1="spot.swatchDark" :c2="spot.swatchLight"
            dish="interior" glyph="◐"
            :size="88"
          />
        </div>
        <div style="transform: rotate(7deg);">
          <Polaroid
            c1="#D4B896" :c2="spot.swatchLight"
            dish="receipt" glyph="₸"
            :size="88"
          />
        </div>
      </div>
    </div>

    <!-- ── Body ───────────────────────────────────────────────── -->
    <div class="px-gutter" style="padding-top: 50px;">

      <!-- Title block -->
      <div class="font-mono text-[10px] text-cocoa-500 uppercase tracking-[1.2px] text-center">
        {{ formatDate(spot.visitedOn) }} · {{ KIND_META[spot.kind].label }} · {{ spot.cuisine }}
      </div>
      <div
        class="font-display text-ink text-center font-normal mt-2"
        style="font-size: 36px; line-height: 1; letter-spacing: -0.5px;"
      >
        {{ spot.name }}
      </div>
      <div class="font-display italic text-[14px] text-cocoa-500 text-center mt-1">
        {{ spot.neighborhood }}
      </div>

      <!-- Rating band -->
      <div class="mt-[22px] py-5 text-center border-t border-b border-rule">
        <div class="inline-flex items-baseline gap-[1px] font-display text-ink leading-none">
          <span class="font-medium italic" style="font-size: 48px;">{{ ratingParts(spot.rating).w }}</span>
          <span class="text-cocoa-500" style="font-size: 22px;">.{{ ratingParts(spot.rating).f }}</span>
          <span class="font-mono text-cocoa-300 ml-[3px] tracking-[0.5px]" style="font-size: 13px;">/10</span>
        </div>
        <div class="font-display italic text-[13px] text-cocoa-500 mt-1">memorable, in a quiet way</div>
      </div>

      <!-- Note with drop cap -->
      <div
        v-if="spot.note"
        class="font-display text-ink mt-[22px]"
        style="font-size: 17px; line-height: 1.5;"
      >
        <span
          class="font-display italic text-paprika float-left"
          style="font-size: 36px; line-height: 0.9; margin-right: 6px; margin-top: 4px;"
        >{{ spot.note.slice(0, 1) }}</span>{{ spot.note.slice(1) }}
      </div>

      <!-- Meta grid -->
      <div class="mt-6 grid grid-cols-2 gap-3.5 py-4 border-t border-rule">
        <div>
          <div class="font-mono text-[9px] text-cocoa-500 uppercase tracking-[1.2px] mb-1">Ordered</div>
          <div class="font-display italic text-[15px] text-ink">{{ spot.dish ?? '—' }}</div>
        </div>
        <div>
          <div class="font-mono text-[9px] text-cocoa-500 uppercase tracking-[1.2px] mb-1">Price</div>
          <div class="font-mono text-[15px] text-ink">
            <span v-for="n in 4" :key="n" :style="{ opacity: n <= spot.price ? '1' : '0.25' }">€</span>
          </div>
        </div>
        <div>
          <div class="font-mono text-[9px] text-cocoa-500 uppercase tracking-[1.2px] mb-1">Date</div>
          <div class="font-display italic text-[15px] text-ink">{{ formatDate(spot.visitedOn) }}</div>
        </div>
        <div>
          <div class="font-mono text-[9px] text-cocoa-500 uppercase tracking-[1.2px] mb-1">Verdict</div>
          <div class="font-display italic text-[15px] text-ink">
            {{ spot.returnable ? 'Going back' : 'One time' }}
          </div>
        </div>
      </div>

      <!-- Going back stamp -->
      <div v-if="spot.returnable" class="flex justify-center mt-4 mb-2">
        <span class="stamp" style="transform: rotate(-4deg);">✓ Going Back</span>
      </div>

      <!-- Delete -->
      <div class="mt-10 mb-2 border-t border-rule pt-6 flex flex-col items-center gap-2">
        <button
          @click="requestDelete"
          :disabled="deleting"
          class="font-mono text-[10px] uppercase tracking-[1.2px] border rounded-[3px] px-4 py-2 cursor-pointer transition-colors disabled:opacity-40"
          :class="confirming
            ? 'bg-paprika text-paper border-paprika'
            : 'bg-transparent text-cocoa-400 border-rule hover:text-paprika hover:border-paprika'"
        >
          {{ deleting ? 'Deleting…' : confirming ? 'Tap again to delete' : 'Delete entry' }}
        </button>
        <div v-if="deleteError" class="font-display italic text-[13px] text-paprika">{{ deleteError }}</div>
        <div v-if="confirming && !deleting" class="font-display italic text-[12px] text-cocoa-400">
          This cannot be undone
        </div>
      </div>

    </div>
  </div>

  <!-- ── Save-to-collection sheet ──────────────────────────── -->
  <Teleport to="body">
    <Transition name="sheet">
      <div
        v-if="sheetOpen"
        class="fixed inset-0 z-50 flex flex-col justify-end"
        style="background: rgba(30,20,10,0.45);"
        @click.self="closeSheet"
      >
        <div class="bg-paper rounded-t-[20px] flex flex-col" style="max-height: 70vh;">

          <!-- Handle -->
          <div class="flex justify-center pt-3 pb-1 shrink-0">
            <div class="w-10 h-1 rounded-full bg-rule" />
          </div>

          <!-- Sheet header -->
          <div class="flex justify-between items-center px-6 py-3 border-b border-rule shrink-0">
            <div class="caption">Save to collection</div>
            <button
              @click="closeSheet"
              class="font-display italic text-[15px] text-cocoa-500 bg-transparent border-none cursor-pointer"
            >done</button>
          </div>

          <!-- Loading -->
          <div v-if="sheetLoading" class="py-10 text-center font-display italic text-[15px] text-cocoa-400">
            Loading…
          </div>

          <!-- Error -->
          <div v-else-if="sheetError" class="py-10 text-center font-display italic text-[15px] text-paprika">
            {{ sheetError }}
          </div>

          <!-- Empty -->
          <div v-else-if="sheetCollections.length === 0" class="py-10 text-center">
            <div class="font-display italic text-[15px] text-cocoa-400">No collections yet.</div>
            <div class="font-display italic text-[13px] text-cocoa-300 mt-1">Go to Collections to create one.</div>
          </div>

          <!-- Collection rows -->
          <div v-else class="overflow-y-auto">
            <div
              v-for="(c, i) in sheetCollections"
              :key="c.id"
              class="flex items-center gap-4 px-6 py-4 cursor-pointer active:bg-paper-warm transition-colors"
              :style="i < sheetCollections.length - 1 ? 'border-bottom: 0.5px solid rgba(60,40,20,0.1)' : ''"
              @click="saveToCollection(c)"
            >
              <div class="flex-1 min-w-0">
                <div class="font-display text-[17px] text-ink font-medium leading-tight truncate">{{ c.name }}</div>
                <div v-if="c.subtitle" class="font-display italic text-[13px] text-cocoa-500 truncate">{{ c.subtitle }}</div>
              </div>
              <!-- Added checkmark -->
              <div
                v-if="addedIds.includes(c.id)"
                class="w-6 h-6 rounded-full bg-ink flex items-center justify-center shrink-0"
              >
                <svg width="10" height="10" viewBox="0 0 16 16" fill="none">
                  <path d="M3 8l4 4 6-6" stroke="#FBF7EE" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </div>
              <!-- Add indicator -->
              <div v-else class="font-display italic text-[22px] text-cocoa-300 shrink-0 leading-none">+</div>
            </div>
          </div>

        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.sheet-enter-active, .sheet-leave-active { transition: opacity 0.2s ease; }
.sheet-enter-active > div, .sheet-leave-active > div { transition: transform 0.25s cubic-bezier(0.22, 0.61, 0.36, 1); }
.sheet-enter-from, .sheet-leave-to { opacity: 0; }
.sheet-enter-from > div, .sheet-leave-to > div { transform: translateY(100%); }
</style>
