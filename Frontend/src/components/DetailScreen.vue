<script setup lang="ts">
import Polaroid from './Polaroid.vue'
import { photoUrl, type Spot } from '../api/spots'
import { KIND_META } from '../data/spots'

const props = defineProps<{ spot: Spot }>()
const emit  = defineEmits<{ back: [] }>()

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

      <!-- Entry number -->
      <div class="absolute top-4 right-4 font-mono text-[10px] uppercase tracking-[1.2px]" style="color: rgba(251,247,238,0.85);">
        Entry № {{ String(spot.id).padStart(3, '0') }}
      </div>

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

    </div>
  </div>
</template>
