<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import Polaroid from './Polaroid.vue'
import { fetchSpots, photoUrl, type Spot } from '../api/spots'
import { KIND_META, type Kind } from '../data/spots'

const emit = defineEmits<{ open: [spot: Spot] }>()

type Filter = 'all' | Kind
const filter = ref<Filter>('all')
const spots  = ref<Spot[]>([])
const loading = ref(true)
const error   = ref<string | null>(null)

onMounted(async () => {
  try {
    spots.value = await fetchSpots()
  } catch {
    error.value = 'Could not load entries. Is the backend running?'
  } finally {
    loading.value = false
  }
})

const filtered = computed(() =>
  filter.value === 'all' ? spots.value : spots.value.filter(s => s.kind === filter.value)
)

const TILTS = [-2, 1.5, -1, 2, -1.5, 1]

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
  <div class="pb-tab-bar lg:pb-12">

    <!-- ── Mobile masthead ──────────────────────────────────── -->
    <div class="lg:hidden px-gutter pt-3.5 pb-2">
      <div class="flex justify-between items-center font-mono text-[10px] text-cocoa-500 uppercase tracking-[1.2px] pb-2 border-b border-rule">
        <span>Vol. III · No. 17</span>
        <span>May, MMXXVI</span>
      </div>
      <div class="font-display text-masthead text-ink mt-3.5" style="letter-spacing:-0.5px;line-height:0.95">
        The <em>Journal</em>
      </div>
      <div class="font-display italic text-[14px] text-cocoa-500 mt-1">
        a personal log of meals worth remembering
      </div>
    </div>

    <!-- ── Desktop masthead ──────────────────────────────────── -->
    <div class="hidden lg:flex justify-between items-end pb-3 border-b border-rule mb-6 px-9 pt-8">
      <div>
        <div class="caption mb-2">The Journal · May MMXXVI</div>
        <div class="font-display text-[56px] text-ink leading-none" style="letter-spacing:-0.5px">
          Recent <em>entries</em>
        </div>
      </div>
      <div class="flex items-center gap-2 border border-rule px-3 py-2 rounded-full font-display italic text-[14px] text-cocoa-500 cursor-pointer hover:bg-paper-warm transition-colors">
        <svg width="13" height="13" viewBox="0 0 16 16" fill="none">
          <circle cx="7" cy="7" r="5" stroke="#8B6F4E" stroke-width="1.4"/>
          <path d="M11 11l4 4" stroke="#8B6F4E" stroke-width="1.4" stroke-linecap="round"/>
        </svg>
        search the archive…
      </div>
    </div>

    <!-- ── Filter chips ─────────────────────────────────────── -->
    <div class="flex gap-1.5 px-gutter lg:px-9 pt-3.5 pb-2.5 overflow-x-auto" style="-webkit-overflow-scrolling:touch">
      <button @click="filter = 'all'" class="chip shrink-0" :class="filter === 'all' ? 'is-active' : ''">All</button>
      <button
        v-for="(meta, k) in KIND_META" :key="k"
        @click="filter = (k as Kind)"
        class="chip shrink-0" :class="filter === k ? 'is-active' : ''"
      >
        <span class="text-[11px]">{{ meta.glyph }}</span>{{ meta.label }}
      </button>
    </div>

    <!-- ── Loading / error states ────────────────────────────── -->
    <div v-if="loading" class="px-gutter py-12 text-center font-display italic text-[15px] text-cocoa-400">
      Loading entries…
    </div>
    <div v-else-if="error" class="px-gutter py-12 text-center font-display italic text-[15px] text-paprika">
      {{ error }}
    </div>

    <!-- ── Mobile: single-column entries ────────────────────── -->
    <div v-else class="lg:hidden px-gutter pt-1">
      <div
        v-for="(spot, i) in filtered" :key="spot.id"
        class="flex gap-3.5 py-5 cursor-pointer"
        style="border-bottom: 0.5px dashed rgba(60,40,20,0.18)"
        @click="emit('open', spot)"
      >
        <Polaroid
          :src="spot.photoPath ? photoUrl(spot.photoPath) : undefined"
          :c1="spot.swatchLight" :c2="spot.swatchDark"
          :dish="spot.dish ?? ''" :glyph="KIND_META[spot.kind].glyph"
          :size="88" :rotate="TILTS[i % 6]"
        />
        <div class="flex-1 min-w-0 pt-0.5">
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
          <div class="font-display text-[13px] leading-[1.4] text-cocoa-700 mb-2">{{ spot.note ?? '' }}</div>
          <div class="flex items-center gap-2.5">
            <div class="inline-flex items-baseline gap-[1px] font-display text-ink leading-none">
              <span class="text-[22px] font-medium italic">{{ ratingParts(spot.rating).w }}</span>
              <span class="text-[13px] text-cocoa-500">.{{ ratingParts(spot.rating).f }}</span>
              <span class="font-mono text-[9px] text-cocoa-300 ml-[3px] tracking-[0.5px]">/10</span>
            </div>
            <span v-if="spot.returnable" class="stamp">Going back</span>
          </div>
        </div>
      </div>
    </div>

    <!-- ── Desktop: two-column grid ─────────────────────────── -->
    <div v-if="!loading && !error" class="hidden lg:grid grid-cols-2 gap-x-6 px-9 pt-1">
      <div
        v-for="(spot, i) in filtered" :key="spot.id"
        class="flex gap-3.5 py-5 cursor-pointer"
        style="border-bottom: 0.5px dashed rgba(60,40,20,0.18)"
        @click="emit('open', spot)"
      >
        <Polaroid
          :src="spot.photoPath ? photoUrl(spot.photoPath) : undefined"
          :c1="spot.swatchLight" :c2="spot.swatchDark"
          :dish="spot.dish ?? ''" :glyph="KIND_META[spot.kind].glyph"
          :size="88" :rotate="TILTS[i % 6]"
        />
        <div class="flex-1 min-w-0 pt-0.5">
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
          <div class="font-display text-[13px] leading-[1.4] text-cocoa-700 mb-2">{{ spot.note ?? '' }}</div>
          <div class="flex items-center gap-2.5">
            <div class="inline-flex items-baseline gap-[1px] font-display text-ink leading-none">
              <span class="text-[22px] font-medium italic">{{ ratingParts(spot.rating).w }}</span>
              <span class="text-[13px] text-cocoa-500">.{{ ratingParts(spot.rating).f }}</span>
              <span class="font-mono text-[9px] text-cocoa-300 ml-[3px] tracking-[0.5px]">/10</span>
            </div>
            <span v-if="spot.returnable" class="stamp">Going back</span>
          </div>
        </div>
      </div>
    </div>

    <!-- ── Footer ────────────────────────────────────────────── -->
    <div v-if="!loading && !error" class="px-gutter lg:px-9 py-5 font-mono text-[9px] text-cocoa-300 uppercase tracking-[1.2px] text-center">
      — End of entries · {{ filtered.length }} of {{ spots.length }} —
    </div>

  </div>
</template>
