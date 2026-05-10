<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Polaroid from './Polaroid.vue'
import { fetchCollections, createCollection, type Collection } from '../api/collections'

const emit = defineEmits<{ open: [collection: Collection] }>()

const collections = ref<Collection[]>([])
const loading     = ref(true)
const error       = ref<string | null>(null)

const creating    = ref(false)
const newName     = ref('')
const newSubtitle = ref('')
const saving      = ref(false)
const createError = ref<string | null>(null)

onMounted(async () => {
  try {
    collections.value = await fetchCollections()
  } catch {
    error.value = 'Could not load collections.'
  } finally {
    loading.value = false
  }
})

function openCreate() {
  newName.value = ''
  newSubtitle.value = ''
  createError.value = null
  creating.value = true
}

function cancelCreate() {
  creating.value = false
}

async function submitCreate() {
  if (!newName.value.trim()) return
  saving.value = true
  createError.value = null
  try {
    const c = await createCollection(newName.value.trim(), newSubtitle.value.trim())
    collections.value.push(c)
    creating.value = false
  } catch {
    createError.value = 'Could not save — please try again.'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="pb-tab-bar lg:pb-12">

    <!-- ── Header ───────────────────────────────────────────── -->
    <div class="flex justify-between items-baseline px-gutter lg:px-9 pt-3.5 lg:pt-8 pb-2">
      <div>
        <div class="caption mb-1">Curated stacks</div>
        <div class="font-display italic text-[32px] text-ink leading-none mt-1">Collections</div>
      </div>
      <button
        @click="openCreate"
        class="font-display italic text-[22px] text-paprika bg-transparent border-none cursor-pointer leading-none"
      >+</button>
    </div>

    <!-- ── Create form ───────────────────────────────────────── -->
    <div v-if="creating" class="px-gutter lg:px-9 py-4 border-b border-rule bg-paper-warm">
      <div class="field-label mb-3">New collection</div>
      <input
        v-model="newName"
        placeholder="Name"
        class="w-full bg-transparent border-0 border-b border-[rgba(60,40,20,0.2)] font-display text-[20px] text-ink font-medium outline-none pb-2 mb-3"
        @keyup.enter="submitCreate"
      />
      <input
        v-model="newSubtitle"
        placeholder="Subtitle — e.g. a Saturday, for visitors…"
        class="w-full bg-transparent border-0 border-b border-[rgba(60,40,20,0.2)] font-display italic text-[15px] text-cocoa-700 outline-none pb-2"
        @keyup.enter="submitCreate"
      />
      <div v-if="createError" class="font-display italic text-[13px] text-paprika mt-2">{{ createError }}</div>
      <div class="flex items-center gap-4 mt-4">
        <button
          @click="submitCreate"
          :disabled="saving || !newName.trim()"
          class="btn btn--primary py-1.5 px-4 text-[14px] disabled:opacity-40"
        >{{ saving ? '…' : 'save' }}</button>
        <button
          @click="cancelCreate"
          class="font-display italic text-[15px] text-cocoa-500 bg-transparent border-none cursor-pointer"
        >cancel</button>
      </div>
    </div>

    <!-- ── Loading / error ──────────────────────────────────── -->
    <div v-if="loading" class="px-gutter py-12 text-center font-display italic text-[15px] text-cocoa-400">
      Loading collections…
    </div>
    <div v-else-if="error" class="px-gutter py-12 text-center font-display italic text-[15px] text-paprika">
      {{ error }}
    </div>

    <!-- ── Empty state ───────────────────────────────────────── -->
    <div
      v-else-if="collections.length === 0"
      class="px-gutter py-12 text-center"
    >
      <div class="font-display italic text-[17px] text-cocoa-400">No collections yet.</div>
      <div class="font-display italic text-[13px] text-cocoa-300 mt-1">Tap + to create your first stack.</div>
    </div>

    <!-- ── Mobile: single column ────────────────────────────── -->
    <div v-else class="lg:hidden px-gutter pt-3.5">
      <div
        v-for="(collection, i) in collections" :key="collection.id"
        class="flex items-center gap-3 py-4 cursor-pointer"
        :style="i < collections.length - 1 ? 'border-bottom: 0.5px dashed rgba(60,40,20,0.18)' : ''"
        @click="emit('open', collection)"
      >
        <!-- Polaroid stack thumbnail -->
        <div class="relative shrink-0" style="width:76px;height:76px">
          <template v-if="collection.swatches.length">
            <div
              v-for="(sw, j) in collection.swatches.slice(0, 3)" :key="j"
              class="absolute"
              :style="`top:${j*2}px;left:${j*4}px;transform:rotate(${(j-1)*5}deg);z-index:${3-j}`"
            >
              <Polaroid :c1="sw.light" :c2="sw.dark" dish="" :size="56" />
            </div>
          </template>
          <!-- Placeholder when collection is empty -->
          <div v-else class="absolute inset-0 flex items-center justify-center">
            <Polaroid c1="#D4B896" c2="#8B6F4E" dish="" glyph="◐" :size="56" />
          </div>
        </div>
        <div class="flex-1 pl-2 min-w-0">
          <div class="font-display text-[22px] text-ink font-medium leading-tight">{{ collection.name }}</div>
          <div class="font-display italic text-[13px] text-cocoa-500 mt-0.5">{{ collection.subtitle }}</div>
          <div class="caption mt-1.5">{{ collection.spotCount }} entries</div>
        </div>
        <div class="font-display italic text-[22px] text-cocoa-400 shrink-0 leading-none">›</div>
      </div>
    </div>

    <!-- ── Desktop: two-column grid ─────────────────────────── -->
    <div v-if="!loading && !error && collections.length" class="hidden lg:grid grid-cols-2 gap-x-6 px-9 pt-3.5">
      <div
        v-for="collection in collections" :key="collection.id"
        class="flex items-center gap-3 py-5 cursor-pointer"
        style="border-bottom: 0.5px dashed rgba(60,40,20,0.18)"
        @click="emit('open', collection)"
      >
        <div class="relative shrink-0" style="width:76px;height:76px">
          <template v-if="collection.swatches.length">
            <div
              v-for="(sw, j) in collection.swatches.slice(0, 3)" :key="j"
              class="absolute"
              :style="`top:${j*2}px;left:${j*4}px;transform:rotate(${(j-1)*5}deg);z-index:${3-j}`"
            >
              <Polaroid :c1="sw.light" :c2="sw.dark" dish="" :size="56" />
            </div>
          </template>
          <div v-else class="absolute inset-0 flex items-center justify-center">
            <Polaroid c1="#D4B896" c2="#8B6F4E" dish="" glyph="◐" :size="56" />
          </div>
        </div>
        <div class="flex-1 pl-2 min-w-0">
          <div class="font-display text-[22px] text-ink font-medium leading-tight">{{ collection.name }}</div>
          <div class="font-display italic text-[13px] text-cocoa-500 mt-0.5">{{ collection.subtitle }}</div>
          <div class="caption mt-1.5">{{ collection.spotCount }} entries</div>
        </div>
        <div class="font-display italic text-[22px] text-cocoa-400 shrink-0 leading-none">›</div>
      </div>
    </div>

  </div>
</template>
