<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import Polaroid from './Polaroid.vue'
import { createSpot, uploadPhoto } from '../api/spots'

declare global {
  interface Window {
    google?: {
      maps: {
        places: {
          Autocomplete: new (
            input: HTMLInputElement,
            opts?: { fields?: string[] }
          ) => {
            addListener: (event: string, handler: () => void) => void
            getPlace: () => {
              formatted_address?: string
              geometry?: { location: { lat: () => number; lng: () => number } }
            }
          }
        }
      }
    }
  }
}

const emit = defineEmits<{ cancel: []; saved: [] }>()

const SWATCH_BY_KIND = {
  coffee: ['#C9A27A', '#7A4E2F'] as const,
  lunch:  ['#E8C26C', '#B07028'] as const,
  dinner: ['#7A2A22', '#D9B26A'] as const,
}

const kindMeta = {
  coffee: { label: 'Coffee', glyph: '◔' },
  lunch:  { label: 'Lunch',  glyph: '◑' },
  dinner: { label: 'Dinner', glyph: '◕' },
}

const kind         = ref<'coffee' | 'lunch' | 'dinner'>('coffee')
const name         = ref('')
const cuisine      = ref('')
const neighborhood = ref('')
const dish         = ref('')
const note         = ref('')
const rating       = ref(8.4)
const price        = ref(2)
const returnable   = ref(true)

const latitude          = ref<number | null>(null)
const longitude         = ref<number | null>(null)
const neighborhoodInput = ref<HTMLInputElement | null>(null)

const photoInput   = ref<HTMLInputElement | null>(null)
const photoPreview = ref<string | null>(null)
const photoPath    = ref<string | null>(null)
const uploading    = ref(false)

const saving = ref(false)
const error  = ref<string | null>(null)

const ratingWhole = computed(() => rating.value.toFixed(1).split('.')[0])
const ratingFrac  = computed(() => rating.value.toFixed(1).split('.')[1])

function loadMapsApi(): Promise<void> {
  return new Promise((resolve, reject) => {
    if (window.google?.maps?.places) { resolve(); return }
    const key = import.meta.env.VITE_GOOGLE_MAPS_API_KEY
    if (!key) { reject(new Error('No API key')); return }
    const s = document.createElement('script')
    s.src = `https://maps.googleapis.com/maps/api/js?key=${key}&libraries=places`
    s.async = true
    s.onload = () => resolve()
    s.onerror = () => reject()
    document.head.appendChild(s)
  })
}

onMounted(async () => {
  try {
    await loadMapsApi()
    const ac = new window.google!.maps.places.Autocomplete(
      neighborhoodInput.value!,
      { fields: ['formatted_address', 'geometry'] }
    )
    ac.addListener('place_changed', () => {
      const place = ac.getPlace()
      if (place.formatted_address) neighborhood.value = place.formatted_address
      const loc = place.geometry?.location
      if (loc) { latitude.value = loc.lat(); longitude.value = loc.lng() }
    })
  } catch {
    // Falls back to plain text input without autocomplete
  }
})

function normaliseImage(file: File): Promise<File> {
  if (!file.type.match(/^image\/hei[cf]/i)) return Promise.resolve(file)
  return new Promise((resolve) => {
    const url = URL.createObjectURL(file)
    const img = new Image()
    img.onload = () => {
      const canvas = document.createElement('canvas')
      canvas.width = img.naturalWidth
      canvas.height = img.naturalHeight
      canvas.getContext('2d')!.drawImage(img, 0, 0)
      URL.revokeObjectURL(url)
      canvas.toBlob(
        blob => resolve(blob
          ? new File([blob], file.name.replace(/\.(heic|heif)$/i, '.jpg'), { type: 'image/jpeg' })
          : file
        ),
        'image/jpeg', 0.9
      )
    }
    img.onerror = () => { URL.revokeObjectURL(url); resolve(file) }
    img.src = url
  })
}

async function onFileSelected(event: Event) {
  const raw = (event.target as HTMLInputElement).files?.[0]
  if (!raw) return

  uploading.value = true
  error.value = null

  try {
    const file = await normaliseImage(raw)
    photoPreview.value = URL.createObjectURL(file)
    photoPath.value = await uploadPhoto(file)
  } catch {
    error.value = 'Photo upload failed — you can still save without one.'
    photoPreview.value = null
    photoPath.value = null
  } finally {
    uploading.value = false
  }
}

async function save() {
  if (!name.value.trim()) return
  saving.value = true
  error.value = null
  try {
    const [swatchLight, swatchDark] = SWATCH_BY_KIND[kind.value]
    await createSpot({
      name:         name.value.trim(),
      kind:         kind.value,
      cuisine:      cuisine.value.trim(),
      neighborhood: neighborhood.value.trim(),
      latitude:     latitude.value,
      longitude:    longitude.value,
      visitedOn:    new Date().toISOString().slice(0, 10),
      dish:         dish.value.trim()  || null,
      note:         note.value.trim()  || null,
      rating:       rating.value,
      price:        price.value,
      returnable:   returnable.value,
      swatchLight,
      swatchDark,
      photoPath:    photoPath.value,
    })
    emit('saved')
  } catch {
    error.value = 'Something went wrong. Please try again.'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="min-h-full bg-paper lg:flex lg:items-start lg:justify-center lg:py-12 lg:px-8">
    <div class="lg:w-140 lg:shadow-card lg:rounded-card lg:overflow-hidden">

      <!-- Header -->
      <div class="flex items-center justify-between px-gutter py-3.5 border-b border-rule-soft sticky top-0 bg-paper z-10">
        <button
          @click="emit('cancel')"
          class="font-display italic text-[15px] text-cocoa-500 bg-transparent border-none cursor-pointer p-0"
        >cancel</button>
        <div class="caption">New Entry</div>
        <button
          @click="save"
          :disabled="saving || uploading || !name.trim()"
          class="btn btn--primary py-1.25 px-3 text-[14px] disabled:opacity-40"
        >{{ saving ? '…' : 'save' }}</button>
      </div>

      <!-- Body -->
      <div class="px-gutter py-4.5 pb-tab-bar lg:pb-8 flex flex-col gap-4.5">

        <!-- Photo slot -->
        <div class="flex justify-center">
          <div class="relative cursor-pointer" @click="photoInput?.click()">
            <Polaroid
              :src="photoPreview ?? undefined"
              :c1="SWATCH_BY_KIND[kind][0]"
              :c2="SWATCH_BY_KIND[kind][1]"
              :dish="dish || 'tap to add'"
              :size="140" :rotate="-3"
              :glyph="kindMeta[kind].glyph"
            />
            <!-- Uploading overlay -->
            <div v-if="uploading" class="absolute inset-0 flex items-center justify-center bg-paper/70">
              <span class="font-mono text-[9px] text-cocoa-500 uppercase tracking-[0.8px]">uploading…</span>
            </div>
            <!-- Add button (hidden once a photo is set) -->
            <div
              v-if="!photoPreview"
              class="absolute -top-1.5 -right-2.5 w-8 h-8 rounded-full bg-paprika text-paper flex items-center justify-center text-lg font-display shadow-fab"
            >+</div>
          </div>
          <input ref="photoInput" type="file" accept="image/*" class="hidden" @change="onFileSelected" />
        </div>

        <!-- Occasion -->
        <div>
          <div class="field-label">The occasion</div>
          <div class="flex gap-1.5">
            <button
              v-for="(meta, k) in kindMeta" :key="k"
              @click="kind = k"
              class="inline-flex items-center gap-1.5 px-[11px] py-1.5 rounded-full font-display italic text-[14px] cursor-pointer transition-colors"
              :class="kind === k
                ? 'bg-ink text-paper border border-ink'
                : 'bg-transparent text-cocoa-700 border border-rule'"
            >
              <span class="text-[11px] opacity-80">{{ meta.glyph }}</span>
              {{ meta.label }}
            </button>
          </div>
        </div>

        <!-- Place name -->
        <div>
          <div class="field-label">Place</div>
          <input
            v-model="name"
            placeholder="What's it called?"
            class="w-full bg-transparent border-0 border-b border-[rgba(60,40,20,0.2)] font-display text-[22px] text-ink font-medium outline-none pb-2 pt-1.5"
          />
        </div>

        <!-- Cuisine -->
        <div>
          <div class="field-label">Type of place</div>
          <input
            v-model="cuisine"
            placeholder="e.g. Espresso bar, Italian…"
            class="w-full bg-transparent border-0 border-b border-[rgba(60,40,20,0.2)] font-display italic text-[16px] text-ink outline-none pb-2 pt-1.5"
          />
        </div>

        <!-- Neighbourhood -->
        <div>
          <div class="field-label">Where</div>
          <div class="flex items-center gap-2 px-3 py-2.5 rounded-[6px] bg-paper-warm">
            <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
              <path d="M8 14s5-5 5-9a5 5 0 10-10 0c0 4 5 9 5 9z M8 7a2 2 0 100-4 2 2 0 000 4z" stroke="#8B6F4E" stroke-width="1.2" fill="none"/>
            </svg>
            <input
              ref="neighborhoodInput"
              v-model="neighborhood"
              @input="latitude = null; longitude = null"
              placeholder="e.g. De Pijp, Amsterdam"
              class="flex-1 bg-transparent border-0 outline-none font-display italic text-[14px] text-cocoa-700 placeholder:text-cocoa-400"
            />
          </div>
        </div>

        <!-- Dish -->
        <div>
          <div class="field-label">What did you have?</div>
          <input
            v-model="dish"
            placeholder="e.g. Cortado + cardamom bun"
            class="w-full bg-transparent border-0 border-b border-[rgba(60,40,20,0.2)] font-display italic text-[16px] text-ink outline-none pb-2 pt-1.5"
          />
        </div>

        <!-- Rating -->
        <div>
          <div class="field-label">
            Rating <span class="opacity-50">· out of ten</span>
          </div>
          <div class="pt-3.5 pb-1">
            <div class="flex items-baseline justify-center gap-px mb-3.5">
              <span class="font-display italic text-[48px] text-ink leading-none font-medium">{{ ratingWhole }}</span>
              <span class="font-display text-[22px] text-cocoa-500">.{{ ratingFrac }}</span>
              <span class="font-mono text-[13px] text-cocoa-300 ml-1 tracking-[0.5px]">/10</span>
            </div>
            <input type="range" min="0" max="10" step="0.1" :value="rating" @input="rating = parseFloat(($event.target as HTMLInputElement).value)" />
            <div class="flex justify-between font-mono text-[9px] text-cocoa-300 uppercase tracking-[0.8px] mt-1">
              <span>regret</span><span>fine</span><span>memorable</span>
            </div>
          </div>
        </div>

        <!-- Price -->
        <div>
          <div class="field-label">Price</div>
          <div class="flex gap-1.5">
            <button
              v-for="n in [1, 2, 3, 4]" :key="n"
              @click="price = n"
              class="flex-1 py-3 rounded-[6px] cursor-pointer font-mono text-[13px] tracking-[1px] transition-colors border"
              :class="price === n
                ? 'bg-ink text-paper border-ink'
                : 'bg-transparent text-cocoa-700 border-rule'"
            >{{ '€'.repeat(n) }}</button>
          </div>
        </div>

        <!-- Notes -->
        <div>
          <div class="field-label">Notes</div>
          <textarea
            v-model="note"
            placeholder="What did it taste like? Who was there? Worth telling someone about?"
            class="w-full min-h-20 resize-none border border-rule rounded-[6px] px-3 py-2.5 font-display text-[15px] text-ink bg-paper outline-none leading-[1.4]"
          />
        </div>

        <!-- Going back toggle -->
        <div class="flex items-center justify-between px-3.5 py-3.5 bg-paper-warm rounded-[6px]">
          <div>
            <div class="font-display text-[16px] text-ink font-medium">I'd go back</div>
            <div class="font-display italic text-[12px] text-cocoa-500">earns a stamp on the entry</div>
          </div>
          <div
            @click="returnable = !returnable"
            class="w-11 h-6.5 rounded-full relative cursor-pointer transition-colors"
            :class="returnable ? 'bg-ink' : 'bg-[rgba(60,40,20,0.2)]'"
          >
            <div
              class="absolute top-0.75 w-5 h-5 rounded-full bg-paper transition-all duration-200"
              :style="{ left: returnable ? '21px' : '3px' }"
            />
          </div>
        </div>

        <!-- Error -->
        <div v-if="error" class="font-display italic text-[13px] text-paprika text-center">
          {{ error }}
        </div>

      </div>
    </div>
  </div>
</template>
